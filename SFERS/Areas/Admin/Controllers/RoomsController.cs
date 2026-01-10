using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.ViewModel;
using SFERS.Models.Entities;

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
            var roomViewModels = new AdminRoomViewModel();

            foreach (var room in rooms)
            {
                var equipment = await dbContext.Equipments.Where(e => e.RoomId == room.Id).ToListAsync();
                roomViewModels.Rooms.Add(new RoomViewModel
                {
                    Id = room.Id,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    IsAvailable = true,  // TODO: Add logic by checking reservation
                    Equipment = equipment.Select(e => e.Name).ToList()
                });
            }

            return View(roomViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminRoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            dbContext.Rooms.Add(new Room
            {
                Name = model.NewRoomName,
                Capacity = model.NewRoomCapacity
            });
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
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
        public async Task<IActionResult> Delete(int id)
        {
            var room = await dbContext.Rooms.FindAsync(id);
            if (room != null)
            {
                dbContext.Rooms.Remove(room);
                await dbContext.SaveChangesAsync();
            }
            // TODO: Remove room from DB
            return RedirectToAction("Index");
        }
    }
}