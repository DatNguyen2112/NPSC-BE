using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removefieldSellableQuantityinSalesOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellableQuantity",
                table: "sm_SalesOrder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SellableQuantity",
                table: "sm_SalesOrder",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
