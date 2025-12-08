using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removemaintenanceContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_AssetMaintenanceSheet_mk_PhongBan_PlaceOfUseId",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropTable(
                name: "sm_MaintenanceContent");

            migrationBuilder.DropSequence(
                name: "MaintenanceContentSequence");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropColumn(
                name: "MaintenanceDescription",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropColumn(
                name: "ResponsiblePerson",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropColumn(
                name: "SheetCreatedPerson",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropColumn(
                name: "TotalMaintenanceCost",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.RenameColumn(
                name: "SheetCreatedDate",
                table: "sm_AssetMaintenanceSheet",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "PlaceOfUseId",
                table: "sm_AssetMaintenanceSheet",
                newName: "PerformerId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_AssetMaintenanceSheet_PlaceOfUseId",
                table: "sm_AssetMaintenanceSheet",
                newName: "IX_sm_AssetMaintenanceSheet_PerformerId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "sm_AssetMaintenanceSheet",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedCost",
                table: "sm_AssetMaintenanceSheet",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceContent",
                table: "sm_AssetMaintenanceSheet",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceLocation",
                table: "sm_AssetMaintenanceSheet",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_AssetMaintenanceSheet_idm_User_PerformerId",
                table: "sm_AssetMaintenanceSheet",
                column: "PerformerId",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_AssetMaintenanceSheet_idm_User_PerformerId",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropColumn(
                name: "EstimatedCost",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropColumn(
                name: "MaintenanceContent",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropColumn(
                name: "MaintenanceLocation",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "sm_AssetMaintenanceSheet",
                newName: "SheetCreatedDate");

            migrationBuilder.RenameColumn(
                name: "PerformerId",
                table: "sm_AssetMaintenanceSheet",
                newName: "PlaceOfUseId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_AssetMaintenanceSheet_PerformerId",
                table: "sm_AssetMaintenanceSheet",
                newName: "IX_sm_AssetMaintenanceSheet_PlaceOfUseId");

            migrationBuilder.CreateSequence<int>(
                name: "MaintenanceContentSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "sm_AssetMaintenanceSheet",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<List<jsonb_Attachment>>(
                name: "Images",
                table: "sm_AssetMaintenanceSheet",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceDescription",
                table: "sm_AssetMaintenanceSheet",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "sm_AssetMaintenanceSheet",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsiblePerson",
                table: "sm_AssetMaintenanceSheet",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SheetCreatedPerson",
                table: "sm_AssetMaintenanceSheet",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalMaintenanceCost",
                table: "sm_AssetMaintenanceSheet",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "sm_MaintenanceContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaintenanceSheetId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: false, defaultValueSql: "CONCAT('SN-', LPAD(NEXTVAL('\"MaintenanceContentSequence\"')::text, 5, '0'))"),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_MaintenanceContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_MaintenanceContent_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_MaintenanceContent_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_MaintenanceContent_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_MaintenanceContent_sm_AssetMaintenanceSheet_MaintenanceS~",
                        column: x => x.MaintenanceSheetId,
                        principalTable: "sm_AssetMaintenanceSheet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_sm_AssetMaintenanceSheet_mk_PhongBan_PlaceOfUseId",
                table: "sm_AssetMaintenanceSheet",
                column: "PlaceOfUseId",
                principalTable: "mk_PhongBan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
