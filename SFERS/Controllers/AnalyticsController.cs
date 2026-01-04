using Microsoft.AspNetCore.Mvc;

namespace SFERS.Controllers
{
    public class AnalyticsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // OPTIONAL: Endpoint for Chart.js to fetch real data via AJAX
        [HttpGet]
        public IActionResult GetUsageData()
        {
            var data = new
            {
                labels = new[] { "Auditorium", "Lab", "Room 3", "Room 4", "Room 5" },
                values = new[] { 120, 90, 45, 30, 60 }
            };
            return Json(data);
        }
    }
}