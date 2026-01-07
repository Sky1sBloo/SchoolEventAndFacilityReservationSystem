
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFERS.Models.ViewModel;

namespace SFERS.Controllers
{
    [Authorize]
    public class EquipmentController : Controller
    {
        public IActionResult Index()
        {
            var equipmentList = new List<EquipmentViewModel>
            {
                new EquipmentViewModel { Name = "Epson Pro Projector", Category = "Projector", AssignedRoom = "Auditorium", Status = "Available" },
                new EquipmentViewModel { Name = "Shure Wireless Mic", Category = "Microphone", AssignedRoom = "Room 3", Status = "In Use" },
                new EquipmentViewModel { Name = "Lenovo Thinkpad T1", Category = "Laptop", AssignedRoom = "Library Storage", Status = "Maintenance" },
                new EquipmentViewModel { Name = "JBL PartyBox", Category = "Audio", AssignedRoom = "Auditorium", Status = "Available" }
            };

            return View(equipmentList);
        }
    }
}
