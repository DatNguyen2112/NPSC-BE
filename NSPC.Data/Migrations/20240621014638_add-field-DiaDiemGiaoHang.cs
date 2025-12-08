using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldDiaDiemGiaoHang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_QuanLyPhieu_sm_CodeType_sm_CodeTypeId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropIndex(
                name: "IX_mk_QuanLyPhieu_sm_CodeTypeId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "sm_CodeTypeId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.AddColumn<decimal>(
                name: "TongTienThanhToan",
                table: "mk_QuanLyPhieu",
                type: "numeric",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TongTienThanhToan",
                table: "mk_QuanLyPhieu");

            migrationBuilder.AddColumn<Guid>(
                name: "sm_CodeTypeId",
                table: "mk_QuanLyPhieu",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mk_QuanLyPhieu_sm_CodeTypeId",
                table: "mk_QuanLyPhieu",
                column: "sm_CodeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_QuanLyPhieu_sm_CodeType_sm_CodeTypeId",
                table: "mk_QuanLyPhieu",
                column: "sm_CodeTypeId",
                principalTable: "sm_CodeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
