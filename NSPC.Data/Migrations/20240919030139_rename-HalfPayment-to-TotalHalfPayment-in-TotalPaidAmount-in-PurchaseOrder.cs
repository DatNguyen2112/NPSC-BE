using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameHalfPaymenttoTotalHalfPaymentinTotalPaidAmountinPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalHalfPayment",
                table: "sm_PurchaseOrder",
                newName: "TotalPaidAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPaidAmount",
                table: "sm_PurchaseOrder",
                newName: "TotalHalfPayment");
        }
    }
}
