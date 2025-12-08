using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addassetusagehistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_AssetUsageHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Operation = table.Column<int>(type: "integer", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Cost = table.Column<decimal>(type: "numeric", nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_sm_AssetUsageHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_AssetUsageHistory_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetUsageHistory_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetUsageHistory_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetUsageHistory_idm_User_UserId",
                        column: x => x.UserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetUsageHistory_sm_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "sm_Asset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetUsageHistory_sm_AssetLocation_LocationId",
                        column: x => x.LocationId,
                        principalTable: "sm_AssetLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetUsageHistory_AssetId",
                table: "sm_AssetUsageHistory",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetUsageHistory_CreatedByUserId",
                table: "sm_AssetUsageHistory",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetUsageHistory_LastModifiedByUserId",
                table: "sm_AssetUsageHistory",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetUsageHistory_LocationId",
                table: "sm_AssetUsageHistory",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetUsageHistory_TenantId",
                table: "sm_AssetUsageHistory",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetUsageHistory_UserId",
                table: "sm_AssetUsageHistory",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_AssetUsageHistory");
        }
    }
}
