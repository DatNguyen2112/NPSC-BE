using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldCong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cong",
                table: "mk_ChamCong",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cong",
                table: "mk_ChamCong");
        }
    }
}
