namespace SFERS.Models.ViewModel
{
    public class ReservationLogViewModel
    {
        public int Id { get; set; }
        public required string Room { get; set; }
        public DateTime Timestamp { get; set; }
        public required string Duration { get; set; }
    }
    public class AdminAnalyticsViewModel
    {
        public List<ReservationLogViewModel> ReservationLogs { get; set; } = new List<ReservationLogViewModel>();
    }
}