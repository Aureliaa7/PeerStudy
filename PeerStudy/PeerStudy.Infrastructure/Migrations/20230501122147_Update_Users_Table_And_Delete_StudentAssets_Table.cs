using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Update_Users_Table_And_Delete_StudentAssets_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAssets");

            migrationBuilder.AddColumn<int>(
                name: "NoTotalPoints",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: default(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoTotalPoints",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "StudentAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Asset = table.Column<int>(type: "int", nullable: false),
                    NumberOfAssets = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAssets_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssets_StudentId",
                table: "StudentAssets",
                column: "StudentId");
        }
    }
}
