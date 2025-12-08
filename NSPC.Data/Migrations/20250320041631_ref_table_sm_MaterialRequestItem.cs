using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class ref_table_sm_MaterialRequestItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "sm_MaterialRequestItem",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "sm_MaterialRequestItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnDate",
                table: "sm_MaterialRequestItem",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedByUserId",
                table: "sm_MaterialRequestItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedByUserName",
                table: "sm_MaterialRequestItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOnDate",
                table: "sm_MaterialRequestItem",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_MaterialRequestItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_MaterialRequestItem_TenantId",
                table: "sm_MaterialRequestItem",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_MaterialRequestItem_Idm_Tenants_TenantId",
                table: "sm_MaterialRequestItem",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_MaterialRequestItem_Idm_Tenants_TenantId",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_MaterialRequestItem_TenantId",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "CreatedOnDate",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserName",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "LastModifiedOnDate",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_MaterialRequestItem");
        }
    }
}
