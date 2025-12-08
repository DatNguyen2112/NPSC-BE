using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addactiontoactivityhistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "sm_ActiviyHisroty",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "sm_ActiviyHisroty",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sm_MaintenanceContent_TenantId",
                table: "sm_MaintenanceContent",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetMaintenanceSheet_TenantId",
                table: "sm_AssetMaintenanceSheet",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationSheet_TenantId",
                table: "sm_AssetLiquidationSheet",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_AssetLiquidationItem_TenantId",
                table: "sm_AssetLiquidationItem",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Asset_TenantId",
                table: "sm_Asset",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Asset_Idm_Tenants_TenantId",
                table: "sm_Asset",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_AssetLiquidationItem_Idm_Tenants_TenantId",
                table: "sm_AssetLiquidationItem",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_AssetLiquidationSheet_Idm_Tenants_TenantId",
                table: "sm_AssetLiquidationSheet",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_AssetMaintenanceSheet_Idm_Tenants_TenantId",
                table: "sm_AssetMaintenanceSheet",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_MaintenanceContent_Idm_Tenants_TenantId",
                table: "sm_MaintenanceContent",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sm_Asset_Idm_Tenants_TenantId",
                table: "sm_Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_AssetLiquidationItem_Idm_Tenants_TenantId",
                table: "sm_AssetLiquidationItem");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_AssetLiquidationSheet_Idm_Tenants_TenantId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_AssetMaintenanceSheet_Idm_Tenants_TenantId",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_MaintenanceContent_Idm_Tenants_TenantId",
                table: "sm_MaintenanceContent");

            migrationBuilder.DropIndex(
                name: "IX_sm_MaintenanceContent_TenantId",
                table: "sm_MaintenanceContent");

            migrationBuilder.DropIndex(
                name: "IX_sm_AssetMaintenanceSheet_TenantId",
                table: "sm_AssetMaintenanceSheet");

            migrationBuilder.DropIndex(
                name: "IX_sm_AssetLiquidationSheet_TenantId",
                table: "sm_AssetLiquidationSheet");

            migrationBuilder.DropIndex(
                name: "IX_sm_AssetLiquidationItem_TenantId",
                table: "sm_AssetLiquidationItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_Asset_TenantId",
                table: "sm_Asset");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "sm_ActiviyHisroty");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "sm_ActiviyHisroty");
        }
    }
}
