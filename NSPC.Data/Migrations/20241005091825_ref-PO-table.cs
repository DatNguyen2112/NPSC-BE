using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class refPOtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VAT",
                table: "sm_PurchaseOrder",
                newName: "VATAmount");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "sm_PurchaseOrder",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "sm_PurchaseOrder",
                newName: "PurchaseReason");

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "sm_PurchaseOrder",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.RenameColumn(
                name: "TotalRemaningAmount",
                table: "sm_PurchaseOrder",
                newName: "RemainingAmount");

            migrationBuilder.RenameColumn(
                name: "TotalPaidAmount",
                table: "sm_PurchaseOrder",
                newName: "PaidAmount");

            migrationBuilder.RenameColumn(
                name: "ReceiveInventoryStatus",
                table: "sm_PurchaseOrder",
                newName: "ImportStatusCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImportStatusCode",
                table: "sm_PurchaseOrder",
                newName: "ReceiveInventoryStatus");

            migrationBuilder.RenameColumn(
                name: "RemainingAmount",
                table: "sm_PurchaseOrder",
                newName: "TotalRemaningAmount");

            migrationBuilder.RenameColumn(
                name: "PaidAmount",
                table: "sm_PurchaseOrder",
                newName: "TotalPaidAmount");


            migrationBuilder.DropColumn(
                name: "Reference",
                table: "sm_PurchaseOrder");

            migrationBuilder.RenameColumn(
                name: "VATAmount",
                table: "sm_PurchaseOrder",
                newName: "VAT");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "sm_PurchaseOrder",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "PurchaseReason",
                table: "sm_PurchaseOrder",
                newName: "Reason");
        }
    }
}
