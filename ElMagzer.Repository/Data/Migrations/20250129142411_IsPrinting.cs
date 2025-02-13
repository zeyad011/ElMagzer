using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElMagzer.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsPrinting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrinted",
                table: "cowsSeeds",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrinted",
                table: "cowsSeeds");
        }
    }
}
