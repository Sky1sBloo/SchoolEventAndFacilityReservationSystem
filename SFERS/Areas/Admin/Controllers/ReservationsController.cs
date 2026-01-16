using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.Entities;
using SFERS.Utilities;

namespace SFERS.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class ReservationsController : Controller
    {
        private ApplicationDbContext dbContext;
        private ReservationManager reservationManager;
        public ReservationsController(ApplicationDbContext context, ReservationManager reservationMgr)
        {
            dbContext = context;
            reservationManager = reservationMgr;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await dbContext.Reservations.Include(r => r.Room)
                .OrderBy(r => r.Status == ReservationStatus.Approved)
                .ThenBy(r => r.Date)
                .ThenBy(r => r.StartTime)
                .ToListAsync();

            var reservationsView = await reservationManager.ConvertToViewModels(reservations);

            return View(reservationsView);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await reservationManager.ApproveReservation(id);
                await LogReservation(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Decline(int id)
        {
            try {
                await reservationManager.RejectReservation(id);
            } catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private async Task LogReservation(int reservationId)
        {
            var log = new ReservationLog
            {
                ReservationId = reservationId,
                Timestamp = DateTime.Now
            };
            dbContext.ReservationLogs.Add(log);
            await dbContext.SaveChangesAsync();
        }
    }
}
