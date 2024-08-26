using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportBudget.Server.Migrations
{
    /// <inheritdoc />
    public partial class ValuesUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewValues",
                table: "BudgetAmountLog");

            migrationBuilder.DropColumn(
                name: "OldValues",
                table: "BudgetAmountLog");

            migrationBuilder.AddColumn<string>(
                name: "Values",
                table: "BudgetAmountLog",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Values",
                table: "BudgetAmountLog");

            migrationBuilder.AddColumn<string>(
                name: "NewValues",
                table: "BudgetAmountLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValues",
                table: "BudgetAmountLog",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
