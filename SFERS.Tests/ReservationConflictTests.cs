using SFERS.Models.Entities;
using SFERS.Utilities;
using NUnit.Framework;

namespace SFERS.Tests;

public class ReservationConflictTests
{
    private ReservationManager reservationManager;

    [SetUp]
    public void Setup()
    {
        reservationManager = new ReservationManager(null);
    }

    #region Time Conflict Tests

    [Test]
    public void IsReservationConflicts_SameDateOverlappingTimes_ReturnsTrue()
    {
        var res1 = new Reservation
        {
            Id = 1,
            Date = new DateTime(2025, 1, 20),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            Purpose = "Meeting"
        };

        var res2 = new Reservation
        {
            Id = 2,
            Date = new DateTime(2025, 1, 20),
            StartTime = new TimeSpan(11, 0, 0),
            EndTime = new TimeSpan(13, 0, 0),
            Purpose = "Workshop"
        };

        bool result = ReservationManager.IsReservationConflicts(res1, res2);

        Assert.That(result);
    }

    [Test]
    public void IsReservationConflicts_DifferentDates_ReturnsFalse()
    {
        var res1 = new Reservation
        {
            Id = 1,
            Date = new DateTime(2025, 1, 20),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            Purpose = "Meeting"
        };

        var res2 = new Reservation
        {
            Id = 2,
            Date = new DateTime(2025, 1, 21),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            Purpose = "Workshop"
        };

        bool result = ReservationManager.IsReservationConflicts(res1, res2);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsReservationConflicts_SameDateNoOverlap_ReturnsFalse()
    {
        var res1 = new Reservation
        {
            Id = 1,
            Date = new DateTime(2025, 1, 20),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            Purpose = "Meeting"
        };

        var res2 = new Reservation
        {
            Id = 2,
            Date = new DateTime(2025, 1, 20),
            StartTime = new TimeSpan(13, 0, 0),
            EndTime = new TimeSpan(15, 0, 0),
            Purpose = "Workshop"
        };

        bool result = ReservationManager.IsReservationConflicts(res1, res2);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsReservationConflicts_BackToBackReservations_ReturnsFalse()
    {
        var res1 = new Reservation
        {
            Id = 1,
            Date = new DateTime(2025, 1, 20),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            Purpose = "Meeting"
        };

        var res2 = new Reservation
        {
            Id = 2,
            Date = new DateTime(2025, 1, 20),
            StartTime = new TimeSpan(12, 0, 0),
            EndTime = new TimeSpan(14, 0, 0),
            Purpose = "Workshop"
        };

        bool result = ReservationManager.IsReservationConflicts(res1, res2);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsReservationConflicts_CompleteOverlap_ReturnsTrue()
    {
        var res1 = new Reservation
        {
            Id = 1,
            Date = new DateTime(2025, 1, 20),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(14, 0, 0),
            Purpose = "Meeting"
        };

        var res2 = new Reservation
        {
            Id = 2,
            Date = new DateTime(2025, 1, 20),
            StartTime = new TimeSpan(11, 0, 0),
            EndTime = new TimeSpan(13, 0, 0),
            Purpose = "Workshop"
        };

        bool result = ReservationManager.IsReservationConflicts(res1, res2);

        Assert.That(result);
    }

    [Test]
    public void IsReservationConflicts_InvalidEndTime_ThrowsException()
    {
        var res1 = new Reservation
        {
            Id = 1,
            Date = new DateTime(2025, 1, 20),
            StartTime = new TimeSpan(12, 0, 0),
            EndTime = new TimeSpan(10, 0, 0),
            Purpose = "Meeting"
        };

        var res2 = new Reservation
        {
            Id = 2,
            Date = new DateTime(2025, 1, 20),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            Purpose = "Workshop"
        };

        Assert.Throws<ArgumentException>(() => ReservationManager.IsReservationConflicts(res1, res2));
    }

    #endregion

    #region Equipment Conflict Tests

    [Test]
    public void IsReservationEquipmentConflict_SameEquipment_ReturnsTrue()
    {
        var res1 = new Reservation { Id = 1, Purpose = "Meeting", Date = DateTime.Now, StartTime = TimeSpan.Zero, EndTime = TimeSpan.Zero };
        var res2 = new Reservation { Id = 2, Purpose = "Workshop", Date = DateTime.Now, StartTime = TimeSpan.Zero, EndTime = TimeSpan.Zero };

        var reservationEquipments = new List<ReservationEquipment>
        {
            new ReservationEquipment { ReservationId = 1, EquipmentId = 1 },
            new ReservationEquipment { ReservationId = 1, EquipmentId = 2 },
            new ReservationEquipment { ReservationId = 2, EquipmentId = 2 },
            new ReservationEquipment { ReservationId = 2, EquipmentId = 3 }
        };

        bool result = ReservationManager.IsReservationEquipmentConflict(res1, res2, reservationEquipments);

        Assert.That(result);
    }

    [Test]
    public void IsReservationEquipmentConflict_DifferentEquipment_ReturnsFalse()
    {
        var res1 = new Reservation { Id = 1, Purpose = "Meeting", Date = DateTime.Now, StartTime = TimeSpan.Zero, EndTime = TimeSpan.Zero };
        var res2 = new Reservation { Id = 2, Purpose = "Workshop", Date = DateTime.Now, StartTime = TimeSpan.Zero, EndTime = TimeSpan.Zero };

        var reservationEquipments = new List<ReservationEquipment>
        {
            new ReservationEquipment { ReservationId = 1, EquipmentId = 1 },
            new ReservationEquipment { ReservationId = 1, EquipmentId = 2 },
            new ReservationEquipment { ReservationId = 2, EquipmentId = 3 },
            new ReservationEquipment { ReservationId = 2, EquipmentId = 4 }
        };

        bool result = ReservationManager.IsReservationEquipmentConflict(res1, res2, reservationEquipments);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsReservationEquipmentConflict_EmptyEquipmentList_ReturnsFalse()
    {
        var res1 = new Reservation { Id = 1, Purpose = "Meeting", Date = DateTime.Now, StartTime = TimeSpan.Zero, EndTime = TimeSpan.Zero };
        var res2 = new Reservation { Id = 2, Purpose = "Workshop", Date = DateTime.Now, StartTime = TimeSpan.Zero, EndTime = TimeSpan.Zero };
        var reservationEquipments = new List<ReservationEquipment>();

        bool result = ReservationManager.IsReservationEquipmentConflict(res1, res2, reservationEquipments);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsReservationEquipmentConflict_MultipleConflicts_ReturnsTrue()
    {
        var res1 = new Reservation { Id = 1, Purpose = "Meeting", Date = DateTime.Now, StartTime = TimeSpan.Zero, EndTime = TimeSpan.Zero };
        var res2 = new Reservation { Id = 2, Purpose = "Workshop", Date = DateTime.Now, StartTime = TimeSpan.Zero, EndTime = TimeSpan.Zero };

        var reservationEquipments = new List<ReservationEquipment>
        {
            new ReservationEquipment { ReservationId = 1, EquipmentId = 1 },
            new ReservationEquipment { ReservationId = 1, EquipmentId = 2 },
            new ReservationEquipment { ReservationId = 2, EquipmentId = 1 },
            new ReservationEquipment { ReservationId = 2, EquipmentId = 2 }
        };

        bool result = ReservationManager.IsReservationEquipmentConflict(res1, res2, reservationEquipments);

        Assert.That(result);
    }

    #endregion
}
