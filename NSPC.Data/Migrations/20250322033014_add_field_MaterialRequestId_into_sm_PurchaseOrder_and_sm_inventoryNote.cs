using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_MaterialRequestId_into_sm_PurchaseOrder_and_sm_inventoryNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaterialRequestCode",
                table: "sm_PurchaseOrder",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialRequestId",
                table: "sm_PurchaseOrder",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialRequestCode",
                table: "sm_InventoryNote",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialRequestId",
                table: "sm_InventoryNote",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaterialRequestCode",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "MaterialRequestId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "MaterialRequestCode",
                table: "sm_InventoryNote");

            migrationBuilder.DropColumn(
                name: "MaterialRequestId",
                table: "sm_InventoryNote");
        }
    }
}
