using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EliteSportsAcademy.Migrations
{
    /// <inheritdoc />
    public partial class IsAgreedToTerms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAgree",
                table: "AspNetUsers",
                newName: "IsAgreedToTerms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAgreedToTerms",
                table: "AspNetUsers",
                newName: "IsAgree");
        }
    }
}
