using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class disablesanpham : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_BaoGiaItem_mk_SanPham_IdSanPham",
                table: "mk_BaoGiaItem");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_Bom_mk_SanPham_IdSanPham",
                table: "mk_Bom");

            migrationBuilder.DropTable(
                name: "mk_SanPham");

            migrationBuilder.DropIndex(
                name: "IX_mk_BaoGiaItem_IdSanPham",
                table: "mk_BaoGiaItem");

            migrationBuilder.DropColumn(
                name: "IdSanPham",
                table: "mk_BaoGiaItem");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_Bom_mk_VatTu_IdSanPham",
                table: "mk_Bom",
                column: "IdSanPham",
                principalTable: "mk_VatTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_Bom_mk_VatTu_IdSanPham",
                table: "mk_Bom");

            migrationBuilder.AddColumn<Guid>(
                name: "IdSanPham",
                table: "mk_BaoGiaItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "mk_SanPham",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric", nullable: true),
                    DonViTinh = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MaSanPham = table.Column<string>(type: "text", nullable: false),
                    TenSanPham = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_SanPham", x => x.Id);
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_mk_Bom_mk_SanPham_IdSanPham",
                table: "mk_Bom",
                column: "IdSanPham",
                principalTable: "mk_SanPham",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
