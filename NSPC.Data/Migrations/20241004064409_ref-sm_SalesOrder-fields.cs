using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class refsm_SalesOrderfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "TotalDiscountsAccount",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "TotalPaidAmount",
                table: "sm_SalesOrder");

            migrationBuilder.RenameColumn(
                name: "TotalRemainingAmount",
                table: "sm_SalesOrder",
                newName: "RemainingAmount");

            migrationBuilder.RenameColumn(
                name: "TotalReceiptInventory",
                table: "sm_SalesOrder",
                newName: "PaidAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RemainingAmount",
                table: "sm_SalesOrder",
                newName: "TotalRemainingAmount");

            migrationBuilder.RenameColumn(
                name: "PaidAmount",
                table: "sm_SalesOrder",
                newName: "TotalReceiptInventory");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "sm_SalesOrder",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDiscountsAccount",
                table: "sm_SalesOrder",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaidAmount",
                table: "sm_SalesOrder",
                type: "numeric",
                nullable: true);
        }
    }
}
