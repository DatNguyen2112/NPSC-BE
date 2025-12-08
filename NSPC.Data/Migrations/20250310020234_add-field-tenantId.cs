using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addfieldtenantId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_WarehouseTransferNoteItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_WarehouseTransferNote",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Supplier",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Stock_Transaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_SalesOrderItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_SalesOrder",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Return_Order_Item",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Return_Order",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_QuotationItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Quotation",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_PurchaseOrderItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_PurchaseOrder",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_ProductInventory",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Product",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Notification_Template_Translation",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Notification_Template",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Notification",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Materials",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_LichSuChamSoc",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_InventoryNoteItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_InventoryNote",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_InventoryCheckNoteItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_InventoryCheckNote",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Email_Verification",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Email_Template_Translations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Email_Template",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Email_Subscribe",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_EInvoiceVatAnalytics",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_EInvoiceItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_EInvoice",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_DebtTransaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_CustomerServiceComment",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Customer",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_CodeType_Translation",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_CodeType",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Cashbook_Transaction",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_Bom",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "sm_ActiviyHisroty",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_XuatNhapTon",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_WorkingDay",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_PhongBan",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_NhomVatTu",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_NguyenVatLieu",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_KiemKho",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_DuAn",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_ChucVu",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_ChamCongItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_ChamCong",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_CauHinhNhanSu",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_CacKhoanTroCap",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_Bom",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_BHXH",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_BangTinhLuong",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "mk_BangLuongItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "IdmRightMapRole",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "IdmRight",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "idm_User",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Idm_Tenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "idm_Role",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "fm_Search_Sample",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "erp_Attachment",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "cata_Province",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "cata_District",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "cata_Commune",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "bsd_Parameter",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "bsd_Navigation_Map_Role",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "bsd_Navigation",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_WarehouseTransferNoteItem");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_WarehouseTransferNote");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Supplier");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_SalesOrder");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Return_Order_Item");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Return_Order");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_QuotationItem");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Quotation");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_ProductInventory");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Product");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Notification_Template_Translation");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Notification_Template");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Notification");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Materials");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_InventoryNoteItem");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_InventoryNote");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_InventoryCheckNoteItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_InventoryCheckNote");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Email_Verification");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Email_Template_Translations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Email_Template");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Email_Subscribe");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_EInvoiceVatAnalytics");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_EInvoiceItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_EInvoice");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_DebtTransaction");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Customer");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_CodeType_Translation");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_CodeType");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_Bom");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "sm_ActiviyHisroty");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_XuatNhapTon");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_WorkingDay");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_PhongBan");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_NhomVatTu");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_NguyenVatLieu");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_KiemKho");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_DuAn");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_ChucVu");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_ChamCongItem");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_ChamCong");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_CauHinhNhanSu");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_CacKhoanTroCap");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_Bom");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_BHXH");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_BangTinhLuong");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "mk_BangLuongItem");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "IdmRightMapRole");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "IdmRight");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "idm_User");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Idm_Tenants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "idm_Role");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "fm_Search_Sample");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "erp_Attachment");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "cata_Province");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "cata_District");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "cata_Commune");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "bsd_Parameter");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "bsd_Navigation_Map_Role");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "bsd_Navigation");
        }
    }
}
