using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_LineNote_into_ConstructionItems_and_MaterialRequestItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LineNote",
                table: "sm_MaterialRequestItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LineNote",
                table: "sm_ConstructionItems",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineNote",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "LineNote",
                table: "sm_ConstructionItems");
        }
    }
}
