using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Add_QuestionVotes_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerVotes_Users_UserId",
                table: "AnswerVotes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AnswerVotes",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "AnswerVotes",
                newName: "VoteType");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerVotes_UserId",
                table: "AnswerVotes",
                newName: "IX_AnswerVotes_AuthorId");

            migrationBuilder.CreateTable(
                name: "QuestionVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoteType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionVotes_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionVotes_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVotes_AuthorId",
                table: "QuestionVotes",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVotes_QuestionId",
                table: "QuestionVotes",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerVotes_Users_AuthorId",
                table: "AnswerVotes",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerVotes_Users_AuthorId",
                table: "AnswerVotes");

            migrationBuilder.DropTable(
                name: "QuestionVotes");

            migrationBuilder.RenameColumn(
                name: "VoteType",
                table: "AnswerVotes",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "AnswerVotes",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerVotes_AuthorId",
                table: "AnswerVotes",
                newName: "IX_AnswerVotes_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerVotes_Users_UserId",
                table: "AnswerVotes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
