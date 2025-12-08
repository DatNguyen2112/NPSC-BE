using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class fixapiNgayLamViec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "SoNgayLamViec",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "TenNgayNghi",
                table: "mk_WorkingDay");

            migrationBuilder.AlterColumn<int>(
                name: "Month",
                table: "mk_WorkingDay",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "mk_WorkingDay",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOverride",
                table: "mk_WorkingDay",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "mk_WorkingDay",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalType",
                table: "mk_WorkingDay",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "mk_WorkingDay",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "mk_WorkingDay",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "IsOverride",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "OriginalType",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "mk_WorkingDay");

            migrationBuilder.AlterColumn<string>(
                name: "Month",
                table: "mk_WorkingDay",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "mk_WorkingDay",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SoNgayLamViec",
                table: "mk_WorkingDay",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "mk_WorkingDay",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TenNgayNghi",
                table: "mk_WorkingDay",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
