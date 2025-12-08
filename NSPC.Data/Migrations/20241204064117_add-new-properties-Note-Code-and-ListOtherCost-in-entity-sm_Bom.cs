using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addnewpropertiesNoteCodeandListOtherCostinentitysm_Bom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "sm_Bom",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<jsonb_OtherCost>>(
                name: "ListOtherCost",
                table: "sm_Bom",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "sm_Bom",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "sm_Bom");

            migrationBuilder.DropColumn(
                name: "ListOtherCost",
                table: "sm_Bom");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "sm_Bom");
        }
    }
}
