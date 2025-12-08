using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_ListPredicateInventory_and_ListTeamInventory_into_sm_Construction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<jsonb_PredicateInventory>>(
                name: "ListPredicateInventory",
                table: "sm_Construction",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<List<jsonb_TeamInventory>>(
                name: "ListTeamInventory",
                table: "sm_Construction",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListPredicateInventory",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "ListTeamInventory",
                table: "sm_Construction");
        }
    }
}
