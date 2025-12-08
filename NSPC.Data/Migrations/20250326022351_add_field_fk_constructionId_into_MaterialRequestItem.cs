using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_fk_constructionId_into_MaterialRequestItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConstructionId",
                table: "sm_MaterialRequestItem",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_sm_MaterialRequestItem_ConstructionId",
                table: "sm_MaterialRequestItem",
                column: "ConstructionId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_MaterialRequestItem_sm_Construction_ConstructionId",
                table: "sm_MaterialRequestItem",
                column: "ConstructionId",
                principalTable: "sm_Construction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_MaterialRequestItem_sm_Construction_ConstructionId",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_MaterialRequestItem_ConstructionId",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "ConstructionId",
                table: "sm_MaterialRequestItem");
        }
    }
}
