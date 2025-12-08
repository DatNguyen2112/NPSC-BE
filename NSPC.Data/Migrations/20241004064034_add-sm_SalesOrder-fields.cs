using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addsm_SalesOrderfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFee",
                table: "sm_SalesOrder",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "sm_SalesOrder",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "sm_SalesOrder",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "sm_SalesOrder",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "sm_SalesOrder",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATAmount",
                table: "sm_SalesOrder",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "VATAmount",
                table: "sm_SalesOrder");
        }
    }
}
