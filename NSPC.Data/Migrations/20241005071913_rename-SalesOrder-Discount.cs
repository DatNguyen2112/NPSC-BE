using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameSalesOrderDiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountType",
                table: "sm_SalesOrderItem",
                newName: "UnitPriceDiscountType");

            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "sm_SalesOrderItem",
                newName: "UnitPriceDiscountPercent");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "sm_SalesOrderItem",
                newName: "UnitPriceDiscountAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitPriceDiscountType",
                table: "sm_SalesOrderItem",
                newName: "DiscountType");

            migrationBuilder.RenameColumn(
                name: "UnitPriceDiscountPercent",
                table: "sm_SalesOrderItem",
                newName: "DiscountPercent");

            migrationBuilder.RenameColumn(
                name: "UnitPriceDiscountAmount",
                table: "sm_SalesOrderItem",
                newName: "DiscountAmount");
        }
    }
}
