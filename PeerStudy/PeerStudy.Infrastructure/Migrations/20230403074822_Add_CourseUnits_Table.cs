using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Add_CourseUnits_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseUnitId",
                table: "CourseResources",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CourseUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseUnits_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseResources_CourseUnitId",
                table: "CourseResources",
                column: "CourseUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseUnits_CourseId",
                table: "CourseUnits",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseResources_CourseUnits_CourseUnitId",
                table: "CourseResources",
                column: "CourseUnitId",
                principalTable: "CourseUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseResources_CourseUnits_CourseUnitId",
                table: "CourseResources");

            migrationBuilder.DropTable(
                name: "CourseUnits");

            migrationBuilder.DropIndex(
                name: "IX_CourseResources_CourseUnitId",
                table: "CourseResources");

            migrationBuilder.DropColumn(
                name: "CourseUnitId",
                table: "CourseResources");
        }
    }
}
