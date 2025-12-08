using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class changephongBanfrommk_PhongBantosm_CodeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_VehicleRequest_mk_PhongBan_DepartmentId",
                table: "sm_VehicleRequest");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_VehicleRequest_sm_CodeType_DepartmentId",
                table: "sm_VehicleRequest",
                column: "DepartmentId",
                principalTable: "sm_CodeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_VehicleRequest_sm_CodeType_DepartmentId",
                table: "sm_VehicleRequest");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_VehicleRequest_mk_PhongBan_DepartmentId",
                table: "sm_VehicleRequest",
                column: "DepartmentId",
                principalTable: "mk_PhongBan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
