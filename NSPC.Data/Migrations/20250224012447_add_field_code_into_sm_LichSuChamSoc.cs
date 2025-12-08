using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_code_into_sm_LichSuChamSoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "sm_LichSuChamSoc",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "sm_LichSuChamSoc");
        }
    }
}
