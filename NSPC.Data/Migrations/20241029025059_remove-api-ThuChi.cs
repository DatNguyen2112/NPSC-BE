using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removeapiThuChi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_ThuChi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mk_ThuChi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    IdDuAn = table.Column<Guid>(type: "uuid", nullable: true),
                    IdMaNhaCungCap = table.Column<Guid>(type: "uuid", nullable: true),
                    ChiChoMucDich = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    IdSupplier = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LoaiChiPhi = table.Column<string>(type: "text", nullable: true),
                    MaChi = table.Column<string>(type: "text", nullable: true),
                    NgayThuChi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SoTien = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_ThuChi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_ThuChi_mk_DuAn_IdDuAn",
                        column: x => x.IdDuAn,
                        principalTable: "mk_DuAn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_ThuChi_sm_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "sm_Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mk_ThuChi_sm_Supplier_IdMaNhaCungCap",
                        column: x => x.IdMaNhaCungCap,
                        principalTable: "sm_Supplier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_CustomerId",
                table: "mk_ThuChi",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_IdDuAn",
                table: "mk_ThuChi",
                column: "IdDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_IdMaNhaCungCap",
                table: "mk_ThuChi",
                column: "IdMaNhaCungCap");
        }
    }
}
