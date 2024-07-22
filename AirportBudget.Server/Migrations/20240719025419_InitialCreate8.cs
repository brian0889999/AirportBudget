using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportBudget.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountSerialNumber",
                table: "BudgetAmount");

            migrationBuilder.AddColumn<int>(
                name: "LinkedBudgetAmountId",
                table: "BudgetAmount",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedBudgetAmountId",
                table: "BudgetAmount");

            migrationBuilder.AddColumn<int>(
                name: "AmountSerialNumber",
                table: "BudgetAmount",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
