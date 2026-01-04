using Microsoft.AspNetCore.Mvc;
using SFERS.Models; 
using System.Collections.Generic;
using System.Linq;

namespace SFERS.Controllers
{
    public class RoomsController : Controller
    {
        // GET: Rooms
        public IActionResult Index()
        {
            // MOCK DATA: Simulating database records
            var rooms = new List<RoomViewModel>
            {
                new RoomViewModel { Id = 1, Name = "Auditorium", Capacity = 200, IsAvailable = true, Equipment = new List<string> { "Projector", "Sound System" } },
                new RoomViewModel { Id = 2, Name = "Laboratory", Capacity = 40, IsAvailable = true, Equipment = new List<string> { "Computers", "Whiteboard" } },
                new RoomViewModel { Id = 3, Name = "Room 3", Capacity = 30, IsAvailable = false, Equipment = new List<string> { "TV" } },
                new RoomViewModel { Id = 4, Name = "Room 4", Capacity = 30, IsAvailable = true, Equipment = new List<string> { "Whiteboard" } },
                new RoomViewModel { Id = 5, Name = "Room 5", Capacity = 30, IsAvailable = true, Equipment = new List<string> { "Projector" } }
            };

            // TODO: DATABASE CHANGE
            // var rooms = _context.Rooms.ToList();

            return View(rooms);
        }

        // GET: Rooms/Details/5
        public IActionResult Details(int id)
        {
            // MOCK DATA: Find room by ID
            var room = new RoomViewModel
            {
                Id = id,
                Name = "Auditorium",
                Capacity = 200,
                IsAvailable = true,
                Equipment = new List<string> { "Projector 4K", "Surround Sound" }
            };

            // TODO: DATABASE CHANGE
            // var room = _context.Rooms.FirstOrDefault(r => r.Id == id);

            if (room == null) return NotFound();

            return PartialView("_RoomDetailsModal", room); // Optional: if using partial views for modals
        }
    }
}