using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class Addsm_Contractentityclasswithpropertiesandannotations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_Contract",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    DocumentTypeCode = table.Column<string>(type: "text", nullable: true),
                    DocumentTypeName = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConstructionCategory = table.Column<string>(type: "text", nullable: true),
                    ConstructionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValueAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    VatPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StatusCode = table.Column<string>(type: "text", nullable: true),
                    StatusName = table.Column<string>(type: "text", nullable: true),
                    StatusColor = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Contract", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Contract_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_Contract_sm_Construction_ConstructionId",
                        column: x => x.ConstructionId,
                        principalTable: "sm_Construction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_Construction_TenantId",
                table: "sm_Construction",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Contract_ConstructionId",
                table: "sm_Contract",
                column: "ConstructionId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Contract_TenantId",
                table: "sm_Contract",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Construction_Idm_Tenants_TenantId",
                table: "sm_Construction",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Construction_Idm_Tenants_TenantId",
                table: "sm_Construction");

            migrationBuilder.DropTable(
                name: "sm_Contract");

            migrationBuilder.DropIndex(
                name: "IX_sm_Construction_TenantId",
                table: "sm_Construction");
        }
    }
}
