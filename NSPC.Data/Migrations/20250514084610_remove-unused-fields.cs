using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removeunusedfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_IssueManagement_mk_ChucVu_IdChucVu",
                table: "sm_IssueManagement");

            migrationBuilder.DropIndex(
                name: "IX_sm_IssueManagement_IdChucVu",
                table: "sm_IssueManagement");

            migrationBuilder.DropColumn(
                name: "IdChucVu",
                table: "sm_IssueManagement");

            migrationBuilder.DropColumn(
                name: "MaPhongBan",
                table: "sm_IssueManagement");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdChucVu",
                table: "sm_IssueManagement",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaPhongBan",
                table: "sm_IssueManagement",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_IssueManagement_IdChucVu",
                table: "sm_IssueManagement",
                column: "IdChucVu");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_IssueManagement_mk_ChucVu_IdChucVu",
                table: "sm_IssueManagement",
                column: "IdChucVu",
                principalTable: "mk_ChucVu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
