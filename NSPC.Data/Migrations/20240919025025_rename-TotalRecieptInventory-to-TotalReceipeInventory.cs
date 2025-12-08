using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameTotalRecieptInventorytoTotalReceipeInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalRecieptInventory",
                table: "sm_PurchaseOrder",
                newName: "TotalReceiptInventory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalReceiptInventory",
                table: "sm_PurchaseOrder",
                newName: "TotalRecieptInventory");
        }
    }
}
