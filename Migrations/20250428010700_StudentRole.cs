using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EliteSportsAcademy.Migrations
{
    /// <inheritdoc />
    public partial class StudentRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Student", "STUDENT" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "User", "USER" });
        }
    }
}
