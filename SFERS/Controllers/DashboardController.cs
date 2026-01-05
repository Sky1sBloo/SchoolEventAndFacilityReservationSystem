using Microsoft.AspNetCore.Mvc;
using SFERS.Models;

namespace SFERS.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IsLoggedIn")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Mock Data for the Dashboard - Updated room names to match Designer 2
            var stats = new DashboardStatsViewModel
            {
                UpcomingReservations = 3,
                CompletedBookings = 12,
                NextBookingTime = "Tomorrow, 9:00 AM",
                RecentActivity = new List<ReservationViewModel>
                {
                    new ReservationViewModel { RoomName = "Laboratory", Date = DateTime.Now.AddDays(1), Status = "Approved", TimeSlot = "9:00 AM - 11:00 AM" },
                    new ReservationViewModel { RoomName = "Room 3", Date = DateTime.Now.AddDays(3), Status = "Pending", TimeSlot = "1:00 PM - 3:00 PM" }
                }
            };
            return View(stats);
        }
    }
}
