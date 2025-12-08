using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addmissingfieldstoAssetMaintenanceSheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "sm_MaintenanceContent",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedPerson",
                table: "sm_MaintenanceContent",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SheetCreatedDate",
                table: "sm_AssetMaintenanceSheet",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SheetCreatedPerson",
                table: "sm_AssetMaintenanceSheet",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "sm_MaintenanceContent");

            migrationBuilder.DropColumn(
                name: "CreatedPerson",
                table: "sm_MaintenanceContent");

            migrationBuilder.DropColumn(
                name: "SheetCreatedDate",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropColumn(
                name: "SheetCreatedPerson",
                table: "sm_AssetMaintenanceSheet");
        }
    }
}
