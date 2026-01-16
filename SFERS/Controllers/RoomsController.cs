using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models;
using SFERS.Models.Entities;
using SFERS.Models.ViewModel;
using SFERS.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFERS.Controllers
{
    [Authorize]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ReservationManager reservationManager;

        public RoomsController(ApplicationDbContext context, ReservationManager reservationMgr)
        {
            dbContext = context;
            reservationManager = reservationMgr;
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
            var timeNow = System.DateTime.Now.TimeOfDay;

            // Current (ongoing) reservation for this room (today where start <= now < end)
            var currentReservationEntity = await dbContext.Reservations
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RoomId == id && r.Date == today && r.StartTime <= timeNow && r.EndTime > timeNow);

            ReservationViewModel? currentVm = null;
            if (currentReservationEntity != null)
            {
                currentVm = await reservationManager.ConvertToViewModel(currentReservationEntity);
                /*
                var currentEquip = await dbContext.ReservationEquipments
                    .Where(re => re.ReservationId == currentReservationEntity.Id)
                    .Select(re => re.Equipment.Name)
                    .ToListAsync();
                
                currentVm = new ReservationViewModel
                {
                    Id = currentReservationEntity.Id,
                    RoomName = room.Name,
                    Date = currentReservationEntity.Date,
                    TimeSlot = $"{currentReservationEntity.StartTime:hh\\:mm} - {currentReservationEntity.EndTime:hh\\:mm}",
                    Purpose = currentReservationEntity.Purpose,
                    Status = currentReservationEntity.Status.ToString(),
                    EquipmentRequested = currentEquip.Count > 0 ? string.Join(", ", currentEquip) : null
                }; */
            }

            // Upcoming reservations: today (but after now) and future days
            var upcomingReservationsEntities = await dbContext.Reservations
                .Where(r => r.RoomId == id && (r.Date > today || (r.Date == today && r.EndTime > timeNow)))
                .OrderBy(r => r.Date)
                .ThenBy(r => r.StartTime)
                .ToListAsync();

            var reservationViewModels = new List<ReservationViewModel>();
            foreach (var r in upcomingReservationsEntities)
            {
                // skip the current reservation if it somehow matches (shouldn't if filter is correct)
                if (currentReservationEntity != null && r.Id == currentReservationEntity.Id)
                    continue;

                var reqEquip = await dbContext.ReservationEquipments
                    .Where(re => re.ReservationId == r.Id)
                    .Select(re => re.Equipment != null ? re.Equipment.Name : "Unknown")
                    .ToListAsync();

                reservationViewModels.Add(new ReservationViewModel
                {
                    Id = r.Id,
                    RoomName = room.Name,
                    Date = r.Date,
                    EquipmentNames = reqEquip,
                    TimeSlot = $"{r.StartTime:hh\\:mm} - {r.EndTime:hh\\:mm}",
                    Purpose = r.Purpose,
                    Status = r.Status.ToString(),
                });
            }

            var vm = new RoomDetailsViewModel
            {
                Room = new RoomViewModel
                {
                    Id = room.Id,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    IsAvailable = currentReservationEntity == null, // available if no current reservation
                    Equipment = equipments
                },
                UpcomingReservations = reservationViewModels,
                CurrentReservation = currentVm
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

        [HttpGet("api/rooms/{roomId}/reservations")]
        public async Task<IActionResult> GetRoomReservations(int roomId)
        {
            var reservations = await dbContext.Reservations
                .Where(r => r.RoomId == roomId && r.Date >= DateTime.Today)
                .OrderByDescending(r => r.Date)
                .Select(r => new
                {
                    r.Date,
                    startTime = r.StartTime.ToString(@"hh\:mm"),
                    endTime = r.EndTime.ToString(@"hh\:mm"),
                    r.Purpose,
                    status = r.Status.ToString()
                })
                .ToListAsync();

            return Json(reservations);
        }
    }

    internal class Rooms : Room
    {
        public new int Capacity { get; set; }
    }
}
