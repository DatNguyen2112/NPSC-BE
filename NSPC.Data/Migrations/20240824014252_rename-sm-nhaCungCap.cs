using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renamesmnhaCungCap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_QuanLyPhieu_mk_NhaCungCap_IdNhaCungCap",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_ThuChi_mk_NhaCungCap_IdMaNhaCungCap",
                table: "mk_ThuChi");

            migrationBuilder.DropTable(
                name: "mk_NhaCungCap");

            migrationBuilder.DropIndex(
                name: "IX_mk_ThuChi_IdMaNhaCungCap",
                table: "mk_ThuChi");

            migrationBuilder.DropIndex(
                name: "IX_mk_QuanLyPhieu_IdNhaCungCap",
                table: "mk_QuanLyPhieu");

            migrationBuilder.AddColumn<Guid>(
                name: "sm_SupplierId",
                table: "mk_ThuChi",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sm_SupplierId",
                table: "mk_QuanLyPhieu",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sm_Supplier",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    TaxCode = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Fax = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    NguoiPhuTrach = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TotalDebtAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Supplier", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_sm_SupplierId",
                table: "mk_ThuChi",
                column: "sm_SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_QuanLyPhieu_sm_SupplierId",
                table: "mk_QuanLyPhieu",
                column: "sm_SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_QuanLyPhieu_sm_Supplier_sm_SupplierId",
                table: "mk_QuanLyPhieu",
                column: "sm_SupplierId",
                principalTable: "sm_Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_ThuChi_sm_Supplier_sm_SupplierId",
                table: "mk_ThuChi",
                column: "sm_SupplierId",
                principalTable: "sm_Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_QuanLyPhieu_sm_Supplier_sm_SupplierId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_ThuChi_sm_Supplier_sm_SupplierId",
                table: "mk_ThuChi");

            migrationBuilder.DropTable(
                name: "sm_Supplier");

            migrationBuilder.DropIndex(
                name: "IX_mk_ThuChi_sm_SupplierId",
                table: "mk_ThuChi");

            migrationBuilder.DropIndex(
                name: "IX_mk_QuanLyPhieu_sm_SupplierId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "sm_SupplierId",
                table: "mk_ThuChi");

            migrationBuilder.DropColumn(
                name: "sm_SupplierId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.CreateTable(
                name: "mk_NhaCungCap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DiaChi = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Fax = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    GiaMacDinh = table.Column<string>(type: "text", nullable: true),
                    HinhThucThanhToan = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MaNhaCungCap = table.Column<string>(type: "text", nullable: true),
                    MaSoThue = table.Column<string>(type: "text", nullable: true),
                    NguoiPhuTrach = table.Column<string>(type: "text", nullable: true),
                    SoDienThoai = table.Column<string>(type: "text", nullable: true),
                    TenNhaCungCap = table.Column<string>(type: "text", nullable: true),
                    TrangThai = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_NhaCungCap", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_IdMaNhaCungCap",
                table: "mk_ThuChi",
                column: "IdMaNhaCungCap");

            migrationBuilder.CreateIndex(
                name: "IX_mk_QuanLyPhieu_IdNhaCungCap",
                table: "mk_QuanLyPhieu",
                column: "IdNhaCungCap");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_QuanLyPhieu_mk_NhaCungCap_IdNhaCungCap",
                table: "mk_QuanLyPhieu",
                column: "IdNhaCungCap",
                principalTable: "mk_NhaCungCap",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_ThuChi_mk_NhaCungCap_IdMaNhaCungCap",
                table: "mk_ThuChi",
                column: "IdMaNhaCungCap",
                principalTable: "mk_NhaCungCap",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
