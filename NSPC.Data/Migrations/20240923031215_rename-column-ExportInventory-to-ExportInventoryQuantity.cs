using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renamecolumnExportInventorytoExportInventoryQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExportInventory",
                table: "sm_Stock_Transaction",
                newName: "ExportInventoryQuantity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExportInventoryQuantity",
                table: "sm_Stock_Transaction",
                newName: "ExportInventory");
        }
    }
}
