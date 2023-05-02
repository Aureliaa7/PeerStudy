using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeerStudy.Infrastructure.Migrations
{
    public partial class Add_CourseUnit_Order_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "CourseUnitSequence");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "CourseUnits",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR CourseUnitSequence");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "CourseUnitSequence");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "CourseUnits");
        }
    }
}
