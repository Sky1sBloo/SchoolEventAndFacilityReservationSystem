using System.ComponentModel.DataAnnotations;
using SFERS.Models.Entities;

namespace SFERS.Models.ViewModel
{
    public class AdminRoomViewModel
    {
        [Required(ErrorMessage = "Room name is required.")]
        public string NewRoomName { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
        public int NewRoomCapacity { get; set; }
        public List<RoomViewModel> Rooms { get; set; } = new List<RoomViewModel>();
    }
}