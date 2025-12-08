using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updateTableStockTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "RecieptInventory",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OpeningInventory",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "InventoryIncreased",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "InventoryDecreased",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExportInventory",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ClosingInventory",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionCode",
                table: "sm_Stock_Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StockTransactionQuantity",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IntitalInventory",
                table: "sm_Product",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_ProjectId",
                table: "sm_Cashbook_Transaction",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Cashbook_Transaction_mk_DuAn_ProjectId",
                table: "sm_Cashbook_Transaction",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Cashbook_Transaction_mk_DuAn_ProjectId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Cashbook_Transaction_ProjectId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "ActionCode",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropColumn(
                name: "StockTransactionQuantity",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropColumn(
                name: "IntitalInventory",
                table: "sm_Product");

            migrationBuilder.AlterColumn<int>(
                name: "RecieptInventory",
                table: "sm_Stock_Transaction",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OpeningInventory",
                table: "sm_Stock_Transaction",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InventoryIncreased",
                table: "sm_Stock_Transaction",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InventoryDecreased",
                table: "sm_Stock_Transaction",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExportInventory",
                table: "sm_Stock_Transaction",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClosingInventory",
                table: "sm_Stock_Transaction",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}
