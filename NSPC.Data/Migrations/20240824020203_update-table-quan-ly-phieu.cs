using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updatetablequanlyphieu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_QuanLyPhieu_sm_Supplier_sm_SupplierId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropIndex(
                name: "IX_mk_QuanLyPhieu_sm_SupplierId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "sm_SupplierId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.CreateIndex(
                name: "IX_mk_QuanLyPhieu_IdNhaCungCap",
                table: "mk_QuanLyPhieu",
                column: "IdNhaCungCap");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_QuanLyPhieu_sm_Supplier_IdNhaCungCap",
                table: "mk_QuanLyPhieu",
                column: "IdNhaCungCap",
                principalTable: "sm_Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_QuanLyPhieu_sm_Supplier_IdNhaCungCap",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropIndex(
                name: "IX_mk_QuanLyPhieu_IdNhaCungCap",
                table: "mk_QuanLyPhieu");

            migrationBuilder.AddColumn<Guid>(
                name: "sm_SupplierId",
                table: "mk_QuanLyPhieu",
                type: "uuid",
                nullable: true);

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
        }
    }
}
