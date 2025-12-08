using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfkrelationshiptosm_Quotationclass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_sm_Quotation_CustomerId",
                table: "sm_Quotation",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Quotation_sm_Customer_CustomerId",
                table: "sm_Quotation",
                column: "CustomerId",
                principalTable: "sm_Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Quotation_sm_Customer_CustomerId",
                table: "sm_Quotation");

            migrationBuilder.DropIndex(
                name: "IX_sm_Quotation_CustomerId",
                table: "sm_Quotation");
        }
    }
}
