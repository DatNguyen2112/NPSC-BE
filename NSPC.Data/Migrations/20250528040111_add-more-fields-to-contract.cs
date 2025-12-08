using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addmorefieldstocontract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SettlementValueAmount",
                table: "sm_Contract");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "sm_Contract",
                newName: "SupplementaryContractRequired");

            migrationBuilder.RenameColumn(
                name: "InvoiceIssuanceDate",
                table: "sm_Contract",
                newName: "SurveyAcceptanceRecordDate");

            migrationBuilder.AddColumn<int>(
                name: "AcceptanceDocumentStatus",
                table: "sm_Contract",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AcceptancePlan",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContractDurationDays",
                table: "sm_Contract",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractNumber",
                table: "sm_Contract",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractSigningDate",
                table: "sm_Contract",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DesignApprovalDate",
                table: "sm_Contract",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HandoverRecordDate",
                table: "sm_Contract",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImplementationStatus",
                table: "sm_Contract",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<List<DateTime>>(
                name: "InvoiceIssuanceDates",
                table: "sm_Contract",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceStatus",
                table: "sm_Contract",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Issues",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SiteSurveyRecordDate",
                table: "sm_Contract",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptanceDocumentStatus",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "AcceptancePlan",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ContractDurationDays",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ContractNumber",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ContractSigningDate",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "DesignApprovalDate",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "HandoverRecordDate",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ImplementationStatus",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "InvoiceIssuanceDates",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "InvoiceStatus",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "Issues",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "SiteSurveyRecordDate",
                table: "sm_Contract");

            migrationBuilder.RenameColumn(
                name: "SurveyAcceptanceRecordDate",
                table: "sm_Contract",
                newName: "InvoiceIssuanceDate");

            migrationBuilder.RenameColumn(
                name: "SupplementaryContractRequired",
                table: "sm_Contract",
                newName: "Status");

            migrationBuilder.AddColumn<decimal>(
                name: "SettlementValueAmount",
                table: "sm_Contract",
                type: "numeric(18,2)",
                nullable: true);
        }
    }
}
