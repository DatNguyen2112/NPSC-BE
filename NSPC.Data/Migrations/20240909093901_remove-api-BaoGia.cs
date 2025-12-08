using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removeapiBaoGia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_BaoGiaItem");

            migrationBuilder.DropTable(
                name: "mk_BaoGia");

            migrationBuilder.AddColumn<Guid>(
                name: "mk_DuAnId",
                table: "sm_Quotation",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Quotation_mk_DuAnId",
                table: "sm_Quotation",
                column: "mk_DuAnId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Quotation_mk_DuAn_mk_DuAnId",
                table: "sm_Quotation",
                column: "mk_DuAnId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Quotation_mk_DuAn_mk_DuAnId",
                table: "sm_Quotation");

            migrationBuilder.DropIndex(
                name: "IX_sm_Quotation_mk_DuAnId",
                table: "sm_Quotation");

            migrationBuilder.DropColumn(
                name: "mk_DuAnId",
                table: "sm_Quotation");

            migrationBuilder.CreateTable(
                name: "mk_BaoGia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdDuAn = table.Column<Guid>(type: "uuid", nullable: false),
                    IdKhachHang = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CustomerAddress = table.Column<string>(type: "text", nullable: true),
                    CustomerCode = table.Column<string>(type: "text", nullable: true),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    CustomerPhoneNumber = table.Column<string>(type: "text", nullable: true),
                    CustomerTaxCode = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LoaiBaoGia = table.Column<string>(type: "text", nullable: true),
                    MaDonHang = table.Column<string>(type: "text", nullable: true),
                    SoPhieu = table.Column<string>(type: "text", nullable: true),
                    TongTien = table.Column<decimal>(type: "numeric", nullable: true),
                    TongTienThanhToan = table.Column<decimal>(type: "numeric", nullable: true),
                    VAT = table.Column<double>(type: "double precision", nullable: false)
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
                        name: "FK_mk_BaoGia_sm_Customer_IdKhachHang",
                        column: x => x.IdKhachHang,
                        principalTable: "sm_Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mk_BaoGiaItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdBaoGia = table.Column<Guid>(type: "uuid", nullable: false),
                    IdVatTu = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric", nullable: true),
                    DonViTinh = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MaVatTu = table.Column<string>(type: "text", nullable: true),
                    QuyCach = table.Column<string>(type: "text", nullable: true),
                    SoLuong = table.Column<int>(type: "integer", nullable: true),
                    TenVatTu = table.Column<string>(type: "text", nullable: true),
                    TongTien = table.Column<decimal>(type: "numeric", nullable: true)
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
                        name: "FK_mk_BaoGiaItem_sm_Product_IdVatTu",
                        column: x => x.IdVatTu,
                        principalTable: "sm_Product",
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
                name: "IX_mk_BaoGiaItem_IdVatTu",
                table: "mk_BaoGiaItem",
                column: "IdVatTu");
        }
    }
}
