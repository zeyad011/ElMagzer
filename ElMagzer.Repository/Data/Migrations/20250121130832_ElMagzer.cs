using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ElMagzer.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class ElMagzer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cuttings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CutName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuttings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "miscarriageTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_miscarriageTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    storeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    HeightCapacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeofCows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeofCows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrederType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    numberofCows = table.Column<int>(type: "int", nullable: true),
                    numberofbatches = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Performance = table.Column<double>(type: "float", nullable: true),
                    ClientsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatchType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numberOfCowOrPieces = table.Column<int>(type: "int", nullable: true),
                    CowOrPiecesType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batches_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cow_Pieces_2",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PieceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Create_At_Divece4 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dateOfExpiere = table.Column<DateTime>(type: "datetime2", nullable: false),
                    techofDevice4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    machien_Id_Device4 = table.Column<int>(type: "int", nullable: false),
                    isExecutions = table.Column<bool>(type: "bit", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    BatchId = table.Column<int>(type: "int", nullable: true),
                    CuttingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cow_Pieces_2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cow_Pieces_2_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cow_Pieces_2_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cow_Pieces_2_cuttings_CuttingId",
                        column: x => x.CuttingId,
                        principalTable: "cuttings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cowsSeeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CowsId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    weight = table.Column<double>(type: "float", nullable: false),
                    TypeofCowsId = table.Column<int>(type: "int", nullable: false),
                    clientId = table.Column<int>(type: "int", nullable: false),
                    suppliersId = table.Column<int>(type: "int", nullable: false),
                    BatchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cowsSeeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cowsSeeds_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_cowsSeeds_TypeofCows_TypeofCowsId",
                        column: x => x.TypeofCowsId,
                        principalTable: "TypeofCows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cowsSeeds_clients_clientId",
                        column: x => x.clientId,
                        principalTable: "clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cowsSeeds_suppliers_suppliersId",
                        column: x => x.suppliersId,
                        principalTable: "suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CowsId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cow_Weight = table.Column<double>(type: "float", nullable: true),
                    Create_At_Divece1 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Create_At_Divece5 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Waste = table.Column<double>(type: "float", nullable: true),
                    Miscarage = table.Column<double>(type: "float", nullable: true),
                    machien_Id_Device1 = table.Column<int>(type: "int", nullable: false),
                    machien_Id_Device5 = table.Column<int>(type: "int", nullable: true),
                    Doctor_Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    techOfDevice1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    techfDevice5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    TypeofCowsId = table.Column<int>(type: "int", nullable: false),
                    CowsSeedId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cows_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cows_TypeofCows_TypeofCowsId",
                        column: x => x.TypeofCowsId,
                        principalTable: "TypeofCows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cows_cowsSeeds_CowsSeedId",
                        column: x => x.CowsSeedId,
                        principalTable: "cowsSeeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cowMiscarriages",
                columns: table => new
                {
                    CowsId = table.Column<int>(type: "int", nullable: false),
                    MiscarriageTypeId = table.Column<int>(type: "int", nullable: false),
                    BarCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cowMiscarriages", x => new { x.CowsId, x.MiscarriageTypeId });
                    table.ForeignKey(
                        name: "FK_cowMiscarriages_Cows_CowsId",
                        column: x => x.CowsId,
                        principalTable: "Cows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cowMiscarriages_miscarriageTypes_MiscarriageTypeId",
                        column: x => x.MiscarriageTypeId,
                        principalTable: "miscarriageTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CowsPieces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pieceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pieceWeight_In = table.Column<double>(type: "float", nullable: false),
                    pieceWeight_Out = table.Column<double>(type: "float", nullable: true),
                    PieceTybe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tybe = table.Column<int>(type: "int", nullable: true),
                    Create_At_Divece2 = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dateOfExpiere = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Create_At_Divece3 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    techOfDevice2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    techOfDevice3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    machien_Id_Device2 = table.Column<int>(type: "int", nullable: false),
                    machien_Id_Device3 = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isExecutions = table.Column<bool>(type: "bit", nullable: false),
                    BatchId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    CowId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CowsPieces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CowsPieces_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CowsPieces_Cows_CowId",
                        column: x => x.CowId,
                        principalTable: "Cows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CowsPieces_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Code", "HeightCapacity", "quantity", "storeName" },
                values: new object[,]
                {
                    { 1, "1", 300, null, "مخزن 1" },
                    { 2, "2", 200, null, "مخزن 2" },
                    { 3, "3", 400, null, "مخزن 3" },
                    { 4, "4", 430, null, "مخزن 4" },
                    { 5, "5", 120, null, "مخزن 5" },
                    { 6, "6", 140, null, "مخزن 6" },
                    { 7, "7", 500, null, "مخزن 7" },
                    { 8, "8", 320, null, "مخزن 8" },
                    { 9, "9", 250, null, "مخزن 9" }
                });

            migrationBuilder.InsertData(
                table: "TypeofCows",
                columns: new[] { "Id", "TypeName" },
                values: new object[,]
                {
                    { 1, "بلدي" },
                    { 2, "برازيلي" },
                    { 3, "هولندي" },
                    { 4, "انجليزي" },
                    { 5, "سويسري" },
                    { 6, "هندي" }
                });

            migrationBuilder.InsertData(
                table: "clients",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "1", "Ahmed" },
                    { 2, "2", "Ziad" },
                    { 3, "3", "Rizk" }
                });

            migrationBuilder.InsertData(
                table: "cuttings",
                columns: new[] { "Id", "Code", "CutName", "Date" },
                values: new object[,]
                {
                    { 1, "1", "Ribeye", new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "2", "Tenderloin", new DateTime(2024, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "3", "Sirloin", new DateTime(2024, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "4", "T-Bone", new DateTime(2024, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "5", "Chuck", new DateTime(2024, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "6", "Flank", new DateTime(2024, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "7", "Brisket", new DateTime(2024, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "8", "Shank", new DateTime(2024, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "9", "Rump", new DateTime(2024, 12, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, "10", "Short Loin", new DateTime(2024, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, "11", "Top Sirloin", new DateTime(2024, 12, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, "12", "Bottom Sirloin", new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, "13", "Plate", new DateTime(2024, 12, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "miscarriageTypes",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "1", "ممبار" },
                    { 2, "2", "فشه" },
                    { 3, "3", "كرشه" },
                    { 4, "4", "طحال" },
                    { 5, "5", "بنكرياس" },
                    { 6, "6", "كبده" },
                    { 7, "7", "مخ" },
                    { 8, "8", "قلب" }
                });

            migrationBuilder.InsertData(
                table: "suppliers",
                columns: new[] { "Id", "Code", "SupplierName" },
                values: new object[,]
                {
                    { 1, "1", "ElRawda" },
                    { 2, "2", "ElMagzer" },
                    { 3, "3", "Shatat" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Batches_OrderId",
                table: "Batches",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Cow_Pieces_2_BatchId",
                table: "Cow_Pieces_2",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Cow_Pieces_2_CuttingId",
                table: "Cow_Pieces_2",
                column: "CuttingId");

            migrationBuilder.CreateIndex(
                name: "IX_Cow_Pieces_2_StoreId",
                table: "Cow_Pieces_2",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_cowMiscarriages_MiscarriageTypeId",
                table: "cowMiscarriages",
                column: "MiscarriageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Cows_BatchId",
                table: "Cows",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Cows_CowsSeedId",
                table: "Cows",
                column: "CowsSeedId");

            migrationBuilder.CreateIndex(
                name: "IX_Cows_TypeofCowsId",
                table: "Cows",
                column: "TypeofCowsId");

            migrationBuilder.CreateIndex(
                name: "IX_CowsPieces_BatchId",
                table: "CowsPieces",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_CowsPieces_CowId",
                table: "CowsPieces",
                column: "CowId");

            migrationBuilder.CreateIndex(
                name: "IX_CowsPieces_StoreId",
                table: "CowsPieces",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_cowsSeeds_BatchId",
                table: "cowsSeeds",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_cowsSeeds_clientId",
                table: "cowsSeeds",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_cowsSeeds_suppliersId",
                table: "cowsSeeds",
                column: "suppliersId");

            migrationBuilder.CreateIndex(
                name: "IX_cowsSeeds_TypeofCowsId",
                table: "cowsSeeds",
                column: "TypeofCowsId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientsId",
                table: "Orders",
                column: "ClientsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cow_Pieces_2");

            migrationBuilder.DropTable(
                name: "cowMiscarriages");

            migrationBuilder.DropTable(
                name: "CowsPieces");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "cuttings");

            migrationBuilder.DropTable(
                name: "miscarriageTypes");

            migrationBuilder.DropTable(
                name: "Cows");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "cowsSeeds");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "TypeofCows");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "clients");
        }
    }
}
