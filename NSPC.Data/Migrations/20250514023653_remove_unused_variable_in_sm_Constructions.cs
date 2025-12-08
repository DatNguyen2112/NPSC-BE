using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class remove_unused_variable_in_sm_Constructions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Construction_sm_Customer_CustomerId",
                table: "sm_Construction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Construction_CustomerId",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "ConstructionAttachments",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "DistrictName",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "ListPredicateInventory",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "ListTeamInventory",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "ProvinceName",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "WardName",
                table: "sm_Construction");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<jsonb_Attachment>>(
                name: "ConstructionAttachments",
                table: "sm_Construction",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "sm_Construction",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "DistrictCode",
                table: "sm_Construction",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DistrictName",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "sm_Construction",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<List<jsonb_PredicateInventory>>(
                name: "ListPredicateInventory",
                table: "sm_Construction",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<List<jsonb_TeamInventory>>(
                name: "ListTeamInventory",
                table: "sm_Construction",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceCode",
                table: "sm_Construction",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceName",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "sm_Construction",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "WardCode",
                table: "sm_Construction",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WardName",
                table: "sm_Construction",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Construction_CustomerId",
                table: "sm_Construction",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Construction_sm_Customer_CustomerId",
                table: "sm_Construction",
                column: "CustomerId",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
