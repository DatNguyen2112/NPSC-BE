







using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addcolumnstotablesmStockTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ClosingInventoryAmount",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OpeningInventoryAmount",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceiptInventoryAmout",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingInventoryAmount",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropColumn(
                name: "OpeningInventoryAmount",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropColumn(
                name: "ReceiptInventoryAmout",
                table: "sm_Stock_Transaction");
        }
    }
}
