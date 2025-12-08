using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addstatuscodeRejectionReasonassetallocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "sm_AssetAllocation",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "sm_AssetAllocation",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "sm_AssetAllocation",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "sm_AssetAllocation");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "sm_AssetAllocation");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "sm_AssetAllocation");
        }
    }
}
