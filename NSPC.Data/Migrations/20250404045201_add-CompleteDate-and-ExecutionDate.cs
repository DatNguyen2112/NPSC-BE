using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addCompleteDateandExecutionDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExecutionDate",
                table: "sm_AssetUsageHistory",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CompleteDate",
                table: "sm_AssetMaintenanceSheet",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionDate",
                table: "sm_AssetUsageHistory");

            migrationBuilder.DropColumn(
                name: "CompleteDate",
                table: "sm_AssetMaintenanceSheet");
        }
    }
}
