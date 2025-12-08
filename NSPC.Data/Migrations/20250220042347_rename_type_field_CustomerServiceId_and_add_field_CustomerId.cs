using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class rename_type_field_CustomerServiceId_and_add_field_CustomerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerServiceId",
                table: "sm_CustomerServiceComment",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "sm_CustomerServiceComment",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_CustomerServiceComment_CustomerId",
                table: "sm_CustomerServiceComment",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_CustomerServiceComment_sm_Customer_CustomerId",
                table: "sm_CustomerServiceComment",
                column: "CustomerId",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_CustomerServiceComment_sm_Customer_CustomerId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropIndex(
                name: "IX_sm_CustomerServiceComment_CustomerId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerServiceId",
                table: "sm_CustomerServiceComment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
