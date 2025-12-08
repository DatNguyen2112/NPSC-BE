using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldsforresolveissue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentsResolve",
                table: "sm_IssueManagement");

            migrationBuilder.DropColumn(
                name: "ContentResolve",
                table: "sm_IssueManagement");
        }
    }
}
