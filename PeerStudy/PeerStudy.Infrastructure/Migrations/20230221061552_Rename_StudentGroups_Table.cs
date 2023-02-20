using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Rename_StudentGroups_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroups_Courses_CourseId",
                table: "StudentGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentStudyGroup_StudentGroups_StudyGroupId",
                table: "StudentStudyGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentGroups",
                table: "StudentGroups");

            migrationBuilder.RenameTable(
                name: "StudentGroups",
                newName: "StudyGroups");

            migrationBuilder.RenameIndex(
                name: "IX_StudentGroups_CourseId",
                table: "StudyGroups",
                newName: "IX_StudyGroups_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyGroups",
                table: "StudyGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentStudyGroup_StudyGroups_StudyGroupId",
                table: "StudentStudyGroup",
                column: "StudyGroupId",
                principalTable: "StudyGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyGroups_Courses_CourseId",
                table: "StudyGroups",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentStudyGroup_StudyGroups_StudyGroupId",
                table: "StudentStudyGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyGroups_Courses_CourseId",
                table: "StudyGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyGroups",
                table: "StudyGroups");

            migrationBuilder.RenameTable(
                name: "StudyGroups",
                newName: "StudentGroups");

            migrationBuilder.RenameIndex(
                name: "IX_StudyGroups_CourseId",
                table: "StudentGroups",
                newName: "IX_StudentGroups_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentGroups",
                table: "StudentGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroups_Courses_CourseId",
                table: "StudentGroups",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentStudyGroup_StudentGroups_StudyGroupId",
                table: "StudentStudyGroup",
                column: "StudyGroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
