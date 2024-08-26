using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportBudget.Server.Migrations
{
    /// <inheritdoc />
    public partial class EntityLogUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetAmountLog");

            migrationBuilder.CreateTable(
                name: "EntityLog",
                columns: table => new
                {
                    EntityLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChangeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityLog", x => x.EntityLogId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityLogId",
                table: "EntityLog",
                column: "EntityLogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityLog");

            migrationBuilder.CreateTable(
                name: "BudgetAmountLog",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BudgetAmountId = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    ChangeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAmountLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_BudgetAmountLog_BudgetAmount_BudgetAmountId",
                        column: x => x.BudgetAmountId,
                        principalTable: "BudgetAmount",
                        principalColumn: "BudgetAmountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmountId",
                table: "BudgetAmountLog",
                column: "BudgetAmountId");
        }
    }
}
