using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldMaInUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_WorkingDay_NgayTrongThang_NgayTrongThangId",
                table: "mk_WorkingDay");

            migrationBuilder.DropIndex(
                name: "IX_mk_WorkingDay_NgayTrongThangId",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "NgayTrongThangId",
                table: "mk_WorkingDay");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NgayTrongThangId",
                table: "mk_WorkingDay",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_mk_WorkingDay_NgayTrongThangId",
                table: "mk_WorkingDay",
                column: "NgayTrongThangId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_WorkingDay_NgayTrongThang_NgayTrongThangId",
                table: "mk_WorkingDay",
                column: "NgayTrongThangId",
                principalTable: "NgayTrongThang",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
