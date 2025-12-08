using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addassetIncident : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "sm_AssetUsageHistory",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "sm_AssetUsageHistory",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sm_AssetIncident",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    IncidentType = table.Column<int>(type: "integer", nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CompensationAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_sm_AssetIncident", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_AssetIncident_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetIncident_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetIncident_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetIncident_sm_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "sm_Asset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetIncident_AssetId",
                table: "sm_AssetIncident",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetIncident_CreatedByUserId",
                table: "sm_AssetIncident",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetIncident_LastModifiedByUserId",
                table: "sm_AssetIncident",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetIncident_TenantId",
                table: "sm_AssetIncident",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_AssetIncident");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "sm_AssetUsageHistory");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "sm_AssetUsageHistory");
        }
    }
}
