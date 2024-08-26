using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportBudget.Server.Migrations
{
    /// <inheritdoc />
    public partial class BudgetAmountIdUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAmountLog_BudgetAmount_BudgetAmountId",
                table: "BudgetAmountLog");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetAmountLog_BudgetAmountId",
                table: "BudgetAmountLog",
                newName: "IX_BudgetAmountId");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetAmountId",
                table: "BudgetAmountLog",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetAmountLog_BudgetAmount_BudgetAmountId",
                table: "BudgetAmountLog",
                column: "BudgetAmountId",
                principalTable: "BudgetAmount",
                principalColumn: "BudgetAmountId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAmountLog_BudgetAmount_BudgetAmountId",
                table: "BudgetAmountLog");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetAmountId",
                table: "BudgetAmountLog",
                newName: "IX_BudgetAmountLog_BudgetAmountId");

            migrationBuilder.AlterColumn<int>(
                name: "BudgetAmountId",
                table: "BudgetAmountLog",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetAmountLog_BudgetAmount_BudgetAmountId",
                table: "BudgetAmountLog",
                column: "BudgetAmountId",
                principalTable: "BudgetAmount",
                principalColumn: "BudgetAmountId");
        }
    }
}
