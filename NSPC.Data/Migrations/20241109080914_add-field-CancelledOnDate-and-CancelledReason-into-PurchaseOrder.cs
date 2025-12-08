using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldCancelledOnDateandCancelledReasonintoPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledOnDate",
                table: "sm_PurchaseOrder",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledReason",
                table: "sm_PurchaseOrder",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledOnDate",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "CancelledReason",
                table: "sm_PurchaseOrder");
        }
    }
}
