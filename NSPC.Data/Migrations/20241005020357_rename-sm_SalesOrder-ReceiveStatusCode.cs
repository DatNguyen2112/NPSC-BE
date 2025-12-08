using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class renamesm_SalesOrderReceiveStatusCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceiveStatusCode",
                table: "sm_SalesOrder",
                newName: "ExportStatusCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExportStatusCode",
                table: "sm_SalesOrder",
                newName: "ReceiveStatusCode");
        }
    }
}
