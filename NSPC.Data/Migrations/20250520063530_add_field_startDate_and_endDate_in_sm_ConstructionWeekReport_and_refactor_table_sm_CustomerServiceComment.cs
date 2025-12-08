using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_startDate_and_endDate_in_sm_ConstructionWeekReport_and_refactor_table_sm_CustomerServiceComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConstructionId",
                table: "sm_CustomerServiceComment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<List<string>>(
                name: "TagIds",
                table: "sm_CustomerServiceComment",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "sm_ConstructionWeekReport",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "sm_ConstructionWeekReport",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_CustomerServiceComment_ConstructionId",
                table: "sm_CustomerServiceComment",
                column: "ConstructionId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_CustomerServiceComment_sm_Construction_ConstructionId",
                table: "sm_CustomerServiceComment",
                column: "ConstructionId",
                principalTable: "sm_Construction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_CustomerServiceComment_sm_Construction_ConstructionId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropIndex(
                name: "IX_sm_CustomerServiceComment_ConstructionId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropColumn(
                name: "ConstructionId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropColumn(
                name: "TagIds",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "sm_ConstructionWeekReport");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "sm_ConstructionWeekReport");
        }
    }
}
