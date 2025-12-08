using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameTotalPaymentContinuetoTotalRemaningAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPaymentContinue",
                table: "sm_SaleOrders",
                newName: "TotalRemaningAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalRemaningAmount",
                table: "sm_SaleOrders",
                newName: "TotalPaymentContinue");
        }
    }
}
