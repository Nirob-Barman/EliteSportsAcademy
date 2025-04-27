using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EliteSportsAcademy.Migrations
{
    /// <inheritdoc />
    public partial class ClassPropery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "classes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "classes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "classes");
        }
    }
}
