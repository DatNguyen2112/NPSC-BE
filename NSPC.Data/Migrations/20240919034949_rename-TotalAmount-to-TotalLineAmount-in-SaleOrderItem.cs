using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameTotalAmounttoTotalLineAmountinSaleOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "sm_SaleOrderItem",
                newName: "TotalLineAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalLineAmount",
                table: "sm_SaleOrderItem",
                newName: "TotalAmount");
        }
    }
}
