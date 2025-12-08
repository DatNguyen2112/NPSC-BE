using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addtableOrderReturnandOrderReturnItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_Return_Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ReasonCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EntityName = table.Column<string>(type: "text", nullable: true),
                    EntityTypeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EntityTypeName = table.Column<string>(type: "text", nullable: true),
                    OriginalDocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalDocumentCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ListPayment = table.Column<List<jsonb_HalfPayment>>(type: "jsonb", nullable: true),
                    RefundSubTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    RemainingRefundAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Note = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    StatusCode = table.Column<string>(type: "text", nullable: true),
                    RefundStatusCode = table.Column<string>(type: "text", nullable: true),
                    CancelledOnDate = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Return_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sm_Return_Order_Item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductCode = table.Column<string>(type: "text", nullable: true),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    ReturnedQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    InitialQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    RemainingQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    InitialUnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    ReturnedUnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    LineAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReturnOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Return_Order_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Return_Order_Item_sm_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "sm_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_Return_Order_Item_sm_Return_Order_ReturnOrderId",
                        column: x => x.ReturnOrderId,
                        principalTable: "sm_Return_Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_Return_Order_Item_ProductId",
                table: "sm_Return_Order_Item",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Return_Order_Item_ReturnOrderId",
                table: "sm_Return_Order_Item",
                column: "ReturnOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_Return_Order_Item");

            migrationBuilder.DropTable(
                name: "sm_Return_Order");
        }
    }
}
