using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class changefieldCashbookTransation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Cashbook_Transaction_mk_DuAn_ProjectId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Cashbook_Transaction_sm_Customer_CustomerId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Cashbook_Transaction_sm_Supplier_SupplierId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Cashbook_Transaction_CustomerId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Cashbook_Transaction_ProjectId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Cashbook_Transaction_SupplierId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.RenameColumn(
                name: "PaymentGroupCode",
                table: "sm_Cashbook_Transaction",
                newName: "EntityUrl");

            migrationBuilder.AddColumn<string>(
                name: "EntityCode",
                table: "sm_Cashbook_Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "sm_Cashbook_Transaction",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                table: "sm_Cashbook_Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityTypeCode",
                table: "sm_Cashbook_Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityTypeName",
                table: "sm_Cashbook_Transaction",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityCode",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "EntityName",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "EntityTypeCode",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "EntityTypeName",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.RenameColumn(
                name: "EntityUrl",
                table: "sm_Cashbook_Transaction",
                newName: "PaymentGroupCode");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "sm_Cashbook_Transaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "sm_Cashbook_Transaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "sm_Cashbook_Transaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_CustomerId",
                table: "sm_Cashbook_Transaction",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_ProjectId",
                table: "sm_Cashbook_Transaction",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_SupplierId",
                table: "sm_Cashbook_Transaction",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Cashbook_Transaction_mk_DuAn_ProjectId",
                table: "sm_Cashbook_Transaction",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Cashbook_Transaction_sm_Customer_CustomerId",
                table: "sm_Cashbook_Transaction",
                column: "CustomerId",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Cashbook_Transaction_sm_Supplier_SupplierId",
                table: "sm_Cashbook_Transaction",
                column: "SupplierId",
                principalTable: "sm_Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
