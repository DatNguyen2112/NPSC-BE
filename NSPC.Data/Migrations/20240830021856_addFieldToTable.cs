using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addFieldToTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "mk_QuanLyPhieu",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "mk_QuanLyPhieu",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiveInventoryStatus",
                table: "mk_QuanLyPhieu",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDiscountsAccount",
                table: "mk_QuanLyPhieu",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalTaxAccount",
                table: "mk_QuanLyPhieu",
                type: "numeric",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "ReceiveInventoryStatus",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "TotalDiscountsAccount",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "TotalTaxAccount",
                table: "mk_QuanLyPhieu");
        }
    }
}
