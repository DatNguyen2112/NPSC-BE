using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_CustomerSource_and_rename_filed_CustomerField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "sm_Customer",
                newName: "CustomerType");

            migrationBuilder.AddColumn<List<string>>(
                name: "CustomerSource",
                table: "sm_Customer",
                type: "text[]",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerSource",
                table: "sm_Customer");

            migrationBuilder.RenameColumn(
                name: "CustomerType",
                table: "sm_Customer",
                newName: "Type");
        }
    }
}
