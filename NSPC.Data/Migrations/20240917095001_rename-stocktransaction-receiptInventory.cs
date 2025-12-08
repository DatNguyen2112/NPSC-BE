using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renamestocktransactionreceiptInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecieptInventory",
                table: "sm_Stock_Transaction",
                newName: "ReceiptInventory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiptInventory",
                table: "sm_Stock_Transaction",
                newName: "RecieptInventory");
        }
    }
}
