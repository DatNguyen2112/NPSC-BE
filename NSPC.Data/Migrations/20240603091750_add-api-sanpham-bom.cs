using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addapisanphambom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mk_DuAn",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaDuAn = table.Column<string>(type: "text", nullable: true),
                    TenDuAn = table.Column<string>(type: "text", nullable: true),
                    TongHopThu = table.Column<string>(type: "text", nullable: true),
                    TongHopChi = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_DuAn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mk_NhaCungCap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaNhaCungCap = table.Column<string>(type: "text", nullable: true),
                    TenNhaCungCap = table.Column<string>(type: "text", nullable: true),
                    DiaChi = table.Column<string>(type: "text", nullable: true),
                    SoDienThoai = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_NhaCungCap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mk_SanPham",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaSanPham = table.Column<string>(type: "text", nullable: false),
                    TenSanPham = table.Column<string>(type: "text", nullable: true),
                    DonViTinh = table.Column<string>(type: "text", nullable: true),
                    DonGia = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_SanPham", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mk_VatTu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaVatTu = table.Column<string>(type: "text", nullable: true),
                    TenVatTu = table.Column<string>(type: "text", nullable: true),
                    DonViTinh = table.Column<string>(type: "text", nullable: true),
                    MoTa = table.Column<string>(type: "text", nullable: true),
                    HinhDaiDien = table.Column<string>(type: "text", nullable: true),
                    HinhDinhKem = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_VatTu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mk_BaoGia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdKhachHang = table.Column<Guid>(type: "uuid", nullable: false),
                    TongTien = table.Column<string>(type: "text", nullable: true),
                    IdDuAn = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_BaoGia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_BaoGia_mk_DuAn_IdDuAn",
                        column: x => x.IdDuAn,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_BaoGia_sm_KhachHang_IdKhachHang",
                        column: x => x.IdKhachHang,
                        principalTable: "sm_KhachHang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mk_ChiPhi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChiPhiType = table.Column<string>(type: "text", nullable: true),
                    MaChi = table.Column<string>(type: "text", nullable: true),
                    IdMaNhaCungCap = table.Column<Guid>(type: "uuid", nullable: false),
                    ChiChoMucDich = table.Column<string>(type: "text", nullable: true),
                    IdKhachHang = table.Column<Guid>(type: "uuid", nullable: false),
                    SoTien = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    IdDuAn = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "mk_Bom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaBom = table.Column<string>(type: "text", nullable: true),
                    IdSanPham = table.Column<Guid>(type: "uuid", nullable: false),
                    mk_SanPhamId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_Bom", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_Bom_mk_SanPham_mk_SanPhamId",
                        column: x => x.mk_SanPhamId,
                        principalTable: "mk_SanPham",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mk_QuanLyKho",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaKho = table.Column<string>(type: "text", nullable: true),
                    MaDonHang = table.Column<string>(type: "text", nullable: true),
                    MucDichNhapKho = table.Column<string>(type: "text", nullable: true),
                    IdVatTu = table.Column<Guid>(type: "uuid", nullable: false),
                    SoLuong = table.Column<string>(type: "text", nullable: true),
                    GiaTien = table.Column<string>(type: "text", nullable: true),
                    TongTien = table.Column<string>(type: "text", nullable: true),
                    LoaiPhieu = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "mk_BaoGiaItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BaoGiaType = table.Column<string>(type: "text", nullable: true),
                    IdVatTu = table.Column<Guid>(type: "uuid", nullable: false),
                    IdSanPham = table.Column<Guid>(type: "uuid", nullable: false),
                    DonGia = table.Column<string>(type: "text", nullable: true),
                    DonVi = table.Column<string>(type: "text", nullable: true),
                    SoLuong = table.Column<string>(type: "text", nullable: true),
                    TongTien = table.Column<string>(type: "text", nullable: true),
                    IdBaoGia = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_BaoGiaItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_BaoGiaItem_mk_BaoGia_IdBaoGia",
                        column: x => x.IdBaoGia,
                        principalTable: "mk_BaoGia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_BaoGiaItem_mk_SanPham_IdSanPham",
                        column: x => x.IdSanPham,
                        principalTable: "mk_SanPham",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_BaoGiaItem_mk_VatTu_IdVatTu",
                        column: x => x.IdVatTu,
                        principalTable: "mk_VatTu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mk_NguyenVatLieu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenNguyenVatLieu = table.Column<string>(type: "text", nullable: false),
                    MaNguyenVatLieu = table.Column<string>(type: "text", nullable: true),
                    DonViTinh = table.Column<string>(type: "text", nullable: true),
                    SoLuong = table.Column<string>(type: "text", nullable: true),
                    DonGia = table.Column<string>(type: "text", nullable: true),
                    IdBom = table.Column<Guid>(type: "uuid", nullable: false),
                    IdVatTu = table.Column<Guid>(type: "uuid", nullable: false),
                    IdSanPham = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_NguyenVatLieu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_NguyenVatLieu_mk_Bom_IdBom",
                        column: x => x.IdBom,
                        principalTable: "mk_Bom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_NguyenVatLieu_mk_Bom_IdSanPham",
                        column: x => x.IdSanPham,
                        principalTable: "mk_Bom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_NguyenVatLieu_mk_VatTu_IdVatTu",
                        column: x => x.IdVatTu,
                        principalTable: "mk_VatTu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_BaoGia_IdDuAn",
                table: "mk_BaoGia",
                column: "IdDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_mk_BaoGia_IdKhachHang",
                table: "mk_BaoGia",
                column: "IdKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_mk_BaoGiaItem_IdBaoGia",
                table: "mk_BaoGiaItem",
                column: "IdBaoGia");

            migrationBuilder.CreateIndex(
                name: "IX_mk_BaoGiaItem_IdSanPham",
                table: "mk_BaoGiaItem",
                column: "IdSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_mk_BaoGiaItem_IdVatTu",
                table: "mk_BaoGiaItem",
                column: "IdVatTu");

            migrationBuilder.CreateIndex(
                name: "IX_mk_Bom_mk_SanPhamId",
                table: "mk_Bom",
                column: "mk_SanPhamId");

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

            migrationBuilder.CreateIndex(
                name: "IX_mk_NguyenVatLieu_IdBom",
                table: "mk_NguyenVatLieu",
                column: "IdBom");

            migrationBuilder.CreateIndex(
                name: "IX_mk_NguyenVatLieu_IdSanPham",
                table: "mk_NguyenVatLieu",
                column: "IdSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_mk_NguyenVatLieu_IdVatTu",
                table: "mk_NguyenVatLieu",
                column: "IdVatTu");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_BaoGiaItem");

            migrationBuilder.DropTable(
                name: "mk_ChiPhi");

            migrationBuilder.DropTable(
                name: "mk_NguyenVatLieu");

            migrationBuilder.DropTable(
                name: "mk_QuanLyKho");

            migrationBuilder.DropTable(
                name: "mk_BaoGia");

            migrationBuilder.DropTable(
                name: "mk_Bom");

            migrationBuilder.DropTable(
                name: "mk_NhaCungCap");

            migrationBuilder.DropTable(
                name: "mk_VatTu");

            migrationBuilder.DropTable(
                name: "mk_DuAn");

            migrationBuilder.DropTable(
                name: "mk_SanPham");
        }
    }
}
