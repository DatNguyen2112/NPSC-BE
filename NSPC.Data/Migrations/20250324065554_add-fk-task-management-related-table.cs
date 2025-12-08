using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfktaskmanagementrelatedtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskManagementAssignee_sm_TaskManagement_sm_TaskManageme~",
                table: "sm_TaskManagementAssignee");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskManagementComment_sm_TaskManagement_sm_TaskManagemen~",
                table: "sm_TaskManagementComment");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskManagementHistory_sm_TaskManagement_sm_TaskManagemen~",
                table: "sm_TaskManagementHistory");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskManagementHistory_sm_TaskManagementId",
                table: "sm_TaskManagementHistory");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskManagementComment_sm_TaskManagementId",
                table: "sm_TaskManagementComment");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskManagementAssignee_sm_TaskManagementId",
                table: "sm_TaskManagementAssignee");

            migrationBuilder.DropColumn(
                name: "sm_TaskManagementId",
                table: "sm_TaskManagementHistory");

            migrationBuilder.DropColumn(
                name: "sm_TaskManagementId",
                table: "sm_TaskManagementComment");

            migrationBuilder.DropColumn(
                name: "sm_TaskManagementId",
                table: "sm_TaskManagementAssignee");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementHistory_TaskManagementId",
                table: "sm_TaskManagementHistory",
                column: "TaskManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementComment_TaskManagementId",
                table: "sm_TaskManagementComment",
                column: "TaskManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementAssignee_TaskManagementId",
                table: "sm_TaskManagementAssignee",
                column: "TaskManagementId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskManagementAssignee_sm_TaskManagement_TaskManagementId",
                table: "sm_TaskManagementAssignee",
                column: "TaskManagementId",
                principalTable: "sm_TaskManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskManagementComment_sm_TaskManagement_TaskManagementId",
                table: "sm_TaskManagementComment",
                column: "TaskManagementId",
                principalTable: "sm_TaskManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskManagementHistory_sm_TaskManagement_TaskManagementId",
                table: "sm_TaskManagementHistory",
                column: "TaskManagementId",
                principalTable: "sm_TaskManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskManagementAssignee_sm_TaskManagement_TaskManagementId",
                table: "sm_TaskManagementAssignee");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskManagementComment_sm_TaskManagement_TaskManagementId",
                table: "sm_TaskManagementComment");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskManagementHistory_sm_TaskManagement_TaskManagementId",
                table: "sm_TaskManagementHistory");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskManagementHistory_TaskManagementId",
                table: "sm_TaskManagementHistory");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskManagementComment_TaskManagementId",
                table: "sm_TaskManagementComment");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskManagementAssignee_TaskManagementId",
                table: "sm_TaskManagementAssignee");

            migrationBuilder.AddColumn<Guid>(
                name: "sm_TaskManagementId",
                table: "sm_TaskManagementHistory",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sm_TaskManagementId",
                table: "sm_TaskManagementComment",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sm_TaskManagementId",
                table: "sm_TaskManagementAssignee",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementHistory_sm_TaskManagementId",
                table: "sm_TaskManagementHistory",
                column: "sm_TaskManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementComment_sm_TaskManagementId",
                table: "sm_TaskManagementComment",
                column: "sm_TaskManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementAssignee_sm_TaskManagementId",
                table: "sm_TaskManagementAssignee",
                column: "sm_TaskManagementId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskManagementAssignee_sm_TaskManagement_sm_TaskManageme~",
                table: "sm_TaskManagementAssignee",
                column: "sm_TaskManagementId",
                principalTable: "sm_TaskManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskManagementComment_sm_TaskManagement_sm_TaskManagemen~",
                table: "sm_TaskManagementComment",
                column: "sm_TaskManagementId",
                principalTable: "sm_TaskManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskManagementHistory_sm_TaskManagement_sm_TaskManagemen~",
                table: "sm_TaskManagementHistory",
                column: "sm_TaskManagementId",
                principalTable: "sm_TaskManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
