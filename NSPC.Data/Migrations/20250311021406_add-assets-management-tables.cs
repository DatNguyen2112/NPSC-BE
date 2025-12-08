using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addassetsmanagementtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_Asset",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TypeCode = table.Column<string>(type: "text", nullable: false),
                    TypeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GroupCode = table.Column<string>(type: "text", nullable: true),
                    GroupName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PurchasedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PlaceOfUseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Depreciation = table.Column<decimal>(type: "numeric", nullable: false),
                    StartOfUseDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MaintenanceCycle = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Serial = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OriginalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OriginBrand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CertificateNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CertificateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ManufactureDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Asset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Asset_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_Asset_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_Asset_mk_PhongBan_PlaceOfUseId",
                        column: x => x.PlaceOfUseId,
                        principalTable: "mk_PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_AssetLiquidationSheet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    LiquidationSheetName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LiquidationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_AssetLiquidationSheet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_AssetLiquidationSheet_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetLiquidationSheet_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_AssetMaintenanceSheet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceOfUseId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaintenancePeriod = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    MaintenanceType = table.Column<int>(type: "integer", nullable: false),
                    MaintenanceDescription = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ResponsiblePerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MaintenancePlace = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TotalMaintenanceCost = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_AssetMaintenanceSheet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_AssetMaintenanceSheet_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetMaintenanceSheet_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetMaintenanceSheet_mk_PhongBan_PlaceOfUseId",
                        column: x => x.PlaceOfUseId,
                        principalTable: "mk_PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetMaintenanceSheet_sm_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "sm_Asset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_AssetLiquidationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    LiquidationSheetId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlaceOfUseId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    LiquidationReason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LiquidationPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_AssetLiquidationItem", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "sm_MaintenanceContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    MaintenanceSheetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_MaintenanceContent", x => x.Id);
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
                name: "IX_sm_Asset_CreatedByUserId",
                table: "sm_Asset",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Asset_LastModifiedByUserId",
                table: "sm_Asset",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Asset_PlaceOfUseId",
                table: "sm_Asset",
                column: "PlaceOfUseId");

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
                name: "IX_sm_AssetLiquidationSheet_CreatedByUserId",
                table: "sm_AssetLiquidationSheet",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationSheet_LastModifiedByUserId",
                table: "sm_AssetLiquidationSheet",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetMaintenanceSheet_AssetId",
                table: "sm_AssetMaintenanceSheet",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetMaintenanceSheet_CreatedByUserId",
                table: "sm_AssetMaintenanceSheet",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetMaintenanceSheet_LastModifiedByUserId",
                table: "sm_AssetMaintenanceSheet",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetMaintenanceSheet_PlaceOfUseId",
                table: "sm_AssetMaintenanceSheet",
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_AssetLiquidationItem");

            migrationBuilder.DropTable(
                name: "sm_MaintenanceContent");

            migrationBuilder.DropTable(
                name: "sm_AssetLiquidationSheet");

            migrationBuilder.DropTable(
                name: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropTable(
                name: "sm_Asset");
        }
    }
}
