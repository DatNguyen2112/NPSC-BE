using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class fixBaseTableService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOnDate",
                table: "idm_User",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "idm_User",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "idm_User",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedByUserId",
                table: "idm_User",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedByUserName",
                table: "idm_User",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "idm_User");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "idm_User");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                table: "idm_User");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserName",
                table: "idm_User");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOnDate",
                table: "idm_User",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }
    }
}
