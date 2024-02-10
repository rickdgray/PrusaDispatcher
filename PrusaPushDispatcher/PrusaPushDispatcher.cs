using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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

            using var client = new HttpClient();

            var lastPoll = DateTimeOffset.Now;
            var status = PrinterStatus.Unknown;

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Polling Printer.");

                var printerStatusResponse = await client.GetAsync(_settings.PrinterUrl, stoppingToken);
                if (printerStatusResponse != null)
                {
                    var printerStatus = await printerStatusResponse.Content.ReadAsStringAsync();
                    if (printerStatus != null)
                    {
                        //TODO: handle json

                        _logger.LogInformation("Found new printer status: {status}.", status);

                        var notification = new Dictionary<string, string>
                    {
                        { "token", _settings.PushoverAppKey },
                        { "user", _settings.PushoverUserKey },
                        { "title", "Printer Status" },
                        { "message", $"Printer status is now: {status}" }
                    };

                        await client.PostAsync(
                                "https://api.pushover.net/1/messages.json",
                                new FormUrlEncodedContent(notification),
                                stoppingToken);

                        _logger.LogInformation("Notification pushed.");
                    }
                }

                _logger.LogDebug("Waiting until next poll time: {pollTime}",
                    DateTimeOffset.Now.AddMinutes(_settings.PollRateInMinutes));

                lastPoll = DateTimeOffset.Now;
                await Task.Delay(_settings.PollRateInMinutes * 60000, stoppingToken);
            }
        }
    }
}
