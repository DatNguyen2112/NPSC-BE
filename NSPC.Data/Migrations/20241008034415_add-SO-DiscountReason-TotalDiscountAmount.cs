using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addSODiscountReasonTotalDiscountAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiscountReason",
                table: "sm_SalesOrder",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDiscountAmount",
                table: "sm_SalesOrder",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.RenameColumn(
                name: "IsVATApplied",
                table: "sm_SalesOrderItem",
                newName: "IsProductVATApplied");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "IsProductVATApplied",
                table: "sm_SalesOrderItem",
                newName: "IsVATApplied");

            migrationBuilder.DropColumn(
                name: "DiscountReason",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "TotalDiscountAmount",
                table: "sm_SalesOrder");
        }
    }
}
