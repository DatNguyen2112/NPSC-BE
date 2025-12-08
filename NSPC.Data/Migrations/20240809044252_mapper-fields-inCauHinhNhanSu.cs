using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class mapperfieldsinCauHinhNhanSu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_CauHinhNhanSu_mk_ChucVu_IdChucVu",
                table: "mk_CauHinhNhanSu");

            migrationBuilder.DropColumn(
                name: "Ma",
                table: "mk_CauHinhNhanSu");

            migrationBuilder.DropColumn(
                name: "TenNhanSu",
                table: "mk_CauHinhNhanSu");

            migrationBuilder.RenameColumn(
                name: "IdChucVu",
                table: "mk_CauHinhNhanSu",
                newName: "IdUser");

            migrationBuilder.RenameIndex(
                name: "IX_mk_CauHinhNhanSu_IdChucVu",
                table: "mk_CauHinhNhanSu",
                newName: "IX_mk_CauHinhNhanSu_IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_CauHinhNhanSu_idm_User_IdUser",
                table: "mk_CauHinhNhanSu",
                column: "IdUser",
                principalTable: "idm_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_CauHinhNhanSu_idm_User_IdUser",
                table: "mk_CauHinhNhanSu");

            migrationBuilder.RenameColumn(
                name: "IdUser",
                table: "mk_CauHinhNhanSu",
                newName: "IdChucVu");

            migrationBuilder.RenameIndex(
                name: "IX_mk_CauHinhNhanSu_IdUser",
                table: "mk_CauHinhNhanSu",
                newName: "IX_mk_CauHinhNhanSu_IdChucVu");

            migrationBuilder.AddColumn<string>(
                name: "Ma",
                table: "mk_CauHinhNhanSu",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenNhanSu",
                table: "mk_CauHinhNhanSu",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_CauHinhNhanSu_mk_ChucVu_IdChucVu",
                table: "mk_CauHinhNhanSu",
                column: "IdChucVu",
                principalTable: "mk_ChucVu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
