using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.Entities;
using SFERS.Models.ViewModel;

namespace SFERS.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public ReservationsController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        // READ user reservations
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();
            if (!int.TryParse(userIdClaim.Value, out var userId)) return Challenge();

            var reservations = await dbContext.Reservations
                .Include(r => r.Room)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Date)
                .ThenBy(r => r.StartTime)
                .ToListAsync();

            var vm = reservations.Select(r =>
            {
                var equipment = dbContext.ReservationEquipments
                    .Where(re => re.ReservationId == r.Id)
                    .Select(re => re.Equipment != null ? re.Equipment.Name : "Unknown")
                    .ToList();
                return new ReservationViewModel
                {
                    Id = r.Id,
                    RoomName = r.Room?.Name ?? "Unassigned",
                    EquipmentNames = equipment,
                    Date = r.Date,
                    TimeSlot = $"{r.StartTime:hh\\:mm} - {r.EndTime:hh\\:mm}",
                    Purpose = r.Purpose,
                    Status = r.Status.ToString(),
                };
            }).ToList();

            return View(vm);
        }

        // CREATE (GET)
        public async Task<IActionResult> Create(int? roomId = null)
        {
            ViewData["Rooms"] = await dbContext.Rooms.OrderBy(r => r.Name).ToListAsync();
            ViewData["Equipments"] = await dbContext.Equipments.Where(e => e.isAvailable).OrderBy(e => e.Name).ToListAsync();

            var model = new ReservationCreateViewModel();
            if (roomId.HasValue) model.RoomId = roomId.Value;
            return View(model);
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationCreateViewModel? model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();
            if (!int.TryParse(userIdClaim.Value, out var userId)) return Challenge();

            ViewData["Rooms"] = await dbContext.Rooms.OrderBy(r => r.Name).ToListAsync();
            ViewData["Equipments"] = await dbContext.Equipments.Where(e => e.isAvailable).OrderBy(e => e.Name).ToListAsync();

            if (model == null || !ModelState.IsValid)
            {
                return View(model);
            }

            var room = await dbContext.Rooms.FindAsync(model.RoomId);
            if (room == null)
            {
                ModelState.AddModelError(nameof(model.RoomId), "Selected room does not exist.");
                return View(model);
            }

            if (model.EndTime <= model.StartTime)
            {
                ModelState.AddModelError(nameof(model.EndTime), "End time must be after start time.");
                return View(model);
            }

            var conflict = await dbContext.Reservations.AnyAsync(r =>
                r.RoomId == model.RoomId &&
                r.Date == model.Date &&
                !(r.EndTime <= model.StartTime || r.StartTime >= model.EndTime)); // overlap

            if (conflict)
            {
                ModelState.AddModelError(string.Empty, "The selected room is already booked for that time slot.");
                return View(model);
            }

            var reservation = new Reservation
            {
                RoomId = model.RoomId,
                Date = model.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Purpose = model.Purpose ?? string.Empty,
                Status = ReservationStatus.Pending,
                UserId = userId
            };

            await dbContext.Reservations.AddAsync(reservation);
            await dbContext.SaveChangesAsync();

            if (model.EquipmentIds is IEnumerable<int> equipmentIds && equipmentIds.Any())
            {
                var validEquipment = await dbContext.Equipments
                    .Where(e => equipmentIds.Contains(e.Id) && e.isAvailable)
                    .ToListAsync();

                foreach (var equipment in validEquipment)
                {
                    dbContext.ReservationEquipments.Add(new ReservationEquipment
                    {
                        ReservationId = reservation.Id,
                        EquipmentId = equipment.Id,
                        Reservation = reservation, // Set required navigation property
                        Equipment = equipment      // Set required navigation property to fix CS9035
                    });
                }
                await dbContext.SaveChangesAsync();
            }

            TempData["Message"] = "Reservation created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // DETAILS (user-owned)
        public async Task<IActionResult> Details(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();
            if (!int.TryParse(userIdClaim.Value, out var userId)) return Challenge();

            var reservation = await dbContext.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (reservation == null) return NotFound();

            var equipment = await dbContext.ReservationEquipments
                .Where(re => re.ReservationId == reservation.Id)
                .Select(re => re.Equipment != null ? re.Equipment.Name : "Unknown")
                .ToListAsync();

            var vm = new ReservationViewModel
            {
                Id = reservation.Id,
                RoomName = reservation.Room?.Name ?? "Unknown",
                Date = reservation.Date,
                EquipmentNames = equipment,
                TimeSlot = $"{reservation.StartTime:hh\\:mm} - {reservation.EndTime:hh\\:mm}",
                Purpose = reservation.Purpose,
                Status = reservation.Status.ToString(),
            };

            return View(vm);
        }

        // DELETE (POST) - cancel reservation (user-owned)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();
            if (!int.TryParse(userIdClaim.Value, out var userId)) return Challenge();

            var reservation = await dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (reservation == null) return NotFound();

            var related = dbContext.ReservationEquipments.Where(re => re.ReservationId == reservation.Id);
            dbContext.ReservationEquipments.RemoveRange(related);
            dbContext.Reservations.Remove(reservation);
            await dbContext.SaveChangesAsync();

            TempData["Message"] = "Reservation cancelled.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Calendar()
        {
            // keep existing placeholder calendar view if needed; admin handles actual calendar events
            return View();
        }
    }
}
