using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class remove_field_planQuantity_in_sm_ConstructionItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlannedQuantity",
                table: "sm_ConstructionItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PlannedQuantity",
                table: "sm_ConstructionItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
