using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class createtableProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_BaoGiaItem_mk_VatTu_IdVatTu",
                table: "mk_BaoGiaItem");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_Bom_mk_VatTu_IdSanPham",
                table: "mk_Bom");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_NguyenVatLieu_mk_VatTu_IdVatTu",
                table: "mk_NguyenVatLieu");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_PhieuItem_mk_VatTu_IdVatTu",
                table: "mk_PhieuItem");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_XuatNhapTon_mk_VatTu_IdVatTu",
                table: "mk_XuatNhapTon");

            migrationBuilder.DropTable(
                name: "mk_VatTu");

            migrationBuilder.CreateTable(
                name: "sm_Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<string>(type: "text", nullable: true),
                    PurchaseUnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    SellingUnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Attachments = table.Column<List<jsonb_Attachment>>(type: "jsonb", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    ProductGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    IdNhomVatTu = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Product_mk_NhomVatTu_IdNhomVatTu",
                        column: x => x.IdNhomVatTu,
                        principalTable: "mk_NhomVatTu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_Product_IdNhomVatTu",
                table: "sm_Product",
                column: "IdNhomVatTu");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_BaoGiaItem_sm_Product_IdVatTu",
                table: "mk_BaoGiaItem",
                column: "IdVatTu",
                principalTable: "sm_Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_Bom_sm_Product_IdSanPham",
                table: "mk_Bom",
                column: "IdSanPham",
                principalTable: "sm_Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_NguyenVatLieu_sm_Product_IdVatTu",
                table: "mk_NguyenVatLieu",
                column: "IdVatTu",
                principalTable: "sm_Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_PhieuItem_sm_Product_IdVatTu",
                table: "mk_PhieuItem",
                column: "IdVatTu",
                principalTable: "sm_Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_XuatNhapTon_sm_Product_IdVatTu",
                table: "mk_XuatNhapTon",
                column: "IdVatTu",
                principalTable: "sm_Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_BaoGiaItem_sm_Product_IdVatTu",
                table: "mk_BaoGiaItem");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_Bom_sm_Product_IdSanPham",
                table: "mk_Bom");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_NguyenVatLieu_sm_Product_IdVatTu",
                table: "mk_NguyenVatLieu");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_PhieuItem_sm_Product_IdVatTu",
                table: "mk_PhieuItem");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_XuatNhapTon_sm_Product_IdVatTu",
                table: "mk_XuatNhapTon");

            migrationBuilder.DropTable(
                name: "sm_Product");

            migrationBuilder.CreateTable(
                name: "mk_VatTu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdNhomVatTu = table.Column<Guid>(type: "uuid", nullable: false),
                    Attachments = table.Column<List<jsonb_Attachment>>(type: "jsonb", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric", nullable: false),
                    DonViTinh = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MaVatTu = table.Column<string>(type: "text", nullable: true),
                    MoTa = table.Column<string>(type: "text", nullable: true),
                    TenVatTu = table.Column<string>(type: "text", nullable: true),
                    TrangThai = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_VatTu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_VatTu_mk_NhomVatTu_IdNhomVatTu",
                        column: x => x.IdNhomVatTu,
                        principalTable: "mk_NhomVatTu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_VatTu_IdNhomVatTu",
                table: "mk_VatTu",
                column: "IdNhomVatTu");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_BaoGiaItem_mk_VatTu_IdVatTu",
                table: "mk_BaoGiaItem",
                column: "IdVatTu",
                principalTable: "mk_VatTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_Bom_mk_VatTu_IdSanPham",
                table: "mk_Bom",
                column: "IdSanPham",
                principalTable: "mk_VatTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_NguyenVatLieu_mk_VatTu_IdVatTu",
                table: "mk_NguyenVatLieu",
                column: "IdVatTu",
                principalTable: "mk_VatTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_PhieuItem_mk_VatTu_IdVatTu",
                table: "mk_PhieuItem",
                column: "IdVatTu",
                principalTable: "mk_VatTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_XuatNhapTon_mk_VatTu_IdVatTu",
                table: "mk_XuatNhapTon",
                column: "IdVatTu",
                principalTable: "mk_VatTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
