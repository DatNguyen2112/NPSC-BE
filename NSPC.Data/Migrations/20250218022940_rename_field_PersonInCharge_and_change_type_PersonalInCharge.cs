using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class rename_field_PersonInCharge_and_change_type_PersonalInCharge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonInCharge",
                table: "sm_Customer");

            migrationBuilder.AddColumn<List<string>>(
                name: "ListPersonInCharge",
                table: "sm_Customer",
                type: "text[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListPersonInCharge",
                table: "sm_Customer");

            migrationBuilder.AddColumn<string>(
                name: "PersonInCharge",
                table: "sm_Customer",
                type: "text",
                nullable: true);
        }
    }
}
