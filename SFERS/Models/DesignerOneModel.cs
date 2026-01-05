using System.ComponentModel.DataAnnotations;

namespace SFERS.Models
{
    // --- AUTHENTICATION MODELS ---
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
    }

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
