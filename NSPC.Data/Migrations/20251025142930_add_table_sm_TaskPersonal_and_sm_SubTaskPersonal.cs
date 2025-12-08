using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_table_sm_TaskPersonal_and_sm_SubTaskPersonal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_TaskPersonal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TaskType = table.Column<string>(type: "text", nullable: true),
                    PriorityLevel = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_sm_TaskPersonal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_TaskPersonal_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_SubTaskPersonal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TaskPersonalId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_SubTaskPersonal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_SubTaskPersonal_sm_TaskPersonal_TaskPersonalId",
                        column: x => x.TaskPersonalId,
                        principalTable: "sm_TaskPersonal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_SubTaskPersonal_TaskPersonalId",
                table: "sm_SubTaskPersonal",
                column: "TaskPersonalId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskPersonal_TenantId",
                table: "sm_TaskPersonal",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_SubTaskPersonal");

            migrationBuilder.DropTable(
                name: "sm_TaskPersonal");
        }
    }
}
