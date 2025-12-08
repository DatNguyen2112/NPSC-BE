using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addsm_Khosm_LaiXeandsm_PhuongTienclasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_Kho",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ma = table.Column<string>(type: "text", nullable: true),
                    Ten = table.Column<string>(type: "text", nullable: true),
                    DiaChi = table.Column<string>(type: "text", nullable: true),
                    LoaiKho = table.Column<string>(type: "text", nullable: true),
                    IsCuaHang = table.Column<bool>(type: "boolean", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    ProvinceCode = table.Column<int>(type: "integer", nullable: true),
                    ProvinceName = table.Column<string>(type: "text", nullable: true),
                    DistrictCode = table.Column<int>(type: "integer", nullable: true),
                    DistrictName = table.Column<string>(type: "text", nullable: true),
                    CommuneCode = table.Column<int>(type: "integer", nullable: true),
                    CommuneName = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Binh = table.Column<decimal>(type: "numeric", nullable: true),
                    VoBinh = table.Column<decimal>(type: "numeric", nullable: true),
                    GasDu = table.Column<decimal>(type: "numeric", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: true),
                    IsInitialized = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_sm_Kho", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Kho_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_Kho_idm_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_Kho_sm_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "sm_Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_PhuongTien",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BienSoXe = table.Column<string>(type: "text", nullable: true),
                    SoKhung = table.Column<string>(type: "text", nullable: true),
                    SoMay = table.Column<string>(type: "text", nullable: true),
                    HangSanXuat = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    NamSanXuat = table.Column<string>(type: "text", nullable: true),
                    TaiTrong = table.Column<string>(type: "text", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_sm_PhuongTien", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_PhuongTien_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_PhuongTien_sm_Kho_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "sm_Kho",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_LaiXe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdPhuongTien = table.Column<Guid>(type: "uuid", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TenTaiXe = table.Column<string>(type: "text", nullable: true),
                    MaTaiXe = table.Column<string>(type: "text", nullable: true),
                    Cccd = table.Column<string>(type: "text", nullable: true),
                    Gplx = table.Column<string>(type: "text", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_sm_LaiXe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_LaiXe_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_LaiXe_idm_User_UserId",
                        column: x => x.UserId,
                        principalTable: "idm_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_LaiXe_sm_PhuongTien_IdPhuongTien",
                        column: x => x.IdPhuongTien,
                        principalTable: "sm_PhuongTien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_Kho_CreatedByUserId",
                table: "sm_Kho",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Kho_CustomerId",
                table: "sm_Kho",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Kho_TenantId",
                table: "sm_Kho",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_LaiXe_IdPhuongTien",
                table: "sm_LaiXe",
                column: "IdPhuongTien",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_LaiXe_TenantId",
                table: "sm_LaiXe",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_LaiXe_UserId",
                table: "sm_LaiXe",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_PhuongTien_TenantId",
                table: "sm_PhuongTien",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_PhuongTien_WarehouseId",
                table: "sm_PhuongTien",
                column: "WarehouseId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_LaiXe");

            migrationBuilder.DropTable(
                name: "sm_PhuongTien");

            migrationBuilder.DropTable(
                name: "sm_Kho");
        }
    }
}
