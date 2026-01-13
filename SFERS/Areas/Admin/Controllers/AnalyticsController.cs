using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
            var model = new AdminAnalyticsViewModel();
            var reservationLogs = dbContext.ReservationLogs.Include(log => log.Reservation)
                .ThenInclude(r => r.Room).Where(l => l.Reservation != null && l.Reservation.Room != null)
                .OrderByDescending(r => r.Timestamp).ToList();
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

            var reservationList = dbContext.Reservations.Where(r => r.Status == Models.Entities.ReservationStatus.Approved).ToList();
            model.ApprovedReservations = reservationList;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertLog(AdminAnalyticsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var reservationLogs = dbContext.ReservationLogs.Include(log => log.Reservation)
                    .ThenInclude(r => r.Room).Where(l => l.Reservation != null && l.Reservation.Room != null)
                    .OrderByDescending(r => r.Timestamp).ToList();
                
                model.ReservationLogs.Clear();
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
                
                model.ApprovedReservations = dbContext.Reservations
                    .Where(r => r.Status == Models.Entities.ReservationStatus.Approved)
                    .Include(r => r.Room)
                    .ToList();
                
                return View("Index", model);
            }

            var reservation = await dbContext.Reservations.FindAsync(model.SelectedReservation);
            if (reservation == null)
            {
                ModelState.AddModelError("", "Reservation not found");
                return View("Index", model);
            }

            var newLog = new Models.Entities.ReservationLog
            {
                ReservationId = reservation.Id,
                Timestamp = DateTime.Now
            };

            dbContext.ReservationLogs.Add(newLog);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public IActionResult ExportReport()
        {
            // Simple mock export logic
            var csvContent = "Report Data, Value\nRooms, 5\nEquipment, 20";
            return File(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv", "Report.csv");
        }

        public async Task<IActionResult> DeleteLog(int id)
        {
            var log = await dbContext.ReservationLogs.FindAsync(id);
            if (log != null)
            {
                dbContext.ReservationLogs.Remove(log);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}