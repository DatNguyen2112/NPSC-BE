using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NPSC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldsactivityLogstotblsmIssueManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<jsonb_IssueActivityLogHistory>>(
                name: "ActivityLogs",
                table: "sm_IssueManagement",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityLogs",
                table: "sm_IssueManagement");
        }
    }
}
