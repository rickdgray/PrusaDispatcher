using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PrusaPushDispatcher.Models;
using System.Net;
using System.Net.Http.Json;

namespace PrusaPushDispatcher
{
    internal class PrusaPushDispatcher(IOptions<Settings> settings,
        ILogger<PrusaPushDispatcher> logger) : BackgroundService
    {
        // All statuses:
        // "IDLE", "BUSY", "PRINTING", "PAUSED", "FINISHED", "STOPPED", "ERROR", "ATTENTION", "READY"
        private static readonly List<string> PrinterStateWhitelist =
        [
            "BUSY", "FINISHED", "ERROR", "ATTENTION"
        ];

        private readonly Settings _settings = settings.Value;
        private readonly ILogger<PrusaPushDispatcher> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting application.");

            // as per new httpclient guidelines and updated digest auth for .NET 6+
            // https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
            // https://github.com/CallumHoughton18/csharp-dotnet-digest-authentication/issues/1
            using var printerClient = new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                Credentials = new NetworkCredential(_settings.PrinterUsername, _settings.PrinterApiKey)
            });

            using var pushoverClient = new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5)
            });

            var lastPoll = DateTimeOffset.Now;
            var lastPrinterState = "IDLE";
            var failCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Polling Printer.");

                PrinterStatus? printerStatus = null;

                try
                {
                    // TODO: better url builder
                    printerStatus = await printerClient.GetFromJsonAsync<PrinterStatus>($"{_settings.PrinterUrl}/api/v1/status", cancellationToken);
                    failCount = 0;
                }
                catch (HttpRequestException)
                {
                    failCount++;
                }

                // at 10 fails, report unresponsive, but don't keep spamming after 10 fails.
                if (printerStatus == null && failCount == 10)
                {
                    _logger.LogInformation("Printer not responding.");

                    lastPrinterState = "UNRESPONSIVE";

                    var notification = new Dictionary<string, string>
                    {
                        { "token", _settings.PushoverAppKey },
                        { "user", _settings.PushoverUserKey },
                        { "title", "Printer State" },
                        { "message", "Printer not responding." }
                    };

                    await pushoverClient.PostAsync(
                        "https://api.pushover.net/1/messages.json",
                        new FormUrlEncodedContent(notification),
                        cancellationToken);

                    _logger.LogInformation("Notification pushed.");
                }
                
                // tell me if status changed
                if (printerStatus != null && printerStatus.Printer.State != lastPrinterState)
                {
                    _logger.LogInformation("Found new printer state: {state}.", printerStatus.Printer.State);

                    if (PrinterStateWhitelist.Contains(printerStatus.Printer.State))
                    {
                        // TODO: customizable notification text
                        var notification = new Dictionary<string, string>
                        {
                            { "token", _settings.PushoverAppKey },
                            { "user", _settings.PushoverUserKey },
                            { "title", "Prusa MK4" },
                            { "message", $"Printer state is now: {printerStatus.Printer.State}" }
                        };

                        await pushoverClient.PostAsync(
                            "https://api.pushover.net/1/messages.json",
                            new FormUrlEncodedContent(notification),
                            cancellationToken);

                        _logger.LogInformation("Notification pushed.");
                    }
                    else
                    {
                        _logger.LogDebug("Ignoring state change not in whitelist: {state}", printerStatus.Printer.State);
                    }

                    lastPrinterState = printerStatus.Printer.State;
                }

                lastPoll = DateTimeOffset.Now;
                await Task.Delay(_settings.PollRateInSeconds * 1000, cancellationToken);
            }
        }
    }
}
