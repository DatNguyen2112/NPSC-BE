using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class createtableInventoryCheckNoteandInventoryCheckNoteItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_InventoryCheckNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WareCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    OrderCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    Tag = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_InventoryCheckNote", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sm_InventoryCheckNoteItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductCode = table.Column<string>(type: "text", nullable: true),
                    ProductName = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    LineNo = table.Column<int>(type: "integer", nullable: false),
                    ActualInventory = table.Column<decimal>(type: "numeric", nullable: false),
                    RealityInventory = table.Column<decimal>(type: "numeric", nullable: false),
                    DifferenceQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    NoteInventory = table.Column<string>(type: "text", nullable: true),
                    ReasonInventory = table.Column<string>(type: "text", nullable: true),
                    DifferenceType = table.Column<string>(type: "text", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckInventoryNoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_InventoryCheckNoteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_InventoryCheckNoteItems_sm_InventoryCheckNote_CheckInven~",
                        column: x => x.CheckInventoryNoteId,
                        principalTable: "sm_InventoryCheckNote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_InventoryCheckNoteItems_sm_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "sm_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_InventoryCheckNoteItems_CheckInventoryNoteId",
                table: "sm_InventoryCheckNoteItems",
                column: "CheckInventoryNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_InventoryCheckNoteItems_ProductId",
                table: "sm_InventoryCheckNoteItems",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_InventoryCheckNoteItems");

            migrationBuilder.DropTable(
                name: "sm_InventoryCheckNote");
        }
    }
}
