using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updatesm_TaskUsageHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "sm_TaskUsageHistory",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "sm_TaskUsageHistory",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskUsageHistory_CreatedByUserId",
                table: "sm_TaskUsageHistory",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskUsageHistory_idm_User_CreatedByUserId",
                table: "sm_TaskUsageHistory",
                column: "CreatedByUserId",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskUsageHistory_idm_User_CreatedByUserId",
                table: "sm_TaskUsageHistory");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskUsageHistory_CreatedByUserId",
                table: "sm_TaskUsageHistory");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "sm_TaskUsageHistory");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "sm_TaskUsageHistory",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
