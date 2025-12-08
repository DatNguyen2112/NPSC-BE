using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_constructionId_and_contractId_into_sm_OrderReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConstructionId",
                table: "sm_Return_Order",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContractId",
                table: "sm_Return_Order",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Return_Order_ConstructionId",
                table: "sm_Return_Order",
                column: "ConstructionId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Return_Order_ContractId",
                table: "sm_Return_Order",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Return_Order_sm_Construction_ConstructionId",
                table: "sm_Return_Order",
                column: "ConstructionId",
                principalTable: "sm_Construction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Return_Order_sm_Contract_ContractId",
                table: "sm_Return_Order",
                column: "ContractId",
                principalTable: "sm_Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Return_Order_sm_Construction_ConstructionId",
                table: "sm_Return_Order");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Return_Order_sm_Contract_ContractId",
                table: "sm_Return_Order");

            migrationBuilder.DropIndex(
                name: "IX_sm_Return_Order_ConstructionId",
                table: "sm_Return_Order");

            migrationBuilder.DropIndex(
                name: "IX_sm_Return_Order_ContractId",
                table: "sm_Return_Order");

            migrationBuilder.DropColumn(
                name: "ConstructionId",
                table: "sm_Return_Order");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "sm_Return_Order");
        }
    }
}
