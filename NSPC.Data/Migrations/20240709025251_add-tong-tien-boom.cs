using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addtongtienboom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TongTien",
                table: "mk_NguyenVatLieu",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TongTien",
                table: "mk_Bom",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TongTienThanhToan",
                table: "mk_Bom",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VAT",
                table: "mk_Bom",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TongTien",
                table: "mk_NguyenVatLieu");

            migrationBuilder.DropColumn(
                name: "TongTien",
                table: "mk_Bom");

            migrationBuilder.DropColumn(
                name: "TongTienThanhToan",
                table: "mk_Bom");

            migrationBuilder.DropColumn(
                name: "VAT",
                table: "mk_Bom");
        }
    }
}
