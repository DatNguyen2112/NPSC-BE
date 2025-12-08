using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removecolumnisQuotationaddcolumnTotalQuotationandaddfkQuotationIdinentityQuotationItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_QuotationItem_sm_Quotation_sm_QuotationId",
                table: "sm_QuotationItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_QuotationItem_sm_QuotationId",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "sm_QuotationId",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "isQuotation",
                table: "sm_Customer");

            migrationBuilder.AddColumn<Guid>(
                name: "QuotationId",
                table: "sm_QuotationItem",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "TotalQuotation",
                table: "sm_Customer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_sm_QuotationItem_QuotationId",
                table: "sm_QuotationItem",
                column: "QuotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_QuotationItem_sm_Quotation_QuotationId",
                table: "sm_QuotationItem",
                column: "QuotationId",
                principalTable: "sm_Quotation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_QuotationItem_sm_Quotation_QuotationId",
                table: "sm_QuotationItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_QuotationItem_QuotationId",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "QuotationId",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "TotalQuotation",
                table: "sm_Customer");

            migrationBuilder.AddColumn<Guid>(
                name: "sm_QuotationId",
                table: "sm_QuotationItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isQuotation",
                table: "sm_Customer",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_sm_QuotationItem_sm_QuotationId",
                table: "sm_QuotationItem",
                column: "sm_QuotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_QuotationItem_sm_Quotation_sm_QuotationId",
                table: "sm_QuotationItem",
                column: "sm_QuotationId",
                principalTable: "sm_Quotation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
