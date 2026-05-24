namespace PostgresMonitor.Web.Services
{
    public class MetricsCollectorBackgroundService : BackgroundService
    {
        private readonly PostgresMetricsService _metricsService;
        private readonly MetricsStorageService _storageService;
        private readonly SettingsService _settingsService;
        private readonly ILogger<MetricsCollectorBackgroundService> _logger;

        public MetricsCollectorBackgroundService(
            PostgresMetricsService metricsService,
            MetricsStorageService storageService,
            SettingsService settingsService,
            ILogger<MetricsCollectorBackgroundService> logger)
        {
            _metricsService = metricsService;
            _storageService = storageService;
            _settingsService = settingsService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Metrics Collector Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                int delaySeconds = 10;

                try
                {
                    var settings = await _settingsService.GetActiveSettingsAsync();

                    if (settings != null)
                    {
                        delaySeconds = Math.Max(1, settings.PollingIntervalSeconds);

                        var metrics = await _metricsService.CollectMetricsAsync();
                        await _storageService.SaveMetricsAsync(metrics);
                        _logger.LogInformation("Metrics collected successfully at {Time} (Interval: {Sec}s)", metrics.Timestamp, delaySeconds);
                    }
                    else
                    {
                        _logger.LogInformation("No active database configured. Waiting before checking again...");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while collecting PostgreSQL metrics.");
                }

                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
            }
        }
    }
}