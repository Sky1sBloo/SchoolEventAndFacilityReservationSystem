namespace SFERS.Models.ViewModel
{
    public class EquipmentViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public string? AssignedRoom { get; set; }

        public string Status { get; set; } = "Available";
    }
}