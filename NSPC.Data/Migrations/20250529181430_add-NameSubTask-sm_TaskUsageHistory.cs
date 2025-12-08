using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addNameSubTasksm_TaskUsageHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionDate",
                table: "sm_TaskUsageHistory");

            migrationBuilder.AddColumn<string>(
                name: "NameSubtask",
                table: "sm_TaskUsageHistory",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameSubtask",
                table: "sm_TaskUsageHistory");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExecutionDate",
                table: "sm_TaskUsageHistory",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
