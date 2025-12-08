using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updatetablecashbook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isStatus",
                table: "sm_Cashbook_Transaction",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "sm_Cashbook_Transaction",
                newName: "TypeCode");

            migrationBuilder.RenameColumn(
                name: "PayerGroup",
                table: "sm_Cashbook_Transaction",
                newName: "SubTypeCode");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceiptDate",
                table: "sm_Cashbook_Transaction",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<decimal>(
                name: "ClosingBanlance",
                table: "sm_Cashbook_Transaction",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "sm_Cashbook_Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OpeningBalance",
                table: "sm_Cashbook_Transaction",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalDocumentCode",
                table: "sm_Cashbook_Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentGroupCode",
                table: "sm_Cashbook_Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodCode",
                table: "sm_Cashbook_Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "sm_Cashbook_Transaction",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingBanlance",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "OpeningBalance",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "OriginalDocumentCode",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "PaymentGroupCode",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "PaymentMethodCode",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.RenameColumn(
                name: "TypeCode",
                table: "sm_Cashbook_Transaction",
                newName: "PaymentMethod");

            migrationBuilder.RenameColumn(
                name: "SubTypeCode",
                table: "sm_Cashbook_Transaction",
                newName: "PayerGroup");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "sm_Cashbook_Transaction",
                newName: "isStatus");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceiptDate",
                table: "sm_Cashbook_Transaction",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);
        }
    }
}
