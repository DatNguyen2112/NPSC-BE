using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class fixpublicvỉtual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_Bom_mk_SanPham_mk_SanPhamId",
                table: "mk_Bom");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_NguyenVatLieu_mk_Bom_IdSanPham",
                table: "mk_NguyenVatLieu");

            migrationBuilder.DropIndex(
                name: "IX_mk_NguyenVatLieu_IdSanPham",
                table: "mk_NguyenVatLieu");

            migrationBuilder.DropIndex(
                name: "IX_mk_Bom_mk_SanPhamId",
                table: "mk_Bom");

            migrationBuilder.DropColumn(
                name: "IdSanPham",
                table: "mk_NguyenVatLieu");

            migrationBuilder.DropColumn(
                name: "mk_SanPhamId",
                table: "mk_Bom");

            migrationBuilder.CreateIndex(
                name: "IX_mk_Bom_IdSanPham",
                table: "mk_Bom",
                column: "IdSanPham");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_Bom_mk_SanPham_IdSanPham",
                table: "mk_Bom",
                column: "IdSanPham",
                principalTable: "mk_SanPham",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_Bom_mk_SanPham_IdSanPham",
                table: "mk_Bom");

            migrationBuilder.DropIndex(
                name: "IX_mk_Bom_IdSanPham",
                table: "mk_Bom");

            migrationBuilder.AddColumn<Guid>(
                name: "IdSanPham",
                table: "mk_NguyenVatLieu",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "mk_SanPhamId",
                table: "mk_Bom",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mk_NguyenVatLieu_IdSanPham",
                table: "mk_NguyenVatLieu",
                column: "IdSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_mk_Bom_mk_SanPhamId",
                table: "mk_Bom",
                column: "mk_SanPhamId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_Bom_mk_SanPham_mk_SanPhamId",
                table: "mk_Bom",
                column: "mk_SanPhamId",
                principalTable: "mk_SanPham",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_NguyenVatLieu_mk_Bom_IdSanPham",
                table: "mk_NguyenVatLieu",
                column: "IdSanPham",
                principalTable: "mk_Bom",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
