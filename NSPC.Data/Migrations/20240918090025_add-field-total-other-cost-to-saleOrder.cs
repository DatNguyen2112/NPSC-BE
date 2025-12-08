using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldtotalothercosttosaleOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "VAT",
                table: "sm_SaleOrders",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalOtherCost",
                table: "sm_SaleOrders",
                type: "numeric",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "sm_PurchaseOrder",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalOtherCost",
                table: "sm_SaleOrders");

            migrationBuilder.AlterColumn<double>(
                name: "VAT",
                table: "sm_SaleOrders",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "sm_PurchaseOrder",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}
