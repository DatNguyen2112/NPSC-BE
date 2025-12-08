using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addassetAllocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_AssetAllocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Operation = table.Column<int>(type: "integer", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FromLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    FromUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToUserId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_sm_AssetAllocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_AssetAllocation_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetAllocation_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetAllocation_idm_User_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetAllocation_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetAllocation_idm_User_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetAllocation_sm_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "sm_Asset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetAllocation_sm_AssetLocation_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "sm_AssetLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetAllocation_sm_AssetLocation_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "sm_AssetLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetAllocation_AssetId",
                table: "sm_AssetAllocation",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetAllocation_CreatedByUserId",
                table: "sm_AssetAllocation",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetAllocation_FromLocationId",
                table: "sm_AssetAllocation",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetAllocation_FromUserId",
                table: "sm_AssetAllocation",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetAllocation_LastModifiedByUserId",
                table: "sm_AssetAllocation",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetAllocation_TenantId",
                table: "sm_AssetAllocation",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetAllocation_ToLocationId",
                table: "sm_AssetAllocation",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetAllocation_ToUserId",
                table: "sm_AssetAllocation",
                column: "ToUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_AssetAllocation");
        }
    }
}
