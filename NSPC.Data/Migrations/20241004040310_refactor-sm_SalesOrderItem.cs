using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class refactorsm_SalesOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_SalesOrderItem_sm_SalesOrder_PurchaseOrderId",
                table: "sm_SalesOrderItem");

            migrationBuilder.RenameColumn(
                name: "TotalLineAmount",
                table: "sm_SalesOrderItem",
                newName: "LineAmount");

            migrationBuilder.RenameColumn(
                name: "PurchaseOrderId",
                table: "sm_SalesOrderItem",
                newName: "SalesOrderId");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "sm_SalesOrderItem",
                newName: "UnitPrice");

            migrationBuilder.RenameIndex(
                name: "IX_sm_SalesOrderItem_PurchaseOrderId",
                table: "sm_SalesOrderItem",
                newName: "IX_sm_SalesOrderItem_SalesOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SalesOrderItem_sm_SalesOrder_SalesOrderId",
                table: "sm_SalesOrderItem",
                column: "SalesOrderId",
                principalTable: "sm_SalesOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_SalesOrderItem_sm_SalesOrder_SalesOrderId",
                table: "sm_SalesOrderItem");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "sm_SalesOrderItem",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "SalesOrderId",
                table: "sm_SalesOrderItem",
                newName: "PurchaseOrderId");

            migrationBuilder.RenameColumn(
                name: "LineAmount",
                table: "sm_SalesOrderItem",
                newName: "TotalLineAmount");

            migrationBuilder.RenameIndex(
                name: "IX_sm_SalesOrderItem_SalesOrderId",
                table: "sm_SalesOrderItem",
                newName: "IX_sm_SalesOrderItem_PurchaseOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SalesOrderItem_sm_SalesOrder_PurchaseOrderId",
                table: "sm_SalesOrderItem",
                column: "PurchaseOrderId",
                principalTable: "sm_SalesOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
