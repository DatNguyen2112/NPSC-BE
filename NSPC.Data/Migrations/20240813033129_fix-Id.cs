using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class fixId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_mk_CauHinhNhanSu",
                table: "mk_CauHinhNhanSu");

            migrationBuilder.DropIndex(
                name: "IX_mk_CauHinhNhanSu_IdUser",
                table: "mk_CauHinhNhanSu");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "mk_CauHinhNhanSu");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdUser",
                table: "mk_CauHinhNhanSu",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_mk_CauHinhNhanSu",
                table: "mk_CauHinhNhanSu",
                column: "IdUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_mk_CauHinhNhanSu",
                table: "mk_CauHinhNhanSu");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdUser",
                table: "mk_CauHinhNhanSu",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "mk_CauHinhNhanSu",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_mk_CauHinhNhanSu",
                table: "mk_CauHinhNhanSu",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_mk_CauHinhNhanSu_IdUser",
                table: "mk_CauHinhNhanSu",
                column: "IdUser");
        }
    }
}
