﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EliteSportsAcademy.Migrations
{
    /// <inheritdoc />
    public partial class ClassModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstructorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstructorEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_classes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "classes");
        }
    }
}
