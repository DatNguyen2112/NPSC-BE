using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldsBangTinhLuongItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AnCa",
                table: "mk_ChamCongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DienThoai",
                table: "mk_ChamCongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LuongCoBan",
                table: "mk_ChamCongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TrangPhuc",
                table: "mk_ChamCongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BhtnNLD",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BhtnNSDLD",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BhxhNLD",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BhxhNSDLD",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BhytNLD",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BhytNSDLD",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "mk_BangLuongItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GiamTruBanThan",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayNhan",
                table: "mk_BangLuongItem",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SoNguoiPhuThuoc",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SoTienPhuThuoc",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ThuNhapCaNhan",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ThuNhapTinhThue",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ThueTNCNPhaiNop",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TongNLD",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TongNSDLD",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TongTatCa",
                table: "mk_BangLuongItem",
                type: "numeric",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnCa",
                table: "mk_ChamCongItem");

            migrationBuilder.DropColumn(
                name: "DienThoai",
                table: "mk_ChamCongItem");

            migrationBuilder.DropColumn(
                name: "LuongCoBan",
                table: "mk_ChamCongItem");

            migrationBuilder.DropColumn(
                name: "TrangPhuc",
                table: "mk_ChamCongItem");

            migrationBuilder.DropColumn(
                name: "BhtnNLD",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "BhtnNSDLD",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "BhxhNLD",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "BhxhNSDLD",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "BhytNLD",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "BhytNSDLD",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "GiamTruBanThan",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "NgayNhan",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "SoNguoiPhuThuoc",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "SoTienPhuThuoc",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "ThuNhapCaNhan",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "ThuNhapTinhThue",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "ThueTNCNPhaiNop",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "TongNLD",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "TongNSDLD",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "TongTatCa",
                table: "mk_BangLuongItem");
        }
    }
}
