using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class refactorsm_SalesOrderItemaddfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "sm_SalesOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "sm_SalesOrderItem",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "sm_SalesOrderItem",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VATAmount",
                table: "sm_SalesOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATPercent",
                table: "sm_SalesOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATableAmount",
                table: "sm_SalesOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropColumn(
                name: "VATAmount",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropColumn(
                name: "VATPercent",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropColumn(
                name: "VATableAmount",
                table: "sm_SalesOrderItem");
        }
    }
}
