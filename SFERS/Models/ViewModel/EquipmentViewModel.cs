namespace SFERS.Models.ViewModel
{
    public class EquipmentViewModel
    {
        public int Id { get; set; }

        // Fix: Initialize strings to empty
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        // Fix: Allow AssignedRoom to be null (nullable string?) because an item might be in storage
        public string? AssignedRoom { get; set; }

        public string Status { get; set; } = "Available";

        // Fix: Initialize Condition
        public string Condition { get; set; } = "Good";
    }
}