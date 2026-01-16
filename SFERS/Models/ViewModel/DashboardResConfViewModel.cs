using System;
using System.Collections.Generic;

namespace SFERS.Models.ViewModel
{
    public class DashboardResConfViewModel
    {
        public int Id { get; set; }
        public required string RoomName { get; set; }
        public List<string> EquipmentNames { get; set; } = new List<string>();
        public DateTime Date { get; set; }
        public string? TimeSlot { get; set; }
        public string? Purpose { get; set; }
        public required string Status { get; set; }

        // Added: who made the reservation (used when listing other users' bookings)
        public string? UserName { get; set; }
    }
}
