using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldUserIdtotblsmIssueManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "sm_IssueManagement",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_IssueManagement_UserId",
                table: "sm_IssueManagement",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_IssueManagement_idm_User_UserId",
                table: "sm_IssueManagement",
                column: "UserId",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_IssueManagement_idm_User_UserId",
                table: "sm_IssueManagement");

            migrationBuilder.DropIndex(
                name: "IX_sm_IssueManagement_UserId",
                table: "sm_IssueManagement");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "sm_IssueManagement");
        }
    }
}
