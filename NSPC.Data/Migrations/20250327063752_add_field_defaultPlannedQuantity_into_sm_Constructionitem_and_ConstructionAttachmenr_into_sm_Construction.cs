using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NSPC.Data;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_field_defaultPlannedQuantity_into_sm_Constructionitem_and_ConstructionAttachmenr_into_sm_Construction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PlannedDefaultQuantity",
                table: "sm_ConstructionItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<List<jsonb_Attachment>>(
                name: "ConstructionAttachments",
                table: "sm_Construction",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlannedDefaultQuantity",
                table: "sm_ConstructionItems");

            migrationBuilder.DropColumn(
                name: "ConstructionAttachments",
                table: "sm_Construction");
        }
    }
}
