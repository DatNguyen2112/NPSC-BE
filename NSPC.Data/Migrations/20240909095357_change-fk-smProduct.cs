using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class changefksmProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Product_mk_NhomVatTu_IdNhomVatTu",
                table: "sm_Product");

            migrationBuilder.DropIndex(
                name: "IX_sm_Product_IdNhomVatTu",
                table: "sm_Product");

            migrationBuilder.DropColumn(
                name: "IdNhomVatTu",
                table: "sm_Product");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Product_ProductGroupId",
                table: "sm_Product",
                column: "ProductGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Product_mk_NhomVatTu_ProductGroupId",
                table: "sm_Product",
                column: "ProductGroupId",
                principalTable: "mk_NhomVatTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Product_mk_NhomVatTu_ProductGroupId",
                table: "sm_Product");

            migrationBuilder.DropIndex(
                name: "IX_sm_Product_ProductGroupId",
                table: "sm_Product");

            migrationBuilder.AddColumn<Guid>(
                name: "IdNhomVatTu",
                table: "sm_Product",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Product_IdNhomVatTu",
                table: "sm_Product",
                column: "IdNhomVatTu");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Product_mk_NhomVatTu_IdNhomVatTu",
                table: "sm_Product",
                column: "IdNhomVatTu",
                principalTable: "mk_NhomVatTu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
