using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldorginalDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "sm_Stock_Transaction",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalDocumentCode",
                table: "sm_Stock_Transaction",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropColumn(
                name: "OriginalDocumentCode",
                table: "sm_Stock_Transaction");
        }
    }
}
