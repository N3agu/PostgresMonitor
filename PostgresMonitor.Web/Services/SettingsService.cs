using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PostgresMonitor.Web.Models;

namespace PostgresMonitor.Web.Services
{
    public class SettingsService
    {
        private readonly string _filePath = "app_settings.json";

        public async Task<List<DbSettings>> GetAllSettingsAsync()
        {
            if (!File.Exists(_filePath)) return new List<DbSettings>();

            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<DbSettings>>(json) ?? new List<DbSettings>();
        }

        public async Task<DbSettings> GetActiveSettingsAsync()
        {
            var all = await GetAllSettingsAsync();
            return all.FirstOrDefault(s => s.IsActive);
        }

        public async Task<DbSettings> GetByIdAsync(Guid id)
        {
            var all = await GetAllSettingsAsync();
            return all.FirstOrDefault(s => s.Id == id);
        }

        public async Task SaveAsync(DbSettings settings)
        {
            var all = await GetAllSettingsAsync();
            var existing = all.FirstOrDefault(s => s.Id == settings.Id);

            if (existing != null)
            {
                all.Remove(existing);
            }

            all.Add(settings);
            await SaveListAsync(all);
        }

        public async Task DeleteAsync(Guid id)
        {
            var all = await GetAllSettingsAsync();
            var existing = all.FirstOrDefault(s => s.Id == id);

            if (existing != null)
            {
                all.Remove(existing);
                await SaveListAsync(all);
            }
        }

        public async Task SetActiveAsync(Guid id)
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