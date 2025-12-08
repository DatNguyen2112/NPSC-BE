using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class createtablesmIssueManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_IssueManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PriorityLevel = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ConstructionId = table.Column<Guid>(type: "uuid", nullable: true),
                    IdChucVu = table.Column<Guid>(type: "uuid", nullable: true),
                    MaPhongBan = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_sm_IssueManagement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_IssueManagement_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_IssueManagement_mk_ChucVu_IdChucVu",
                        column: x => x.IdChucVu,
                        principalTable: "mk_ChucVu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_IssueManagement_sm_Construction_ConstructionId",
                        column: x => x.ConstructionId,
                        principalTable: "sm_Construction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_IssueManagement_ConstructionId",
                table: "sm_IssueManagement",
                column: "ConstructionId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_IssueManagement_IdChucVu",
                table: "sm_IssueManagement",
                column: "IdChucVu");

            migrationBuilder.CreateIndex(
                name: "IX_sm_IssueManagement_TenantId",
                table: "sm_IssueManagement",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_IssueManagement");
        }
    }
}
