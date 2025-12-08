using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renamePaymentMethodtoPaymentMethodCodeinPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "sm_PurchaseOrder",
                newName: "PaymentMethodCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentMethodCode",
                table: "sm_PurchaseOrder",
                newName: "PaymentMethod");
        }
    }
}
