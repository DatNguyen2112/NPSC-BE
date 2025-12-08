using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addapiquanlyphieunhapxuat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_QuanLyKho");

            migrationBuilder.AddColumn<Guid>(
                name: "IdVatTu",
                table: "mk_VatTu",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "mk_PhieuNhapKho",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaKho = table.Column<string>(type: "text", nullable: true),
                    MaDonHang = table.Column<string>(type: "text", nullable: true),
                    MucDichNhapKho = table.Column<string>(type: "text", nullable: true),
                    IdVatTu = table.Column<Guid>(type: "uuid", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: true),
                    GiaTien = table.Column<decimal>(type: "numeric", nullable: true),
                    TongTien = table.Column<decimal>(type: "numeric", nullable: true),
                    IdDuAn = table.Column<Guid>(type: "uuid", nullable: false),
                    IdNhaCungCap = table.Column<Guid>(type: "uuid", nullable: false),
                    sm_CodeTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_PhieuNhapKho", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_PhieuNhapKho_mk_DuAn_IdDuAn",
                        column: x => x.IdDuAn,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_PhieuNhapKho_mk_NhaCungCap_IdNhaCungCap",
                        column: x => x.IdNhaCungCap,
                        principalTable: "mk_NhaCungCap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_PhieuNhapKho_sm_CodeType_sm_CodeTypeId",
                        column: x => x.sm_CodeTypeId,
                        principalTable: "sm_CodeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mk_PhieuXuatKho",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaKho = table.Column<string>(type: "text", nullable: true),
                    MaDonHang = table.Column<string>(type: "text", nullable: true),
                    MucDichNhapKho = table.Column<string>(type: "text", nullable: true),
                    IdVatTu = table.Column<Guid>(type: "uuid", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: true),
                    GiaTien = table.Column<decimal>(type: "numeric", nullable: true),
                    TongTien = table.Column<decimal>(type: "numeric", nullable: true),
                    IdDuAn = table.Column<Guid>(type: "uuid", nullable: false),
                    IdNhaCungCap = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_PhieuXuatKho", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_PhieuXuatKho_mk_DuAn_IdDuAn",
                        column: x => x.IdDuAn,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_PhieuXuatKho_mk_NhaCungCap_IdNhaCungCap",
                        column: x => x.IdNhaCungCap,
                        principalTable: "mk_NhaCungCap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_VatTu_IdVatTu",
                table: "mk_VatTu",
                column: "IdVatTu");

            migrationBuilder.CreateIndex(
                name: "IX_mk_PhieuNhapKho_IdDuAn",
                table: "mk_PhieuNhapKho",
                column: "IdDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_mk_PhieuNhapKho_IdNhaCungCap",
                table: "mk_PhieuNhapKho",
                column: "IdNhaCungCap");

            migrationBuilder.CreateIndex(
                name: "IX_mk_PhieuNhapKho_sm_CodeTypeId",
                table: "mk_PhieuNhapKho",
                column: "sm_CodeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_PhieuXuatKho_IdDuAn",
                table: "mk_PhieuXuatKho",
                column: "IdDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_mk_PhieuXuatKho_IdNhaCungCap",
                table: "mk_PhieuXuatKho",
                column: "IdNhaCungCap");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_VatTu_mk_PhieuNhapKho_IdVatTu",
                table: "mk_VatTu",
                column: "IdVatTu",
                principalTable: "mk_PhieuNhapKho",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_VatTu_mk_PhieuXuatKho_IdVatTu",
                table: "mk_VatTu",
                column: "IdVatTu",
                principalTable: "mk_PhieuXuatKho",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_VatTu_mk_PhieuNhapKho_IdVatTu",
                table: "mk_VatTu");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_VatTu_mk_PhieuXuatKho_IdVatTu",
                table: "mk_VatTu");

            migrationBuilder.DropTable(
                name: "mk_PhieuNhapKho");

            migrationBuilder.DropTable(
                name: "mk_PhieuXuatKho");

            migrationBuilder.DropIndex(
                name: "IX_mk_VatTu_IdVatTu",
                table: "mk_VatTu");

            migrationBuilder.DropColumn(
                name: "IdVatTu",
                table: "mk_VatTu");

            migrationBuilder.CreateTable(
                name: "mk_QuanLyKho",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdDuAn = table.Column<Guid>(type: "uuid", nullable: false),
                    IdNhaCungCap = table.Column<Guid>(type: "uuid", nullable: false),
                    IdVatTu = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GiaTien = table.Column<decimal>(type: "numeric", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LoaiPhieu = table.Column<string>(type: "text", nullable: true),
                    MaDonHang = table.Column<string>(type: "text", nullable: true),
                    MaKho = table.Column<string>(type: "text", nullable: true),
                    MucDichNhapKho = table.Column<string>(type: "text", nullable: true),
                    SoLuong = table.Column<int>(type: "integer", nullable: true),
                    TongTien = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_QuanLyKho", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_QuanLyKho_mk_DuAn_IdDuAn",
                        column: x => x.IdDuAn,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_QuanLyKho_mk_NhaCungCap_IdNhaCungCap",
                        column: x => x.IdNhaCungCap,
                        principalTable: "mk_NhaCungCap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_QuanLyKho_mk_VatTu_IdVatTu",
                        column: x => x.IdVatTu,
                        principalTable: "mk_VatTu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_QuanLyKho_IdDuAn",
                table: "mk_QuanLyKho",
                column: "IdDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_mk_QuanLyKho_IdNhaCungCap",
                table: "mk_QuanLyKho",
                column: "IdNhaCungCap");

            migrationBuilder.CreateIndex(
                name: "IX_mk_QuanLyKho_IdVatTu",
                table: "mk_QuanLyKho",
                column: "IdVatTu");
        }
    }
}
