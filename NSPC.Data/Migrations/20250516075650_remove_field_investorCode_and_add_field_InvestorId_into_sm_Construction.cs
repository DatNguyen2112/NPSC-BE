using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class remove_field_investorCode_and_add_field_InvestorId_into_sm_Construction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvestorCode",
                table: "sm_Construction");

            migrationBuilder.AddColumn<Guid>(
                name: "InvestorId",
                table: "sm_Construction",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_sm_Construction_InvestorId",
                table: "sm_Construction",
                column: "InvestorId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Construction_sm_Investor_InvestorId",
                table: "sm_Construction",
                column: "InvestorId",
                principalTable: "sm_Investor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Construction_sm_Investor_InvestorId",
                table: "sm_Construction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Construction_InvestorId",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "InvestorId",
                table: "sm_Construction");

            migrationBuilder.AddColumn<string>(
                name: "InvestorCode",
                table: "sm_Construction",
                type: "text",
                nullable: true);
        }
    }
}
