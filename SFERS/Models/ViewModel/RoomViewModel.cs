using System.Collections.Generic;

namespace SFERS.Models
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; } // Added ?
        public int Capacity { get; set; }
        public List<string>? Equipment { get; set; } // Added ?
        public bool IsAvailable { get; set; }
    }

    public class EquipmentViewModel
    {
        public string? Category { get; set; } // Added ?
        public string? Name { get; set; } // Added ?
        public string? AssignedRoom { get; set; } // Added ?
        public string? Status { get; set; } // Added ?
    }
}