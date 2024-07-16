using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportBudget.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subject6",
                columns: table => new
                {
                    Subject6Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject6Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject6SerialNumber = table.Column<int>(type: "int", nullable: false),
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
                    Subject7SerialNumber = table.Column<int>(type: "int", nullable: false),
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
                    Subject8SerialNumber = table.Column<int>(type: "int", nullable: false),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subject6");

            migrationBuilder.DropTable(
                name: "Subject7");

            migrationBuilder.DropTable(
                name: "Subject8");
        }
    }
}
