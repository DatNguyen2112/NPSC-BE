using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class testmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubTotal",
                table: "sm_Stock_Transaction",
                newName: "TotalRecieptInventory");

            migrationBuilder.RenameColumn(
                name: "InventoryQuantity",
                table: "sm_Stock_Transaction",
                newName: "RecieptInventory");

            migrationBuilder.RenameColumn(
                name: "DifferentAmount",
                table: "sm_Stock_Transaction",
                newName: "InventoryIncreased");

            migrationBuilder.RenameColumn(
                name: "MucDich",
                table: "mk_KiemKho",
                newName: "WareCode");

            migrationBuilder.RenameColumn(
                name: "Ma",
                table: "mk_KiemKho",
                newName: "TypeName");

            migrationBuilder.RenameColumn(
                name: "ListKho",
                table: "mk_KiemKho",
                newName: "ListWare");

            migrationBuilder.RenameColumn(
                name: "GhiChu",
                table: "mk_KiemKho",
                newName: "TypeCode");

            migrationBuilder.RenameColumn(
                name: "DenNgay",
                table: "mk_KiemKho",
                newName: "ToDate");

            migrationBuilder.AddColumn<int>(
                name: "ExportInventory",
                table: "sm_Stock_Transaction",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InventoryDecreased",
                table: "sm_Stock_Transaction",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalExportInventory",
                table: "sm_Stock_Transaction",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckInventoryCode",
                table: "mk_KiemKho",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "mk_KiemKho",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "mk_KiemKho",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExportInventory",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropColumn(
                name: "InventoryDecreased",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropColumn(
                name: "TotalExportInventory",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropColumn(
                name: "CheckInventoryCode",
                table: "mk_KiemKho");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "mk_KiemKho");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "mk_KiemKho");

            migrationBuilder.RenameColumn(
                name: "TotalRecieptInventory",
                table: "sm_Stock_Transaction",
                newName: "SubTotal");

            migrationBuilder.RenameColumn(
                name: "RecieptInventory",
                table: "sm_Stock_Transaction",
                newName: "InventoryQuantity");

            migrationBuilder.RenameColumn(
                name: "InventoryIncreased",
                table: "sm_Stock_Transaction",
                newName: "DifferentAmount");

            migrationBuilder.RenameColumn(
                name: "WareCode",
                table: "mk_KiemKho",
                newName: "MucDich");

            migrationBuilder.RenameColumn(
                name: "TypeName",
                table: "mk_KiemKho",
                newName: "Ma");

            migrationBuilder.RenameColumn(
                name: "TypeCode",
                table: "mk_KiemKho",
                newName: "GhiChu");

            migrationBuilder.RenameColumn(
                name: "ToDate",
                table: "mk_KiemKho",
                newName: "DenNgay");

            migrationBuilder.RenameColumn(
                name: "ListWare",
                table: "mk_KiemKho",
                newName: "ListKho");
        }
    }
}
