namespace PostgresMonitor.Web.Services
{
    public class MetricsCollectorBackgroundService : BackgroundService
    {
        private readonly PostgresMetricsService _metricsService;
        private readonly MetricsStorageService _storageService;
        private readonly ILogger<MetricsCollectorBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);

        public MetricsCollectorBackgroundService(
            PostgresMetricsService metricsService,
            MetricsStorageService storageService,
            ILogger<MetricsCollectorBackgroundService> logger)
        {
            _metricsService = metricsService;
            _storageService = storageService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Metrics Collector Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var metrics = await _metricsService.CollectMetricsAsync();
                    await _storageService.SaveMetricsAsync(metrics);
                    _logger.LogInformation("Metrics collected and saved successfully at {Time}", metrics.Timestamp);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while collecting PostgreSQL metrics.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}