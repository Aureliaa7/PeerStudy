using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Add_PostponedAssignments_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostponedAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudyGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoTotalPaidPoints = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostponedAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostponedAssignments_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PostponedAssignments_StudyGroups_StudyGroupId",
                        column: x => x.StudyGroupId,
                        principalTable: "StudyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostponedAssignments_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostponedAssignments_AssignmentId",
                table: "PostponedAssignments",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PostponedAssignments_StudentId",
                table: "PostponedAssignments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PostponedAssignments_StudyGroupId",
                table: "PostponedAssignments",
                column: "StudyGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostponedAssignments");
        }
    }
}
