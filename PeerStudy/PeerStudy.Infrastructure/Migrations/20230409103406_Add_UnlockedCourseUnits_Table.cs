using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Add_UnlockedCourseUnits_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "CourseUnits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NoPointsToUnlock",
                table: "CourseUnits",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UnlockedCourseUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnlockedCourseUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnlockedCourseUnits_CourseUnits_CourseUnitId",
                        column: x => x.CourseUnitId,
                        principalTable: "CourseUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnlockedCourseUnits_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnlockedCourseUnits_CourseUnitId",
                table: "UnlockedCourseUnits",
                column: "CourseUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UnlockedCourseUnits_StudentId",
                table: "UnlockedCourseUnits",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnlockedCourseUnits");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "CourseUnits");

            migrationBuilder.DropColumn(
                name: "NoPointsToUnlock",
                table: "CourseUnits");
        }
    }
}
