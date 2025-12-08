using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_LineNo_into_constuctionItems_and_materialRequestItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LineNo",
                table: "sm_MaterialRequestItem",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LineNo",
                table: "sm_ConstructionItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineNo",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "LineNo",
                table: "sm_ConstructionItems");
        }
    }
}
