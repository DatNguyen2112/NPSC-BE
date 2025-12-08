using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldAttachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhDaiDien",
                table: "mk_VatTu");

            migrationBuilder.DropColumn(
                name: "HinhDinhKem",
                table: "mk_VatTu");

            migrationBuilder.AddColumn<List<jsonb_Attachment>>(
                name: "Attachments",
                table: "mk_VatTu",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "mk_VatTu");

            migrationBuilder.AddColumn<string>(
                name: "HinhDaiDien",
                table: "mk_VatTu",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhDinhKem",
                table: "mk_VatTu",
                type: "text",
                nullable: true);
        }
    }
}
