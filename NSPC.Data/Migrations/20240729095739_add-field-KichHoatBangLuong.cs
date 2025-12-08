using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldKichHoatBangLuong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KichHoatBangLuong",
                table: "mk_ChamCong",
                newName: "KichHoatBangChamCong");

            migrationBuilder.AddColumn<bool>(
                name: "KichHoatBangLuong",
                table: "mk_BangTinhLuong",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KichHoatBangLuong",
                table: "mk_BangTinhLuong");

            migrationBuilder.RenameColumn(
                name: "KichHoatBangChamCong",
                table: "mk_ChamCong",
                newName: "KichHoatBangLuong");
        }
    }
}
