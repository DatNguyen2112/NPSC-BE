using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updateassetLiquidattionSheetandremoveitsitems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_AssetLiquidationItem");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "LiquidationSheetName",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LiquidationDate",
                table: "sm_AssetLiquidationSheet",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "sm_AssetLiquidationSheet",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "DecisionNumber",
                table: "sm_AssetLiquidationSheet",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LiquidationReason",
                table: "sm_AssetLiquidationSheet",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LiquidationValue",
                table: "sm_AssetLiquidationSheet",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LiquidatorId",
                table: "sm_AssetLiquidationSheet",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationSheet_AssetId",
                table: "sm_AssetLiquidationSheet",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationSheet_LiquidatorId",
                table: "sm_AssetLiquidationSheet",
                column: "LiquidatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_AssetLiquidationSheet_idm_User_LiquidatorId",
                table: "sm_AssetLiquidationSheet",
                column: "LiquidatorId",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_AssetLiquidationSheet_sm_Asset_AssetId",
                table: "sm_AssetLiquidationSheet",
                column: "AssetId",
                principalTable: "sm_Asset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_AssetLiquidationSheet_idm_User_LiquidatorId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_AssetLiquidationSheet_sm_Asset_AssetId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropIndex(
                name: "IX_sm_AssetLiquidationSheet_AssetId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropIndex(
                name: "IX_sm_AssetLiquidationSheet_LiquidatorId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "DecisionNumber",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "LiquidationReason",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "LiquidationValue",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropColumn(
                name: "LiquidatorId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LiquidationDate",
                table: "sm_AssetLiquidationSheet",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<List<jsonb_Attachment>>(
                name: "Images",
                table: "sm_AssetLiquidationSheet",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LiquidationSheetName",
                table: "sm_AssetLiquidationSheet",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "sm_AssetLiquidationSheet",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "sm_AssetLiquidationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LiquidationSheetId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceOfUseId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LiquidationPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    LiquidationReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_AssetLiquidationItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_AssetLiquidationItem_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetLiquidationItem_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetLiquidationItem_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetLiquidationItem_mk_PhongBan_PlaceOfUseId",
                        column: x => x.PlaceOfUseId,
                        principalTable: "mk_PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetLiquidationItem_sm_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "sm_Asset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetLiquidationItem_sm_AssetLiquidationSheet_Liquidatio~",
                        column: x => x.LiquidationSheetId,
                        principalTable: "sm_AssetLiquidationSheet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationItem_AssetId",
                table: "sm_AssetLiquidationItem",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationItem_CreatedByUserId",
                table: "sm_AssetLiquidationItem",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationItem_LastModifiedByUserId",
                table: "sm_AssetLiquidationItem",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationItem_LiquidationSheetId",
                table: "sm_AssetLiquidationItem",
                column: "LiquidationSheetId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationItem_PlaceOfUseId",
                table: "sm_AssetLiquidationItem",
                column: "PlaceOfUseId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationItem_TenantId",
                table: "sm_AssetLiquidationItem",
                column: "TenantId");
        }
    }
}
