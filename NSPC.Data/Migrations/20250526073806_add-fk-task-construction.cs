using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfktaskconstruction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConstructionId",
                table: "sm_Task",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_sm_Task_ConstructionId",
                table: "sm_Task",
                column: "ConstructionId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Task_sm_Construction_ConstructionId",
                table: "sm_Task",
                column: "ConstructionId",
                principalTable: "sm_Construction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Task_sm_Construction_ConstructionId",
                table: "sm_Task");

            migrationBuilder.DropIndex(
                name: "IX_sm_Task_ConstructionId",
                table: "sm_Task");

            migrationBuilder.DropColumn(
                name: "ConstructionId",
                table: "sm_Task");
        }
    }
}
