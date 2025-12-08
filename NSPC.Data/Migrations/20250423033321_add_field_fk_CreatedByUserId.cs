using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_fk_CreatedByUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskManagement_sm_TaskManagement_ParentId",
                table: "sm_TaskManagement");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskManagement_ParentId",
                table: "sm_TaskManagement");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "sm_TaskManagement");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementComment_CreatedByUserId",
                table: "sm_TaskManagementComment",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskManagementComment_idm_User_CreatedByUserId",
                table: "sm_TaskManagementComment",
                column: "CreatedByUserId",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskManagementComment_idm_User_CreatedByUserId",
                table: "sm_TaskManagementComment");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskManagementComment_CreatedByUserId",
                table: "sm_TaskManagementComment");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "sm_TaskManagement",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagement_ParentId",
                table: "sm_TaskManagement",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskManagement_sm_TaskManagement_ParentId",
                table: "sm_TaskManagement",
                column: "ParentId",
                principalTable: "sm_TaskManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
