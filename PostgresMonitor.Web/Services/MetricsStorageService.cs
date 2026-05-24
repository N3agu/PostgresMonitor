using System.Text.Json;
using PostgresMonitor.Web.Models;

namespace PostgresMonitor.Web.Services
{
    public class MetricsStorageService
    {
        private readonly string _filePath = "metrics_history.json";

        public async Task SaveMetricsAsync(HealthMetrics metrics)
        {
            var history = await GetMetricsHistoryAsync();
            history.Add(metrics);

            if (history.Count > 1000)
            {
                history.RemoveAt(0);
            }

            var json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<List<HealthMetrics>> GetMetricsHistoryAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<HealthMetrics>();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<HealthMetrics>();
            }

            return JsonSerializer.Deserialize<List<HealthMetrics>>(json) ?? new List<HealthMetrics>();
        }
    }
}