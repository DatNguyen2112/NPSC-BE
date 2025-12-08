using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class adduserIdtoasset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "sm_Asset",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Asset_UserId",
                table: "sm_Asset",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Asset_idm_User_UserId",
                table: "sm_Asset",
                column: "UserId",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Asset_idm_User_UserId",
                table: "sm_Asset");

            migrationBuilder.DropIndex(
                name: "IX_sm_Asset_UserId",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "sm_Asset");
        }
    }
}
