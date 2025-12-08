using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addIconClassandnewCodeType_Itemclass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconClass",
                table: "sm_CodeType",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sm_CodeType_Item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    IconClass = table.Column<string>(type: "text", nullable: true),
                    CodeTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_CodeType_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_CodeType_Item_Idm_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Idm_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_CodeType_Item_sm_CodeType_CodeTypeId",
                        column: x => x.CodeTypeId,
                        principalTable: "sm_CodeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_CodeType_Item_CodeTypeId",
                table: "sm_CodeType_Item",
                column: "CodeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_CodeType_Item_TenantId",
                table: "sm_CodeType_Item",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_CodeType_Item");

            migrationBuilder.DropColumn(
                name: "IconClass",
                table: "sm_CodeType");
        }
    }
}
