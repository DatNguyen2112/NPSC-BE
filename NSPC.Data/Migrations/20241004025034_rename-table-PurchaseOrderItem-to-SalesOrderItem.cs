using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renametablePurchaseOrderItemtoSalesOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_Product_ProductId",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_SalesOrder_PurchaseOrderId",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sm_PurchaseOrderItem",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.RenameTable(
                name: "sm_PurchaseOrderItem",
                newName: "sm_SalesOrderItem");

            migrationBuilder.RenameIndex(
                name: "IX_sm_PurchaseOrderItem_PurchaseOrderId",
                table: "sm_SalesOrderItem",
                newName: "IX_sm_SalesOrderItem_PurchaseOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_PurchaseOrderItem_ProductId",
                table: "sm_SalesOrderItem",
                newName: "IX_sm_SalesOrderItem_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sm_SalesOrderItem",
                table: "sm_SalesOrderItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SalesOrderItem_sm_Product_ProductId",
                table: "sm_SalesOrderItem",
                column: "ProductId",
                principalTable: "sm_Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SalesOrderItem_sm_SalesOrder_PurchaseOrderId",
                table: "sm_SalesOrderItem",
                column: "PurchaseOrderId",
                principalTable: "sm_SalesOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_SalesOrderItem_sm_Product_ProductId",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_SalesOrderItem_sm_SalesOrder_PurchaseOrderId",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sm_SalesOrderItem",
                table: "sm_SalesOrderItem");

            migrationBuilder.RenameTable(
                name: "sm_SalesOrderItem",
                newName: "sm_PurchaseOrderItem");

            migrationBuilder.RenameIndex(
                name: "IX_sm_SalesOrderItem_PurchaseOrderId",
                table: "sm_PurchaseOrderItem",
                newName: "IX_sm_PurchaseOrderItem_PurchaseOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_SalesOrderItem_ProductId",
                table: "sm_PurchaseOrderItem",
                newName: "IX_sm_PurchaseOrderItem_ProductId");

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
                name: "FK_sm_PurchaseOrderItem_sm_SalesOrder_PurchaseOrderId",
                table: "sm_PurchaseOrderItem",
                column: "PurchaseOrderId",
                principalTable: "sm_SalesOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
