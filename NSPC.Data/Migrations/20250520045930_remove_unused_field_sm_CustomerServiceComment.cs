using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class remove_unused_field_sm_CustomerServiceComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_CustomerServiceComment_sm_Customer_CustomerId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_CustomerServiceComment_sm_LichSuChamSoc_CustomerServiceId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropIndex(
                name: "IX_sm_CustomerServiceComment_CustomerId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropIndex(
                name: "IX_sm_CustomerServiceComment_CustomerServiceId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropColumn(
                name: "CustomerServiceId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropColumn(
                name: "CustomerType",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropColumn(
                name: "IsSystemLog",
                table: "sm_CustomerServiceComment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "sm_CustomerServiceComment",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerServiceId",
                table: "sm_CustomerServiceComment",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerType",
                table: "sm_CustomerServiceComment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemLog",
                table: "sm_CustomerServiceComment",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_sm_CustomerServiceComment_CustomerId",
                table: "sm_CustomerServiceComment",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_CustomerServiceComment_CustomerServiceId",
                table: "sm_CustomerServiceComment",
                column: "CustomerServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_CustomerServiceComment_sm_Customer_CustomerId",
                table: "sm_CustomerServiceComment",
                column: "CustomerId",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_CustomerServiceComment_sm_LichSuChamSoc_CustomerServiceId",
                table: "sm_CustomerServiceComment",
                column: "CustomerServiceId",
                principalTable: "sm_LichSuChamSoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
