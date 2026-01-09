using Microsoft.AspNetCore.Mvc;
using SFERS.Models.ViewModel;

namespace SFERS.Controllers.Admin
{
    [Area("Admin")]
    public class RoomsController : Controller
    {
        // GET: /AdminRooms
        public IActionResult Index()
        {
            // TODO: Fetch from Database
            var rooms = new List<RoomViewModel>
            {
                new RoomViewModel { Id = 1, Name = "Auditorium", Capacity = 200, IsAvailable = true, Equipment = new List<string>{"Projector", "Sound System"} },
                new RoomViewModel { Id = 2, Name = "Lab 1", Capacity = 40, IsAvailable = false, Equipment = new List<string>{"Computers"} }
            };
            return View(rooms);
        }

        // POST: /AdminRooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: _context.Rooms.Add(model);
                // TODO: _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Index", model); // Return with errors
        }

        // POST: /AdminRooms/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, RoomViewModel model)
        {
            // TODO: Find room by Id and update properties
            // var room = _context.Rooms.Find(id);
            // room.Name = model.Name;
            // room.IsAvailable = model.IsAvailable;
            // _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: /AdminRooms/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            // TODO: Remove room from DB
            return RedirectToAction("Index");
        }
    }
}