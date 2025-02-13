using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElMagzer.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class EngNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameENG",
                table: "suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameENG",
                table: "clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "NameENG",
                value: null);

            migrationBuilder.UpdateData(
                table: "clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "NameENG",
                value: null);

            migrationBuilder.UpdateData(
                table: "clients",
                keyColumn: "Id",
                keyValue: 3,
                column: "NameENG",
                value: null);

            migrationBuilder.UpdateData(
                table: "suppliers",
                keyColumn: "Id",
                keyValue: 1,
                column: "NameENG",
                value: null);

            migrationBuilder.UpdateData(
                table: "suppliers",
                keyColumn: "Id",
                keyValue: 2,
                column: "NameENG",
                value: null);

            migrationBuilder.UpdateData(
                table: "suppliers",
                keyColumn: "Id",
                keyValue: 3,
                column: "NameENG",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameENG",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "NameENG",
                table: "clients");
        }
    }
}
