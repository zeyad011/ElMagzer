using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElMagzer.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class Edite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cowsSeeds_TypeofCows_TypeofCowsId",
                table: "cowsSeeds");

            migrationBuilder.DropForeignKey(
                name: "FK_cowsSeeds_clients_clientId",
                table: "cowsSeeds");

            migrationBuilder.DropForeignKey(
                name: "FK_cowsSeeds_suppliers_suppliersId",
                table: "cowsSeeds");

            migrationBuilder.AlterColumn<double>(
                name: "weight",
                table: "cowsSeeds",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "suppliersId",
                table: "cowsSeeds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "clientId",
                table: "cowsSeeds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TypeofCowsId",
                table: "cowsSeeds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_cowsSeeds_TypeofCows_TypeofCowsId",
                table: "cowsSeeds",
                column: "TypeofCowsId",
                principalTable: "TypeofCows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cowsSeeds_clients_clientId",
                table: "cowsSeeds",
                column: "clientId",
                principalTable: "clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cowsSeeds_suppliers_suppliersId",
                table: "cowsSeeds",
                column: "suppliersId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cowsSeeds_TypeofCows_TypeofCowsId",
                table: "cowsSeeds");

            migrationBuilder.DropForeignKey(
                name: "FK_cowsSeeds_clients_clientId",
                table: "cowsSeeds");

            migrationBuilder.DropForeignKey(
                name: "FK_cowsSeeds_suppliers_suppliersId",
                table: "cowsSeeds");

            migrationBuilder.AlterColumn<double>(
                name: "weight",
                table: "cowsSeeds",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "suppliersId",
                table: "cowsSeeds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "clientId",
                table: "cowsSeeds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TypeofCowsId",
                table: "cowsSeeds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_cowsSeeds_TypeofCows_TypeofCowsId",
                table: "cowsSeeds",
                column: "TypeofCowsId",
                principalTable: "TypeofCows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_cowsSeeds_clients_clientId",
                table: "cowsSeeds",
                column: "clientId",
                principalTable: "clients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_cowsSeeds_suppliers_suppliersId",
                table: "cowsSeeds",
                column: "suppliersId",
                principalTable: "suppliers",
                principalColumn: "Id");
        }
    }
}
