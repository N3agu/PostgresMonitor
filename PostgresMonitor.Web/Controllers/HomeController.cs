using Microsoft.AspNetCore.Mvc;
using PostgresMonitor.Web.Services;

namespace PostgresMonitor.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly MetricsStorageService _storageService;

        public HomeController(MetricsStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<IActionResult> Index()
        {
            var history = await _storageService.GetMetricsHistoryAsync();
            return View(history);
        }
    }
}