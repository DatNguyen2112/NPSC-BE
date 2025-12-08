using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removefieldWareCodeinsm_Product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WareCode",
                table: "sm_Product");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WareCode",
                table: "sm_Product",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);
        }
    }
}
