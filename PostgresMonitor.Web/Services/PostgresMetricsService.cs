using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Npgsql;
using PostgresMonitor.Web.Models;

namespace PostgresMonitor.Web.Services
{
    public class PostgresMetricsService
    {
        private readonly SettingsService _settingsService;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;

        public PostgresMetricsService(SettingsService settingsService)
        {
            _settingsService = settingsService;

            if (OperatingSystem.IsWindows())
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                _cpuCounter.NextValue();
            }
        }

        public async Task<HealthMetrics> CollectMetricsAsync()
        {
            var metrics = new HealthMetrics { Timestamp = DateTime.UtcNow };

            if (OperatingSystem.IsWindows())
            {
                metrics.CpuUsage = Math.Round(_cpuCounter.NextValue(), 2);
                metrics.MemoryUsage = Math.Round(_ramCounter.NextValue(), 2);
            }
            else
            {
                metrics.CpuUsage = -1;
                metrics.MemoryUsage = -1;
            }

            var settings = await _settingsService.GetSettingsAsync();

            if (string.IsNullOrWhiteSpace(settings.Password))
            {
                throw new InvalidOperationException("Database password is not configured. Please update settings.");
            }

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = settings.Host,
                Port = settings.Port,
                Database = settings.Database,
                Username = settings.Username,
                Password = settings.Password,
                Timeout = 5
            };

            using var connection = new NpgsqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            using (var cmd = new NpgsqlCommand("SELECT count(*) FROM pg_stat_activity WHERE state = 'active';", connection))
            {
                metrics.ActiveConnections = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            using (var cmd = new NpgsqlCommand("SELECT count(*) FROM pg_stat_activity WHERE state = 'active' AND now() - query_start > interval '2 seconds';", connection))
            {
                metrics.SlowQueries = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            return metrics;
        }
    }
}