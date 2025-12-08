using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class CreateSmDebtTransactionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sm_DebtTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    EntityType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    EntityName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    OriginalDocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginalDocumentCode = table.Column<string>(type: "text", nullable: true),
                    OriginalDocumentType = table.Column<string>(type: "text", nullable: true),
                    ChangeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DebtAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Note = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    LastModifiedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sm_DebtTransaction", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sm_DebtTransaction");
        }
    }
}
