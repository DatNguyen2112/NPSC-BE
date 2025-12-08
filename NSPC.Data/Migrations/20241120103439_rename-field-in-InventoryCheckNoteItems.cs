using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renamefieldinInventoryCheckNoteItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RealityInventory",
                table: "sm_InventoryCheckNoteItems",
                newName: "RecordedQuantity");

            migrationBuilder.RenameColumn(
                name: "ActualInventory",
                table: "sm_InventoryCheckNoteItems",
                newName: "ActualQuantity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecordedQuantity",
                table: "sm_InventoryCheckNoteItems",
                newName: "RealityInventory");

            migrationBuilder.RenameColumn(
                name: "ActualQuantity",
                table: "sm_InventoryCheckNoteItems",
                newName: "ActualInventory");
        }
    }
}
