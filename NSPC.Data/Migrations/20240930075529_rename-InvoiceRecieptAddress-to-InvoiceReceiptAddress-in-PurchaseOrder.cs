using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameInvoiceRecieptAddresstoInvoiceReceiptAddressinPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvoiceRecieptAddress",
                table: "sm_PurchaseOrder",
                newName: "InvoiceReceiptAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvoiceReceiptAddress",
                table: "sm_PurchaseOrder",
                newName: "InvoiceRecieptAddress");
        }
    }
}
