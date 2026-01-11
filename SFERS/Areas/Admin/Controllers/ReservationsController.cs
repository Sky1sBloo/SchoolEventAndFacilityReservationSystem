using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var reservations = dbContext.Reservations.ToList().OrderBy(r => r.Date).ThenBy(r => r.StartTime);

            // TODO: Fetch from Database
            var reservationsView = new List<AdminReservationViewModel>();
            foreach (var reservation in reservations)
            {
                var room = dbContext.Rooms.Find(reservation.RoomId);
                var status = reservation.Status.ToString();

                reservationsView.Add(new AdminReservationViewModel
                {
                    Id = reservation.Id,
                    RoomName = room != null ? room.Name : "Unknown",
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
    }
}
