using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldstotblsmIssueActivityLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<jsonb_Attachment>>(
                name: "AttachmentsResolve",
                table: "sm_IssueActivityLog",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentResovle",
                table: "sm_IssueActivityLog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonCancel",
                table: "sm_IssueActivityLog",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonReopen",
                table: "sm_IssueActivityLog",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentsResolve",
                table: "sm_IssueActivityLog");

            migrationBuilder.DropColumn(
                name: "ContentResovle",
                table: "sm_IssueActivityLog");

            migrationBuilder.DropColumn(
                name: "ReasonCancel",
                table: "sm_IssueActivityLog");

            migrationBuilder.DropColumn(
                name: "ReasonReopen",
                table: "sm_IssueActivityLog");
        }
    }
}
