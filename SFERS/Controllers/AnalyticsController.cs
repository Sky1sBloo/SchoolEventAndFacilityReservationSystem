using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFERS.Controllers
{
    [Authorize]
    public class AnalyticsController : Controller
    {
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult GetUsageData()
        {
            var data = new
            {
                labels = new[] { "Auditorium", "Laboratory", "Room 3", "Room 4", "Room 5" },
                values = new[] { 120, 90, 45, 30, 60 }
            };
            return Json(data);
        }
    }
}
