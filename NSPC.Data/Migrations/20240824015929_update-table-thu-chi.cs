using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updatetablethuchi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_ThuChi_sm_Supplier_sm_SupplierId",
                table: "mk_ThuChi");

            migrationBuilder.DropIndex(
                name: "IX_mk_ThuChi_sm_SupplierId",
                table: "mk_ThuChi");

            migrationBuilder.RenameColumn(
                name: "sm_SupplierId",
                table: "mk_ThuChi",
                newName: "IdSupplier");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_IdMaNhaCungCap",
                table: "mk_ThuChi",
                column: "IdMaNhaCungCap");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_ThuChi_sm_Supplier_IdMaNhaCungCap",
                table: "mk_ThuChi",
                column: "IdMaNhaCungCap",
                principalTable: "sm_Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_ThuChi_sm_Supplier_IdMaNhaCungCap",
                table: "mk_ThuChi");

            migrationBuilder.DropIndex(
                name: "IX_mk_ThuChi_IdMaNhaCungCap",
                table: "mk_ThuChi");

            migrationBuilder.RenameColumn(
                name: "IdSupplier",
                table: "mk_ThuChi",
                newName: "sm_SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ThuChi_sm_SupplierId",
                table: "mk_ThuChi",
                column: "sm_SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_ThuChi_sm_Supplier_sm_SupplierId",
                table: "mk_ThuChi",
                column: "sm_SupplierId",
                principalTable: "sm_Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
