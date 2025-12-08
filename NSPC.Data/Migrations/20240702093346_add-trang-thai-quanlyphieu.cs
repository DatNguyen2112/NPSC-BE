using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addtrangthaiquanlyphieu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActionMadeByUserId",
                table: "mk_QuanLyPhieu",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionMadeByUserName",
                table: "mk_QuanLyPhieu",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActionMadeOnDate",
                table: "mk_QuanLyPhieu",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LyDoTuChoi",
                table: "mk_QuanLyPhieu",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrangThai",
                table: "mk_QuanLyPhieu",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionMadeByUserId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "ActionMadeByUserName",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "ActionMadeOnDate",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "LyDoTuChoi",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "mk_QuanLyPhieu");
        }
    }
}
