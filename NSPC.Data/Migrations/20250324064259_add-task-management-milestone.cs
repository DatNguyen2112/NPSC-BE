using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addtaskmanagementmilestone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TaskManagementCommentReplyId",
                table: "sm_TaskManagementComment",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sm_TaskManagementMileStone",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskManagementId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_sm_TaskManagementMileStone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_TaskManagementMileStone_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_TaskManagementMileStone_sm_TaskManagement_TaskManagement~",
                        column: x => x.TaskManagementId,
                        principalTable: "sm_TaskManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementMileStone_TaskManagementId",
                table: "sm_TaskManagementMileStone",
                column: "TaskManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementMileStone_TenantId",
                table: "sm_TaskManagementMileStone",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_TaskManagementMileStone");

            migrationBuilder.DropColumn(
                name: "TaskManagementCommentReplyId",
                table: "sm_TaskManagementComment");
        }
    }
}
