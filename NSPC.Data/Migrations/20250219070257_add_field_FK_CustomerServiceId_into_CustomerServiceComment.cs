using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_FK_CustomerServiceId_into_CustomerServiceComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerServiceId",
                table: "sm_CustomerServiceComment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_sm_CustomerServiceComment_CustomerServiceId",
                table: "sm_CustomerServiceComment",
                column: "CustomerServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_CustomerServiceComment_sm_LichSuChamSoc_CustomerServiceId",
                table: "sm_CustomerServiceComment",
                column: "CustomerServiceId",
                principalTable: "sm_LichSuChamSoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_CustomerServiceComment_sm_LichSuChamSoc_CustomerServiceId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropIndex(
                name: "IX_sm_CustomerServiceComment_CustomerServiceId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropColumn(
                name: "CustomerServiceId",
                table: "sm_CustomerServiceComment");
        }
    }
}
