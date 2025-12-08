using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class updateFieldDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_QuanLyPhieu_mk_DuAn_IdDuAn",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_QuanLyPhieu_sm_Supplier_IdNhaCungCap",
                table: "mk_QuanLyPhieu");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "mk_QuanLyPhieu",
                newName: "WareCode");

            migrationBuilder.RenameColumn(
                name: "TongTienThanhToan",
                table: "mk_QuanLyPhieu",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "TongTien",
                table: "mk_QuanLyPhieu",
                newName: "SubTotal");

            migrationBuilder.RenameColumn(
                name: "MucDichNhapKho",
                table: "mk_QuanLyPhieu",
                newName: "TypeName");

            migrationBuilder.RenameColumn(
                name: "MaPhieu",
                table: "mk_QuanLyPhieu",
                newName: "TypeCode");

            migrationBuilder.RenameColumn(
                name: "MaKho",
                table: "mk_QuanLyPhieu",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "MaDonHang",
                table: "mk_QuanLyPhieu",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "LoaiPhieu",
                table: "mk_QuanLyPhieu",
                newName: "OrderCode");

            migrationBuilder.RenameColumn(
                name: "IdNhaCungCap",
                table: "mk_QuanLyPhieu",
                newName: "SupplierId");

            migrationBuilder.RenameColumn(
                name: "IdDuAn",
                table: "mk_QuanLyPhieu",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "DiaDiemGiaoHang",
                table: "mk_QuanLyPhieu",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_mk_QuanLyPhieu_IdNhaCungCap",
                table: "mk_QuanLyPhieu",
                newName: "IX_mk_QuanLyPhieu_SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_mk_QuanLyPhieu_IdDuAn",
                table: "mk_QuanLyPhieu",
                newName: "IX_mk_QuanLyPhieu_ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_QuanLyPhieu_mk_DuAn_ProjectId",
                table: "mk_QuanLyPhieu",
                column: "ProjectId",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_QuanLyPhieu_sm_Supplier_SupplierId",
                table: "mk_QuanLyPhieu",
                column: "SupplierId",
                principalTable: "sm_Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mk_QuanLyPhieu_mk_DuAn_ProjectId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_QuanLyPhieu_sm_Supplier_SupplierId",
                table: "mk_QuanLyPhieu");

            migrationBuilder.RenameColumn(
                name: "WareCode",
                table: "mk_QuanLyPhieu",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "TypeName",
                table: "mk_QuanLyPhieu",
                newName: "MucDichNhapKho");

            migrationBuilder.RenameColumn(
                name: "TypeCode",
                table: "mk_QuanLyPhieu",
                newName: "MaPhieu");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "mk_QuanLyPhieu",
                newName: "TongTienThanhToan");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "mk_QuanLyPhieu",
                newName: "IdNhaCungCap");

            migrationBuilder.RenameColumn(
                name: "SubTotal",
                table: "mk_QuanLyPhieu",
                newName: "TongTien");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "mk_QuanLyPhieu",
                newName: "MaKho");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "mk_QuanLyPhieu",
                newName: "MaDonHang");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "mk_QuanLyPhieu",
                newName: "IdDuAn");

            migrationBuilder.RenameColumn(
                name: "OrderCode",
                table: "mk_QuanLyPhieu",
                newName: "LoaiPhieu");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "mk_QuanLyPhieu",
                newName: "DiaDiemGiaoHang");

            migrationBuilder.RenameIndex(
                name: "IX_mk_QuanLyPhieu_SupplierId",
                table: "mk_QuanLyPhieu",
                newName: "IX_mk_QuanLyPhieu_IdNhaCungCap");

            migrationBuilder.RenameIndex(
                name: "IX_mk_QuanLyPhieu_ProjectId",
                table: "mk_QuanLyPhieu",
                newName: "IX_mk_QuanLyPhieu_IdDuAn");

            migrationBuilder.AddForeignKey(
                name: "FK_mk_QuanLyPhieu_mk_DuAn_IdDuAn",
                table: "mk_QuanLyPhieu",
                column: "IdDuAn",
                principalTable: "mk_DuAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_QuanLyPhieu_sm_Supplier_IdNhaCungCap",
                table: "mk_QuanLyPhieu",
                column: "IdNhaCungCap",
                principalTable: "sm_Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
