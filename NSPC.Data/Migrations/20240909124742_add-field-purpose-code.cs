using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldpurposecode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Purpose",
                table: "sm_Cashbook_Transaction",
                newName: "PurposeCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PurposeCode",
                table: "sm_Cashbook_Transaction",
                newName: "Purpose");
        }
    }
}
