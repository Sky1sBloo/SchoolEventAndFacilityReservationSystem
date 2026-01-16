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

        private async Task<AdminAnalyticsViewModel> BuildViewModel()
        {
            var model = new AdminAnalyticsViewModel();
            var reservationLogs = await dbContext.ReservationLogs.Include(log => log.Reservation)
                           .ThenInclude(r => r != null ? r.Room : null).Where(l => l.Reservation != null && l.Reservation.Room != null)
                           .OrderByDescending(r => r.Timestamp).ToListAsync();

            foreach (var log in reservationLogs)
            {
                var roomName = log.Reservation?.Room?.Name ?? "Unknown";
                var duration = log.Reservation != null ? log.Reservation.EndTime - log.Reservation.StartTime : TimeSpan.Zero;
                var equipmentList = await dbContext.ReservationEquipments.Where(r => r.ReservationId == log.ReservationId)
                    .Select(r => r.Equipment != null ? r.Equipment.Name : "Unknown").ToListAsync();
                model.ReservationLogs.Add(new ReservationLogViewModel
                {
                    Id = log.Id,
                    Room = roomName,
                    Timestamp = log.Timestamp,
                    Duration = $"{duration.Hours}h {duration.Minutes}m",
                    Equipment = equipmentList
                });
            }

            var reservationList = await dbContext.Reservations.Where(r => r.Status == Models.Entities.ReservationStatus.Approved).ToListAsync();
            model.ApprovedReservations = reservationList;
            return model;
        }

        public async Task<IActionResult> Index()
        {
            var model = await BuildViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertLog(AdminAnalyticsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = await BuildViewModel();
                return View("Index", viewModel);
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