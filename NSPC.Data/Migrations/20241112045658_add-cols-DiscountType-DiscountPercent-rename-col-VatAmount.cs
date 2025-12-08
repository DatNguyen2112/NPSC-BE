using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addcolsDiscountTypeDiscountPercentrenamecolVatAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VatAmount",
                table: "sm_Quotation",
                newName: "TotalVatAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "sm_Quotation",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "sm_Quotation",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "sm_Quotation");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "sm_Quotation");

            migrationBuilder.RenameColumn(
                name: "TotalVatAmount",
                table: "sm_Quotation",
                newName: "VatAmount");
        }
    }
}
