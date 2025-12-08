using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addtblkiemkhoxuatnhapton : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mk_KiemKho",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ma = table.Column<string>(type: "text", nullable: true),
                    MucDich = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    ListKho = table.Column<List<string>>(type: "text[]", nullable: true),
                    DenNgay = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_KiemKho", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_KiemKho_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mk_XuatNhapTon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaVatTu = table.Column<string>(type: "text", nullable: true),
                    TenVatTu = table.Column<string>(type: "text", nullable: true),
                    DonGia = table.Column<decimal>(type: "numeric", nullable: true),
                    DonViTinh = table.Column<string>(type: "text", nullable: true),
                    MaKho = table.Column<string>(type: "text", nullable: true),
                    SoLuong = table.Column<int>(type: "integer", nullable: true),
                    SoLuongKiemKe = table.Column<int>(type: "integer", nullable: true),
                    SoLuongChenhLech = table.Column<int>(type: "integer", nullable: true),
                    TongTien = table.Column<decimal>(type: "numeric", nullable: true),
                    LoaiXuatNhapTon = table.Column<string>(type: "text", nullable: true),
                    IdKiemKho = table.Column<Guid>(type: "uuid", nullable: true),
                    IdVatTu = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_XuatNhapTon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_XuatNhapTon_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_XuatNhapTon_mk_KiemKho_IdKiemKho",
                        column: x => x.IdKiemKho,
                        principalTable: "mk_KiemKho",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_XuatNhapTon_mk_VatTu_IdVatTu",
                        column: x => x.IdVatTu,
                        principalTable: "mk_VatTu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_KiemKho_CreatedByUserId",
                table: "mk_KiemKho",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_XuatNhapTon_CreatedByUserId",
                table: "mk_XuatNhapTon",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_XuatNhapTon_IdKiemKho",
                table: "mk_XuatNhapTon",
                column: "IdKiemKho");

            migrationBuilder.CreateIndex(
                name: "IX_mk_XuatNhapTon_IdVatTu",
                table: "mk_XuatNhapTon",
                column: "IdVatTu");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_XuatNhapTon");

            migrationBuilder.DropTable(
                name: "mk_KiemKho");
        }
    }
}
