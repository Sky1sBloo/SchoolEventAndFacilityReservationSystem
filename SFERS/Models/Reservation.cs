using System;
using System.Collections.Generic;

namespace SFERS.Models.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public required RoomType RoomType { get; set; }
    public required DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int UserId { get; set; }
    public required Account User { get; set; }
    // Add other fields like Status, Notes, etc.
}