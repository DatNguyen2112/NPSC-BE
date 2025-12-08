using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_ApprovalType_TaskNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskNotification_sm_TaskApprover_ApproverId",
                table: "sm_TaskNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskNotification_sm_TaskExecutor_ExecutorId",
                table: "sm_TaskNotification");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskNotification_ApproverId",
                table: "sm_TaskNotification");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskNotification_ExecutorId",
                table: "sm_TaskNotification");

            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "sm_TaskNotification");

            migrationBuilder.DropColumn(
                name: "ExecutorId",
                table: "sm_TaskNotification");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalType",
                table: "sm_TaskNotification",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalType",
                table: "sm_TaskNotification");

            migrationBuilder.AddColumn<Guid>(
                name: "ApproverId",
                table: "sm_TaskNotification",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExecutorId",
                table: "sm_TaskNotification",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskNotification_ApproverId",
                table: "sm_TaskNotification",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskNotification_ExecutorId",
                table: "sm_TaskNotification",
                column: "ExecutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskNotification_sm_TaskApprover_ApproverId",
                table: "sm_TaskNotification",
                column: "ApproverId",
                principalTable: "sm_TaskApprover",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskNotification_sm_TaskExecutor_ExecutorId",
                table: "sm_TaskNotification",
                column: "ExecutorId",
                principalTable: "sm_TaskExecutor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
