using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addapiBHXH : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mk_BHXH",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ToDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BHXHNguoiLaoDong = table.Column<decimal>(type: "numeric", nullable: true),
                    BHYTNguoiLaoDong = table.Column<decimal>(type: "numeric", nullable: true),
                    BHTNNguoiLaoDong = table.Column<decimal>(type: "numeric", nullable: true),
                    BHXHNguoiSuDungLaoDong = table.Column<decimal>(type: "numeric", nullable: true),
                    BHYTNguoiSuDungLaoDong = table.Column<decimal>(type: "numeric", nullable: true),
                    BHTNNguoiSuDungLaoDong = table.Column<decimal>(type: "numeric", nullable: true),
                    TongNguoiLaoDong = table.Column<decimal>(type: "numeric", nullable: true),
                    TongNguoiSuDungLaoDong = table.Column<decimal>(type: "numeric", nullable: true),
                    TongTatCa = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mk_BHXH", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mk_BHXH");
        }
    }
}
