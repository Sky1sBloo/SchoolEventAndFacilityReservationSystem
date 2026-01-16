namespace SFERS.Models.Entities
{
    public class ReservationEquipment
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
        public int EquipmentId { get; set; }
        public Equipment? Equipment { get; set; }
    }
}