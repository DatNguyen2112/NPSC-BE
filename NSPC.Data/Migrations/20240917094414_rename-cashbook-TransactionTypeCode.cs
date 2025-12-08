using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renamecashbookTransactionTypeCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "sm_Cashbook_Transaction",
                newName: "TransactionTypeCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionTypeCode",
                table: "sm_Cashbook_Transaction",
                newName: "TransactionType");
        }
    }
}
