using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renametableSaleOrderItemtoPurchaseOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_SaleOrderItem_sm_Product_ProductId",
                table: "sm_SaleOrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_SaleOrderItem_sm_PurchaseOrder_IdPhieu",
                table: "sm_SaleOrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sm_SaleOrderItem",
                table: "sm_SaleOrderItem");

            migrationBuilder.RenameTable(
                name: "sm_SaleOrderItem",
                newName: "sm_PurchaseOrderItem");

            migrationBuilder.RenameIndex(
                name: "IX_sm_SaleOrderItem_ProductId",
                table: "sm_PurchaseOrderItem",
                newName: "IX_sm_PurchaseOrderItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_SaleOrderItem_IdPhieu",
                table: "sm_PurchaseOrderItem",
                newName: "IX_sm_PurchaseOrderItem_IdPhieu");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sm_PurchaseOrderItem",
                table: "sm_PurchaseOrderItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_Product_ProductId",
                table: "sm_PurchaseOrderItem",
                column: "ProductId",
                principalTable: "sm_Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_PurchaseOrder_IdPhieu",
                table: "sm_PurchaseOrderItem",
                column: "IdPhieu",
                principalTable: "sm_PurchaseOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_Product_ProductId",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_PurchaseOrder_IdPhieu",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sm_PurchaseOrderItem",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.RenameTable(
                name: "sm_PurchaseOrderItem",
                newName: "sm_SaleOrderItem");

            migrationBuilder.RenameIndex(
                name: "IX_sm_PurchaseOrderItem_ProductId",
                table: "sm_SaleOrderItem",
                newName: "IX_sm_SaleOrderItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_PurchaseOrderItem_IdPhieu",
                table: "sm_SaleOrderItem",
                newName: "IX_sm_SaleOrderItem_IdPhieu");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sm_SaleOrderItem",
                table: "sm_SaleOrderItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SaleOrderItem_sm_Product_ProductId",
                table: "sm_SaleOrderItem",
                column: "ProductId",
                principalTable: "sm_Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SaleOrderItem_sm_PurchaseOrder_IdPhieu",
                table: "sm_SaleOrderItem",
                column: "IdPhieu",
                principalTable: "sm_PurchaseOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
