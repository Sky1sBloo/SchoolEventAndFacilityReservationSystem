using System.ComponentModel.DataAnnotations;

namespace SFERS.Models.ViewModel
{
    // --- RESERVATION MODELS ---
    public class DashboardStatsViewModel
    {
        public int UpcomingReservations { get; set; }
        public int CompletedBookings { get; set; }
        public string? NextBookingTime { get; set; }
        public List<ReservationViewModel>? RecentActivity { get; set; }
    }

    public class ReservationViewModel
    {
        public int Id { get; set; }
        public string? RoomName { get; set; }
        public DateTime Date { get; set; }
        public string? TimeSlot { get; set; }
        public string? Purpose { get; set; }
        public string? Status { get; set; }
        public string? EquipmentRequested { get; set; }
    }
}
