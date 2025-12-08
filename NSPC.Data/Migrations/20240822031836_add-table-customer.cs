using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addtablecustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_BaoGia_sm_KhachHang_IdKhachHang",
                table: "mk_BaoGia");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_ThuChi_sm_KhachHang_IdKhachHang",
                table: "mk_ThuChi");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_LichSuChamSoc_sm_Customer_sm_CustomerId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropTable(
                name: "sm_KhachHang");

            migrationBuilder.DropIndex(
                name: "IX_sm_LichSuChamSoc_sm_CustomerId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropIndex(
                name: "IX_mk_ThuChi_IdKhachHang",
                table: "mk_ThuChi");

            migrationBuilder.DropColumn(
                name: "KhachHangId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.AddColumn<Guid>(
                name: "sm_CustomerId",
                table: "sm_LichSuChamSoc",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sm_CustomerId",
                table: "mk_ThuChi",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sm_Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    TaxCode = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    BaoGia = table.Column<string>(type: "text", nullable: true),
                    Fax = table.Column<string>(type: "text", nullable: true),
                    Sex = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    Birthdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LinkFacebook = table.Column<string>(type: "text", nullable: true),
                    LinkTiktok = table.Column<string>(type: "text", nullable: true),
                    LinkTelegram = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<List<string>>(type: "text[]", nullable: true),
                    InitialRequirement = table.Column<string>(type: "text", nullable: true),
                    InformationToCopy = table.Column<string>(type: "text", nullable: true),
                    TotalQuotationCount = table.Column<int>(type: "integer", nullable: false),
                    LastCareOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TotalCareTimes = table.Column<int>(type: "integer", nullable: false),
                    NhuCauBanDau = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Customer_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_LichSuChamSoc_sm_CustomerId",
                table: "sm_LichSuChamSoc",
                column: "sm_CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_sm_CustomerId",
                table: "mk_ThuChi",
                column: "sm_CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Customer_CreatedByUserId",
                table: "sm_Customer",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_BaoGia_sm_Customer_IdKhachHang",
                table: "mk_BaoGia",
                column: "IdKhachHang",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_ThuChi_sm_Customer_sm_CustomerId",
                table: "mk_ThuChi",
                column: "sm_CustomerId",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_LichSuChamSoc_sm_Customer_sm_CustomerId",
                table: "sm_LichSuChamSoc",
                column: "sm_CustomerId",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_BaoGia_sm_Customer_IdKhachHang",
                table: "mk_BaoGia");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_ThuChi_sm_Customer_sm_CustomerId",
                table: "mk_ThuChi");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_LichSuChamSoc_sm_Customer_sm_CustomerId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropTable(
                name: "sm_Customer");

            migrationBuilder.DropIndex(
                name: "IX_sm_LichSuChamSoc_sm_CustomerId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropIndex(
                name: "IX_mk_ThuChi_sm_CustomerId",
                table: "mk_ThuChi");

            migrationBuilder.DropColumn(
                name: "sm_CustomerId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "sm_CustomerId",
                table: "mk_ThuChi");

            migrationBuilder.AddColumn<Guid>(
                name: "KhachHangId",
                table: "sm_LichSuChamSoc",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "sm_KhachHang",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BaoGia = table.Column<string>(type: "text", nullable: true),
                    Birthdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DiaChi = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    LastCareOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LinkFacebook = table.Column<string>(type: "text", nullable: true),
                    LinkTelegram = table.Column<string>(type: "text", nullable: true),
                    LinkTiktok = table.Column<string>(type: "text", nullable: true),
                    LoaiKhachHang = table.Column<List<string>>(type: "text[]", nullable: true),
                    Ma = table.Column<string>(type: "text", nullable: true),
                    MaSoThue = table.Column<string>(type: "text", nullable: true),
                    NhuCauBanDau = table.Column<string>(type: "text", nullable: true),
                    SoDienThoai = table.Column<string>(type: "text", nullable: true),
                    Ten = table.Column<string>(type: "text", nullable: true),
                    TotalCareTimes = table.Column<int>(type: "integer", nullable: false),
                    TrangThai = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_KhachHang", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_KhachHang_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_LichSuChamSoc_KhachHangId",
                table: "sm_LichSuChamSoc",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_IdKhachHang",
                table: "mk_ThuChi",
                column: "IdKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_sm_KhachHang_CreatedByUserId",
                table: "sm_KhachHang",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_BaoGia_sm_KhachHang_IdKhachHang",
                table: "mk_BaoGia",
                column: "IdKhachHang",
                principalTable: "sm_KhachHang",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_ThuChi_sm_KhachHang_IdKhachHang",
                table: "mk_ThuChi",
                column: "IdKhachHang",
                principalTable: "sm_KhachHang",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_LichSuChamSoc_sm_KhachHang_KhachHangId",
                table: "sm_LichSuChamSoc",
                column: "KhachHangId",
                principalTable: "sm_KhachHang",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
