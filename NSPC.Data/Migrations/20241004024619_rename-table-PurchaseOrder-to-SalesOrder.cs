using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renametablePurchaseOrdertoSalesOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrder_sm_Customer_CustomerId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_PurchaseOrder_PurchaseOrderId",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sm_PurchaseOrder",
                table: "sm_PurchaseOrder");

            migrationBuilder.RenameTable(
                name: "sm_PurchaseOrder",
                newName: "sm_SalesOrder");

            migrationBuilder.RenameIndex(
                name: "IX_sm_PurchaseOrder_CustomerId",
                table: "sm_SalesOrder",
                newName: "IX_sm_SalesOrder_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sm_SalesOrder",
                table: "sm_SalesOrder",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_SalesOrder_PurchaseOrderId",
                table: "sm_PurchaseOrderItem",
                column: "PurchaseOrderId",
                principalTable: "sm_SalesOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SalesOrder_sm_Customer_CustomerId",
                table: "sm_SalesOrder",
                column: "CustomerId",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_SalesOrder_PurchaseOrderId",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_SalesOrder_sm_Customer_CustomerId",
                table: "sm_SalesOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sm_SalesOrder",
                table: "sm_SalesOrder");

            migrationBuilder.RenameTable(
                name: "sm_SalesOrder",
                newName: "sm_PurchaseOrder");

            migrationBuilder.RenameIndex(
                name: "IX_sm_SalesOrder_CustomerId",
                table: "sm_PurchaseOrder",
                newName: "IX_sm_PurchaseOrder_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sm_PurchaseOrder",
                table: "sm_PurchaseOrder",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrder_sm_Customer_CustomerId",
                table: "sm_PurchaseOrder",
                column: "CustomerId",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrderItem_sm_PurchaseOrder_PurchaseOrderId",
                table: "sm_PurchaseOrderItem",
                column: "PurchaseOrderId",
                principalTable: "sm_PurchaseOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
