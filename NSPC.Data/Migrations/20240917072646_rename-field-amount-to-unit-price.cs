using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renamefieldamounttounitprice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "sm_Stock_Transaction",
                newName: "UnitPrice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "sm_Stock_Transaction",
                newName: "Amount");
        }
    }
}
