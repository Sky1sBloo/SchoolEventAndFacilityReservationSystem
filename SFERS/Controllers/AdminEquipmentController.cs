using Microsoft.AspNetCore.Mvc;
using SFERS.Models.ViewModel; // This fixes the missing reference

namespace SFERS.Controllers
{
    public class AdminEquipmentController : Controller
    {
        // GET: /AdminEquipment/Index
        public IActionResult Index()
        {
            // Mock Data matching the table structure
            var equipment = new List<EquipmentViewModel>
            {
                new EquipmentViewModel { Id=101, Name="Epson Pro", Category="Projector", AssignedRoom="Auditorium", Status="Available" },
                new EquipmentViewModel { Id=102, Name="Dell Latitude", Category="Laptop", AssignedRoom="Lab 1", Status="In Use" }
            };
            return View(equipment);
        }

        // POST: /AdminEquipment/Create
        [HttpPost]
        public IActionResult Create(EquipmentViewModel model)
        {
            // Default status for new items if not set
            if (string.IsNullOrEmpty(model.Status))
            {
                model.Status = "Available";
            }

            // TODO: Add database save logic here

            return RedirectToAction("Index");
        }

        // POST: /AdminEquipment/UpdateStatus
        [HttpPost]
        public IActionResult UpdateStatus(int id, string newStatus)
        {
            // TODO: Add logic to find item by ID and update status
            return RedirectToAction("Index");
        }
    }
}