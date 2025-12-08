using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renametableSaleOrderstoPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_SaleOrderItem_sm_SaleOrders_IdPhieu",
                table: "sm_SaleOrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_SaleOrders_mk_DuAn_mk_DuAnId",
                table: "sm_SaleOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_SaleOrders_sm_Supplier_SupplierId",
                table: "sm_SaleOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sm_SaleOrders",
                table: "sm_SaleOrders");

            migrationBuilder.RenameTable(
                name: "sm_SaleOrders",
                newName: "sm_PurchaseOrder");

            migrationBuilder.RenameIndex(
                name: "IX_sm_SaleOrders_SupplierId",
                table: "sm_PurchaseOrder",
                newName: "IX_sm_PurchaseOrder_SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_SaleOrders_mk_DuAnId",
                table: "sm_PurchaseOrder",
                newName: "IX_sm_PurchaseOrder_mk_DuAnId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sm_PurchaseOrder",
                table: "sm_PurchaseOrder",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrder_mk_DuAn_mk_DuAnId",
                table: "sm_PurchaseOrder",
                column: "mk_DuAnId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrder_sm_Supplier_SupplierId",
                table: "sm_PurchaseOrder",
                column: "SupplierId",
                principalTable: "sm_Supplier",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrder_mk_DuAn_mk_DuAnId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrder_sm_Supplier_SupplierId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_SaleOrderItem_sm_PurchaseOrder_IdPhieu",
                table: "sm_SaleOrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sm_PurchaseOrder",
                table: "sm_PurchaseOrder");

            migrationBuilder.RenameTable(
                name: "sm_PurchaseOrder",
                newName: "sm_SaleOrders");

            migrationBuilder.RenameIndex(
                name: "IX_sm_PurchaseOrder_SupplierId",
                table: "sm_SaleOrders",
                newName: "IX_sm_SaleOrders_SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_PurchaseOrder_mk_DuAnId",
                table: "sm_SaleOrders",
                newName: "IX_sm_SaleOrders_mk_DuAnId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sm_SaleOrders",
                table: "sm_SaleOrders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SaleOrderItem_sm_SaleOrders_IdPhieu",
                table: "sm_SaleOrderItem",
                column: "IdPhieu",
                principalTable: "sm_SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SaleOrders_mk_DuAn_mk_DuAnId",
                table: "sm_SaleOrders",
                column: "mk_DuAnId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SaleOrders_sm_Supplier_SupplierId",
                table: "sm_SaleOrders",
                column: "SupplierId",
                principalTable: "sm_Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
