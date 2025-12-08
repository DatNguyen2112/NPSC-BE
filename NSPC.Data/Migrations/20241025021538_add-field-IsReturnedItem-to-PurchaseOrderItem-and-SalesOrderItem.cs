using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldIsReturnedItemtoPurchaseOrderItemandSalesOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReturnedItem",
                table: "sm_SalesOrderItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReturnedItem",
                table: "sm_PurchaseOrderItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReturnedItem",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropColumn(
                name: "IsReturnedItem",
                table: "sm_PurchaseOrderItem");
        }
    }
}
