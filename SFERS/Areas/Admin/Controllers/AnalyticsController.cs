using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.ViewModel;

namespace SFERS.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AnalyticsController : Controller
    {
        private ApplicationDbContext dbContext;
        public AnalyticsController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            // We create the model and fill it with data
            var model = new AdminAnalyticsViewModel();
            var reservationLogs = dbContext.ReservationLogs.Include(log => log.Reservation).ThenInclude(r => r.Room).OrderByDescending(r => r.Timestamp).ToList();
            foreach (var log in reservationLogs)
            {
                var roomName = log.Reservation?.Room?.Name ?? "Unknown";
                var duration = log.Reservation != null ? log.Reservation.EndTime - log.Reservation.StartTime : TimeSpan.Zero;
                model.ReservationLogs.Add(new ReservationLogViewModel
                {
                    Id = log.Id,
                    Room = roomName,
                    Timestamp = log.Timestamp,
                    Duration = $"{duration.Hours}h {duration.Minutes}m"
                });
            }

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