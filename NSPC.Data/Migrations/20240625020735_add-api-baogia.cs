using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addapibaogia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_BaoGiaItem_mk_SanPham_IdSanPham",
                table: "mk_BaoGiaItem");

            migrationBuilder.DropIndex(
                name: "IX_mk_BaoGiaItem_IdSanPham",
                table: "mk_BaoGiaItem");

            migrationBuilder.DropColumn(
                name: "IdSanPham",
                table: "mk_BaoGiaItem");

            migrationBuilder.RenameColumn(
                name: "DonVi",
                table: "mk_BaoGiaItem",
                newName: "TenVatTu");

            migrationBuilder.RenameColumn(
                name: "BaoGiaType",
                table: "mk_BaoGiaItem",
                newName: "MaVatTu");

            migrationBuilder.RenameColumn(
                name: "TongTien",
                table: "mk_BaoGia",
                newName: "TongTienThanhToan");

            migrationBuilder.AddColumn<string>(
                name: "DonViTinh",
                table: "mk_BaoGiaItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiBaoGia",
                table: "mk_BaoGia",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaDonHang",
                table: "mk_BaoGia",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonViTinh",
                table: "mk_BaoGiaItem");

            migrationBuilder.DropColumn(
                name: "LoaiBaoGia",
                table: "mk_BaoGia");

            migrationBuilder.DropColumn(
                name: "MaDonHang",
                table: "mk_BaoGia");

            migrationBuilder.RenameColumn(
                name: "TenVatTu",
                table: "mk_BaoGiaItem",
                newName: "DonVi");

            migrationBuilder.RenameColumn(
                name: "MaVatTu",
                table: "mk_BaoGiaItem",
                newName: "BaoGiaType");

            migrationBuilder.RenameColumn(
                name: "TongTienThanhToan",
                table: "mk_BaoGia",
                newName: "TongTien");

            migrationBuilder.AddColumn<Guid>(
                name: "IdSanPham",
                table: "mk_BaoGiaItem",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_mk_BaoGiaItem_IdSanPham",
                table: "mk_BaoGiaItem",
                column: "IdSanPham");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_BaoGiaItem_mk_SanPham_IdSanPham",
                table: "mk_BaoGiaItem",
                column: "IdSanPham",
                principalTable: "mk_SanPham",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
