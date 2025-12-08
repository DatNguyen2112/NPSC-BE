using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addassetdefaultvalues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "AssetLiquidationSheetSequence");

            migrationBuilder.CreateSequence<int>(
                name: "AssetMaintenanceSheetSequence");

            migrationBuilder.CreateSequence<int>(
                name: "AssetSequence");

            migrationBuilder.CreateSequence<int>(
                name: "MaintenanceContentSequence");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_MaintenanceContent",
                type: "text",
                nullable: false,
                defaultValueSql: "CONCAT('SN-', LPAD(NEXTVAL('\"MaintenanceContentSequence\"')::text, 5, '0'))",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_AssetMaintenanceSheet",
                type: "text",
                nullable: false,
                defaultValueSql: "CONCAT('MSN-', LPAD(NEXTVAL('\"AssetMaintenanceSheetSequence\"')::text, 5, '0'))",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_AssetLiquidationSheet",
                type: "text",
                nullable: false,
                defaultValueSql: "CONCAT('LSN-', LPAD(NEXTVAL('\"AssetLiquidationSheetSequence\"')::text, 5, '0'))",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_Asset",
                type: "text",
                nullable: false,
                defaultValueSql: "CONCAT('AN-', LPAD(NEXTVAL('\"AssetSequence\"')::text, 5, '0'))",
                oldClrType: typeof(string),
                oldType: "text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "AssetLiquidationSheetSequence");

            migrationBuilder.DropSequence(
                name: "AssetMaintenanceSheetSequence");

            migrationBuilder.DropSequence(
                name: "AssetSequence");

            migrationBuilder.DropSequence(
                name: "MaintenanceContentSequence");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_MaintenanceContent",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValueSql: "CONCAT('SN-', LPAD(NEXTVAL('\"MaintenanceContentSequence\"')::text, 5, '0'))");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_AssetMaintenanceSheet",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValueSql: "CONCAT('MSN-', LPAD(NEXTVAL('\"AssetMaintenanceSheetSequence\"')::text, 5, '0'))");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_AssetLiquidationSheet",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValueSql: "CONCAT('LSN-', LPAD(NEXTVAL('\"AssetLiquidationSheetSequence\"')::text, 5, '0'))");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_Asset",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValueSql: "CONCAT('AN-', LPAD(NEXTVAL('\"AssetSequence\"')::text, 5, '0'))");
        }
    }
}
