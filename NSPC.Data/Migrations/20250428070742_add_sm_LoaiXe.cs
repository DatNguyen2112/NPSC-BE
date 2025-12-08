using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_sm_LoaiXe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LoaiXeId",
                table: "sm_PhuongTien",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "sm_LoaiXe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenLoaiXe = table.Column<string>(type: "text", nullable: true),
                    MoTa = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_sm_LoaiXe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_LoaiXe_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_PhuongTien_LoaiXeId",
                table: "sm_PhuongTien",
                column: "LoaiXeId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_LoaiXe_TenantId",
                table: "sm_LoaiXe",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PhuongTien_sm_LoaiXe_LoaiXeId",
                table: "sm_PhuongTien",
                column: "LoaiXeId",
                principalTable: "sm_LoaiXe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_PhuongTien_sm_LoaiXe_LoaiXeId",
                table: "sm_PhuongTien");

            migrationBuilder.DropTable(
                name: "sm_LoaiXe");

            migrationBuilder.DropIndex(
                name: "IX_sm_PhuongTien_LoaiXeId",
                table: "sm_PhuongTien");

            migrationBuilder.DropColumn(
                name: "LoaiXeId",
                table: "sm_PhuongTien");
        }
    }
}
