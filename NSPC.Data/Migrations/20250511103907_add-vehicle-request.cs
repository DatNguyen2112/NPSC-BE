using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addvehiclerequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_VehicleRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestCode = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentName = table.Column<string>(type: "text", nullable: true),
                    ContactPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProjectName = table.Column<string>(type: "text", nullable: true),
                    Purpose = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    NumPassengers = table.Column<int>(type: "integer", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DepartureLocation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DestinationLocation = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RequestedVehicleTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedVehicleTypeName = table.Column<string>(type: "text", nullable: true),
                    RequestedVehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedVehiclePlateNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RejectNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_VehicleRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_VehicleRequest_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_VehicleRequest_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_VehicleRequest_idm_User_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_VehicleRequest_idm_User_UserId",
                        column: x => x.UserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_VehicleRequest_mk_DuAn_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_VehicleRequest_mk_PhongBan_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "mk_PhongBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_VehicleRequest_sm_LoaiXe_RequestedVehicleTypeId",
                        column: x => x.RequestedVehicleTypeId,
                        principalTable: "sm_LoaiXe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_VehicleRequest_sm_PhuongTien_RequestedVehicleId",
                        column: x => x.RequestedVehicleId,
                        principalTable: "sm_PhuongTien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_VehicleRequest_CreatedByUserId",
                table: "sm_VehicleRequest",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_VehicleRequest_DepartmentId",
                table: "sm_VehicleRequest",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_VehicleRequest_LastModifiedByUserId",
                table: "sm_VehicleRequest",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_VehicleRequest_ProjectId",
                table: "sm_VehicleRequest",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_VehicleRequest_RequestedVehicleId",
                table: "sm_VehicleRequest",
                column: "RequestedVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_VehicleRequest_RequestedVehicleTypeId",
                table: "sm_VehicleRequest",
                column: "RequestedVehicleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_VehicleRequest_TenantId",
                table: "sm_VehicleRequest",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_VehicleRequest_UserId",
                table: "sm_VehicleRequest",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_VehicleRequest");
        }
    }
}
