using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class mappingusernameintaskmanagementassignee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskManagementAssignee_UserId",
                table: "sm_TaskManagementAssignee",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskManagementAssignee_idm_User_UserId",
                table: "sm_TaskManagementAssignee",
                column: "UserId",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskManagementAssignee_idm_User_UserId",
                table: "sm_TaskManagementAssignee");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskManagementAssignee_UserId",
                table: "sm_TaskManagementAssignee");
        }
    }
}
