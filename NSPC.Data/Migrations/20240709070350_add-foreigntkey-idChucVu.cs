using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addforeigntkeyidChucVu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdChucVu",
                table: "idm_User",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_idm_User_IdChucVu",
                table: "idm_User",
                column: "IdChucVu");

            migrationBuilder.AddForeignKey(
                name: "FK_idm_User_mk_ChucVu_IdChucVu",
                table: "idm_User",
                column: "IdChucVu",
                principalTable: "mk_ChucVu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_idm_User_mk_ChucVu_IdChucVu",
                table: "idm_User");

            migrationBuilder.DropIndex(
                name: "IX_idm_User_IdChucVu",
                table: "idm_User");

            migrationBuilder.DropColumn(
                name: "IdChucVu",
                table: "idm_User");
        }
    }
}
