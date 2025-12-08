using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameHalfPaymenttoListPaymentinPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HalfPayment",
                table: "sm_PurchaseOrder",
                newName: "ListPayment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ListPayment",
                table: "sm_PurchaseOrder",
                newName: "HalfPayment");
        }
    }
}
