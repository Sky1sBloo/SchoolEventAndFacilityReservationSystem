using Microsoft.AspNetCore.Mvc;
using SFERS.Models.ViewModel;

namespace SFERS.Controllers.Admin
{
    [Area("Admin")]
    public class AnalyticsController : Controller
    {
        public IActionResult Index()
        {
            // We create the model and fill it with data
            var model = new AnalyticsViewModel
            {
                // Populating data for the "Most Used Rooms" Bar Chart
                RoomLabels = new[] { "Auditorium", "Lab 1", "Room 3", "Room 4" },
                RoomUsageData = new[] { 120, 90, 45, 30 },

                // Populating data for "Equipment Demand" Pie Chart
                ProjectorCount = 40,
                MicCount = 20,
                LaptopCount = 15
            };

            return View(model);
        }

        public IActionResult ExportReport()
        {
            // Simple mock export logic
            var csvContent = "Report Data, Value\nRooms, 5\nEquipment, 20";
            return File(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv", "Report.csv");
        }
    }
}