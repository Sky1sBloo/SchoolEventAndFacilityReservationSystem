using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFERS.Data;
using SFERS.Models.ViewModel;

namespace SFERS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public DashboardController(ApplicationDbContext context)
        {
            dbContext = context;
        }
        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();
            if (!int.TryParse(userIdClaim.Value, out var userId)) return Challenge();

            var unapprovedCount = dbContext.Reservations
                .Count(r => r.Status == Models.Entities.ReservationStatus.Pending && r.Date >= DateTime.Today && r.UserId == userId);
            var approvedCount = dbContext.Reservations
                .Count(r => r.Status == Models.Entities.ReservationStatus.Approved && r.Date >= DateTime.Today && r.UserId == userId);

            var userReservations = dbContext.Reservations
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Date)
                .Take(5)
                .Select(r => new ReservationViewModel
                {
                    RoomName = r.Room != null ? r.Room.Name : "Unassigned",
                    Date = r.Date,
                    Status = r.Status.ToString(),
                    TimeSlot = $"{r.StartTime:hh\\:mm} - {r.EndTime:hh\\:mm}"
                })
                .ToList();

            var nextbooking = dbContext.Reservations
                .Where(r => r.UserId == userId && r.Status == Models.Entities.ReservationStatus.Approved && r.Date >= DateTime.Today)
                .OrderBy(r => r.Date)
                .ThenBy(r => r.StartTime)
                .FirstOrDefault();
            string nextBookingTime = "No upcoming bookings";
            if (nextbooking != null)
            {
                nextBookingTime = $"{nextbooking.Date:d} at {nextbooking.StartTime:hh\\:mm}";
            }
            var stats = new DashboardStatsViewModel
            {
                UpcomingReservations = unapprovedCount,
                CompletedBookings = approvedCount,
                NextBookingTime = nextBookingTime,
                RecentActivity = userReservations
            };
            return View(stats);
        }
    }
}
