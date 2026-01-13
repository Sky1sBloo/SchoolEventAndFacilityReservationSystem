namespace SFERS.Models.Entities
{
    public class ReservationEquipment
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public required Reservation Reservation { get; set; }
        public int EquipmentId { get; set; }
        public required Equipment Equipment { get; set; }
    }
}