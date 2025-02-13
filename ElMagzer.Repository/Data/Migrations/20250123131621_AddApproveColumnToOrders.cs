using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElMagzer.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApproveColumnToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Approve",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approve",
                table: "Orders");
        }
    }
}
