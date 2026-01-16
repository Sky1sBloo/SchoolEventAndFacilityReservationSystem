using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.Entities;
using SFERS.Models.ViewModel;

namespace SFERS.Pages.Dashboard;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    // Current ongoing booking for the signed-in user (if any)
    public DashboardResConfViewModel? CurrentBooking { get; set; }

    // Counts for user
    public int CompletedBookingsCount { get; set; }
    public int UpcomingBookingsCount { get; set; }

    // Future bookings by other users (today and onward)
    public List<DashboardResConfViewModel> FutureBookingsByOthers { get; set; } = new();

    public async Task OnGetAsync()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(idClaim, out var userId))
        {
            // no user id — nothing to show
            return;
        }

        var now = DateTime.Now;
        var today = DateTime.Today;
        var timeNow = now.TimeOfDay;

        // Current (ongoing) booking for this user
        var current = await _db.Reservations
            .Include(r => r.Room)
            .Include(r => r.User)
            .Where(r => r.UserId == userId && r.Date == today && r.StartTime <= timeNow && r.EndTime > timeNow)
            .OrderBy(r => r.StartTime)
            .FirstOrDefaultAsync();

        if (current != null)
        {
            var equip = await _db.ReservationEquipments
                .Where(re => re.ReservationId == current.Id)
                .Select(re => re.Equipment.Name)
                .ToListAsync();

            CurrentBooking = new DashboardResConfViewModel
            {
                Id = current.Id,
                RoomName = current.Room?.Name ?? "Unknown",
                Date = current.Date,
                TimeSlot = $"{current.StartTime:hh\\:mm} - {current.EndTime:hh\\:mm}",
                Purpose = current.Purpose,
                Status = current.Status.ToString(),
                EquipmentNames = equip,
                UserName = current.User?.FullName ?? current.User?.Email
            };
        }

        // Completed bookings (ended before now)
        CompletedBookingsCount = await _db.Reservations
            .Where(r => r.UserId == userId &&
                        (r.Date < today || (r.Date == today && r.EndTime <= timeNow)))
            .CountAsync();

        // Upcoming bookings for this user (future or later today)
        UpcomingBookingsCount = await _db.Reservations
            .Where(r => r.UserId == userId &&
                        (r.Date > today || (r.Date == today && r.StartTime > timeNow)))
            .CountAsync();

        // Future bookings by other users (today and onward)
        var otherReservations = await _db.Reservations
            .Where(r => r.Date >= today && r.UserId != userId)
            .Include(r => r.Room)
            .Include(r => r.User)
            .OrderBy(r => r.Date)
            .ThenBy(r => r.StartTime)
            .Take(50)
            .ToListAsync();

        foreach (var r in otherReservations)
        {
            var equip = await _db.ReservationEquipments
                .Where(re => re.ReservationId == r.Id)
                .Select(re => re.Equipment.Name)
                .ToListAsync();

            FutureBookingsByOthers.Add(new DashboardResConfViewModel
            {
                Id = r.Id,
                UserName = r.User?.FullName ?? r.User?.Email ?? "Unknown",
                RoomName = r.Room?.Name ?? "Unknown",
                Date = r.Date,
                TimeSlot = $"{r.StartTime:hh\\:mm} - {r.EndTime:hh\\:mm}",
                Purpose = r.Purpose,
                Status = r.Status.ToString(),
                EquipmentNames = equip
            });
        }
    }
}
