using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addsm_SalesOrderItemGoodsAmountSalesOrderDiscountAmountAfterLineDiscountGoodsAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AfterLineDiscountGoodsAmount",
                table: "sm_SalesOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GoodsAmount",
                table: "sm_SalesOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SalesOrderDiscountAmount",
                table: "sm_SalesOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfterLineDiscountGoodsAmount",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropColumn(
                name: "GoodsAmount",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropColumn(
                name: "SalesOrderDiscountAmount",
                table: "sm_SalesOrderItem");
        }
    }
}
