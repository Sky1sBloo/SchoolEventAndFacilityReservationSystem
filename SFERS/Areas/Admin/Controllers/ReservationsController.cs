using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.Entities;
using SFERS.Models.ViewModel; 

namespace SFERS.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class ReservationsController : Controller
    {
        public ApplicationDbContext dbContext;
        public ReservationsController(ApplicationDbContext context)
        {
            dbContext = context;
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
            var reservation = dbContext.Reservations.Find(id);
            if (reservation != null)
            {
                reservation.Status = ReservationStatus.Approved;
                await dbContext.SaveChangesAsync();
                await LogReservation(reservation.Id);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Decline(int id)
        {
            var reservation = dbContext.Reservations.Find(id);
            if (reservation != null)
            {
                reservation.Status = ReservationStatus.Declined;
                await dbContext.SaveChangesAsync();
            }
            // TODO: Update reservation status to Declined
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
