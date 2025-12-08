using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updatefkIDNhomVatTu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdNhomVatTu",
                table: "mk_VatTu",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "TenNhom",
                table: "mk_NhomVatTu",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "MaNhom",
                table: "mk_NhomVatTu",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_mk_VatTu_IdNhomVatTu",
                table: "mk_VatTu",
                column: "IdNhomVatTu");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_VatTu_mk_NhomVatTu_IdNhomVatTu",
                table: "mk_VatTu",
                column: "IdNhomVatTu",
                principalTable: "mk_NhomVatTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_VatTu_mk_NhomVatTu_IdNhomVatTu",
                table: "mk_VatTu");

            migrationBuilder.DropIndex(
                name: "IX_mk_VatTu_IdNhomVatTu",
                table: "mk_VatTu");

            migrationBuilder.DropColumn(
                name: "IdNhomVatTu",
                table: "mk_VatTu");

            migrationBuilder.DropColumn(
                name: "MaNhom",
                table: "mk_NhomVatTu");

            migrationBuilder.AlterColumn<string>(
                name: "TenNhom",
                table: "mk_NhomVatTu",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
