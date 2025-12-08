using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class reftabletaskmanagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskManagement_mk_DuAn_ProjectId",
                table: "sm_TaskManagement");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "sm_TaskManagement",
                newName: "ConstructionId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_TaskManagement_ProjectId",
                table: "sm_TaskManagement",
                newName: "IX_sm_TaskManagement_ConstructionId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskManagement_sm_Construction_ConstructionId",
                table: "sm_TaskManagement",
                column: "ConstructionId",
                principalTable: "sm_Construction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskManagement_sm_Construction_ConstructionId",
                table: "sm_TaskManagement");

            migrationBuilder.RenameColumn(
                name: "ConstructionId",
                table: "sm_TaskManagement",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_sm_TaskManagement_ConstructionId",
                table: "sm_TaskManagement",
                newName: "IX_sm_TaskManagement_ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskManagement_mk_DuAn_ProjectId",
                table: "sm_TaskManagement",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
