using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.ViewModel;
using SFERS.Utilities;

namespace SFERS.Controllers
{
    [Authorize]
    public class AnalyticsController : Controller
    {
        private ApplicationDbContext dbContext;
        private ReservationManager reservationManager;
        public AnalyticsController(ApplicationDbContext context, ReservationManager reservationMgr)
        {
            dbContext = context;
            reservationManager = reservationMgr;
        }
        public async Task<IActionResult> Index()
        {
            var model = await CreateAnalyticsViewModel();
            return View(model);
        }

        private async Task<AnalyticsViewModel> CreateAnalyticsViewModel()
        {
            var model = new AnalyticsViewModel();
            var reservations = await dbContext.Reservations
                .Include(r => r.Room)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
            var equipmentUsages = await dbContext.ReservationEquipments
                .Include(re => re.Equipment)
                .ToListAsync();
            var weekReservations = reservations
                .Where(r => r.Date >= DateTime.Now.AddDays(-7))
                .ToList();
            
            model.RecentReservations = await reservationManager.ConvertToViewModels(weekReservations);

            foreach (var res in reservations)
            {
                if (res.RoomId != null)
                {
                    if (!model.MonthlyReservationCounts.ContainsKey(res.RoomId.Value))
                    {
                        model.MonthlyReservationCounts[res.RoomId.Value] = new RoomUsageViewModel
                        {
                            Id = res.RoomId.Value,
                            RoomName = res.Room != null ? res.Room.Name : "Unknown",
                            UsageCount = 0
                        };
                    }
                    model.MonthlyReservationCounts[res.RoomId.Value].UsageCount++;
                }
            }
            foreach (var usage in equipmentUsages)
            {
                if (!model.EquipmentUsages.ContainsKey(usage.EquipmentId))
                {
                    model.EquipmentUsages[usage.EquipmentId] = new EquipmentUsageViewModel
                    {
                        EquipmentName = usage.Equipment != null ? usage.Equipment.Name : "Unknown",
                        UsageCount = 0
                    };
                }
                model.EquipmentUsages[usage.EquipmentId].UsageCount++;
            }

            return model;
        }

        [HttpGet]
        public IActionResult GetUsageData()
        {
            var data = new
            {
                labels = new[] { "Auditorium", "Laboratory", "Room 3", "Room 4", "Room 5" },
                values = new[] { 120, 90, 45, 30, 60 }
            };
            return Json(data);
        }
    }
}
