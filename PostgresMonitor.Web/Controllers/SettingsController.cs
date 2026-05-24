using Microsoft.AspNetCore.Mvc;
using PostgresMonitor.Web.Models;
using PostgresMonitor.Web.Services;
using System;
using System.Threading.Tasks;

namespace PostgresMonitor.Web.Controllers
{
    public class SettingsController : Controller
    {
        private readonly SettingsService _settingsService;

        public SettingsController(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<IActionResult> Index()
        {
            var settingsList = await _settingsService.GetAllSettingsAsync();
            return View(settingsList);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id.HasValue && id.Value != Guid.Empty)
            {
                var existing = await _settingsService.GetByIdAsync(id.Value);
                if (existing != null) return View(existing);
            }
            return View(new DbSettings());
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DbSettings settings)
        {
            ModelState.Remove("Password");

            var existingDb = await _settingsService.GetByIdAsync(settings.Id);

            if (existingDb == null && string.IsNullOrWhiteSpace(settings.Password))
            {
                ModelState.AddModelError("Password", "A password is required when adding a new database connection.");
            }

            if (ModelState.IsValid)
            {
                await _settingsService.SaveAsync(settings);
                TempData["SuccessMessage"] = "Database configuration saved successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _settingsService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Database configuration deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(Guid id)
        {
            await _settingsService.SetActiveAsync(id);
            TempData["SuccessMessage"] = "Active database changed. The collector will now monitor this database.";
            return RedirectToAction(nameof(Index));
        }
    }
}