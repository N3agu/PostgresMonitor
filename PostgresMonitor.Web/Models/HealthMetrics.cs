namespace PostgresMonitor.Web.Models
{
    public class HealthMetrics
    {
        public DateTime Timestamp { get; set; }

        public int ActiveConnections { get; set; }
        public int SlowQueries { get; set; }

        public double CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
    }
}
