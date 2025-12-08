using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addapibangtinhluong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mk_BangTinhLuong",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenBangTinhLuong = table.Column<string>(type: "text", nullable: true),
                    TenCongTy = table.Column<string>(type: "text", nullable: true),
                    DiaChiCongTy = table.Column<string>(type: "text", nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SoNgayCongTrongThang = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_BangTinhLuong", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mk_BangLuongItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaSo = table.Column<string>(type: "text", nullable: true),
                    TenNhanVien = table.Column<string>(type: "text", nullable: true),
                    ChucVu = table.Column<string>(type: "text", nullable: true),
                    LuongCoBan = table.Column<decimal>(type: "numeric", nullable: true),
                    Tong = table.Column<decimal>(type: "numeric", nullable: true),
                    NgayCong = table.Column<decimal>(type: "numeric", nullable: true),
                    Luong = table.Column<decimal>(type: "numeric", nullable: true),
                    ThucLinh = table.Column<decimal>(type: "numeric", nullable: true),
                    IdBangTinhLuong = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_BangLuongItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_BangLuongItem_mk_BangTinhLuong_IdBangTinhLuong",
                        column: x => x.IdBangTinhLuong,
                        principalTable: "mk_BangTinhLuong",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mk_CacKhoanTroCap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnCa = table.Column<decimal>(type: "numeric", nullable: true),
                    DienThoai = table.Column<decimal>(type: "numeric", nullable: true),
                    TrangPhuc = table.Column<decimal>(type: "numeric", nullable: true),
                    mk_BangLuongItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_CacKhoanTroCap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_CacKhoanTroCap_mk_BangLuongItem_mk_BangLuongItemId",
                        column: x => x.mk_BangLuongItemId,
                        principalTable: "mk_BangLuongItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_BangLuongItem_IdBangTinhLuong",
                table: "mk_BangLuongItem",
                column: "IdBangTinhLuong");

            migrationBuilder.CreateIndex(
                name: "IX_mk_CacKhoanTroCap_mk_BangLuongItemId",
                table: "mk_CacKhoanTroCap",
                column: "mk_BangLuongItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_CacKhoanTroCap");

            migrationBuilder.DropTable(
                name: "mk_BangLuongItem");

            migrationBuilder.DropTable(
                name: "mk_BangTinhLuong");
        }
    }
}
