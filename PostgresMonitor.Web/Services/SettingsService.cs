using System.Text.Json;
using PostgresMonitor.Web.Models;

namespace PostgresMonitor.Web.Services
{
    public class SettingsService
    {
        private readonly string _filePath = "app_settings.json";

        public virtual async Task<List<DbSettings>> GetAllSettingsAsync()
        {
            if (!File.Exists(_filePath)) return new List<DbSettings>();

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<DbSettings>>(json) ?? new List<DbSettings>();
        }

        public virtual async Task<DbSettings> GetActiveSettingsAsync()
        {
            var all = await GetAllSettingsAsync();
            return all.FirstOrDefault(s => s.IsActive);
        }

        public virtual async Task<DbSettings> GetByIdAsync(Guid id)
        {
            var all = await GetAllSettingsAsync();
            return all.FirstOrDefault(s => s.Id == id);
        }

        public virtual async Task SaveAsync(DbSettings settings)
        {
            var all = await GetAllSettingsAsync();
            var existing = all.FirstOrDefault(s => s.Id == settings.Id);

            if (existing != null)
            {
                if (string.IsNullOrWhiteSpace(settings.Password))
                {
                    settings.Password = existing.Password;
                }

                all.Remove(existing);
            }

            all.Add(settings);
            await SaveListAsync(all);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var all = await GetAllSettingsAsync();
            var existing = all.FirstOrDefault(s => s.Id == id);

            if (existing != null)
            {
                all.Remove(existing);
                await SaveListAsync(all);
            }
        }

        public virtual async Task SetActiveAsync(Guid id)
        {
            var all = await GetAllSettingsAsync();
            foreach (var setting in all)
            {
                setting.IsActive = (setting.Id == id);
            }
            await SaveListAsync(all);
        }

        private async Task SaveListAsync(List<DbSettings> list)
        {
            var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}