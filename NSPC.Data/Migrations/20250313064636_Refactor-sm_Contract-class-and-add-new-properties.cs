using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class Refactorsm_Contractclassandaddnewproperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "sm_Contract");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedOnDate",
                table: "sm_Contract",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedByUserId",
                table: "sm_Contract",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentTypeColor",
                table: "sm_Contract",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedByUserName",
                table: "sm_Contract",
                type: "text",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Contract_sm_Contract_ParentId",
                table: "sm_Contract");

            migrationBuilder.DropIndex(
                name: "IX_sm_Contract_ParentId",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "DocumentTypeColor",
                table: "sm_Contract");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserName",
                table: "sm_Contract");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedOnDate",
                table: "sm_Contract",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedByUserId",
                table: "sm_Contract",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationId",
                table: "sm_Contract",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
