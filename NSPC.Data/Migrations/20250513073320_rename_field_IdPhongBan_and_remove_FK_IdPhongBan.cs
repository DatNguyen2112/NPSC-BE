using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class rename_field_IdPhongBan_and_remove_FK_IdPhongBan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_idm_User_mk_PhongBan_IdPhongBan",
                table: "idm_User");

            migrationBuilder.DropIndex(
                name: "IX_idm_User_IdPhongBan",
                table: "idm_User");

            migrationBuilder.DropColumn(
                name: "IdPhongBan",
                table: "idm_User");

            migrationBuilder.AddColumn<string>(
                name: "MaPhongBan",
                table: "idm_User",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaTo",
                table: "idm_User",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaPhongBan",
                table: "idm_User");

            migrationBuilder.DropColumn(
                name: "MaTo",
                table: "idm_User");

            migrationBuilder.AddColumn<Guid>(
                name: "IdPhongBan",
                table: "idm_User",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_idm_User_IdPhongBan",
                table: "idm_User",
                column: "IdPhongBan");

            migrationBuilder.AddForeignKey(
                name: "FK_idm_User_mk_PhongBan_IdPhongBan",
                table: "idm_User",
                column: "IdPhongBan",
                principalTable: "mk_PhongBan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
