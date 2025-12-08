using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameTotalPaymentContinuetoTotalRemainingAmountinPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPaymentContinue",
                table: "sm_PurchaseOrder",
                newName: "TotalRemainingAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalRemainingAmount",
                table: "sm_PurchaseOrder",
                newName: "TotalPaymentContinue");
        }
    }
}
