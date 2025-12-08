using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldidSanPham : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdSanPham",
                table: "mk_BaoGiaItem",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_mk_BaoGiaItem_IdSanPham",
                table: "mk_BaoGiaItem",
                column: "IdSanPham");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_BaoGiaItem_mk_SanPham_IdSanPham",
                table: "mk_BaoGiaItem",
                column: "IdSanPham",
                principalTable: "mk_SanPham",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_BaoGiaItem_mk_SanPham_IdSanPham",
                table: "mk_BaoGiaItem");

            migrationBuilder.DropIndex(
                name: "IX_mk_BaoGiaItem_IdSanPham",
                table: "mk_BaoGiaItem");

            migrationBuilder.DropColumn(
                name: "IdSanPham",
                table: "mk_BaoGiaItem");
        }
    }
}
