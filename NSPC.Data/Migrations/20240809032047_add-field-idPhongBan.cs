using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldidPhongBan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
