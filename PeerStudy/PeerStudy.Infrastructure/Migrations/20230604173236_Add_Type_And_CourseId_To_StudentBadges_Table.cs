using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Add_Type_And_CourseId_To_StudentBadges_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "StudentBadges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "StudentBadges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentBadges_CourseId",
                table: "StudentBadges",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentBadges_Courses_CourseId",
                table: "StudentBadges",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentBadges_Courses_CourseId",
                table: "StudentBadges");

            migrationBuilder.DropIndex(
                name: "IX_StudentBadges_CourseId",
                table: "StudentBadges");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "StudentBadges");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "StudentBadges");
        }
    }
}
