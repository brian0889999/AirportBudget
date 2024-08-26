using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportBudget.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subject8");

            migrationBuilder.DropTable(
                name: "Subject7");

            migrationBuilder.DropTable(
                name: "Subject6");

            migrationBuilder.CreateTable(
                name: "BudgetAmountLog",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BudgetAmountId = table.Column<int>(type: "int", nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChangeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAmountLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_BudgetAmountLog_BudgetAmount_BudgetAmountId",
                        column: x => x.BudgetAmountId,
                        principalTable: "BudgetAmount",
                        principalColumn: "BudgetAmountId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmountLog_BudgetAmountId",
                table: "BudgetAmountLog",
                column: "BudgetAmountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetAmountLog");

            migrationBuilder.CreateTable(
                name: "Subject6",
                columns: table => new
                {
                    Subject6Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Subject6Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject6", x => x.Subject6Id);
                    table.ForeignKey(
                        name: "FK_Subject6_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subject7",
                columns: table => new
                {
                    Subject7Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject6Id = table.Column<int>(type: "int", nullable: false),
                    Subject7Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject7", x => x.Subject7Id);
                    table.ForeignKey(
                        name: "FK_Subject7_Subject6_Subject6Id",
                        column: x => x.Subject6Id,
                        principalTable: "Subject6",
                        principalColumn: "Subject6Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subject8",
                columns: table => new
                {
                    Subject8Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject7Id = table.Column<int>(type: "int", nullable: false),
                    Subject8Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject8", x => x.Subject8Id);
                    table.ForeignKey(
                        name: "FK_Subject8_Subject7_Subject7Id",
                        column: x => x.Subject7Id,
                        principalTable: "Subject7",
                        principalColumn: "Subject7Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subject6_GroupId",
                table: "Subject6",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject7_Subject6Id",
                table: "Subject7",
                column: "Subject6Id");

            migrationBuilder.CreateIndex(
                name: "IX_Subject8_Subject7Id",
                table: "Subject8",
                column: "Subject7Id");
        }
    }
}
