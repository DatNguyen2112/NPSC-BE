using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_table_sm_ConstructionWeekReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_ConstructionWeekReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    LastWeekPlan = table.Column<string>(type: "text", nullable: true),
                    ProcessResult = table.Column<string>(type: "text", nullable: true),
                    NextWeekPlan = table.Column<string>(type: "text", nullable: true),
                    FileAttachments = table.Column<List<jsonb_Attachment>>(type: "jsonb", nullable: true),
                    ConstructionId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_sm_ConstructionWeekReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_ConstructionWeekReport_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_ConstructionWeekReport_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_ConstructionWeekReport_sm_Construction_ConstructionId",
                        column: x => x.ConstructionId,
                        principalTable: "sm_Construction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_ConstructionWeekReport_ConstructionId",
                table: "sm_ConstructionWeekReport",
                column: "ConstructionId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_ConstructionWeekReport_CreatedByUserId",
                table: "sm_ConstructionWeekReport",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_ConstructionWeekReport_TenantId",
                table: "sm_ConstructionWeekReport",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_ConstructionWeekReport");
        }
    }
}
