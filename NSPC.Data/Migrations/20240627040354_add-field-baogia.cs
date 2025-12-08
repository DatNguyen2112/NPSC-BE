using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldbaogia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuyCach",
                table: "mk_BaoGiaItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverDiaChi",
                table: "mk_BaoGia",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverSoDienThoai",
                table: "mk_BaoGia",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverTen",
                table: "mk_BaoGia",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuyCach",
                table: "mk_BaoGiaItem");

            migrationBuilder.DropColumn(
                name: "ReceiverDiaChi",
                table: "mk_BaoGia");

            migrationBuilder.DropColumn(
                name: "ReceiverSoDienThoai",
                table: "mk_BaoGia");

            migrationBuilder.DropColumn(
                name: "ReceiverTen",
                table: "mk_BaoGia");
        }
    }
}
