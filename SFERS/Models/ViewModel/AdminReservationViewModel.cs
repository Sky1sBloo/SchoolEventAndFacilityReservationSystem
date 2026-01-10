namespace SFERS.Models.ViewModel
{
    public class AdminReservationViewModel
    {
        public int Id { get; set; }
        public required string RoomName { get; set; }
        public DateTime Date { get; set; }
        public string? TimeSlot { get; set; }
        public string? Purpose { get; set; }
        public required string Status { get; set; }
    }
}
