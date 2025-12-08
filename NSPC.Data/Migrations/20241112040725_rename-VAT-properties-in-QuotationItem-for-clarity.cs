using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameVATpropertiesinQuotationItemforclarity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VATableAmount",
                table: "sm_QuotationItem",
                newName: "LineVATableAmount");

            migrationBuilder.RenameColumn(
                name: "VATPercent",
                table: "sm_QuotationItem",
                newName: "LineVATPercent");

            migrationBuilder.RenameColumn(
                name: "VATCode",
                table: "sm_QuotationItem",
                newName: "LineVATCode");

            migrationBuilder.RenameColumn(
                name: "VATAmount",
                table: "sm_QuotationItem",
                newName: "LineVATAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LineVATableAmount",
                table: "sm_QuotationItem",
                newName: "VATableAmount");

            migrationBuilder.RenameColumn(
                name: "LineVATPercent",
                table: "sm_QuotationItem",
                newName: "VATPercent");

            migrationBuilder.RenameColumn(
                name: "LineVATCode",
                table: "sm_QuotationItem",
                newName: "VATCode");

            migrationBuilder.RenameColumn(
                name: "LineVATAmount",
                table: "sm_QuotationItem",
                newName: "VATAmount");
        }
    }
}
