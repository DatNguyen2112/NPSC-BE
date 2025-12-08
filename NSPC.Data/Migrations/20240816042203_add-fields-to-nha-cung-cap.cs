using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldstonhacungcap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "mk_NhaCungCap",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "mk_NhaCungCap",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GiaMacDinh",
                table: "mk_NhaCungCap",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhThucThanhToan",
                table: "mk_NhaCungCap",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaSoThue",
                table: "mk_NhaCungCap",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NguoiPhuTrach",
                table: "mk_NhaCungCap",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "mk_NhaCungCap",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "mk_NhaCungCap");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "mk_NhaCungCap");

            migrationBuilder.DropColumn(
                name: "GiaMacDinh",
                table: "mk_NhaCungCap");

            migrationBuilder.DropColumn(
                name: "HinhThucThanhToan",
                table: "mk_NhaCungCap");

            migrationBuilder.DropColumn(
                name: "MaSoThue",
                table: "mk_NhaCungCap");

            migrationBuilder.DropColumn(
                name: "NguoiPhuTrach",
                table: "mk_NhaCungCap");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "mk_NhaCungCap");
        }
    }
}
