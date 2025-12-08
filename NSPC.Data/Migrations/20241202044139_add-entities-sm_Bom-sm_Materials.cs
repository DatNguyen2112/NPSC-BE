using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addentitiessm_Bomsm_Materials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_Bom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubTotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Bom", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Bom_sm_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "sm_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sm_Materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialCode = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialName = table.Column<string>(type: "text", nullable: true),
                    MaterialUnit = table.Column<string>(type: "text", nullable: true),
                    LineVatPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    LineVatAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    LineAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    BomId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_Materials_sm_Bom_BomId",
                        column: x => x.BomId,
                        principalTable: "sm_Bom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_Bom_ProductId",
                table: "sm_Bom",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Materials_BomId",
                table: "sm_Materials",
                column: "BomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_Materials");

            migrationBuilder.DropTable(
                name: "sm_Bom");
        }
    }
}
