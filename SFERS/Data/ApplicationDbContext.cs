using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using SFERS.Models.Entities;
using SFERS.Utilities;

namespace SFERS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Administrator with full access" },
                new Role { Id = 2, Name = "User", Description = "Regular user with limited access" }
            );
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    FullName = "Admin User",
                    Email = "admin@email.com",
                    Password = PasswordManager.HashPassword("Admin@123"),
                    RoleId = 1,
                    Role = null!
                }
            );
            modelBuilder.Entity<EquipmentCategory>().HasData(
                new EquipmentCategory { Id = 1, Name = "Audio-Visual" },
                new EquipmentCategory { Id = 2, Name = "Computing" },
                new EquipmentCategory { Id = 3, Name = "Furniture" }
            );
            modelBuilder.Entity<Equipment>().HasOne(e => e.Room)
                .WithMany().HasForeignKey(e => e.RoomId).OnDelete(DeleteBehavior.SetNull);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentCategory> EquipmentCategories { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationEquipment> ReservationEquipments { get; set; }
        public DbSet<ReservationLog> ReservationLogs { get; set; }
    }
}