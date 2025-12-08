using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addactivityHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IssueLogId",
                table: "sm_IssueManagement",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_IssueManagement_IssueLogId",
                table: "sm_IssueManagement",
                column: "IssueLogId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_IssueManagement_sm_IssueActivityLog_IssueLogId",
                table: "sm_IssueManagement",
                column: "IssueLogId",
                principalTable: "sm_IssueActivityLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_IssueManagement_sm_IssueActivityLog_IssueLogId",
                table: "sm_IssueManagement");

            migrationBuilder.DropIndex(
                name: "IX_sm_IssueManagement_IssueLogId",
                table: "sm_IssueManagement");

            migrationBuilder.DropColumn(
                name: "IssueLogId",
                table: "sm_IssueManagement");
        }
    }
}
