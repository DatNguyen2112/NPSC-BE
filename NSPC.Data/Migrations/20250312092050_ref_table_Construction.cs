using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class ref_table_Construction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistrictCode",
                table: "sm_Construction",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DistrictName",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceCode",
                table: "sm_Construction",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceName",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardCode",
                table: "sm_Construction",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WardName",
                table: "sm_Construction",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "DistrictName",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "ProvinceName",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "WardName",
                table: "sm_Construction");
        }
    }
}
