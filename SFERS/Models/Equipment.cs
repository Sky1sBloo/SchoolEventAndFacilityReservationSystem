namespace SFERS.Models.Entities
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public required int CategoryId { get; set; }
        public int? RoomId { get; set; }
        public required bool isAvailable { get; set; } = false;
        public EquipmentCategory? Category { get; set; }
        public Room? Room { get; set; }
    }
}