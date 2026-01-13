using System;
using System.Collections.Generic;

namespace SFERS.Models.Entities;

public enum ReservationStatus
{
    Pending,
    Approved,
    Declined
}
public class Reservation
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public Room? Room { get; set; }
    public required DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public required string Purpose { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public int UserId { get; set; }
    public Account? User { get; set; }
}