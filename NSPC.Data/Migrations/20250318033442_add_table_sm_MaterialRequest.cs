using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_table_sm_MaterialRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_MaterialRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    DateProcess = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: true),
                    PriorityName = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<string>(type: "text", nullable: true),
                    StatusName = table.Column<string>(type: "text", nullable: true),
                    MaterialRequestItem = table.Column<List<jsonb_MaterialRequest>>(type: "jsonb", nullable: true),
                    ListHistoryProcess = table.Column<List<jsonb_HistoryProcess>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_sm_MaterialRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_MaterialRequest_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_MaterialRequest_sm_Construction_ConstructionId",
                        column: x => x.ConstructionId,
                        principalTable: "sm_Construction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_MaterialRequest_ConstructionId",
                table: "sm_MaterialRequest",
                column: "ConstructionId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_MaterialRequest_TenantId",
                table: "sm_MaterialRequest",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_MaterialRequest");
        }
    }
}
