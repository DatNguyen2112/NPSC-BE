using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class refPoItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "sm_PurchaseOrder",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "sm_PurchaseOrder",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "sm_PurchaseOrder",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.RenameColumn(
                name: "TotalLineAmount",
                table: "sm_PurchaseOrderItem",
                newName: "LineAmount");

            migrationBuilder.RenameColumn(
                name: "Amout",
                table: "sm_PurchaseOrderItem",
                newName: "UnitPrice");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPercent",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);


            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LineAmount",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "sm_PurchaseOrderItem",
                newName: "UnitPriceDiscountPercent");

            migrationBuilder.AddColumn<decimal>(
                name: "AfterLineDiscountGoodsAmount",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedUnitPrice",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GoodsAmount",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsProductVATApplied",
                table: "sm_PurchaseOrderItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LineNo",
                table: "sm_PurchaseOrderItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SalesOrderDiscountAmount",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPriceDiscountAmount",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UnitPriceDiscountType",
                table: "sm_PurchaseOrderItem",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VATAmount",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "VATCode",
                table: "sm_PurchaseOrderItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VATPercent",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VATableAmount",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "AfterLineDiscountGoodsAmount",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "DiscountedUnitPrice",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "GoodsAmount",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "IsProductVATApplied",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "LineNo",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "SalesOrderDiscountAmount",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "UnitPriceDiscountAmount",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "UnitPriceDiscountType",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "VATAmount",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "VATCode",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "VATPercent",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "VATableAmount",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.RenameColumn(
                name: "UnitPriceDiscountPercent",
                table: "sm_PurchaseOrderItem",
                newName: "DiscountPercent");

            migrationBuilder.AlterColumn<decimal>(
                 name: "UnitPrice",
                 table: "sm_PurchaseOrderItem",
                 type: "numeric",
                 nullable: true,
                 oldClrType: typeof(decimal),
                 oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "LineAmount",
                table: "sm_PurchaseOrderItem",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountPercent",
                table: "sm_PurchaseOrderItem",
                type: "integer",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.RenameColumn(
                name: "LineAmount",
                table: "sm_PurchaseOrderItem",
                newName: "TotalLineAmount");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "sm_PurchaseOrderItem",
                newName: "Amout");


            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "sm_PurchaseOrder");
        }
    }
}
