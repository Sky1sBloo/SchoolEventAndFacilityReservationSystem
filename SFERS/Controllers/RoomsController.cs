using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models;
using SFERS.Models.Entities;
using SFERS.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFERS.Controllers
{
    [Authorize]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public RoomsController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await dbContext.Rooms
                .OrderBy(r => r.Name)
                .ToListAsync();

            var model = rooms.Select(r => new RoomViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                IsAvailable = true, // keep simple; availability can be refined
                Equipment = dbContext.Equipments.Where(e => e.RoomId == r.Id).Select(e => e.Name).ToList()
            }).ToList();

            return View(model);
        }

        // Return partial that includes upcoming reservations (today and onward) for a specific room
        public async Task<IActionResult> Details(int id)
        {
            var room = await dbContext.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            var equipments = await dbContext.Equipments
                .Where(e => e.RoomId == id)
                .Select(e => e.Name)
                .ToListAsync();

            var today = DateTime.Today;
            var reservations = await dbContext.Reservations
                .Where(r => r.RoomId == id && r.Date >= today)
                .OrderBy(r => r.Date)
                .ThenBy(r => r.StartTime)
                .ToListAsync();

            var reservationViewModels = new List<ReservationViewModel>();
            foreach (var r in reservations)
            {
                var reqEquip = await dbContext.ReservationEquipments
                    .Where(re => re.ReservationId == r.Id)
                    .Select(re => re.Equipment.Name)
                    .ToListAsync();

                reservationViewModels.Add(new ReservationViewModel
                {
                    Id = r.Id,
                    RoomName = room.Name,
                    Date = r.Date,
                    TimeSlot = $"{r.StartTime:hh\\:mm} - {r.EndTime:hh\\:mm}",
                    Purpose = r.Purpose,
                    Status = r.Status.ToString(),
                    EquipmentRequested = reqEquip.Count > 0 ? string.Join(", ", reqEquip) : null
                });
            }

            var vm = new RoomDetailsViewModel
            {
                Room = new RoomViewModel
                {
                    Id = room.Id,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    IsAvailable = true,
                    Equipment = equipments
                },
                UpcomingReservations = reservationViewModels
            };

            return PartialView("_RoomDetailsModal", vm);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                var room = new Rooms
                {
                    Name = model.Name,
                    Capacity = model.Capacity
                };

                dbContext.Rooms.Add(room);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var room = await dbContext.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            var model = new RoomViewModel
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity
            };

            return View(model);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var room = await dbContext.Rooms.FindAsync(id);
                if (room == null) return NotFound();

                room.Name = model.Name;
                room.Capacity = model.Capacity;

                dbContext.Rooms.Update(room);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var room = await dbContext.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            var model = new RoomViewModel
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity
            };

            return View(model);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await dbContext.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            dbContext.Rooms.Remove(room);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Reserve
        public IActionResult Reserve()
        {
            return View();
        }

        // POST: Rooms/Reserve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(ReservationCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var reservation = new Reservation
                {
                    RoomId = model.RoomId,
                    Date = model.Date,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Purpose = model.Purpose ?? string.Empty,
                    Status = ReservationStatus.Pending // default status
                };

                dbContext.Reservations.Add(reservation);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }

    internal class Rooms : Room
    {
        public new int Capacity { get; set; }
    }
}
