using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updatetablecustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_ThuChi_sm_Customer_sm_CustomerId",
                table: "mk_ThuChi");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_LichSuChamSoc_sm_Customer_sm_CustomerId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropIndex(
                name: "IX_sm_LichSuChamSoc_sm_CustomerId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "sm_CustomerId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "IdKhachHang",
                table: "mk_ThuChi");

            migrationBuilder.RenameColumn(
                name: "sm_CustomerId",
                table: "mk_ThuChi",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_mk_ThuChi_sm_CustomerId",
                table: "mk_ThuChi",
                newName: "IX_mk_ThuChi_CustomerId");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "sm_LichSuChamSoc",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_sm_LichSuChamSoc_CustomerId",
                table: "sm_LichSuChamSoc",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_ThuChi_sm_Customer_CustomerId",
                table: "mk_ThuChi",
                column: "CustomerId",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_LichSuChamSoc_sm_Customer_CustomerId",
                table: "sm_LichSuChamSoc",
                column: "CustomerId",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_ThuChi_sm_Customer_CustomerId",
                table: "mk_ThuChi");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_LichSuChamSoc_sm_Customer_CustomerId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropIndex(
                name: "IX_sm_LichSuChamSoc_CustomerId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "mk_ThuChi",
                newName: "sm_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_mk_ThuChi_CustomerId",
                table: "mk_ThuChi",
                newName: "IX_mk_ThuChi_sm_CustomerId");

            migrationBuilder.AddColumn<Guid>(
                name: "sm_CustomerId",
                table: "sm_LichSuChamSoc",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdKhachHang",
                table: "mk_ThuChi",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_LichSuChamSoc_sm_CustomerId",
                table: "sm_LichSuChamSoc",
                column: "sm_CustomerId");

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
    }
}
