using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldQuotationCodeandSellableQuantityintoSalesOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuotationCode",
                table: "sm_SalesOrder",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SellableQuantity",
                table: "sm_SalesOrder",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuotationCode",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "SellableQuantity",
                table: "sm_SalesOrder");
        }
    }
}
