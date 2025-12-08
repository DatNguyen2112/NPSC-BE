using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_fk_constructionId_into_purchaseOrder_and_salesOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConstructionId",
                table: "sm_SalesOrder",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConstructionId",
                table: "sm_PurchaseOrder",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_SalesOrder_ConstructionId",
                table: "sm_SalesOrder",
                column: "ConstructionId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_PurchaseOrder_ConstructionId",
                table: "sm_PurchaseOrder",
                column: "ConstructionId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrder_sm_Construction_ConstructionId",
                table: "sm_PurchaseOrder",
                column: "ConstructionId",
                principalTable: "sm_Construction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SalesOrder_sm_Construction_ConstructionId",
                table: "sm_SalesOrder",
                column: "ConstructionId",
                principalTable: "sm_Construction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrder_sm_Construction_ConstructionId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_SalesOrder_sm_Construction_ConstructionId",
                table: "sm_SalesOrder");

            migrationBuilder.DropIndex(
                name: "IX_sm_SalesOrder_ConstructionId",
                table: "sm_SalesOrder");

            migrationBuilder.DropIndex(
                name: "IX_sm_PurchaseOrder_ConstructionId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "ConstructionId",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "ConstructionId",
                table: "sm_PurchaseOrder");
        }
    }
}
