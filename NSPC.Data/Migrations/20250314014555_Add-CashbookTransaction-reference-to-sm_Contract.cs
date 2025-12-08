using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class AddCashbookTransactionreferencetosm_Contract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConstructionId",
                table: "sm_Cashbook_Transaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContractId",
                table: "sm_Cashbook_Transaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_ConstructionId",
                table: "sm_Cashbook_Transaction",
                column: "ConstructionId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_ContractId",
                table: "sm_Cashbook_Transaction",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Cashbook_Transaction_sm_Construction_ConstructionId",
                table: "sm_Cashbook_Transaction",
                column: "ConstructionId",
                principalTable: "sm_Construction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Cashbook_Transaction_sm_Contract_ContractId",
                table: "sm_Cashbook_Transaction",
                column: "ContractId",
                principalTable: "sm_Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Cashbook_Transaction_sm_Construction_ConstructionId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Cashbook_Transaction_sm_Contract_ContractId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Cashbook_Transaction_ConstructionId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Cashbook_Transaction_ContractId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "ConstructionId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "sm_Cashbook_Transaction");
        }
    }
}
