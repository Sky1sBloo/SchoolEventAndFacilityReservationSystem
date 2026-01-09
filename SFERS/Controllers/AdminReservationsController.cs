using Microsoft.AspNetCore.Mvc;
using SFERS.Models.ViewModel;

namespace SFERS.Controllers
{
    public class AdminReservationsController : Controller
    {
        // GET: /AdminReservations
        public IActionResult Index()
        {
            // TODO: Fetch from Database
            var reservations = new List<AdminReservationViewModel>
            {
                new AdminReservationViewModel
                {
                    Id = 1,
                    RoomName = "Auditorium",
                    Date = DateTime.Now.AddDays(1),
                    TimeSlot = "08:00 AM - 12:00 PM",
                    Purpose = "School Assembly",
                    Status = "Pending"
                },
                new AdminReservationViewModel
                {
                    Id = 2,
                    RoomName = "Laboratory",
                    Date = DateTime.Now.AddDays(3),
                    TimeSlot = "10:00 AM - 12:00 PM",
                    Purpose = "Chemistry Exam",
                    Status = "Pending"
                },
                new AdminReservationViewModel
                {
                    Id = 3,
                    RoomName = "Room 3",
                    Date = DateTime.Now.AddDays(5),
                    TimeSlot = "01:00 PM - 03:00 PM",
                    Purpose = "Faculty Meeting",
                    Status = "Approved"
                }
            };

            return View(reservations);
        }

        // POST: /AdminReservations/Approve/5
        [HttpPost]
        public IActionResult Approve(int id)
        {
            // TODO: Update reservation status to Approved
            return RedirectToAction("Index");
        }

        // POST: /AdminReservations/Decline/5
        [HttpPost]
        public IActionResult Decline(int id)
        {
            // TODO: Update reservation status to Declined
            return RedirectToAction("Index");
        }
    }
}
