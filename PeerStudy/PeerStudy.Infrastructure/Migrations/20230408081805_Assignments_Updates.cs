using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Assignments_Updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Courses_CourseId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "StudentAssignments");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Assignments",
                newName: "StudyGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_CourseId",
                table: "Assignments",
                newName: "IX_Assignments_StudyGroupId");

            migrationBuilder.AddColumn<Guid>(
                name: "StudyGroupId",
                table: "StudentAssignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Assignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseUnitId",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignments_StudyGroupId",
                table: "StudentAssignments",
                column: "StudyGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CourseUnitId",
                table: "Assignments",
                column: "CourseUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_CourseUnits_CourseUnitId",
                table: "Assignments",
                column: "CourseUnitId",
                principalTable: "CourseUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_StudyGroups_StudyGroupId",
                table: "Assignments",
                column: "StudyGroupId",
                principalTable: "StudyGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssignments_StudyGroups_StudyGroupId",
                table: "StudentAssignments",
                column: "StudyGroupId",
                principalTable: "StudyGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_CourseUnits_CourseUnitId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_StudyGroups_StudyGroupId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssignments_StudyGroups_StudyGroupId",
                table: "StudentAssignments");

            migrationBuilder.DropIndex(
                name: "IX_StudentAssignments_StudyGroupId",
                table: "StudentAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_CourseUnitId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "StudyGroupId",
                table: "StudentAssignments");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "CourseUnitId",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "StudyGroupId",
                table: "Assignments",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Assignments_StudyGroupId",
                table: "Assignments",
                newName: "IX_Assignments_CourseId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "StudentAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Courses_CourseId",
                table: "Assignments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
