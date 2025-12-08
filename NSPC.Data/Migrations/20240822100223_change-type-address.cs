using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class changetypeaddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "sm_Customer");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "sm_Customer");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "sm_Customer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistrictCode",
                table: "sm_Customer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceCode",
                table: "sm_Customer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WardCode",
                table: "sm_Customer",
                type: "text",
                nullable: true);
        }
    }
}
