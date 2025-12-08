using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addvattongtienphieunhapxuat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TongTien",
                table: "mk_QuanLyPhieu",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VAT",
                table: "mk_QuanLyPhieu",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TongTien",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "VAT",
                table: "mk_QuanLyPhieu");
        }
    }
}
