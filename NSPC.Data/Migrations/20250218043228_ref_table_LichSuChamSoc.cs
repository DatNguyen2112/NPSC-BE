using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class ref_table_LichSuChamSoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerServiceContent",
                table: "sm_LichSuChamSoc",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime?[]>(
                name: "DateRange",
                table: "sm_LichSuChamSoc",
                type: "timestamp without time zone[]",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantPersonId",
                table: "sm_LichSuChamSoc",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "sm_LichSuChamSoc",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriorityColor",
                table: "sm_LichSuChamSoc",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "sm_LichSuChamSoc",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusCode",
                table: "sm_LichSuChamSoc",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_LichSuChamSoc_ProjectId",
                table: "sm_LichSuChamSoc",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_LichSuChamSoc_mk_DuAn_ProjectId",
                table: "sm_LichSuChamSoc",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_LichSuChamSoc_mk_DuAn_ProjectId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropIndex(
                name: "IX_sm_LichSuChamSoc_ProjectId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "CustomerServiceContent",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "DateRange",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "ParticipantPersonId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "PriorityColor",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "sm_LichSuChamSoc");
        }
    }
}
