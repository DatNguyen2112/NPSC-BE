using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_SaleOrders_mk_DuAn_mk_DuAnId",
                table: "sm_SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_sm_SaleOrders_mk_DuAnId",
                table: "sm_SaleOrders");

            migrationBuilder.DropColumn(
                name: "mk_DuAnId",
                table: "sm_SaleOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "mk_DuAnId",
                table: "sm_SaleOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_SaleOrders_mk_DuAnId",
                table: "sm_SaleOrders",
                column: "mk_DuAnId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SaleOrders_mk_DuAn_mk_DuAnId",
                table: "sm_SaleOrders",
                column: "mk_DuAnId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
