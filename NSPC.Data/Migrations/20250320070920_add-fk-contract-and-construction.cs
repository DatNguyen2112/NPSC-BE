using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfkcontractandconstruction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConstructionId",
                table: "sm_InventoryNote",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContractId",
                table: "sm_InventoryNote",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_InventoryNote_ConstructionId",
                table: "sm_InventoryNote",
                column: "ConstructionId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_InventoryNote_ContractId",
                table: "sm_InventoryNote",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_InventoryNote_sm_Construction_ConstructionId",
                table: "sm_InventoryNote",
                column: "ConstructionId",
                principalTable: "sm_Construction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_InventoryNote_sm_Contract_ContractId",
                table: "sm_InventoryNote",
                column: "ContractId",
                principalTable: "sm_Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_InventoryNote_sm_Construction_ConstructionId",
                table: "sm_InventoryNote");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_InventoryNote_sm_Contract_ContractId",
                table: "sm_InventoryNote");

            migrationBuilder.DropIndex(
                name: "IX_sm_InventoryNote_ConstructionId",
                table: "sm_InventoryNote");

            migrationBuilder.DropIndex(
                name: "IX_sm_InventoryNote_ContractId",
                table: "sm_InventoryNote");

            migrationBuilder.DropColumn(
                name: "ConstructionId",
                table: "sm_InventoryNote");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "sm_InventoryNote");
        }
    }
}
