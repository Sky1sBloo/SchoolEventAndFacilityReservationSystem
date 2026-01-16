using SFERS.Models.Entities;

namespace SFERS.Models.ViewModel
{
    public class RoomUsageViewModel
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int UsageCount { get; set; }
    }

    public class EquipmentUsageViewModel
    {
        public string EquipmentName { get; set; } = string.Empty;
        public int UsageCount { get; set; }
    }

    public class AnalyticsViewModel
    {
        public Dictionary<int, RoomUsageViewModel> MonthlyReservationCounts { get; set; } = new Dictionary<int, RoomUsageViewModel>();
        public Dictionary<int, EquipmentUsageViewModel> EquipmentUsages { get; set; } = new Dictionary<int, EquipmentUsageViewModel>();
        public List<ReservationViewModel> RecentReservations { get; set; } = new List<ReservationViewModel>();
    }
}