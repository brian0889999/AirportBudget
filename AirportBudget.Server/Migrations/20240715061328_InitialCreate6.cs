using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportBudget.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAmount_BudgetDetail_BudgetDetailId",
                table: "BudgetAmount");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAmount_Group_GroupId",
                table: "BudgetAmount");

            migrationBuilder.DropTable(
                name: "BudgetDetail");

            migrationBuilder.DropIndex(
                name: "IX_BudgetAmount_BudgetDetailId",
                table: "BudgetAmount");

            migrationBuilder.DropColumn(
                name: "BudgetDetailId",
                table: "BudgetAmount");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "BudgetAmount",
                newName: "AmountYear");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "Budget",
                newName: "FinalBudgetAmount");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDate",
                table: "BudgetAmount",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "BudgetAmount",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AmountSerialNumber",
                table: "BudgetAmount",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "BudgetAmount",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AnnualBudgetAmount",
                table: "Budget",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedYear",
                table: "Budget",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetAmount_Group_GroupId",
                table: "BudgetAmount",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetAmount_Group_GroupId",
                table: "BudgetAmount");

            migrationBuilder.DropColumn(
                name: "AmountSerialNumber",
                table: "BudgetAmount");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "BudgetAmount");

            migrationBuilder.DropColumn(
                name: "AnnualBudgetAmount",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "CreatedYear",
                table: "Budget");

            migrationBuilder.RenameColumn(
                name: "AmountYear",
                table: "BudgetAmount",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "FinalBudgetAmount",
                table: "Budget",
                newName: "Year");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDate",
                table: "BudgetAmount",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "BudgetAmount",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BudgetDetailId",
                table: "BudgetAmount",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BudgetDetail",
                columns: table => new
                {
                    BudgetDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    AnnualBudgetAmount = table.Column<int>(type: "int", nullable: false),
                    BudgetAmountId = table.Column<int>(type: "int", nullable: false),
                    FinalBudgetAmount = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetDetail", x => x.BudgetDetailId);
                    table.ForeignKey(
                        name: "FK_BudgetDetail_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "BudgetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetDetail_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmount_BudgetDetailId",
                table: "BudgetAmount",
                column: "BudgetDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetDetail_BudgetId",
                table: "BudgetDetail",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetDetail_GroupId",
                table: "BudgetDetail",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetAmount_BudgetDetail_BudgetDetailId",
                table: "BudgetAmount",
                column: "BudgetDetailId",
                principalTable: "BudgetDetail",
                principalColumn: "BudgetDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetAmount_Group_GroupId",
                table: "BudgetAmount",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "GroupId");
        }
    }
}
