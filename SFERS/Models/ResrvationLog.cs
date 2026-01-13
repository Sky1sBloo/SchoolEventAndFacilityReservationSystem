namespace SFERS.Models.Entities
{
    public class ReservationLog
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
        public required DateTime Timestamp { get; set; }
    }
}