using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updatesm_tasknotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_sm_TaskNotification_CreatedByUserId",
                table: "sm_TaskNotification",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_TaskNotification_idm_User_CreatedByUserId",
                table: "sm_TaskNotification",
                column: "CreatedByUserId",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_TaskNotification_idm_User_CreatedByUserId",
                table: "sm_TaskNotification");

            migrationBuilder.DropIndex(
                name: "IX_sm_TaskNotification_CreatedByUserId",
                table: "sm_TaskNotification");
        }
    }
}
