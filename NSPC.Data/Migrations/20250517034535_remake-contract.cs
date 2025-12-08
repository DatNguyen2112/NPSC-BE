using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data.Data.Entity.Contract;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class remakecontract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Contract_sm_Contract_ParentId",
                table: "sm_Contract");

            migrationBuilder.DropIndex(
                name: "IX_sm_Contract_ParentId",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ConstructionCategory",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "DocumentTypeCode",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "DocumentTypeColor",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "DocumentTypeName",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "StatusColor",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "StatusName",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ValueAmount",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "VatPercent",
                table: "sm_Contract");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_Contract",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AcceptanceValueBeforeVatAmount",
                table: "sm_Contract",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcceptanceYear",
                table: "sm_Contract",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<List<ContractAppendixItem>>(
                name: "Appendices",
                table: "sm_Contract",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDate",
                table: "sm_Contract",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignmentAYear",
                table: "sm_Contract",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ConsultingServiceId",
                table: "sm_Contract",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ExpectedAcceptanceMonth",
                table: "sm_Contract",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpectedApprovalMonth",
                table: "sm_Contract",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedVolume",
                table: "sm_Contract",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceIssuanceDate",
                table: "sm_Contract",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SettlementValueAmount",
                table: "sm_Contract",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "sm_Contract",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "TaxRatePercentage",
                table: "sm_Contract",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateStageId",
                table: "sm_Contract",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "ValueBeforeVatAmount",
                table: "sm_Contract",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Contract_ConsultingServiceId",
                table: "sm_Contract",
                column: "ConsultingServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Contract_CreatedByUserId",
                table: "sm_Contract",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Contract_LastModifiedByUserId",
                table: "sm_Contract",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Contract_TemplateStageId",
                table: "sm_Contract",
                column: "TemplateStageId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Contract_idm_User_CreatedByUserId",
                table: "sm_Contract",
                column: "CreatedByUserId",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Contract_idm_User_LastModifiedByUserId",
                table: "sm_Contract",
                column: "LastModifiedByUserId",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Contract_sm_CodeType_ConsultingServiceId",
                table: "sm_Contract",
                column: "ConsultingServiceId",
                principalTable: "sm_CodeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Contract_sm_TemplateStage_TemplateStageId",
                table: "sm_Contract",
                column: "TemplateStageId",
                principalTable: "sm_TemplateStage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Contract_idm_User_CreatedByUserId",
                table: "sm_Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Contract_idm_User_LastModifiedByUserId",
                table: "sm_Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Contract_sm_CodeType_ConsultingServiceId",
                table: "sm_Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Contract_sm_TemplateStage_TemplateStageId",
                table: "sm_Contract");

            migrationBuilder.DropIndex(
                name: "IX_sm_Contract_ConsultingServiceId",
                table: "sm_Contract");

            migrationBuilder.DropIndex(
                name: "IX_sm_Contract_CreatedByUserId",
                table: "sm_Contract");

            migrationBuilder.DropIndex(
                name: "IX_sm_Contract_LastModifiedByUserId",
                table: "sm_Contract");

            migrationBuilder.DropIndex(
                name: "IX_sm_Contract_TemplateStageId",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "AcceptanceValueBeforeVatAmount",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "AcceptanceYear",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "Appendices",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ApprovalDate",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "AssignmentAYear",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ConsultingServiceId",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ExpectedAcceptanceMonth",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ExpectedApprovalMonth",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ExpectedVolume",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "InvoiceIssuanceDate",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "SettlementValueAmount",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "TaxRatePercentage",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "TemplateStageId",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "ValueBeforeVatAmount",
                table: "sm_Contract");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_Contract",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "ConstructionCategory",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentTypeCode",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentTypeColor",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentTypeName",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "sm_Contract",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "sm_Contract",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "sm_Contract",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "StatusCode",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusColor",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusName",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "sm_Contract",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueAmount",
                table: "sm_Contract",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatPercent",
                table: "sm_Contract",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Contract_ParentId",
                table: "sm_Contract",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Contract_sm_Contract_ParentId",
                table: "sm_Contract",
                column: "ParentId",
                principalTable: "sm_Contract",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
