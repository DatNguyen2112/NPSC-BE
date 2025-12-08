using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_constructionTemplateId_into_sm_Constructions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConstructionTemplateId",
                table: "sm_Construction",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_sm_Construction_ConstructionTemplateId",
                table: "sm_Construction",
                column: "ConstructionTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Construction_sm_ProjectTemplate_ConstructionTemplateId",
                table: "sm_Construction",
                column: "ConstructionTemplateId",
                principalTable: "sm_ProjectTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Construction_sm_ProjectTemplate_ConstructionTemplateId",
                table: "sm_Construction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Construction_ConstructionTemplateId",
                table: "sm_Construction");

            migrationBuilder.DropColumn(
                name: "ConstructionTemplateId",
                table: "sm_Construction");
        }
    }
}
