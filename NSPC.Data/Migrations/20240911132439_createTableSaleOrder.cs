using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class createTableSaleOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "sm_SaleOrdersId",
                table: "mk_PhieuItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sm_SaleOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WareCode = table.Column<string>(type: "text", nullable: true),
                    TypeCode = table.Column<string>(type: "text", nullable: true),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    TypeName = table.Column<string>(type: "text", nullable: true),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    VAT = table.Column<double>(type: "double precision", nullable: false),
                    TotalDiscountsAccount = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalTaxAccount = table.Column<decimal>(type: "numeric", nullable: true),
                    ReceiveInventoryStatus = table.Column<string>(type: "text", nullable: true),
                    PaymentStatus = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    mk_DuAnId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    LyDoTuChoi = table.Column<string>(type: "text", nullable: true),
                    ActionMadeByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActionMadeByUserName = table.Column<string>(type: "text", nullable: true),
                    ActionMadeOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_SaleOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_SaleOrders_mk_DuAn_mk_DuAnId",
                        column: x => x.mk_DuAnId,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SaleOrders_sm_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "sm_Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_PhieuItem_sm_SaleOrdersId",
                table: "mk_PhieuItem",
                column: "sm_SaleOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SaleOrders_mk_DuAnId",
                table: "sm_SaleOrders",
                column: "mk_DuAnId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SaleOrders_SupplierId",
                table: "sm_SaleOrders",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_PhieuItem_sm_SaleOrders_sm_SaleOrdersId",
                table: "mk_PhieuItem",
                column: "sm_SaleOrdersId",
                principalTable: "sm_SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_PhieuItem_sm_SaleOrders_sm_SaleOrdersId",
                table: "mk_PhieuItem");

            migrationBuilder.DropTable(
                name: "sm_SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_mk_PhieuItem_sm_SaleOrdersId",
                table: "mk_PhieuItem");

            migrationBuilder.DropColumn(
                name: "sm_SaleOrdersId",
                table: "mk_PhieuItem");
        }
    }
}
