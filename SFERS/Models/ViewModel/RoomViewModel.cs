//using System.Collections.Generic;
namespace SFERS.Models.ViewModel
{
    public class RoomViewModel
    {
        public int Id { get; set; }

        // Fix: Initialize Name
        public string Name { get; set; } = string.Empty;

        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public List<string> Reservations { get; set; } = new List<string>();

        // Fix: Initialize the list so it is never null
        public List<string> Equipment { get; set; } = new List<string>();
    }
}