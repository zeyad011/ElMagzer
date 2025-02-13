using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElMagzer.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class Engtype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TypeNameENG",
                table: "TypeofCows",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TypeofCows",
                keyColumn: "Id",
                keyValue: 1,
                column: "TypeNameENG",
                value: null);

            migrationBuilder.UpdateData(
                table: "TypeofCows",
                keyColumn: "Id",
                keyValue: 2,
                column: "TypeNameENG",
                value: null);

            migrationBuilder.UpdateData(
                table: "TypeofCows",
                keyColumn: "Id",
                keyValue: 3,
                column: "TypeNameENG",
                value: null);

            migrationBuilder.UpdateData(
                table: "TypeofCows",
                keyColumn: "Id",
                keyValue: 4,
                column: "TypeNameENG",
                value: null);

            migrationBuilder.UpdateData(
                table: "TypeofCows",
                keyColumn: "Id",
                keyValue: 5,
                column: "TypeNameENG",
                value: null);

            migrationBuilder.UpdateData(
                table: "TypeofCows",
                keyColumn: "Id",
                keyValue: 6,
                column: "TypeNameENG",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeNameENG",
                table: "TypeofCows");
        }
    }
}
