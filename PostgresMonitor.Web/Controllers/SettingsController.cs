using Microsoft.AspNetCore.Mvc;
using PostgresMonitor.Web.Models;
using PostgresMonitor.Web.Services;

namespace PostgresMonitor.Web.Controllers
{
    public class SettingsController : Controller
    {
        private readonly SettingsService _settingsService;

        public SettingsController(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = await _settingsService.GetSettingsAsync();
            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> Save(DbSettings settings)
        {
            if (ModelState.IsValid)
            {
                await _settingsService.SaveSettingsAsync(settings);
                TempData["SuccessMessage"] = "Settings saved successfully! The background service will use these on its next run.";
                return RedirectToAction("Index");
            }
            return View("Index", settings);
        }
    }
}