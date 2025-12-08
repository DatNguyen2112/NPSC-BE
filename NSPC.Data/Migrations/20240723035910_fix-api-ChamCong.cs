using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class fixapiChamCong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KyTen",
                table: "mk_ChamCongItem");

            migrationBuilder.AddColumn<Guid>(
                name: "NgayTrongThangId",
                table: "mk_WorkingDay",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "KichHoatBangLuong",
                table: "mk_ChamCong",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Nam",
                table: "mk_ChamCong",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Thang",
                table: "mk_ChamCong",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NgayTrongThang",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ngay = table.Column<int>(type: "integer", nullable: false),
                    ThuTrongTuan = table.Column<string>(type: "text", nullable: true),
                    LoaiNgay = table.Column<string>(type: "text", nullable: true),
                    Cong = table.Column<decimal>(type: "numeric", nullable: true),
                    mk_ChamCongItemId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NgayTrongThang", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NgayTrongThang_mk_ChamCongItem_mk_ChamCongItemId",
                        column: x => x.mk_ChamCongItemId,
                        principalTable: "mk_ChamCongItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_WorkingDay_NgayTrongThangId",
                table: "mk_WorkingDay",
                column: "NgayTrongThangId");

            migrationBuilder.CreateIndex(
                name: "IX_NgayTrongThang_mk_ChamCongItemId",
                table: "NgayTrongThang",
                column: "mk_ChamCongItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_WorkingDay_NgayTrongThang_NgayTrongThangId",
                table: "mk_WorkingDay",
                column: "NgayTrongThangId",
                principalTable: "NgayTrongThang",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_WorkingDay_NgayTrongThang_NgayTrongThangId",
                table: "mk_WorkingDay");

            migrationBuilder.DropTable(
                name: "NgayTrongThang");

            migrationBuilder.DropIndex(
                name: "IX_mk_WorkingDay_NgayTrongThangId",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "NgayTrongThangId",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "KichHoatBangLuong",
                table: "mk_ChamCong");

            migrationBuilder.DropColumn(
                name: "Nam",
                table: "mk_ChamCong");

            migrationBuilder.DropColumn(
                name: "Thang",
                table: "mk_ChamCong");

            migrationBuilder.AddColumn<string>(
                name: "KyTen",
                table: "mk_ChamCongItem",
                type: "text",
                nullable: true);
        }
    }
}
