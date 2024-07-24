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
                name: "Budget",
                columns: table => new
                {
                    BudgetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BudgetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject6 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subject7 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subject8 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnnualBudgetAmount = table.Column<int>(type: "int", nullable: false),
                    FinalBudgetAmount = table.Column<int>(type: "int", nullable: false),
                    CreatedYear = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.BudgetId);
                    table.ForeignKey(
                        name: "FK_Budget_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subject6",
                columns: table => new
                {
                    Subject6Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject6Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject6SerialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
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
                    Subject7Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject7FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject7SerialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject7", x => x.Subject7Id);
                    table.ForeignKey(
                        name: "FK_Subject7_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subject8",
                columns: table => new
                {
                    Subject8Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject8Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject8SerialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subject8FullSerialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject8", x => x.Subject8Id);
                    table.ForeignKey(
                        name: "FK_Subject8_Group_GroupId",
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
                    System = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LastPasswordChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrCount = table.Column<int>(type: "int", nullable: false),
                    ErrDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaymentPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExTax = table.Column<bool>(type: "bit", nullable: false),
                    Reconciled = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedYear = table.Column<int>(type: "int", nullable: false),
                    AmountYear = table.Column<int>(type: "int", nullable: false),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    LinkedBudgetAmountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAmount", x => x.BudgetAmountId);
                    table.ForeignKey(
                        name: "FK_BudgetAmount_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "BudgetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budget_GroupId",
                table: "Budget",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmount_BudgetId",
                table: "BudgetAmount",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject6_GroupId",
                table: "Subject6",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject7_GroupId",
                table: "Subject7",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject8_GroupId",
                table: "Subject8",
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
                name: "Subject6");

            migrationBuilder.DropTable(
                name: "Subject7");

            migrationBuilder.DropTable(
                name: "Subject8");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "Group");
        }
    }
}
