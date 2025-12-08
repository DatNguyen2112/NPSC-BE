using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addassetStatustoassetUsageHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_MaintenanceContent");

            migrationBuilder.DropIndex(
                name: "IX_sm_AssetLiquidationSheet_LiquidationSheetId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropIndex(
                name: "IX_sm_AssetLiquidationSheet_PlaceOfUseId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "LiquidationPrice",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "LiquidationSheetId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "PlaceOfUseId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.AddColumn<int>(
                name: "AssetStatus",
                table: "sm_AssetUsageHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepreciationUnit",
                table: "sm_Asset",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetStatus",
                table: "sm_AssetUsageHistory");

            migrationBuilder.DropColumn(
                name: "DepreciationUnit",
                table: "sm_Asset");

            migrationBuilder.AddColumn<decimal>(
                name: "LiquidationPrice",
                table: "sm_AssetLiquidationSheet",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LiquidationSheetId",
                table: "sm_AssetLiquidationSheet",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "sm_AssetLiquidationSheet",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlaceOfUseId",
                table: "sm_AssetLiquidationSheet",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sm_MaintenanceContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false, defaultValueSql: "CONCAT('SN-', LPAD(NEXTVAL('\"MaintenanceContentSequence\"')::text, 5, '0'))"),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MaintenanceSheetId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_MaintenanceContent", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationSheet_LiquidationSheetId",
                table: "sm_AssetLiquidationSheet",
                column: "LiquidationSheetId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationSheet_PlaceOfUseId",
                table: "sm_AssetLiquidationSheet",
                column: "PlaceOfUseId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_MaintenanceContent_CreatedByUserId",
                table: "sm_MaintenanceContent",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_MaintenanceContent_LastModifiedByUserId",
                table: "sm_MaintenanceContent",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_MaintenanceContent_MaintenanceSheetId",
                table: "sm_MaintenanceContent",
                column: "MaintenanceSheetId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_MaintenanceContent_TenantId",
                table: "sm_MaintenanceContent",
                column: "TenantId");
        }
    }
}
