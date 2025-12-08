using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class refactor_table_sm_Constructions_and_add_sm_ExecutionTeams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletionByCompany",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompletionByInvestor",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "sm_Construction",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentStatusCode",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentStatusName",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutionStatusCode",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutionStatusName",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvestorCode",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerTypeCode",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriorityCode",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriorityName",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoltageTypeCode",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sm_ExecutionTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConstructionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaPhongBan = table.Column<string>(type: "text", nullable: true),
                    MaTo = table.Column<string>(type: "text", nullable: true),
                    EmployeeAvatarUrl = table.Column<string>(type: "text", nullable: true),
                    EmployeeName = table.Column<string>(type: "text", nullable: true),
                    UserType = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_sm_ExecutionTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_ExecutionTeams_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_ExecutionTeams_sm_Construction_ConstructionId",
                        column: x => x.ConstructionId,
                        principalTable: "sm_Construction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_ExecutionTeams_ConstructionId",
                table: "sm_ExecutionTeams",
                column: "ConstructionId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_ExecutionTeams_TenantId",
                table: "sm_ExecutionTeams",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_ExecutionTeams");

            migrationBuilder.DropColumn(
                name: "CompletionByCompany",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "CompletionByInvestor",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "DocumentStatusCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "DocumentStatusName",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "ExecutionStatusCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "ExecutionStatusName",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "InvestorCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "OwnerTypeCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "PriorityCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "PriorityName",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "VoltageTypeCode",
                table: "sm_Construction");
        }
    }
}
