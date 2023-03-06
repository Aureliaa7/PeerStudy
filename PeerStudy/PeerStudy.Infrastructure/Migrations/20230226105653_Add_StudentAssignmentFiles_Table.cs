using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Add_StudentAssignmentFiles_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentAssignmentFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriveFileId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssignmentFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAssignmentFiles_StudentAssignments_StudentAssignmentId",
                        column: x => x.StudentAssignmentId,
                        principalTable: "StudentAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignmentFiles_StudentAssignmentId",
                table: "StudentAssignmentFiles",
                column: "StudentAssignmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAssignmentFiles");
        }
    }
}
