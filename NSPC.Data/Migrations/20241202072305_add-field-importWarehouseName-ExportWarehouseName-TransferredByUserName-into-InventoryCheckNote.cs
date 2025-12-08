using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldimportWarehouseNameExportWarehouseNameTransferredByUserNameintoInventoryCheckNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExportWarehouseName",
                table: "sm_WarehouseTransferNote",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportWarehouseName",
                table: "sm_WarehouseTransferNote",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransferredByUserName",
                table: "sm_WarehouseTransferNote",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExportWarehouseName",
                table: "sm_WarehouseTransferNote");

            migrationBuilder.DropColumn(
                name: "ImportWarehouseName",
                table: "sm_WarehouseTransferNote");

            migrationBuilder.DropColumn(
                name: "TransferredByUserName",
                table: "sm_WarehouseTransferNote");
        }
    }
}
