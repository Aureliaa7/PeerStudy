using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class WorkItems_Table_Updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_Users_StudentId",
                table: "WorkItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkItems_StudentId",
                table: "WorkItems");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "WorkItems");

            migrationBuilder.RenameColumn(
                name: "AssignedTo",
                table: "WorkItems",
                newName: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItems_AssignedToId",
                table: "WorkItems",
                column: "AssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_Users_AssignedToId",
                table: "WorkItems",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_Users_AssignedToId",
                table: "WorkItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkItems_AssignedToId",
                table: "WorkItems");

            migrationBuilder.RenameColumn(
                name: "AssignedToId",
                table: "WorkItems",
                newName: "AssignedTo");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "WorkItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WorkItems_StudentId",
                table: "WorkItems",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_Users_StudentId",
                table: "WorkItems",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
