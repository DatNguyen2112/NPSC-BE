using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class countfieldTong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_CacKhoanTroCap_mk_BangLuongItem_mk_BangLuongItemId",
                table: "mk_CacKhoanTroCap");

            migrationBuilder.DropIndex(
                name: "IX_mk_CacKhoanTroCap_mk_BangLuongItemId",
                table: "mk_CacKhoanTroCap");

            migrationBuilder.DropColumn(
                name: "mk_BangLuongItemId",
                table: "mk_CacKhoanTroCap");

            migrationBuilder.AddColumn<Guid>(
                name: "CacKhoanTroCapId",
                table: "mk_BangLuongItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mk_BangLuongItem_CacKhoanTroCapId",
                table: "mk_BangLuongItem",
                column: "CacKhoanTroCapId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_BangLuongItem_mk_CacKhoanTroCap_CacKhoanTroCapId",
                table: "mk_BangLuongItem",
                column: "CacKhoanTroCapId",
                principalTable: "mk_CacKhoanTroCap",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_BangLuongItem_mk_CacKhoanTroCap_CacKhoanTroCapId",
                table: "mk_BangLuongItem");

            migrationBuilder.DropIndex(
                name: "IX_mk_BangLuongItem_CacKhoanTroCapId",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "CacKhoanTroCapId",
                table: "mk_BangLuongItem");

            migrationBuilder.AddColumn<Guid>(
                name: "mk_BangLuongItemId",
                table: "mk_CacKhoanTroCap",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mk_CacKhoanTroCap_mk_BangLuongItemId",
                table: "mk_CacKhoanTroCap",
                column: "mk_BangLuongItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_CacKhoanTroCap_mk_BangLuongItem_mk_BangLuongItemId",
                table: "mk_CacKhoanTroCap",
                column: "mk_BangLuongItemId",
                principalTable: "mk_BangLuongItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
