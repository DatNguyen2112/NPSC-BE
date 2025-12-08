using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class storetemplateStateinjsonb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Contract_sm_TemplateStage_TemplateStageId",
                table: "sm_Contract");

            migrationBuilder.DropIndex(
                name: "IX_sm_Contract_TemplateStageId",
                table: "sm_Contract");

            migrationBuilder.AddColumn<jsonb_TemplateStage>(
                name: "TemplateStage",
                table: "sm_Contract",
                type: "jsonb",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateStage",
                table: "sm_Contract");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Contract_TemplateStageId",
                table: "sm_Contract",
                column: "TemplateStageId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Contract_sm_TemplateStage_TemplateStageId",
                table: "sm_Contract",
                column: "TemplateStageId",
                principalTable: "sm_TemplateStage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
