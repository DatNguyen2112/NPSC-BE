using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updatepropertiestasktemplateStage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateStages",
                table: "sm_Task");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDateTime",
                table: "sm_Task",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDateTime",
                table: "sm_Task",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "sm_Task",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConstructionId",
                table: "sm_Task",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_Task",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateStageId",
                table: "sm_Task",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_Task_TemplateStageId",
                table: "sm_Task",
                column: "TemplateStageId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Task_sm_TemplateStage_TemplateStageId",
                table: "sm_Task",
                column: "TemplateStageId",
                principalTable: "sm_TemplateStage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Task_sm_TemplateStage_TemplateStageId",
                table: "sm_Task");

            migrationBuilder.DropIndex(
                name: "IX_sm_Task_TemplateStageId",
                table: "sm_Task");

            migrationBuilder.DropColumn(
                name: "TemplateStageId",
                table: "sm_Task");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDateTime",
                table: "sm_Task",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDateTime",
                table: "sm_Task",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "sm_Task",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ConstructionId",
                table: "sm_Task",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "sm_Task",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<List<jsonb_TemplateStage>>(
                name: "TemplateStages",
                table: "sm_Task",
                type: "jsonb",
                nullable: true);
        }
    }
}
