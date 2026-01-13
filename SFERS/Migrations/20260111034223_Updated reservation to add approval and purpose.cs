using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SFERS.Migrations
{
    /// <inheritdoc />
    public partial class Updatedreservationtoaddapprovalandpurpose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "Reservations");
        }
    }
}
