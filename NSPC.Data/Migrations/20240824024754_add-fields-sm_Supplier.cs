using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldssm_Supplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistrictCode",
                table: "sm_Supplier",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DistrictName",
                table: "sm_Supplier",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceCode",
                table: "sm_Supplier",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceName",
                table: "sm_Supplier",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardCode",
                table: "sm_Supplier",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WardName",
                table: "sm_Supplier",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "sm_Supplier");

            migrationBuilder.DropColumn(
                name: "DistrictName",
                table: "sm_Supplier");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "sm_Supplier");

            migrationBuilder.DropColumn(
                name: "ProvinceName",
                table: "sm_Supplier");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "sm_Supplier");

            migrationBuilder.DropColumn(
                name: "WardName",
                table: "sm_Supplier");
        }
    }
}
