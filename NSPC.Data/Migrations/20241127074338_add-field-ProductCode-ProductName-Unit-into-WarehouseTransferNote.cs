using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldProductCodeProductNameUnitintoWarehouseTransferNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "sm_WarehouseTransferNoteItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "sm_WarehouseTransferNoteItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "sm_WarehouseTransferNoteItem",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "sm_WarehouseTransferNoteItem");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "sm_WarehouseTransferNoteItem");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "sm_WarehouseTransferNoteItem");
        }
    }
}
