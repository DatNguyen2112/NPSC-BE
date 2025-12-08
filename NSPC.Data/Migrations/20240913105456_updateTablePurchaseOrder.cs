using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updateTablePurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackageAndDeliveryType",
                table: "sm_PurchaseOrder");

            migrationBuilder.AddColumn<List<jsonb_HalfPayment>>(
                name: "HalfPayment",
                table: "sm_PurchaseOrder",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalHalfPayment",
                table: "sm_PurchaseOrder",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaymentContinue",
                table: "sm_PurchaseOrder",
                type: "numeric",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HalfPayment",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "TotalHalfPayment",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "TotalPaymentContinue",
                table: "sm_PurchaseOrder");

            migrationBuilder.AddColumn<string>(
                name: "PackageAndDeliveryType",
                table: "sm_PurchaseOrder",
                type: "text",
                nullable: true);
        }
    }
}
