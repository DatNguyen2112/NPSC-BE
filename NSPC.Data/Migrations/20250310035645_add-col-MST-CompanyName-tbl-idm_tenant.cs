using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addcolMSTCompanyNametblidm_tenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Idm_Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MST",
                table: "Idm_Tenants",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Idm_Tenants");

            migrationBuilder.DropColumn(
                name: "MST",
                table: "Idm_Tenants");
        }
    }
}
