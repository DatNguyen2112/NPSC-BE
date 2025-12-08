using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renamePurchaseOrderfieldsandFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_PurchaseOrder_IdPhieu",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.RenameColumn(
                name: "SalesOrderDiscountAmount",
                table: "sm_SalesOrderItem",
                newName: "OrderDiscountAmount");

            migrationBuilder.RenameColumn(
                name: "SalesOrderDiscountAmount",
                table: "sm_PurchaseOrderItem",
                newName: "OrderDiscountAmount");

            migrationBuilder.RenameColumn(
                name: "IdPhieu",
                table: "sm_PurchaseOrderItem",
                newName: "PurchaseOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_PurchaseOrderItem_IdPhieu",
                table: "sm_PurchaseOrderItem",
                newName: "IX_sm_PurchaseOrderItem_PurchaseOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_PurchaseOrder_PurchaseOrderId",
                table: "sm_PurchaseOrderItem",
                column: "PurchaseOrderId",
                principalTable: "sm_PurchaseOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_PurchaseOrder_PurchaseOrderId",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.RenameColumn(
                name: "OrderDiscountAmount",
                table: "sm_SalesOrderItem",
                newName: "SalesOrderDiscountAmount");

            migrationBuilder.RenameColumn(
                name: "PurchaseOrderId",
                table: "sm_PurchaseOrderItem",
                newName: "IdPhieu");

            migrationBuilder.RenameColumn(
                name: "OrderDiscountAmount",
                table: "sm_PurchaseOrderItem",
                newName: "SalesOrderDiscountAmount");

            migrationBuilder.RenameIndex(
                name: "IX_sm_PurchaseOrderItem_PurchaseOrderId",
                table: "sm_PurchaseOrderItem",
                newName: "IX_sm_PurchaseOrderItem_IdPhieu");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_PurchaseOrder_IdPhieu",
                table: "sm_PurchaseOrderItem",
                column: "IdPhieu",
                principalTable: "sm_PurchaseOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
