using System.ComponentModel.DataAnnotations;
using SFERS.Models.Entities;

namespace SFERS.Models.ViewModel
{
    public class AdminEquipmentViewModel
    {
        [Required(ErrorMessage = "Equipment name is required.")]
        public string NewEquipmentName { get; set; } = string.Empty;
        public int NewEquipmentCategoryId { get; set; }
        public required List<EquipmentViewModel> Equipments { get; set; }
        public required List<EquipmentCategory> EquipmentCategories { get; set; }
        public required List<Room> Rooms { get; set; }
    }
}