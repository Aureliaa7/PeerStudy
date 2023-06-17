using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Add_ChangedBy_Column_ForWorkItems_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChangedById",
                table: "WorkItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WorkItems_ChangedById",
                table: "WorkItems",
                column: "ChangedById");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_Users_ChangedById",
                table: "WorkItems",
                column: "ChangedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_Users_ChangedById",
                table: "WorkItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkItems_ChangedById",
                table: "WorkItems");

            migrationBuilder.DropColumn(
                name: "ChangedById",
                table: "WorkItems");
        }
    }
}
