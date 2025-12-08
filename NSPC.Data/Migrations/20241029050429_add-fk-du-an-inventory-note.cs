using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfkduaninventorynote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrder_mk_DuAn_mk_DuAnId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_sm_PurchaseOrder_mk_DuAnId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "mk_DuAnId",
                table: "sm_PurchaseOrder");

            migrationBuilder.CreateIndex(
                name: "IX_sm_InventoryNote_ProjectId",
                table: "sm_InventoryNote",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_InventoryNote_mk_DuAn_ProjectId",
                table: "sm_InventoryNote",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_InventoryNote_mk_DuAn_ProjectId",
                table: "sm_InventoryNote");

            migrationBuilder.DropIndex(
                name: "IX_sm_InventoryNote_ProjectId",
                table: "sm_InventoryNote");

            migrationBuilder.AddColumn<Guid>(
                name: "mk_DuAnId",
                table: "sm_PurchaseOrder",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_PurchaseOrder_mk_DuAnId",
                table: "sm_PurchaseOrder",
                column: "mk_DuAnId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrder_mk_DuAn_mk_DuAnId",
                table: "sm_PurchaseOrder",
                column: "mk_DuAnId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
