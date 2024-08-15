using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportBudget.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subject7_Group_GroupId",
                table: "Subject7");

            migrationBuilder.DropForeignKey(
                name: "FK_Subject8_Group_GroupId",
                table: "Subject8");

            migrationBuilder.DropColumn(
                name: "Subject8FullSerialCode",
                table: "Subject8");

            migrationBuilder.DropColumn(
                name: "Subject8SerialCode",
                table: "Subject8");

            migrationBuilder.DropColumn(
                name: "Subject7FullName",
                table: "Subject7");

            migrationBuilder.DropColumn(
                name: "Subject7SerialCode",
                table: "Subject7");

            migrationBuilder.DropColumn(
                name: "Subject6SerialCode",
                table: "Subject6");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Subject8",
                newName: "Subject7Id");

            migrationBuilder.RenameIndex(
                name: "IX_Subject8_GroupId",
                table: "Subject8",
                newName: "IX_Subject8_Subject7Id");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Subject7",
                newName: "Subject6Id");

            migrationBuilder.RenameIndex(
                name: "IX_Subject7_GroupId",
                table: "Subject7",
                newName: "IX_Subject7_Subject6Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subject7_Subject6_Subject6Id",
                table: "Subject7",
                column: "Subject6Id",
                principalTable: "Subject6",
                principalColumn: "Subject6Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subject8_Subject7_Subject7Id",
                table: "Subject8",
                column: "Subject7Id",
                principalTable: "Subject7",
                principalColumn: "Subject7Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subject7_Subject6_Subject6Id",
                table: "Subject7");

            migrationBuilder.DropForeignKey(
                name: "FK_Subject8_Subject7_Subject7Id",
                table: "Subject8");

            migrationBuilder.RenameColumn(
                name: "Subject7Id",
                table: "Subject8",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Subject8_Subject7Id",
                table: "Subject8",
                newName: "IX_Subject8_GroupId");

            migrationBuilder.RenameColumn(
                name: "Subject6Id",
                table: "Subject7",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Subject7_Subject6Id",
                table: "Subject7",
                newName: "IX_Subject7_GroupId");

            migrationBuilder.AddColumn<string>(
                name: "Subject8FullSerialCode",
                table: "Subject8",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject8SerialCode",
                table: "Subject8",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject7FullName",
                table: "Subject7",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject7SerialCode",
                table: "Subject7",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject6SerialCode",
                table: "Subject6",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Subject7_Group_GroupId",
                table: "Subject7",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subject8_Group_GroupId",
                table: "Subject8",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
