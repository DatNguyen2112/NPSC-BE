using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addentityInventoryNoteandInventoryNoteItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_InventoryNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EntityName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EntityTypeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EntityTypeName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    OriginalDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginalDocumentType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    OriginalDocumentCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TransactionTypeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TransactionTypeName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InventoryCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    InventoryName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProjectName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Note = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    TypeCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    StatusCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    StatusName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_InventoryNote", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sm_InventoryNoteItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ProductName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Unit = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Note = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    InventoryNoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_InventoryNoteItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sm_InventoryNoteItem_sm_InventoryNote_InventoryNoteId",
                        column: x => x.InventoryNoteId,
                        principalTable: "sm_InventoryNote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sm_InventoryNoteItem_InventoryNoteId",
                table: "sm_InventoryNoteItem",
                column: "InventoryNoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_InventoryNoteItem");

            migrationBuilder.DropTable(
                name: "sm_InventoryNote");
        }
    }
}
