using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addassetlocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_AssetLocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ManagementUnitId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_sm_AssetLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_AssetLocation_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetLocation_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetLocation_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetLocation_mk_PhongBan_ManagementUnitId",
                        column: x => x.ManagementUnitId,
                        principalTable: "mk_PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AssetLocation_sm_AssetLocation_ParentId",
                        column: x => x.ParentId,
                        principalTable: "sm_AssetLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLocation_CreatedByUserId",
                table: "sm_AssetLocation",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLocation_LastModifiedByUserId",
                table: "sm_AssetLocation",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLocation_ManagementUnitId",
                table: "sm_AssetLocation",
                column: "ManagementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLocation_ParentId",
                table: "sm_AssetLocation",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLocation_TenantId",
                table: "sm_AssetLocation",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_AssetLocation");
        }
    }
}
