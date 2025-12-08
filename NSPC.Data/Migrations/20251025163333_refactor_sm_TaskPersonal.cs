using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class refactor_sm_TaskPersonal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "sm_TaskPersonal",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "sm_SubTaskPersonal",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "sm_SubTaskPersonal",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnDate",
                table: "sm_SubTaskPersonal",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedByUserId",
                table: "sm_SubTaskPersonal",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedByUserName",
                table: "sm_SubTaskPersonal",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOnDate",
                table: "sm_SubTaskPersonal",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LineNo",
                table: "sm_SubTaskPersonal",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_SubTaskPersonal",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_SubTaskPersonal_TenantId",
                table: "sm_SubTaskPersonal",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SubTaskPersonal_Idm_Tenants_TenantId",
                table: "sm_SubTaskPersonal",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_SubTaskPersonal_Idm_Tenants_TenantId",
                table: "sm_SubTaskPersonal");

            migrationBuilder.DropIndex(
                name: "IX_sm_SubTaskPersonal_TenantId",
                table: "sm_SubTaskPersonal");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "sm_TaskPersonal");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "sm_SubTaskPersonal");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "sm_SubTaskPersonal");

            migrationBuilder.DropColumn(
                name: "CreatedOnDate",
                table: "sm_SubTaskPersonal");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                table: "sm_SubTaskPersonal");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserName",
                table: "sm_SubTaskPersonal");

            migrationBuilder.DropColumn(
                name: "LastModifiedOnDate",
                table: "sm_SubTaskPersonal");

            migrationBuilder.DropColumn(
                name: "LineNo",
                table: "sm_SubTaskPersonal");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_SubTaskPersonal");
        }
    }
}
