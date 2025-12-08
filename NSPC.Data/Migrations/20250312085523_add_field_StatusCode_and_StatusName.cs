using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_StatusCode_and_StatusName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusCode",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusName",
                table: "sm_Construction",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "StatusName",
                table: "sm_Construction");
        }
    }
}
