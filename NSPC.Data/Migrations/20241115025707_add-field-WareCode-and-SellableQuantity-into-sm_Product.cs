using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldWareCodeandSellableQuantityintosm_Product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SellableQuantity",
                table: "sm_Product",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "WareCode",
                table: "sm_Product",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellableQuantity",
                table: "sm_Product");

            migrationBuilder.DropColumn(
                name: "WareCode",
                table: "sm_Product");
        }
    }
}
