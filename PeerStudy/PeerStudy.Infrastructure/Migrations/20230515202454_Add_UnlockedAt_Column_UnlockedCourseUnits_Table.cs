using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Add_UnlockedAt_Column_UnlockedCourseUnits_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UnlockedAt",
                table: "UnlockedCourseUnits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnlockedAt",
                table: "UnlockedCourseUnits");
        }
    }
}
