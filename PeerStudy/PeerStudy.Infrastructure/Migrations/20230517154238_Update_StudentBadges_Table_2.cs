using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Update_StudentBadges_Table_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentBadges",
                table: "StudentBadges");

            migrationBuilder.DropColumn(
                name: "NumberOfBadges",
                table: "StudentBadges");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "StudentBadges",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "EarnedAt",
                table: "StudentBadges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentBadges",
                table: "StudentBadges",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentBadges_StudentId",
                table: "StudentBadges",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentBadges",
                table: "StudentBadges");

            migrationBuilder.DropIndex(
                name: "IX_StudentBadges_StudentId",
                table: "StudentBadges");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StudentBadges");

            migrationBuilder.DropColumn(
                name: "EarnedAt",
                table: "StudentBadges");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfBadges",
                table: "StudentBadges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentBadges",
                table: "StudentBadges",
                columns: new[] { "StudentId", "BadgeId" });
        }
    }
}
