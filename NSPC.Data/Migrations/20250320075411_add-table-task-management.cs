using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addtabletaskmanagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_TaskManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Attachments = table.Column<List<jsonb_Attachment>>(type: "jsonb", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                    table.PrimaryKey("PK_sm_TaskManagement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_TaskManagement_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_TaskManagement_mk_DuAn_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_TaskManagement_sm_TaskManagement_ParentId",
                        column: x => x.ParentId,
                        principalTable: "sm_TaskManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_TaskManagementAssignee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskManagementId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    sm_TaskManagementId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_sm_TaskManagementAssignee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_TaskManagementAssignee_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_TaskManagementAssignee_sm_TaskManagement_sm_TaskManageme~",
                        column: x => x.sm_TaskManagementId,
                        principalTable: "sm_TaskManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_TaskManagementComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskManagementId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    sm_TaskManagementId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_sm_TaskManagementComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_TaskManagementComment_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_TaskManagementComment_sm_TaskManagement_sm_TaskManagemen~",
                        column: x => x.sm_TaskManagementId,
                        principalTable: "sm_TaskManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_TaskManagementHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskManagementId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: true),
                    sm_TaskManagementId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_sm_TaskManagementHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_TaskManagementHistory_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_TaskManagementHistory_sm_TaskManagement_sm_TaskManagemen~",
                        column: x => x.sm_TaskManagementId,
                        principalTable: "sm_TaskManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagement_ParentId",
                table: "sm_TaskManagement",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagement_ProjectId",
                table: "sm_TaskManagement",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagement_TenantId",
                table: "sm_TaskManagement",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementAssignee_sm_TaskManagementId",
                table: "sm_TaskManagementAssignee",
                column: "sm_TaskManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementAssignee_TenantId",
                table: "sm_TaskManagementAssignee",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementComment_sm_TaskManagementId",
                table: "sm_TaskManagementComment",
                column: "sm_TaskManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementComment_TenantId",
                table: "sm_TaskManagementComment",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementHistory_sm_TaskManagementId",
                table: "sm_TaskManagementHistory",
                column: "sm_TaskManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementHistory_TenantId",
                table: "sm_TaskManagementHistory",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_TaskManagementAssignee");

            migrationBuilder.DropTable(
                name: "sm_TaskManagementComment");

            migrationBuilder.DropTable(
                name: "sm_TaskManagementHistory");

            migrationBuilder.DropTable(
                name: "sm_TaskManagement");
        }
    }
}
