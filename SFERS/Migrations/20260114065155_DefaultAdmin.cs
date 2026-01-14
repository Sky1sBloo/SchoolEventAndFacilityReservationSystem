using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SFERS.Migrations
{
    /// <inheritdoc />
    public partial class DefaultAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "RoleId" },
                values: new object[] { 1, "admin@email.com", "Admin User", "$2a$12$TPUMpgjrrXIHFBCShb.AGesfyRVZzyRiFa134MS826NVP02MUAiay", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
