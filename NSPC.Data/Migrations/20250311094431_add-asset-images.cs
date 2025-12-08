using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addassetimages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<jsonb_Attachment>>(
                name: "Images",
                table: "sm_AssetMaintenanceSheet",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<List<jsonb_Attachment>>(
                name: "Images",
                table: "sm_AssetLiquidationSheet",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<List<jsonb_Attachment>>(
                name: "Documents",
                table: "sm_Asset",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<List<jsonb_Attachment>>(
                name: "Images",
                table: "sm_Asset",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "Documents",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "sm_Asset");
        }
    }
}
