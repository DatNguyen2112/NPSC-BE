using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addsm_SubTaskExecutor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_SubTaskExecutor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubTaskId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_sm_SubTaskExecutor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_SubTaskExecutor_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SubTaskExecutor_idm_User_UserId",
                        column: x => x.UserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_SubTaskExecutor_sm_SubTask_SubTaskId",
                        column: x => x.SubTaskId,
                        principalTable: "sm_SubTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_SubTaskExecutor_SubTaskId",
                table: "sm_SubTaskExecutor",
                column: "SubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SubTaskExecutor_TenantId",
                table: "sm_SubTaskExecutor",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SubTaskExecutor_UserId",
                table: "sm_SubTaskExecutor",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_SubTaskExecutor");
        }
    }
}
