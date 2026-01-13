using System.ComponentModel.DataAnnotations;
using SFERS.Models.Entities;

namespace SFERS.Models.ViewModel
{
    public class ReservationLogViewModel
    {
        public int Id { get; set; }
        public required string Room { get; set; }
        public List<string> Equipment { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; }
        public required string Duration { get; set; }
    }
    public class AdminAnalyticsViewModel
    {
        public List<ReservationLogViewModel> ReservationLogs { get; set; } = new List<ReservationLogViewModel>();
        public List<Reservation> ApprovedReservations { get; set; } = new List<Reservation>();
        [Required(ErrorMessage = "Please select a reservation.")]
        public int? SelectedReservation { get; set; }
        [Required(ErrorMessage = "Please select a date.")]
        public DateTime? SelectedDate { get; set; }
    }
}