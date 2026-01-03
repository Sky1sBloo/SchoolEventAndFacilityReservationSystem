using System;
using System.ComponentModel.DataAnnotations;

namespace SFERS.Models.Entities;

public class ReservationForm
{
    [Required]
    public int RoomTypeId { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public TimeSpan StartTime { get; set; }
    [Required]
    public TimeSpan EndTime { get; set; }
    public required string Notes { get; set; }
    // UserId might be set from session
}