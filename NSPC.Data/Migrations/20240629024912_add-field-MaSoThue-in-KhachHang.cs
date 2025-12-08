using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldMaSoThueinKhachHang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaSoThue",
                table: "sm_KhachHang",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverMST",
                table: "mk_BaoGia",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoPhieu",
                table: "mk_BaoGia",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaSoThue",
                table: "sm_KhachHang");

            migrationBuilder.DropColumn(
                name: "ReceiverMST",
                table: "mk_BaoGia");

            migrationBuilder.DropColumn(
                name: "SoPhieu",
                table: "mk_BaoGia");
        }
    }
}
