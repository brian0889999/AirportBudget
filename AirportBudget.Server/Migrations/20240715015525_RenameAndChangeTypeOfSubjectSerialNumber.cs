using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportBudget.Server.Migrations
{
    /// <inheritdoc />
    public partial class RenameAndChangeTypeOfSubjectSerialNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject8SerialNumber",
                table: "Subject8");

            migrationBuilder.DropColumn(
                name: "Subject7SerialNumber",
                table: "Subject7");

            migrationBuilder.DropColumn(
                name: "Subject6SerialNumber",
                table: "Subject6");

            migrationBuilder.AddColumn<string>(
                name: "Subject8SerialCode",
                table: "Subject8",
                type: "nvarchar(50)",
                maxLength: 50,
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject8SerialCode",
                table: "Subject8");

            migrationBuilder.DropColumn(
                name: "Subject7SerialCode",
                table: "Subject7");

            migrationBuilder.DropColumn(
                name: "Subject6SerialCode",
                table: "Subject6");

            migrationBuilder.AddColumn<int>(
                name: "Subject8SerialNumber",
                table: "Subject8",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Subject7SerialNumber",
                table: "Subject7",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Subject6SerialNumber",
                table: "Subject6",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
