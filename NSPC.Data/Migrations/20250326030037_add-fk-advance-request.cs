using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfkadvancerequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdvanceRequestId",
                table: "sm_Cashbook_Transaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_AdvanceRequestId",
                table: "sm_Cashbook_Transaction",
                column: "AdvanceRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Cashbook_Transaction_sm_AdvanceRequest_AdvanceRequestId",
                table: "sm_Cashbook_Transaction",
                column: "AdvanceRequestId",
                principalTable: "sm_AdvanceRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Cashbook_Transaction_sm_AdvanceRequest_AdvanceRequestId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Cashbook_Transaction_AdvanceRequestId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "AdvanceRequestId",
                table: "sm_Cashbook_Transaction");
        }
    }
}
