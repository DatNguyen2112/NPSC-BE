using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addcolumnsfortaxpurposes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProductVATApplied",
                table: "sm_QuotationItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "VATAmount",
                table: "sm_QuotationItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "VATCode",
                table: "sm_QuotationItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VATPercent",
                table: "sm_QuotationItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATableAmount",
                table: "sm_QuotationItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProductVATApplied",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "VATAmount",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "VATCode",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "VATPercent",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "VATableAmount",
                table: "sm_QuotationItem");
        }
    }
}
