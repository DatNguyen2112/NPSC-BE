using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addtaskmanagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_Task",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PriorityLevel = table.Column<int>(type: "integer", nullable: false),
                    TemplateStages = table.Column<List<jsonb_TemplateStage>>(type: "jsonb", nullable: true),
                    Attachments = table.Column<List<jsonb_Attachment>>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_sm_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Task_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_SubTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Attachments = table.Column<List<jsonb_Attachment>>(type: "jsonb", nullable: true),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_sm_SubTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_SubTask_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SubTask_idm_User_UserId",
                        column: x => x.UserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SubTask_sm_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "sm_Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_TaskApprover",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_sm_TaskApprover", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_TaskApprover_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_TaskApprover_idm_User_UserId",
                        column: x => x.UserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_TaskApprover_sm_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "sm_Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_TaskExecutor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_sm_TaskExecutor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_TaskExecutor_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_TaskExecutor_idm_User_UserId",
                        column: x => x.UserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_TaskExecutor_sm_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "sm_Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_TaskUsageHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityType = table.Column<int>(type: "integer", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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
                    table.PrimaryKey("PK_sm_TaskUsageHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_TaskUsageHistory_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_TaskUsageHistory_sm_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "sm_Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_SubTask_TaskId",
                table: "sm_SubTask",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SubTask_TenantId",
                table: "sm_SubTask",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SubTask_UserId",
                table: "sm_SubTask",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Task_TenantId",
                table: "sm_Task",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskApprover_TaskId",
                table: "sm_TaskApprover",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskApprover_TenantId",
                table: "sm_TaskApprover",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskApprover_UserId",
                table: "sm_TaskApprover",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskExecutor_TaskId",
                table: "sm_TaskExecutor",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskExecutor_TenantId",
                table: "sm_TaskExecutor",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskExecutor_UserId",
                table: "sm_TaskExecutor",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskUsageHistory_TaskId",
                table: "sm_TaskUsageHistory",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskUsageHistory_TenantId",
                table: "sm_TaskUsageHistory",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_SubTask");

            migrationBuilder.DropTable(
                name: "sm_TaskApprover");

            migrationBuilder.DropTable(
                name: "sm_TaskExecutor");

            migrationBuilder.DropTable(
                name: "sm_TaskUsageHistory");

            migrationBuilder.DropTable(
                name: "sm_Task");
        }
    }
}
