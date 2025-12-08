using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removeunusedfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalExportInventory",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropColumn(
                name: "TotalRecieptInventory",
                table: "sm_Stock_Transaction");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalExportInventory",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRecieptInventory",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true);
        }
    }
}
