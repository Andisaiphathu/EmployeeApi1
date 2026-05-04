using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "PASTE_HASH_HERE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserAccounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "+di5Ao45B+jcam99mN3A2zP4JYEyn2UeXI2a0qxjDxQfLqX0CoCDds9kporuzf9H");
        }
    }
}
