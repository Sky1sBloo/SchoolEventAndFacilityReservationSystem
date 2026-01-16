using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.Entities;
using SFERS.Models.ViewModel;

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

                bool isEqipmentConflict = IsReservationEquipmentConflict(reservation, res, await dbContext.ReservationEquipments.ToListAsync());
                if (isEqipmentConflict || (reservation.RoomId != null && reservation.RoomId == res.RoomId))
                {
                    if (res.Status == ReservationStatus.Approved &&
                        IsReservationConflicts(reservation, res))
                    {
                        if (isEqipmentConflict)
                        {
                            throw new InvalidOperationException("Equipment conflicts with an existing approved reservation.");
                        }
                        else
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
            if (reservation1.EndTime <= reservation1.StartTime || reservation2.EndTime <= reservation2.StartTime)
            {
                throw new ArgumentException("End time must be after start time.");
            }
            return reservation1.StartTime < reservation2.EndTime && reservation2.StartTime < reservation1.EndTime;
        }

        public bool IsReservationEquipmentConflict(Reservation reservation1, Reservation reservation2, List<ReservationEquipment> reservationEquipments)
        {
            var reservedEquipment1 = reservationEquipments
                .Where(re => re.ReservationId == reservation1.Id)
                .Select(re => re.EquipmentId)
                .ToList();

            var reservedEquipment2 = reservationEquipments
                .Where(re => re.ReservationId == reservation2.Id)
                .Select(re => re.EquipmentId)
                .ToList();

            return reservedEquipment1.Intersect(reservedEquipment2).Any();
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

        public async Task<List<ReservationViewModel>> ConvertToViewModels(List<Reservation> reservations)
        {
            var reservationViewModels = new List<ReservationViewModel>();
            foreach (var reservation in reservations)
            {
                List<string> equipmentNames = await dbContext.ReservationEquipments
                    .Where(re => re.ReservationId == reservation.Id)
                    .Select(re => re.Equipment.Name)
                    .ToListAsync();

                reservationViewModels.Add(new ReservationViewModel
                {
                    Id = reservation.Id,
                    RoomName = reservation.Room != null ? reservation.Room.Name : "Unknown",

                    Date = reservation.Date,
                    TimeSlot = $"{reservation.StartTime:hh\\:mm} - {reservation.EndTime:hh\\:mm}",
                    Status = reservation.Status.ToString(),
                    Purpose = reservation.Purpose
                });
            }
            return reservationViewModels;
        }
    }
}