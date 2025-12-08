using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removemanagementunitfromassetlocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_AssetLocation_mk_PhongBan_ManagementUnitId",
                table: "sm_AssetLocation");

            migrationBuilder.DropIndex(
                name: "IX_sm_AssetLocation_ManagementUnitId",
                table: "sm_AssetLocation");

            migrationBuilder.DropColumn(
                name: "ManagementUnitId",
                table: "sm_AssetLocation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ManagementUnitId",
                table: "sm_AssetLocation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLocation_ManagementUnitId",
                table: "sm_AssetLocation",
                column: "ManagementUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_AssetLocation_mk_PhongBan_ManagementUnitId",
                table: "sm_AssetLocation",
                column: "ManagementUnitId",
                principalTable: "mk_PhongBan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
