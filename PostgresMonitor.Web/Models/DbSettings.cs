namespace PostgresMonitor.Web.Models
{
    public class DbSettings
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ConnectionName { get; set; } = "Local Postgres";
        public bool IsActive { get; set; } = false;

        public int PollingIntervalSeconds { get; set; } = 10;
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5432;
        public string Database { get; set; } = "postgres";
        public string Username { get; set; } = "postgres";
        public string Password { get; set; } = "";
    }
}