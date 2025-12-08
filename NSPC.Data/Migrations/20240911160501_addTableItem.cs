using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addTableItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_PhieuItem");

            migrationBuilder.DropTable(
                name: "mk_QuanLyPhieu");

            migrationBuilder.RenameColumn(
                name: "TotalDiscountsAccount",
                table: "sm_SaleOrders",
                newName: "TotalHalfPayment");

            migrationBuilder.AddColumn<List<jsonb_HalfPayment>>(
                name: "HalfPayment",
                table: "sm_SaleOrders",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<List<jsonb_OtherCost>>(
                name: "OtherCost",
                table: "sm_SaleOrders",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDiscountsAccounts",
                table: "sm_SaleOrders",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "mk_DuAnId",
                table: "sm_SaleOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sm_SaleOrderItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductCode = table.Column<string>(type: "text", nullable: true),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    Amout = table.Column<decimal>(type: "numeric", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    DiscountPercent = table.Column<int>(type: "integer", nullable: true),
                    IdPhieu = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_sm_SaleOrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_SaleOrderItem_sm_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "sm_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SaleOrderItem_sm_SaleOrders_IdPhieu",
                        column: x => x.IdPhieu,
                        principalTable: "sm_SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_SaleOrders_mk_DuAnId",
                table: "sm_SaleOrders",
                column: "mk_DuAnId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SaleOrderItem_IdPhieu",
                table: "sm_SaleOrderItem",
                column: "IdPhieu");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SaleOrderItem_ProductId",
                table: "sm_SaleOrderItem",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SaleOrders_mk_DuAn_mk_DuAnId",
                table: "sm_SaleOrders",
                column: "mk_DuAnId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_SaleOrders_mk_DuAn_mk_DuAnId",
                table: "sm_SaleOrders");

            migrationBuilder.DropTable(
                name: "sm_SaleOrderItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_SaleOrders_mk_DuAnId",
                table: "sm_SaleOrders");

            migrationBuilder.DropColumn(
                name: "HalfPayment",
                table: "sm_SaleOrders");

            migrationBuilder.DropColumn(
                name: "OtherCost",
                table: "sm_SaleOrders");

            migrationBuilder.DropColumn(
                name: "TotalDiscountsAccounts",
                table: "sm_SaleOrders");

            migrationBuilder.DropColumn(
                name: "mk_DuAnId",
                table: "sm_SaleOrders");

            migrationBuilder.RenameColumn(
                name: "TotalHalfPayment",
                table: "sm_SaleOrders",
                newName: "TotalDiscountsAccount");

            migrationBuilder.CreateTable(
                name: "mk_QuanLyPhieu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionMadeByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActionMadeByUserName = table.Column<string>(type: "text", nullable: true),
                    ActionMadeOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LyDoTuChoi = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    OrderCode = table.Column<string>(type: "text", nullable: true),
                    PaymentStatus = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    ReceiveInventoryStatus = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    SubTotal = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalDiscountsAccount = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalTaxAccount = table.Column<decimal>(type: "numeric", nullable: true),
                    TypeCode = table.Column<string>(type: "text", nullable: true),
                    TypeName = table.Column<string>(type: "text", nullable: true),
                    VAT = table.Column<double>(type: "double precision", nullable: false),
                    WareCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_QuanLyPhieu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_QuanLyPhieu_mk_DuAn_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_QuanLyPhieu_sm_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "sm_Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mk_PhieuItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdPhieu = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amout = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DiscountPercent = table.Column<int>(type: "integer", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ProductCode = table.Column<string>(type: "text", nullable: true),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    sm_SaleOrdersId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_PhieuItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_PhieuItem_mk_QuanLyPhieu_IdPhieu",
                        column: x => x.IdPhieu,
                        principalTable: "mk_QuanLyPhieu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_PhieuItem_sm_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "sm_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_PhieuItem_sm_SaleOrders_sm_SaleOrdersId",
                        column: x => x.sm_SaleOrdersId,
                        principalTable: "sm_SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_PhieuItem_IdPhieu",
                table: "mk_PhieuItem",
                column: "IdPhieu");

            migrationBuilder.CreateIndex(
                name: "IX_mk_PhieuItem_ProductId",
                table: "mk_PhieuItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_PhieuItem_sm_SaleOrdersId",
                table: "mk_PhieuItem",
                column: "sm_SaleOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_QuanLyPhieu_ProjectId",
                table: "mk_QuanLyPhieu",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_QuanLyPhieu_SupplierId",
                table: "mk_QuanLyPhieu",
                column: "SupplierId");
        }
    }
}
