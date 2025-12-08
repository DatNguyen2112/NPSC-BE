using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renamecolumnReceiptInventoryAmout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiptInventoryAmout",
                table: "sm_Stock_Transaction",
                newName: "ReceiptInventoryAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiptInventoryAmount",
                table: "sm_Stock_Transaction",
                newName: "ReceiptInventoryAmout");
        }
    }
}
