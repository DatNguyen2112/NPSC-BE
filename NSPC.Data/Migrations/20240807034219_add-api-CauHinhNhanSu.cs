using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addapiCauHinhNhanSu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mk_CauHinhNhanSu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ma = table.Column<string>(type: "text", nullable: true),
                    TenNhanSu = table.Column<string>(type: "text", nullable: true),
                    IdChucVu = table.Column<Guid>(type: "uuid", nullable: true),
                    LuongCoBan = table.Column<decimal>(type: "numeric", nullable: true),
                    AnCa = table.Column<decimal>(type: "numeric", nullable: true),
                    DienThoai = table.Column<decimal>(type: "numeric", nullable: true),
                    TrangPhuc = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_CauHinhNhanSu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mk_CauHinhNhanSu_mk_ChucVu_IdChucVu",
                        column: x => x.IdChucVu,
                        principalTable: "mk_ChucVu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mk_CauHinhNhanSu_IdChucVu",
                table: "mk_CauHinhNhanSu",
                column: "IdChucVu");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_CauHinhNhanSu");
        }
    }
}
