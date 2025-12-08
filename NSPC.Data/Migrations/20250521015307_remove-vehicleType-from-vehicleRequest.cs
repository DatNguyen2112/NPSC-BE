using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removevehicleTypefromvehicleRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_VehicleRequest_sm_LoaiXe_RequestedVehicleTypeId",
                table: "sm_VehicleRequest");

            migrationBuilder.DropIndex(
                name: "IX_sm_VehicleRequest_RequestedVehicleTypeId",
                table: "sm_VehicleRequest");

            migrationBuilder.DropColumn(
                name: "RequestedVehicleTypeId",
                table: "sm_VehicleRequest");

            migrationBuilder.DropColumn(
                name: "RequestedVehicleTypeName",
                table: "sm_VehicleRequest");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequestedVehicleTypeId",
                table: "sm_VehicleRequest",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedVehicleTypeName",
                table: "sm_VehicleRequest",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_VehicleRequest_RequestedVehicleTypeId",
                table: "sm_VehicleRequest",
                column: "RequestedVehicleTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_VehicleRequest_sm_LoaiXe_RequestedVehicleTypeId",
                table: "sm_VehicleRequest",
                column: "RequestedVehicleTypeId",
                principalTable: "sm_LoaiXe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
