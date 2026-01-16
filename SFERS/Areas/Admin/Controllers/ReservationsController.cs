using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.Entities;
using SFERS.Models.ViewModel;
using SFERS.Utilities;

namespace SFERS.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class ReservationsController : Controller
    {
        private ApplicationDbContext dbContext;
        private ReservationManager reservationManager;
        public ReservationsController(ApplicationDbContext context, ReservationManager reservationMgr)
        {
            dbContext = context;
            reservationManager = reservationMgr;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await dbContext.Reservations
                .OrderBy(r => r.Status == ReservationStatus.Approved)
                .ThenBy(r => r.Date)
                .ThenBy(r => r.StartTime)
                .ToListAsync();

            // TODO: Fetch from Database
            var reservationsView = new List<AdminReservationViewModel>();
            foreach (var reservation in reservations)
            {
                var room = dbContext.Rooms.Find(reservation.RoomId);
                var status = reservation.Status.ToString();
                List<string> equipmentNames = await dbContext.ReservationEquipments
                    .Where(re => re.ReservationId == reservation.Id)
                    .Select(re => re.Equipment.Name)
                    .ToListAsync();

                reservationsView.Add(new AdminReservationViewModel
                {
                    Id = reservation.Id,
                    RoomName = room != null ? room.Name : "Unknown",
                    EquipmentNames = equipmentNames,
                    Date = reservation.Date,
                    TimeSlot = $"{reservation.StartTime:hh\\:mm} - {reservation.EndTime:hh\\:mm}",
                    Purpose = reservation.Purpose,
                    Status = status
                });
            }

            return View(reservationsView);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await reservationManager.ApproveReservation(id);
                await LogReservation(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Decline(int id)
        {
            try {
                await reservationManager.RejectReservation(id);
            } catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private async Task LogReservation(int reservationId)
        {
            var log = new ReservationLog
            {
                ReservationId = reservationId,
                Timestamp = DateTime.Now
            };
            dbContext.ReservationLogs.Add(log);
            await dbContext.SaveChangesAsync();
        }
    }
}
