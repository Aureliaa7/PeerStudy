using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Update_Courses_And_StudyGroups_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriveFolderId",
                table: "StudyGroups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudyGroupsDriveFolderId",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StudyGroupFiles_OwnerId",
                table: "StudyGroupFiles",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseResources_OwnerId",
                table: "CourseResources",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseResources_Users_OwnerId",
                table: "CourseResources",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyGroupFiles_Users_OwnerId",
                table: "StudyGroupFiles",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseResources_Users_OwnerId",
                table: "CourseResources");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyGroupFiles_Users_OwnerId",
                table: "StudyGroupFiles");

            migrationBuilder.DropIndex(
                name: "IX_StudyGroupFiles_OwnerId",
                table: "StudyGroupFiles");

            migrationBuilder.DropIndex(
                name: "IX_CourseResources_OwnerId",
                table: "CourseResources");

            migrationBuilder.DropColumn(
                name: "DriveFolderId",
                table: "StudyGroups");

            migrationBuilder.DropColumn(
                name: "StudyGroupsDriveFolderId",
                table: "Courses");
        }
    }
}
