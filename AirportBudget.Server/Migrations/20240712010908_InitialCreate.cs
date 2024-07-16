using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportBudget.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    BudgetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BudgetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject6 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subject7 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subject8 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.BudgetId);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    RolePermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.RolePermissionId);
                });

            migrationBuilder.CreateTable(
                name: "BudgetDetail",
                columns: table => new
                {
                    BudgetDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnnualBudgetAmount = table.Column<int>(type: "int", nullable: false),
                    FinalBudgetAmount = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    BudgetAmountId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Account = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RolePermissionId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    System = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LastPasswordChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrCount = table.Column<int>(type: "int", nullable: false),
                    ErrDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_RolePermission_RolePermissionId",
                        column: x => x.RolePermissionId,
                        principalTable: "RolePermission",
                        principalColumn: "RolePermissionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetAmount",
                columns: table => new
                {
                    BudgetAmountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RequestAmount = table.Column<int>(type: "int", nullable: false),
                    PaymentAmount = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaymentPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExTax = table.Column<bool>(type: "bit", nullable: false),
                    Reconciled = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedYear = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    BudgetDetailId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAmount", x => x.BudgetAmountId);
                    table.ForeignKey(
                        name: "FK_BudgetAmount_BudgetDetail_BudgetDetailId",
                        column: x => x.BudgetDetailId,
                        principalTable: "BudgetDetail",
                        principalColumn: "BudgetDetailId");
                    table.ForeignKey(
                        name: "FK_BudgetAmount_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "BudgetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetAmount_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "GroupId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmount_BudgetDetailId",
                table: "BudgetAmount",
                column: "BudgetDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmount_BudgetId",
                table: "BudgetAmount",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmount_GroupId",
                table: "BudgetAmount",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetDetail_BudgetId",
                table: "BudgetDetail",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetDetail_GroupId",
                table: "BudgetDetail",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_User_GroupId",
                table: "User",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_User_RolePermissionId",
                table: "User",
                column: "RolePermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetAmount");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "BudgetDetail");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "Group");
        }
    }
}
