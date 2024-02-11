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

            using var pushoverClient = new HttpClient();

            var lastPoll = DateTimeOffset.Now;
            var lastPrinterState = "IDLE";

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Polling Printer.");

                // TODO: better url builder
                var printerStatus = await printerClient.GetFromJsonAsync<PrinterStatus>($"{_settings.PrinterUrl}/api/v1/status", cancellationToken);

                if (printerStatus == null)
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
                else if (printerStatus.Printer.State != lastPrinterState)
                {
                    _logger.LogInformation("Found new printer status: {status}.", printerStatus.Printer.State);

                    var notification = new Dictionary<string, string>
                    {
                        { "token", _settings.PushoverAppKey },
                        { "user", _settings.PushoverUserKey },
                        { "title", "Printer State" },
                        { "message", $"Printer state is now: {printerStatus.Printer.State}" }
                    };

                    await pushoverClient.PostAsync(
                        "https://api.pushover.net/1/messages.json",
                        new FormUrlEncodedContent(notification),
                        cancellationToken);

                    _logger.LogInformation("Notification pushed.");

                    lastPrinterState = printerStatus.Printer.State;
                }

                lastPoll = DateTimeOffset.Now;
                await Task.Delay(_settings.PollRateInSeconds * 1000, cancellationToken);
            }
        }
    }
}
