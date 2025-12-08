using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class add_table_sm_MaterialRequestItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_MaterialRequestItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestQuantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_MaterialRequestItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_MaterialRequestItem_sm_MaterialRequest_MaterialRequestId",
                        column: x => x.MaterialRequestId,
                        principalTable: "sm_MaterialRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_MaterialRequestItem_sm_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "sm_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_MaterialRequestItem_MaterialRequestId",
                table: "sm_MaterialRequestItem",
                column: "MaterialRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_MaterialRequestItem_ProductId",
                table: "sm_MaterialRequestItem",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_MaterialRequestItem");
        }
    }
}
