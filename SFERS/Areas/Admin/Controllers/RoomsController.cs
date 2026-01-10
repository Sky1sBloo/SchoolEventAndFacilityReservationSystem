using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.ViewModel;

namespace SFERS.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class RoomsController : Controller
    {
        private ApplicationDbContext dbContext;

        public RoomsController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        // GET: /AdminRooms
        public async Task<IActionResult> Index()
        {
            var rooms = await dbContext.Rooms.ToListAsync();
            var roomViewModels = new List<RoomViewModel>();

            // TODO: Fetch from Database
            foreach (var room in rooms)
            {
                var equipment = await dbContext.Equipments.Where(e => e.RoomId == room.Id).ToListAsync();
                roomViewModels.Add(new RoomViewModel
                {
                    Id = room.Id,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    IsAvailable = true,  // TODO: Add logic by checking reservation
                    Equipment = equipment.Select(e => e.Name).ToList()
                });
            }
            /*
            var rooms = new List<RoomViewModel>
            {
                new RoomViewModel { Id = 1, Name = "Auditorium", Capacity = 200, IsAvailable = true, Equipment = new List<string>{"Projector", "Sound System"} },
                new RoomViewModel { Id = 2, Name = "Lab 1", Capacity = 40, IsAvailable = false, Equipment = new List<string>{"Computers"} }
            }; */
            return View(roomViewModels);
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