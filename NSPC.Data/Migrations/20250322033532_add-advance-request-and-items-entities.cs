using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data.Data.Entity.JsonbEntity;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addadvancerequestanditemsentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_AdvanceRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    ConstructionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PriorityLevelCode = table.Column<string>(type: "text", nullable: true),
                    PriorityLevelName = table.Column<string>(type: "text", nullable: true),
                    PriorityLevelColor = table.Column<string>(type: "text", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<string>(type: "text", nullable: true),
                    StatusName = table.Column<string>(type: "text", nullable: true),
                    StatusColor = table.Column<string>(type: "text", nullable: true),
                    TotalLineAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    VatPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    AdvanceRequestHistories = table.Column<List<jsonb_AdvanceRequestHistory>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_sm_AdvanceRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_AdvanceRequest_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AdvanceRequest_sm_Construction_ConstructionId",
                        column: x => x.ConstructionId,
                        principalTable: "sm_Construction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_AdvanceRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    AdvancePurpose = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    LineAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    sm_AdvanceRequestId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_sm_AdvanceRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_AdvanceRequestItems_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_AdvanceRequestItems_sm_AdvanceRequest_sm_AdvanceRequestId",
                        column: x => x.sm_AdvanceRequestId,
                        principalTable: "sm_AdvanceRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_AdvanceRequest_ConstructionId",
                table: "sm_AdvanceRequest",
                column: "ConstructionId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AdvanceRequest_TenantId",
                table: "sm_AdvanceRequest",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AdvanceRequestItems_sm_AdvanceRequestId",
                table: "sm_AdvanceRequestItems",
                column: "sm_AdvanceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AdvanceRequestItems_TenantId",
                table: "sm_AdvanceRequestItems",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_AdvanceRequestItems");

            migrationBuilder.DropTable(
                name: "sm_AdvanceRequest");
        }
    }
}
