using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SFERS.Migrations
{
    /// <inheritdoc />
    public partial class Addedondeletesetnullonequipmentroomrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Rooms_RoomId",
                table: "Equipments");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Rooms_RoomId",
                table: "Equipments",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Rooms_RoomId",
                table: "Equipments");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Rooms_RoomId",
                table: "Equipments",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }
    }
}
