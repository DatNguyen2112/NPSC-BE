using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addaddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistrictCode",
                table: "sm_Customer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceCode",
                table: "sm_Customer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WardCode",
                table: "sm_Customer",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
