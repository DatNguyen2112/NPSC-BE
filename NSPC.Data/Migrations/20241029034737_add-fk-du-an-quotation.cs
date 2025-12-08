using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfkduanquotation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Quotation_mk_DuAn_mk_DuAnId",
                table: "sm_Quotation");

            migrationBuilder.DropIndex(
                name: "IX_sm_Quotation_mk_DuAnId",
                table: "sm_Quotation");

            migrationBuilder.DropColumn(
                name: "mk_DuAnId",
                table: "sm_Quotation");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Quotation_ProjectId",
                table: "sm_Quotation",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Quotation_mk_DuAn_ProjectId",
                table: "sm_Quotation",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Quotation_mk_DuAn_ProjectId",
                table: "sm_Quotation");

            migrationBuilder.DropIndex(
                name: "IX_sm_Quotation_ProjectId",
                table: "sm_Quotation");

            migrationBuilder.AddColumn<Guid>(
                name: "mk_DuAnId",
                table: "sm_Quotation",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Quotation_mk_DuAnId",
                table: "sm_Quotation",
                column: "mk_DuAnId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Quotation_mk_DuAn_mk_DuAnId",
                table: "sm_Quotation",
                column: "mk_DuAnId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
