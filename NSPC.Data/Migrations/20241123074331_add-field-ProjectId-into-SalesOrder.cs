using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldProjectIdintoSalesOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "sm_SalesOrder",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_SalesOrder_ProjectId",
                table: "sm_SalesOrder",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SalesOrder_mk_DuAn_ProjectId",
                table: "sm_SalesOrder",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_SalesOrder_mk_DuAn_ProjectId",
                table: "sm_SalesOrder");

            migrationBuilder.DropIndex(
                name: "IX_sm_SalesOrder_ProjectId",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "sm_SalesOrder");
        }
    }
}
