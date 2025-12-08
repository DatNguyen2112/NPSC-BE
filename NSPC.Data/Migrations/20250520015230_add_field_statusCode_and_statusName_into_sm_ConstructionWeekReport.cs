using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_statusCode_and_statusName_into_sm_ConstructionWeekReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusCode",
                table: "sm_ConstructionWeekReport",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusName",
                table: "sm_ConstructionWeekReport",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "sm_ConstructionWeekReport");

            migrationBuilder.DropColumn(
                name: "StatusName",
                table: "sm_ConstructionWeekReport");
        }
    }
}
