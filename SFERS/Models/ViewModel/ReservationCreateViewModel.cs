using System.ComponentModel.DataAnnotations;

namespace SFERS.Models.ViewModel
{
    public class ReservationCreateViewModel
    {
         {
        [Required(ErrorMessage = "Please select a room.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Please select a date.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please enter a start time.")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Please enter an end time.")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Please provide the purpose.")]
        [StringLength(500)]
        public string? Purpose { get; set; }
    }
}


