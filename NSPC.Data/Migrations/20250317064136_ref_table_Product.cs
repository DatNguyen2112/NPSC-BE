using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class ref_table_Product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualQuantity",
                table: "sm_Product",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceQuantity",
                table: "sm_Product",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PlanQuantity",
                table: "sm_Product",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualQuantity",
                table: "sm_Product");

            migrationBuilder.DropColumn(
                name: "BalanceQuantity",
                table: "sm_Product");

            migrationBuilder.DropColumn(
                name: "PlanQuantity",
                table: "sm_Product");
        }
    }
}
