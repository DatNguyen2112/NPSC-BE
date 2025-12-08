using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removefkusersubtask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_SubTask_idm_User_UserId",
                table: "sm_SubTask");

            migrationBuilder.DropIndex(
                name: "IX_sm_SubTask_UserId",
                table: "sm_SubTask");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "sm_SubTask");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "sm_SubTask",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_SubTask_UserId",
                table: "sm_SubTask",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SubTask_idm_User_UserId",
                table: "sm_SubTask",
                column: "UserId",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
