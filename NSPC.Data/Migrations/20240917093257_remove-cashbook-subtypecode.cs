using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class removecashbooksubtypecode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTypeCode",
                table: "sm_Cashbook_Transaction");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubTypeCode",
                table: "sm_Cashbook_Transaction",
                type: "text",
                nullable: true);
        }
    }
}
