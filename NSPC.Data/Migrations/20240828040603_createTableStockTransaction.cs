using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class createTableStockTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_Stock_Transaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeCode = table.Column<string>(type: "text", nullable: true),
                    TypeName = table.Column<string>(type: "text", nullable: true),
                    ProductCode = table.Column<string>(type: "text", nullable: true),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    WareCode = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    OpeningInventory = table.Column<int>(type: "integer", nullable: true),
                    ClosingInventory = table.Column<int>(type: "integer", nullable: true),
                    InventoryQuantity = table.Column<int>(type: "integer", nullable: true),
                    DifferentAmount = table.Column<int>(type: "integer", nullable: true),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: true),
                    CheckInventoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Stock_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Stock_Transaction_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_Stock_Transaction_mk_KiemKho_CheckInventoryId",
                        column: x => x.CheckInventoryId,
                        principalTable: "mk_KiemKho",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_Stock_Transaction_sm_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "sm_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_Stock_Transaction_CheckInventoryId",
                table: "sm_Stock_Transaction",
                column: "CheckInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Stock_Transaction_CreatedByUserId",
                table: "sm_Stock_Transaction",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Stock_Transaction_ProductId",
                table: "sm_Stock_Transaction",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_Stock_Transaction");
        }
    }
}
