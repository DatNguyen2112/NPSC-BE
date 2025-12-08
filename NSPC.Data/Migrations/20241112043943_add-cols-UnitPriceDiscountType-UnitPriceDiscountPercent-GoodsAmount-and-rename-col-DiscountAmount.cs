using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addcolsUnitPriceDiscountTypeUnitPriceDiscountPercentGoodsAmountandrenamecolDiscountAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "sm_QuotationItem",
                newName: "UnitPriceDiscountAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "GoodsAmount",
                table: "sm_QuotationItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPriceDiscountPercent",
                table: "sm_QuotationItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitPriceDiscountType",
                table: "sm_QuotationItem",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoodsAmount",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "UnitPriceDiscountPercent",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "UnitPriceDiscountType",
                table: "sm_QuotationItem");

            migrationBuilder.RenameColumn(
                name: "UnitPriceDiscountAmount",
                table: "sm_QuotationItem",
                newName: "DiscountAmount");
        }
    }
}
