using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addprojectId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_ProjectId",
                table: "sm_Cashbook_Transaction",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Cashbook_Transaction_mk_DuAn_ProjectId",
                table: "sm_Cashbook_Transaction",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Cashbook_Transaction_mk_DuAn_ProjectId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Cashbook_Transaction_ProjectId",
                table: "sm_Cashbook_Transaction");
        }
    }
}
