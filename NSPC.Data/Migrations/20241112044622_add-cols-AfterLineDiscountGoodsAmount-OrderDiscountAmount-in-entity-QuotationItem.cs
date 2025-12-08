using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addcolsAfterLineDiscountGoodsAmountOrderDiscountAmountinentityQuotationItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AfterLineDiscountGoodsAmount",
                table: "sm_QuotationItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderDiscountAmount",
                table: "sm_QuotationItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfterLineDiscountGoodsAmount",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "OrderDiscountAmount",
                table: "sm_QuotationItem");
        }
    }
}
