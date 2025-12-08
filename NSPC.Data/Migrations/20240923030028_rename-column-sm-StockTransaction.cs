using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renamecolumnsmStockTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiptInventory",
                table: "sm_Stock_Transaction",
                newName: "ReceiptInventoryQuantity");

            migrationBuilder.RenameColumn(
                name: "OpeningInventory",
                table: "sm_Stock_Transaction",
                newName: "OpeningInventoryQuantity");

            migrationBuilder.RenameColumn(
                name: "ClosingInventory",
                table: "sm_Stock_Transaction",
                newName: "ClosingInventoryQuantity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiptInventoryQuantity",
                table: "sm_Stock_Transaction",
                newName: "ReceiptInventory");

            migrationBuilder.RenameColumn(
                name: "OpeningInventoryQuantity",
                table: "sm_Stock_Transaction",
                newName: "OpeningInventory");

            migrationBuilder.RenameColumn(
                name: "ClosingInventoryQuantity",
                table: "sm_Stock_Transaction",
                newName: "ClosingInventory");
        }
    }
}
