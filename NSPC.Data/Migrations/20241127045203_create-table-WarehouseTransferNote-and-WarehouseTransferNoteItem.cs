using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class createtableWarehouseTransferNoteandWarehouseTransferNoteItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_WarehouseTransferNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferNoteCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExportWarehouseCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ImportWarehouseCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TransferredOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    StatusCode = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_WarehouseTransferNote", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sm_WarehouseTransferNoteItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LineNo = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    LineNote = table.Column<string>(type: "text", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferNoteID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_WarehouseTransferNoteItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_WarehouseTransferNoteItem_sm_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "sm_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sm_WarehouseTransferNoteItem_sm_WarehouseTransferNote_Trans~",
                        column: x => x.TransferNoteID,
                        principalTable: "sm_WarehouseTransferNote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_WarehouseTransferNoteItem_ProductId",
                table: "sm_WarehouseTransferNoteItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_WarehouseTransferNoteItem_TransferNoteID",
                table: "sm_WarehouseTransferNoteItem",
                column: "TransferNoteID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_WarehouseTransferNoteItem");

            migrationBuilder.DropTable(
                name: "sm_WarehouseTransferNote");
        }
    }
}
