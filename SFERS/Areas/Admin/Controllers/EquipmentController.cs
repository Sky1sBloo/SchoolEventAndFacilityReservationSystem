using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.ViewModel; // This fixes the missing reference

namespace SFERS.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class EquipmentController : Controller
    {
        private ApplicationDbContext dbContext;
        public EquipmentController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public async Task<IActionResult> Index()
        {
            var equipment = new AdminEquipmentViewModel()
            {
                EquipmentCategories = await dbContext.EquipmentCategories.ToListAsync(),
                Equipments = new List<EquipmentViewModel>(),
                Rooms = await dbContext.Rooms.ToListAsync()
            };

            equipment.Equipments = await dbContext.Equipments
                .Select(e => new EquipmentViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Category = e.Category != null ? e.Category.Name : "Uncategorized",
                    AssignedRoom = e.Room != null ? e.Room.Name : null,
                    Status = e.isAvailable ? "Available" : "In Use"
                }).ToListAsync();

            return View(equipment);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminEquipmentViewModel model)
        {
            dbContext.Equipments.Add(new Models.Entities.Equipment
            {
                Name = model.NewEquipmentName,
                CategoryId = model.NewEquipmentCategoryId,
                RoomId = null,
                isAvailable = true
            });
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // POST: /AdminEquipment/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> Update(int id, int? roomId, bool isAvailable)
        {
            var equipment = await dbContext.Equipments.FindAsync(id);
            if (equipment != null)
            {
                equipment.RoomId = string.IsNullOrEmpty(roomId.ToString()) ? null : roomId;
                equipment.isAvailable = isAvailable;
                await dbContext.SaveChangesAsync();
            }
            // TODO: Add logic to find item by ID and update status
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var equipment = dbContext.Equipments.Find(id);
            if (equipment != null)
            {
                dbContext.Equipments.Remove(equipment);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }   
    }
}