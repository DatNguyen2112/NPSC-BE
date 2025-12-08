using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldProjectIdintoOrderReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "sm_Return_Order",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Return_Order_ProjectId",
                table: "sm_Return_Order",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Return_Order_mk_DuAn_ProjectId",
                table: "sm_Return_Order",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Return_Order_mk_DuAn_ProjectId",
                table: "sm_Return_Order");

            migrationBuilder.DropIndex(
                name: "IX_sm_Return_Order_ProjectId",
                table: "sm_Return_Order");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "sm_Return_Order");
        }
    }
}
