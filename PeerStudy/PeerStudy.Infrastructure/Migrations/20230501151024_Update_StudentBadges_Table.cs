using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Update_StudentBadges_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RewardingDate",
                table: "StudentBadges");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RewardingDate",
                table: "StudentBadges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
