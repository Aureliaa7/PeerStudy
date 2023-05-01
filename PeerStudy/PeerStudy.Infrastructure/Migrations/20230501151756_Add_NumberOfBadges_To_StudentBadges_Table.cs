using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Add_NumberOfBadges_To_StudentBadges_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfBadges",
                table: "StudentBadges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfBadges",
                table: "StudentBadges");
        }
    }
}
