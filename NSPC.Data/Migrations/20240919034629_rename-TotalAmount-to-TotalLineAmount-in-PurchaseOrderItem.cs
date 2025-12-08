using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameTotalAmounttoTotalLineAmountinPurchaseOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "sm_PurchaseOrderItem",
                newName: "TotalLineAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalLineAmount",
                table: "sm_PurchaseOrderItem",
                newName: "TotalAmount");
        }
    }
}
