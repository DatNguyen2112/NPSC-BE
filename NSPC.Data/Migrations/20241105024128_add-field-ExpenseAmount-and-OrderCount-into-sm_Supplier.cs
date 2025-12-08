using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldExpenseAmountandOrderCountintosm_Supplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExpenseAmount",
                table: "sm_Supplier",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderCount",
                table: "sm_Supplier",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpenseAmount",
                table: "sm_Supplier");

            migrationBuilder.DropColumn(
                name: "OrderCount",
                table: "sm_Supplier");
        }
    }
}
