using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFERS.Models;
using System.Collections.Generic;

namespace SFERS.Controllers
{
    [Authorize]
    public class RoomsController : Controller
    {
        public IActionResult Index()
        {
            var rooms = new List<RoomViewModel>
            {
                new RoomViewModel { Id = 1, Name = "Auditorium", Capacity = 200, IsAvailable = true, Equipment = new List<string> { "Epson Pro Projector", "JBL PartyBox" } },
                new RoomViewModel { Id = 2, Name = "Laboratory", Capacity = 40, IsAvailable = true, Equipment = new List<string> { "Lenovo Thinkpad T1", "Whiteboard" } },
                new RoomViewModel { Id = 3, Name = "Room 3", Capacity = 30, IsAvailable = false, Equipment = new List<string> { "TV", "Shure Wireless Mic" } },
                new RoomViewModel { Id = 4, Name = "Room 4", Capacity = 30, IsAvailable = true, Equipment = new List<string> { "Whiteboard" } },
                new RoomViewModel { Id = 5, Name = "Room 5", Capacity = 30, IsAvailable = true, Equipment = new List<string> { "Epson Pro Projector" } }
            };

            return View(rooms);
        }

        public IActionResult Details(int id)
        {
            var room = new RoomViewModel
            {
                Id = id,
                Name = "Auditorium",
                Capacity = 200,
                IsAvailable = true,
                Equipment = new List<string> { "Epson Pro Projector", "JBL PartyBox" }
            };

            if (room == null) return NotFound();

            return PartialView("_RoomDetailsModal", room);
        }
    }
}
