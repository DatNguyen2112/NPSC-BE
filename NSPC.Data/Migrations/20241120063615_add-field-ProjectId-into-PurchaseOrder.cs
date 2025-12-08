using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldProjectIdintoPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "sm_PurchaseOrder",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_PurchaseOrder_ProjectId",
                table: "sm_PurchaseOrder",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrder_mk_DuAn_ProjectId",
                table: "sm_PurchaseOrder",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrder_mk_DuAn_ProjectId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_sm_PurchaseOrder_ProjectId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "sm_PurchaseOrder");
        }
    }
}
