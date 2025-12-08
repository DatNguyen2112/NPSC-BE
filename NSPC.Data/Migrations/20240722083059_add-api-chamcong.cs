using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addapichamcong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mk_ChamCong",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenBangChamCong = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_ChamCong", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mk_ChamCongItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaSo = table.Column<string>(type: "text", nullable: true),
                    HoVaTen = table.Column<string>(type: "text", nullable: true),
                    ChucVu = table.Column<string>(type: "text", nullable: true),
                    NgayCong = table.Column<decimal>(type: "numeric", nullable: true),
                    LamThemNgayThuong = table.Column<decimal>(type: "numeric", nullable: true),
                    LamThemChuNhat = table.Column<decimal>(type: "numeric", nullable: true),
                    LamThemNgayLe = table.Column<decimal>(type: "numeric", nullable: true),
                    KyTen = table.Column<string>(type: "text", nullable: true),
                    IdChamCong = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_ChamCongItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_ChamCongItem_mk_ChamCong_IdChamCong",
                        column: x => x.IdChamCong,
                        principalTable: "mk_ChamCong",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_ChamCongItem_IdChamCong",
                table: "mk_ChamCongItem",
                column: "IdChamCong");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_ChamCongItem");

            migrationBuilder.DropTable(
                name: "mk_ChamCong");
        }
    }
}
