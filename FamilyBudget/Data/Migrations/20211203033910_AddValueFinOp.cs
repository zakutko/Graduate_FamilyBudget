using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyBudget.Data.Migrations
{
    public partial class AddValueFinOp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "FinOperations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "FinOperations");
        }
    }
}
