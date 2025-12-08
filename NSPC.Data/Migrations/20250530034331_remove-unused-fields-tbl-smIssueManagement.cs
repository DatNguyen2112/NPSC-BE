using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NPSC.Data;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removeunusedfieldstblsmIssueManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityLogs",
                table: "sm_IssueManagement");

            migrationBuilder.DropColumn(
                name: "AttachmentsResolve",
                table: "sm_IssueManagement");

            migrationBuilder.DropColumn(
                name: "ContentResolve",
                table: "sm_IssueManagement");

            migrationBuilder.DropColumn(
                name: "ReasonCancel",
                table: "sm_IssueManagement");

            migrationBuilder.DropColumn(
                name: "ReasonReopen",
                table: "sm_IssueManagement");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<jsonb_IssueActivityLogHistory>>(
                name: "ActivityLogs",
                table: "sm_IssueManagement",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<List<jsonb_Attachment>>(
                name: "AttachmentsResolve",
                table: "sm_IssueManagement",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentResolve",
                table: "sm_IssueManagement",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonCancel",
                table: "sm_IssueManagement",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonReopen",
                table: "sm_IssueManagement",
                type: "text",
                nullable: true);
        }
    }
}
