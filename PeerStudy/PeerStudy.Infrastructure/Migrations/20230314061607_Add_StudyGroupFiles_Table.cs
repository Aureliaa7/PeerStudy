using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Add_StudyGroupFiles_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "CourseResources",
                newName: "FileName");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StudentAssignmentFiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "StudentAssignmentFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "CourseResources",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "StudyGroupFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudyGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriveFileId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyGroupFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyGroupFiles_StudyGroups_StudyGroupId",
                        column: x => x.StudyGroupId,
                        principalTable: "StudyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyGroupFiles_StudyGroupId",
                table: "StudyGroupFiles",
                column: "StudyGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyGroupFiles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StudentAssignmentFiles");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "StudentAssignmentFiles");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "CourseResources");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "CourseResources",
                newName: "Title");
        }
    }
}
