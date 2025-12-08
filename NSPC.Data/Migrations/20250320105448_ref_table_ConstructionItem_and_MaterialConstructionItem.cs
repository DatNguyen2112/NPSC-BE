using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class ref_table_ConstructionItem_and_MaterialConstructionItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PlannedQuantity",
                table: "sm_MaterialRequestItem",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "PlannedQuantity",
                table: "sm_ConstructionItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GapQuantity",
                table: "sm_ConstructionItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RealQuantity",
                table: "sm_ConstructionItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlannedQuantity",
                table: "sm_MaterialRequestItem");

            migrationBuilder.DropColumn(
                name: "GapQuantity",
                table: "sm_ConstructionItems");

            migrationBuilder.DropColumn(
                name: "RealQuantity",
                table: "sm_ConstructionItems");

            migrationBuilder.AlterColumn<string>(
                name: "PlannedQuantity",
                table: "sm_ConstructionItems",
                type: "text",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
