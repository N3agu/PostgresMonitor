using System.Text.Json;
using PostgresMonitor.Web.Models;

namespace PostgresMonitor.Web.Services
{
    public class SettingsService
    {
        private readonly string _filePath = "app_settings.json";

        public async Task<DbSettings> GetSettingsAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new DbSettings();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<DbSettings>(json) ?? new DbSettings();
        }

        public async Task SaveSettingsAsync(DbSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}