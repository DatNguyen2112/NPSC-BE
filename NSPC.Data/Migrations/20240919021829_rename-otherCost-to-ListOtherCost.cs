using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renameotherCosttoListOtherCost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtherCost",
                table: "sm_SaleOrders",
                newName: "ListOtherCost");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ListOtherCost",
                table: "sm_SaleOrders",
                newName: "OtherCost");
        }
    }
}
