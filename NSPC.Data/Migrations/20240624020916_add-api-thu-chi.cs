using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addapithuchi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_ChiPhi");

            migrationBuilder.AddColumn<string>(
                name: "MaPhieu",
                table: "mk_QuanLyPhieu",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "mk_ThuChi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LoaiChiPhi = table.Column<string>(type: "text", nullable: true),
                    MaChi = table.Column<string>(type: "text", nullable: true),
                    IdMaNhaCungCap = table.Column<Guid>(type: "uuid", nullable: true),
                    ChiChoMucDich = table.Column<string>(type: "text", nullable: true),
                    IdKhachHang = table.Column<Guid>(type: "uuid", nullable: true),
                    SoTien = table.Column<decimal>(type: "numeric", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    IdDuAn = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_ThuChi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_ThuChi_mk_DuAn_IdDuAn",
                        column: x => x.IdDuAn,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_ThuChi_mk_NhaCungCap_IdMaNhaCungCap",
                        column: x => x.IdMaNhaCungCap,
                        principalTable: "mk_NhaCungCap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_ThuChi_sm_KhachHang_IdKhachHang",
                        column: x => x.IdKhachHang,
                        principalTable: "sm_KhachHang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_IdDuAn",
                table: "mk_ThuChi",
                column: "IdDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_IdKhachHang",
                table: "mk_ThuChi",
                column: "IdKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_IdMaNhaCungCap",
                table: "mk_ThuChi",
                column: "IdMaNhaCungCap");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_ThuChi");

            migrationBuilder.DropColumn(
                name: "MaPhieu",
                table: "mk_QuanLyPhieu");

            migrationBuilder.CreateTable(
                name: "mk_ChiPhi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdDuAn = table.Column<Guid>(type: "uuid", nullable: false),
                    IdKhachHang = table.Column<Guid>(type: "uuid", nullable: false),
                    IdMaNhaCungCap = table.Column<Guid>(type: "uuid", nullable: false),
                    ChiChoMucDich = table.Column<string>(type: "text", nullable: true),
                    ChiPhiType = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MaChi = table.Column<string>(type: "text", nullable: true),
                    SoTien = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_ChiPhi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_ChiPhi_mk_DuAn_IdDuAn",
                        column: x => x.IdDuAn,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_ChiPhi_mk_NhaCungCap_IdMaNhaCungCap",
                        column: x => x.IdMaNhaCungCap,
                        principalTable: "mk_NhaCungCap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_ChiPhi_sm_KhachHang_IdKhachHang",
                        column: x => x.IdKhachHang,
                        principalTable: "sm_KhachHang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_ChiPhi_IdDuAn",
                table: "mk_ChiPhi",
                column: "IdDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ChiPhi_IdKhachHang",
                table: "mk_ChiPhi",
                column: "IdKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ChiPhi_IdMaNhaCungCap",
                table: "mk_ChiPhi",
                column: "IdMaNhaCungCap");
        }
    }
}
