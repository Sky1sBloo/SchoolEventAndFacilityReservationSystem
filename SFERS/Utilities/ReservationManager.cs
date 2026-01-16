using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.Entities;

namespace SFERS.Utilities
{
    public class ReservationManager
    {
        private ApplicationDbContext dbContext;
        public ReservationManager(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public async Task ApproveReservation(int reservationId)
        {
            var reservation = await dbContext.Reservations.FindAsync(reservationId);
            if (reservation == null)
            {
                throw new ArgumentException("Reservation not found.");
            }

            var equipment = await dbContext.ReservationEquipments
                .Where(re => re.ReservationId == reservationId)
                .ToListAsync();

            var reservationsSameDate = await dbContext.Reservations.Where(r => r.Date == reservation.Date && r.Id != reservationId).ToListAsync();
            foreach (var res in reservationsSameDate)
            {
                var reservedEquipment = await dbContext.ReservationEquipments
                    .Where(re => re.ReservationId == res.Id)
                    .Select(re => re.EquipmentId)
                    .ToListAsync();

                bool isEqipmentConflict = reservation.RoomId == null && reservedEquipment.Intersect(equipment.Select(e => e.EquipmentId)).Any();
                if (isEqipmentConflict || reservation.RoomId != null)
                {
                    if (res.Status == ReservationStatus.Approved &&
                        IsReservationConflicts(reservation, res))
                    {
                        if (isEqipmentConflict)
                        {
                            throw new InvalidOperationException("Equipment conflicts with an existing approved reservation.");
                        } else
                        {
                            throw new InvalidOperationException("Room conflicts with an existing approved reservation.");
                        }
                    }
                }
            }
            reservation.Status = ReservationStatus.Approved;
            await dbContext.SaveChangesAsync();
        }

        public async Task RejectReservation(int reservationId)
        {
            var reservation = await dbContext.Reservations.FindAsync(reservationId);
            if (reservation == null)
            {
                throw new ArgumentException("Reservation not found.");
            }
            reservation.Status = ReservationStatus.Declined;
            await dbContext.SaveChangesAsync();
        }

        public bool IsReservationConflicts(Reservation reservation1, Reservation reservation2)
        {
            if (reservation1.Date != reservation2.Date)
            {
                return false;
            }
            return reservation1.StartTime < reservation2.EndTime && reservation2.StartTime < reservation1.EndTime;
        }

        public async Task LogReservation(int reservationId)
        {
            var reservation = await dbContext.Reservations.FindAsync(reservationId);
            if (reservation != null)
            {
                var logEntry = new ReservationLog
                {
                    ReservationId = reservation.Id,
                    Timestamp = DateTime.UtcNow
                };
                dbContext.ReservationLogs.Add(logEntry);
                await dbContext.SaveChangesAsync();
            }
        }

    }
}