using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;

namespace PrusaPushDispatcher
{
    internal class PrusaPushDispatcher : BackgroundService
    {
        private readonly Settings _settings;
        private readonly ILogger<PrusaPushDispatcher> _logger;

        public PrusaPushDispatcher(IOptions<Settings> settings,
            ILogger<PrusaPushDispatcher> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting application.");

            if (string.IsNullOrWhiteSpace(_settings.PushoverUserKey))
            {
                throw new ArgumentNullException("PushoverUserKey", "The user key was not specified!");
            }

            if (string.IsNullOrWhiteSpace(_settings.PushoverAppKey))
            {
                throw new ArgumentNullException("PushoverAppKey", "The app key was not specified!");
            }

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
            var lastPrinterStatus = PrinterStatus.Unknown;

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Polling Printer.");

                var printerStatus = await printerClient.GetFromJsonAsync<PrinterStatus>(_settings.PrinterUrl, cancellationToken);

                if (printerStatus == null)
                {
                    _logger.LogInformation("Printer not responding.");

                    var notification = new Dictionary<string, string>
                    {
                        { "token", _settings.PushoverAppKey },
                        { "user", _settings.PushoverUserKey },
                        { "title", "Printer Status" },
                        { "message", "Printer not responding." }
                    };

                    await pushoverClient.PostAsync(
                        "https://api.pushover.net/1/messages.json",
                        new FormUrlEncodedContent(notification),
                        cancellationToken);

                    _logger.LogInformation("Notification pushed.");
                }
                else if (printerStatus != lastPrinterStatus)
                {
                    _logger.LogInformation("Found new printer status: {status}.", printerStatus);

                    var notification = new Dictionary<string, string>
                    {
                        { "token", _settings.PushoverAppKey },
                        { "user", _settings.PushoverUserKey },
                        { "title", "Printer Status" },
                        { "message", $"Printer status is now: {printerStatus}" }
                    };

                    await pushoverClient.PostAsync(
                        "https://api.pushover.net/1/messages.json",
                        new FormUrlEncodedContent(notification),
                        cancellationToken);

                    _logger.LogInformation("Notification pushed.");

                    lastPrinterStatus = printerStatus;
                }

                _logger.LogDebug("Waiting until next poll time: {pollTime}",
                    DateTimeOffset.Now.AddMinutes(_settings.PollRateInSeconds));

                lastPoll = DateTimeOffset.Now;
                await Task.Delay(_settings.PollRateInSeconds, cancellationToken);
            }
        }
    }
}
