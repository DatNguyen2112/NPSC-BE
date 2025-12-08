using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldaddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistrictCode",
                table: "sm_Customer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistrictName",
                table: "sm_Customer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceCode",
                table: "sm_Customer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceName",
                table: "sm_Customer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WardCode",
                table: "sm_Customer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WardName",
                table: "sm_Customer",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "sm_Customer");

            migrationBuilder.DropColumn(
                name: "DistrictName",
                table: "sm_Customer");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "sm_Customer");

            migrationBuilder.DropColumn(
                name: "ProvinceName",
                table: "sm_Customer");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "sm_Customer");

            migrationBuilder.DropColumn(
                name: "WardName",
                table: "sm_Customer");
        }
    }
}
