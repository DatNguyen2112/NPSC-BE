using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addSalesOrderItemProductVATfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVATApplied",
                table: "sm_SalesOrderItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VATCode",
                table: "sm_SalesOrderItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExportVATCode",
                table: "sm_Product",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExportVATPercent",
                table: "sm_Product",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ImportVATCode",
                table: "sm_Product",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ImportVATPercent",
                table: "sm_Product",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsVATApplied",
                table: "sm_Product",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVATApplied",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropColumn(
                name: "VATCode",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropColumn(
                name: "ExportVATCode",
                table: "sm_Product");

            migrationBuilder.DropColumn(
                name: "ExportVATPercent",
                table: "sm_Product");

            migrationBuilder.DropColumn(
                name: "ImportVATCode",
                table: "sm_Product");

            migrationBuilder.DropColumn(
                name: "ImportVATPercent",
                table: "sm_Product");

            migrationBuilder.DropColumn(
                name: "IsVATApplied",
                table: "sm_Product");
        }
    }
}
