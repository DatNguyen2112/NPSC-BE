using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class changeprojectfrommk_DuAntosm_Construction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_VehicleRequest_mk_DuAn_ProjectId",
                table: "sm_VehicleRequest");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_VehicleRequest_sm_Construction_ProjectId",
                table: "sm_VehicleRequest",
                column: "ProjectId",
                principalTable: "sm_Construction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_VehicleRequest_sm_Construction_ProjectId",
                table: "sm_VehicleRequest");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_VehicleRequest_mk_DuAn_ProjectId",
                table: "sm_VehicleRequest",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
