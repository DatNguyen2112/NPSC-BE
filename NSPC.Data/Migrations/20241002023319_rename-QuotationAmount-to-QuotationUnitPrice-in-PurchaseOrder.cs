using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameQuotationAmounttoQuotationUnitPriceinPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuotationAmount",
                table: "sm_PurchaseOrderItem",
                newName: "QuotationUnitPrice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuotationUnitPrice",
                table: "sm_PurchaseOrderItem",
                newName: "QuotationAmount");
        }
    }
}
