using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFERS.Models;

namespace SFERS.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        public IActionResult Index()
        {
            var reservations = new List<ReservationViewModel>
            {
                new ReservationViewModel { Id=1, RoomName = "Auditorium", Date = DateTime.Now, Status = "Ongoing", TimeSlot = "8:00 AM - 5:00 PM", Purpose="School Assembly" },
                new ReservationViewModel { Id=2, RoomName = "Laboratory", Date = DateTime.Now.AddDays(2), Status = "Approved", TimeSlot = "10:00 AM - 12:00 PM", Purpose="Chemistry Exam" },
                new ReservationViewModel { Id=3, RoomName = "Room 3", Date = DateTime.Now.AddDays(5), Status = "Pending", TimeSlot = "2:00 PM - 3:00 PM", Purpose="Faculty Meeting" }
            };
            return View(reservations);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(ReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // In a real app, save to DB here.
            TempData["Message"] = "Reservation created successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult Calendar()
        {
            // Pass reservation data to calendar view
            var reservations = new List<ReservationViewModel>
            {
                new ReservationViewModel { Id=1, RoomName = "Auditorium", Date = DateTime.Now, Status = "Ongoing", TimeSlot = "8:00 AM - 5:00 PM", Purpose="School Assembly" },
                new ReservationViewModel { Id=2, RoomName = "Laboratory", Date = DateTime.Now.AddDays(2), Status = "Approved", TimeSlot = "10:00 AM - 12:00 PM", Purpose="Chemistry Exam" },
                new ReservationViewModel { Id=3, RoomName = "Room 3", Date = DateTime.Now.AddDays(5), Status = "Pending", TimeSlot = "2:00 PM - 3:00 PM", Purpose="Faculty Meeting" }
            };
            return View(reservations);
        }
    }
}
