using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_code_name_unit_into_sm_ConstructionItems_and_sm_MaterialRequestItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "sm_MaterialRequestItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "sm_MaterialRequestItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "sm_MaterialRequestItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "sm_ConstructionItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "sm_ConstructionItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "sm_ConstructionItems",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "sm_ConstructionItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "sm_ConstructionItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "sm_ConstructionItems");
        }
    }
}
