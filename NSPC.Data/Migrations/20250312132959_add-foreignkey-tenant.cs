using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSPC.Data.Migrations
{
    public partial class addforeignkeytenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_sm_WarehouseTransferNoteItem_TenantId",
                table: "sm_WarehouseTransferNoteItem",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_WarehouseTransferNote_TenantId",
                table: "sm_WarehouseTransferNote",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Supplier_TenantId",
                table: "sm_Supplier",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Stock_Transaction_TenantId",
                table: "sm_Stock_Transaction",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SalesOrderItem_TenantId",
                table: "sm_SalesOrderItem",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_SalesOrder_TenantId",
                table: "sm_SalesOrder",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Return_Order_Item_TenantId",
                table: "sm_Return_Order_Item",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Return_Order_TenantId",
                table: "sm_Return_Order",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_QuotationItem_TenantId",
                table: "sm_QuotationItem",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Quotation_TenantId",
                table: "sm_Quotation",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_PurchaseOrderItem_TenantId",
                table: "sm_PurchaseOrderItem",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_PurchaseOrder_TenantId",
                table: "sm_PurchaseOrder",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_ProductInventory_TenantId",
                table: "sm_ProductInventory",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Product_TenantId",
                table: "sm_Product",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Notification_Template_Translation_TenantId",
                table: "sm_Notification_Template_Translation",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Notification_Template_TenantId",
                table: "sm_Notification_Template",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Notification_TenantId",
                table: "sm_Notification",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Materials_TenantId",
                table: "sm_Materials",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_LichSuChamSoc_TenantId",
                table: "sm_LichSuChamSoc",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_InventoryNoteItem_TenantId",
                table: "sm_InventoryNoteItem",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_InventoryNote_TenantId",
                table: "sm_InventoryNote",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_InventoryCheckNoteItems_TenantId",
                table: "sm_InventoryCheckNoteItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_InventoryCheckNote_TenantId",
                table: "sm_InventoryCheckNote",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Email_Verification_TenantId",
                table: "sm_Email_Verification",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Email_Template_Translations_TenantId",
                table: "sm_Email_Template_Translations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Email_Template_TenantId",
                table: "sm_Email_Template",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Email_Subscribe_TenantId",
                table: "sm_Email_Subscribe",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_EInvoiceVatAnalytics_TenantId",
                table: "sm_EInvoiceVatAnalytics",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_EInvoiceItems_TenantId",
                table: "sm_EInvoiceItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_EInvoice_TenantId",
                table: "sm_EInvoice",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_DebtTransaction_TenantId",
                table: "sm_DebtTransaction",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_CustomerServiceComment_TenantId",
                table: "sm_CustomerServiceComment",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Customer_TenantId",
                table: "sm_Customer",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_CodeType_Translation_TenantId",
                table: "sm_CodeType_Translation",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_CodeType_TenantId",
                table: "sm_CodeType",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Cashbook_Transaction_TenantId",
                table: "sm_Cashbook_Transaction",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_Bom_TenantId",
                table: "sm_Bom",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_sm_ActiviyHisroty_TenantId",
                table: "sm_ActiviyHisroty",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_XuatNhapTon_TenantId",
                table: "mk_XuatNhapTon",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_WorkingDay_TenantId",
                table: "mk_WorkingDay",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_PhongBan_TenantId",
                table: "mk_PhongBan",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_NhomVatTu_TenantId",
                table: "mk_NhomVatTu",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_NguyenVatLieu_TenantId",
                table: "mk_NguyenVatLieu",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_KiemKho_TenantId",
                table: "mk_KiemKho",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_DuAn_TenantId",
                table: "mk_DuAn",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ChucVu_TenantId",
                table: "mk_ChucVu",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ChamCongItem_TenantId",
                table: "mk_ChamCongItem",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_ChamCong_TenantId",
                table: "mk_ChamCong",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_CauHinhNhanSu_TenantId",
                table: "mk_CauHinhNhanSu",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_CacKhoanTroCap_TenantId",
                table: "mk_CacKhoanTroCap",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_Bom_TenantId",
                table: "mk_Bom",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_BHXH_TenantId",
                table: "mk_BHXH",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_BangTinhLuong_TenantId",
                table: "mk_BangTinhLuong",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_mk_BangLuongItem_TenantId",
                table: "mk_BangLuongItem",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_IdmRightMapRole_TenantId",
                table: "IdmRightMapRole",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_IdmRight_TenantId",
                table: "IdmRight",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_idm_User_TenantId",
                table: "idm_User",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_idm_Role_TenantId",
                table: "idm_Role",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_fm_Search_Sample_TenantId",
                table: "fm_Search_Sample",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_erp_Attachment_TenantId",
                table: "erp_Attachment",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_cata_Province_TenantId",
                table: "cata_Province",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_cata_District_TenantId",
                table: "cata_District",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_cata_Commune_TenantId",
                table: "cata_Commune",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_bsd_Parameter_TenantId",
                table: "bsd_Parameter",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_bsd_Navigation_Map_Role_TenantId",
                table: "bsd_Navigation_Map_Role",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_bsd_Navigation_TenantId",
                table: "bsd_Navigation",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_bsd_Navigation_Idm_Tenants_TenantId",
                table: "bsd_Navigation",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_bsd_Navigation_Map_Role_Idm_Tenants_TenantId",
                table: "bsd_Navigation_Map_Role",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_bsd_Parameter_Idm_Tenants_TenantId",
                table: "bsd_Parameter",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cata_Commune_Idm_Tenants_TenantId",
                table: "cata_Commune",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cata_District_Idm_Tenants_TenantId",
                table: "cata_District",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cata_Province_Idm_Tenants_TenantId",
                table: "cata_Province",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_erp_Attachment_Idm_Tenants_TenantId",
                table: "erp_Attachment",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_fm_Search_Sample_Idm_Tenants_TenantId",
                table: "fm_Search_Sample",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_idm_Role_Idm_Tenants_TenantId",
                table: "idm_Role",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_idm_User_Idm_Tenants_TenantId",
                table: "idm_User",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdmRight_Idm_Tenants_TenantId",
                table: "IdmRight",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdmRightMapRole_Idm_Tenants_TenantId",
                table: "IdmRightMapRole",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_BangLuongItem_Idm_Tenants_TenantId",
                table: "mk_BangLuongItem",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_BangTinhLuong_Idm_Tenants_TenantId",
                table: "mk_BangTinhLuong",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_BHXH_Idm_Tenants_TenantId",
                table: "mk_BHXH",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_Bom_Idm_Tenants_TenantId",
                table: "mk_Bom",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_CacKhoanTroCap_Idm_Tenants_TenantId",
                table: "mk_CacKhoanTroCap",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_CauHinhNhanSu_Idm_Tenants_TenantId",
                table: "mk_CauHinhNhanSu",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_ChamCong_Idm_Tenants_TenantId",
                table: "mk_ChamCong",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_ChamCongItem_Idm_Tenants_TenantId",
                table: "mk_ChamCongItem",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_ChucVu_Idm_Tenants_TenantId",
                table: "mk_ChucVu",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_DuAn_Idm_Tenants_TenantId",
                table: "mk_DuAn",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_KiemKho_Idm_Tenants_TenantId",
                table: "mk_KiemKho",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_NguyenVatLieu_Idm_Tenants_TenantId",
                table: "mk_NguyenVatLieu",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_NhomVatTu_Idm_Tenants_TenantId",
                table: "mk_NhomVatTu",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_PhongBan_Idm_Tenants_TenantId",
                table: "mk_PhongBan",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_WorkingDay_Idm_Tenants_TenantId",
                table: "mk_WorkingDay",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mk_XuatNhapTon_Idm_Tenants_TenantId",
                table: "mk_XuatNhapTon",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_ActiviyHisroty_Idm_Tenants_TenantId",
                table: "sm_ActiviyHisroty",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Bom_Idm_Tenants_TenantId",
                table: "sm_Bom",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Cashbook_Transaction_Idm_Tenants_TenantId",
                table: "sm_Cashbook_Transaction",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_CodeType_Idm_Tenants_TenantId",
                table: "sm_CodeType",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_CodeType_Translation_Idm_Tenants_TenantId",
                table: "sm_CodeType_Translation",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Customer_Idm_Tenants_TenantId",
                table: "sm_Customer",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_CustomerServiceComment_Idm_Tenants_TenantId",
                table: "sm_CustomerServiceComment",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_DebtTransaction_Idm_Tenants_TenantId",
                table: "sm_DebtTransaction",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_EInvoice_Idm_Tenants_TenantId",
                table: "sm_EInvoice",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_EInvoiceItems_Idm_Tenants_TenantId",
                table: "sm_EInvoiceItems",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_EInvoiceVatAnalytics_Idm_Tenants_TenantId",
                table: "sm_EInvoiceVatAnalytics",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Email_Subscribe_Idm_Tenants_TenantId",
                table: "sm_Email_Subscribe",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Email_Template_Idm_Tenants_TenantId",
                table: "sm_Email_Template",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Email_Template_Translations_Idm_Tenants_TenantId",
                table: "sm_Email_Template_Translations",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Email_Verification_Idm_Tenants_TenantId",
                table: "sm_Email_Verification",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_InventoryCheckNote_Idm_Tenants_TenantId",
                table: "sm_InventoryCheckNote",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_InventoryCheckNoteItems_Idm_Tenants_TenantId",
                table: "sm_InventoryCheckNoteItems",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_InventoryNote_Idm_Tenants_TenantId",
                table: "sm_InventoryNote",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_InventoryNoteItem_Idm_Tenants_TenantId",
                table: "sm_InventoryNoteItem",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_LichSuChamSoc_Idm_Tenants_TenantId",
                table: "sm_LichSuChamSoc",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Materials_Idm_Tenants_TenantId",
                table: "sm_Materials",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Notification_Idm_Tenants_TenantId",
                table: "sm_Notification",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Notification_Template_Idm_Tenants_TenantId",
                table: "sm_Notification_Template",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Notification_Template_Translation_Idm_Tenants_TenantId",
                table: "sm_Notification_Template_Translation",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Product_Idm_Tenants_TenantId",
                table: "sm_Product",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_ProductInventory_Idm_Tenants_TenantId",
                table: "sm_ProductInventory",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrder_Idm_Tenants_TenantId",
                table: "sm_PurchaseOrder",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_PurchaseOrderItem_Idm_Tenants_TenantId",
                table: "sm_PurchaseOrderItem",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Quotation_Idm_Tenants_TenantId",
                table: "sm_Quotation",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_QuotationItem_Idm_Tenants_TenantId",
                table: "sm_QuotationItem",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Return_Order_Idm_Tenants_TenantId",
                table: "sm_Return_Order",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Return_Order_Item_Idm_Tenants_TenantId",
                table: "sm_Return_Order_Item",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SalesOrder_Idm_Tenants_TenantId",
                table: "sm_SalesOrder",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_SalesOrderItem_Idm_Tenants_TenantId",
                table: "sm_SalesOrderItem",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Stock_Transaction_Idm_Tenants_TenantId",
                table: "sm_Stock_Transaction",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_Supplier_Idm_Tenants_TenantId",
                table: "sm_Supplier",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_WarehouseTransferNote_Idm_Tenants_TenantId",
                table: "sm_WarehouseTransferNote",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sm_WarehouseTransferNoteItem_Idm_Tenants_TenantId",
                table: "sm_WarehouseTransferNoteItem",
                column: "TenantId",
                principalTable: "Idm_Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bsd_Navigation_Idm_Tenants_TenantId",
                table: "bsd_Navigation");

            migrationBuilder.DropForeignKey(
                name: "FK_bsd_Navigation_Map_Role_Idm_Tenants_TenantId",
                table: "bsd_Navigation_Map_Role");

            migrationBuilder.DropForeignKey(
                name: "FK_bsd_Parameter_Idm_Tenants_TenantId",
                table: "bsd_Parameter");

            migrationBuilder.DropForeignKey(
                name: "FK_cata_Commune_Idm_Tenants_TenantId",
                table: "cata_Commune");

            migrationBuilder.DropForeignKey(
                name: "FK_cata_District_Idm_Tenants_TenantId",
                table: "cata_District");

            migrationBuilder.DropForeignKey(
                name: "FK_cata_Province_Idm_Tenants_TenantId",
                table: "cata_Province");

            migrationBuilder.DropForeignKey(
                name: "FK_erp_Attachment_Idm_Tenants_TenantId",
                table: "erp_Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_fm_Search_Sample_Idm_Tenants_TenantId",
                table: "fm_Search_Sample");

            migrationBuilder.DropForeignKey(
                name: "FK_idm_Role_Idm_Tenants_TenantId",
                table: "idm_Role");

            migrationBuilder.DropForeignKey(
                name: "FK_idm_User_Idm_Tenants_TenantId",
                table: "idm_User");

            migrationBuilder.DropForeignKey(
                name: "FK_IdmRight_Idm_Tenants_TenantId",
                table: "IdmRight");

            migrationBuilder.DropForeignKey(
                name: "FK_IdmRightMapRole_Idm_Tenants_TenantId",
                table: "IdmRightMapRole");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_BangLuongItem_Idm_Tenants_TenantId",
                table: "mk_BangLuongItem");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_BangTinhLuong_Idm_Tenants_TenantId",
                table: "mk_BangTinhLuong");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_BHXH_Idm_Tenants_TenantId",
                table: "mk_BHXH");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_Bom_Idm_Tenants_TenantId",
                table: "mk_Bom");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_CacKhoanTroCap_Idm_Tenants_TenantId",
                table: "mk_CacKhoanTroCap");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_CauHinhNhanSu_Idm_Tenants_TenantId",
                table: "mk_CauHinhNhanSu");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_ChamCong_Idm_Tenants_TenantId",
                table: "mk_ChamCong");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_ChamCongItem_Idm_Tenants_TenantId",
                table: "mk_ChamCongItem");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_ChucVu_Idm_Tenants_TenantId",
                table: "mk_ChucVu");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_DuAn_Idm_Tenants_TenantId",
                table: "mk_DuAn");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_KiemKho_Idm_Tenants_TenantId",
                table: "mk_KiemKho");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_NguyenVatLieu_Idm_Tenants_TenantId",
                table: "mk_NguyenVatLieu");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_NhomVatTu_Idm_Tenants_TenantId",
                table: "mk_NhomVatTu");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_PhongBan_Idm_Tenants_TenantId",
                table: "mk_PhongBan");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_WorkingDay_Idm_Tenants_TenantId",
                table: "mk_WorkingDay");

            migrationBuilder.DropForeignKey(
                name: "FK_mk_XuatNhapTon_Idm_Tenants_TenantId",
                table: "mk_XuatNhapTon");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_ActiviyHisroty_Idm_Tenants_TenantId",
                table: "sm_ActiviyHisroty");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Bom_Idm_Tenants_TenantId",
                table: "sm_Bom");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Cashbook_Transaction_Idm_Tenants_TenantId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_CodeType_Idm_Tenants_TenantId",
                table: "sm_CodeType");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_CodeType_Translation_Idm_Tenants_TenantId",
                table: "sm_CodeType_Translation");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Customer_Idm_Tenants_TenantId",
                table: "sm_Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_CustomerServiceComment_Idm_Tenants_TenantId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_DebtTransaction_Idm_Tenants_TenantId",
                table: "sm_DebtTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_EInvoice_Idm_Tenants_TenantId",
                table: "sm_EInvoice");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_EInvoiceItems_Idm_Tenants_TenantId",
                table: "sm_EInvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_EInvoiceVatAnalytics_Idm_Tenants_TenantId",
                table: "sm_EInvoiceVatAnalytics");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Email_Subscribe_Idm_Tenants_TenantId",
                table: "sm_Email_Subscribe");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Email_Template_Idm_Tenants_TenantId",
                table: "sm_Email_Template");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Email_Template_Translations_Idm_Tenants_TenantId",
                table: "sm_Email_Template_Translations");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Email_Verification_Idm_Tenants_TenantId",
                table: "sm_Email_Verification");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_InventoryCheckNote_Idm_Tenants_TenantId",
                table: "sm_InventoryCheckNote");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_InventoryCheckNoteItems_Idm_Tenants_TenantId",
                table: "sm_InventoryCheckNoteItems");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_InventoryNote_Idm_Tenants_TenantId",
                table: "sm_InventoryNote");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_InventoryNoteItem_Idm_Tenants_TenantId",
                table: "sm_InventoryNoteItem");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_LichSuChamSoc_Idm_Tenants_TenantId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Materials_Idm_Tenants_TenantId",
                table: "sm_Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Notification_Idm_Tenants_TenantId",
                table: "sm_Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Notification_Template_Idm_Tenants_TenantId",
                table: "sm_Notification_Template");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Notification_Template_Translation_Idm_Tenants_TenantId",
                table: "sm_Notification_Template_Translation");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Product_Idm_Tenants_TenantId",
                table: "sm_Product");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_ProductInventory_Idm_Tenants_TenantId",
                table: "sm_ProductInventory");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrder_Idm_Tenants_TenantId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_PurchaseOrderItem_Idm_Tenants_TenantId",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Quotation_Idm_Tenants_TenantId",
                table: "sm_Quotation");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_QuotationItem_Idm_Tenants_TenantId",
                table: "sm_QuotationItem");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Return_Order_Idm_Tenants_TenantId",
                table: "sm_Return_Order");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Return_Order_Item_Idm_Tenants_TenantId",
                table: "sm_Return_Order_Item");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_SalesOrder_Idm_Tenants_TenantId",
                table: "sm_SalesOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_SalesOrderItem_Idm_Tenants_TenantId",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Stock_Transaction_Idm_Tenants_TenantId",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_Supplier_Idm_Tenants_TenantId",
                table: "sm_Supplier");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_WarehouseTransferNote_Idm_Tenants_TenantId",
                table: "sm_WarehouseTransferNote");

            migrationBuilder.DropForeignKey(
                name: "FK_sm_WarehouseTransferNoteItem_Idm_Tenants_TenantId",
                table: "sm_WarehouseTransferNoteItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_WarehouseTransferNoteItem_TenantId",
                table: "sm_WarehouseTransferNoteItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_WarehouseTransferNote_TenantId",
                table: "sm_WarehouseTransferNote");

            migrationBuilder.DropIndex(
                name: "IX_sm_Supplier_TenantId",
                table: "sm_Supplier");

            migrationBuilder.DropIndex(
                name: "IX_sm_Stock_Transaction_TenantId",
                table: "sm_Stock_Transaction");

            migrationBuilder.DropIndex(
                name: "IX_sm_SalesOrderItem_TenantId",
                table: "sm_SalesOrderItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_SalesOrder_TenantId",
                table: "sm_SalesOrder");

            migrationBuilder.DropIndex(
                name: "IX_sm_Return_Order_Item_TenantId",
                table: "sm_Return_Order_Item");

            migrationBuilder.DropIndex(
                name: "IX_sm_Return_Order_TenantId",
                table: "sm_Return_Order");

            migrationBuilder.DropIndex(
                name: "IX_sm_QuotationItem_TenantId",
                table: "sm_QuotationItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_Quotation_TenantId",
                table: "sm_Quotation");

            migrationBuilder.DropIndex(
                name: "IX_sm_PurchaseOrderItem_TenantId",
                table: "sm_PurchaseOrderItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_PurchaseOrder_TenantId",
                table: "sm_PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_sm_ProductInventory_TenantId",
                table: "sm_ProductInventory");

            migrationBuilder.DropIndex(
                name: "IX_sm_Product_TenantId",
                table: "sm_Product");

            migrationBuilder.DropIndex(
                name: "IX_sm_Notification_Template_Translation_TenantId",
                table: "sm_Notification_Template_Translation");

            migrationBuilder.DropIndex(
                name: "IX_sm_Notification_Template_TenantId",
                table: "sm_Notification_Template");

            migrationBuilder.DropIndex(
                name: "IX_sm_Notification_TenantId",
                table: "sm_Notification");

            migrationBuilder.DropIndex(
                name: "IX_sm_Materials_TenantId",
                table: "sm_Materials");

            migrationBuilder.DropIndex(
                name: "IX_sm_LichSuChamSoc_TenantId",
                table: "sm_LichSuChamSoc");

            migrationBuilder.DropIndex(
                name: "IX_sm_InventoryNoteItem_TenantId",
                table: "sm_InventoryNoteItem");

            migrationBuilder.DropIndex(
                name: "IX_sm_InventoryNote_TenantId",
                table: "sm_InventoryNote");

            migrationBuilder.DropIndex(
                name: "IX_sm_InventoryCheckNoteItems_TenantId",
                table: "sm_InventoryCheckNoteItems");

            migrationBuilder.DropIndex(
                name: "IX_sm_InventoryCheckNote_TenantId",
                table: "sm_InventoryCheckNote");

            migrationBuilder.DropIndex(
                name: "IX_sm_Email_Verification_TenantId",
                table: "sm_Email_Verification");

            migrationBuilder.DropIndex(
                name: "IX_sm_Email_Template_Translations_TenantId",
                table: "sm_Email_Template_Translations");

            migrationBuilder.DropIndex(
                name: "IX_sm_Email_Template_TenantId",
                table: "sm_Email_Template");

            migrationBuilder.DropIndex(
                name: "IX_sm_Email_Subscribe_TenantId",
                table: "sm_Email_Subscribe");

            migrationBuilder.DropIndex(
                name: "IX_sm_EInvoiceVatAnalytics_TenantId",
                table: "sm_EInvoiceVatAnalytics");

            migrationBuilder.DropIndex(
                name: "IX_sm_EInvoiceItems_TenantId",
                table: "sm_EInvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_sm_EInvoice_TenantId",
                table: "sm_EInvoice");

            migrationBuilder.DropIndex(
                name: "IX_sm_DebtTransaction_TenantId",
                table: "sm_DebtTransaction");

            migrationBuilder.DropIndex(
                name: "IX_sm_CustomerServiceComment_TenantId",
                table: "sm_CustomerServiceComment");

            migrationBuilder.DropIndex(
                name: "IX_sm_Customer_TenantId",
                table: "sm_Customer");

            migrationBuilder.DropIndex(
                name: "IX_sm_CodeType_Translation_TenantId",
                table: "sm_CodeType_Translation");

            migrationBuilder.DropIndex(
                name: "IX_sm_CodeType_TenantId",
                table: "sm_CodeType");

            migrationBuilder.DropIndex(
                name: "IX_sm_Cashbook_Transaction_TenantId",
                table: "sm_Cashbook_Transaction");

            migrationBuilder.DropIndex(
                name: "IX_sm_Bom_TenantId",
                table: "sm_Bom");

            migrationBuilder.DropIndex(
                name: "IX_sm_ActiviyHisroty_TenantId",
                table: "sm_ActiviyHisroty");

            migrationBuilder.DropIndex(
                name: "IX_mk_XuatNhapTon_TenantId",
                table: "mk_XuatNhapTon");

            migrationBuilder.DropIndex(
                name: "IX_mk_WorkingDay_TenantId",
                table: "mk_WorkingDay");

            migrationBuilder.DropIndex(
                name: "IX_mk_PhongBan_TenantId",
                table: "mk_PhongBan");

            migrationBuilder.DropIndex(
                name: "IX_mk_NhomVatTu_TenantId",
                table: "mk_NhomVatTu");

            migrationBuilder.DropIndex(
                name: "IX_mk_NguyenVatLieu_TenantId",
                table: "mk_NguyenVatLieu");

            migrationBuilder.DropIndex(
                name: "IX_mk_KiemKho_TenantId",
                table: "mk_KiemKho");

            migrationBuilder.DropIndex(
                name: "IX_mk_DuAn_TenantId",
                table: "mk_DuAn");

            migrationBuilder.DropIndex(
                name: "IX_mk_ChucVu_TenantId",
                table: "mk_ChucVu");

            migrationBuilder.DropIndex(
                name: "IX_mk_ChamCongItem_TenantId",
                table: "mk_ChamCongItem");

            migrationBuilder.DropIndex(
                name: "IX_mk_ChamCong_TenantId",
                table: "mk_ChamCong");

            migrationBuilder.DropIndex(
                name: "IX_mk_CauHinhNhanSu_TenantId",
                table: "mk_CauHinhNhanSu");

            migrationBuilder.DropIndex(
                name: "IX_mk_CacKhoanTroCap_TenantId",
                table: "mk_CacKhoanTroCap");

            migrationBuilder.DropIndex(
                name: "IX_mk_Bom_TenantId",
                table: "mk_Bom");

            migrationBuilder.DropIndex(
                name: "IX_mk_BHXH_TenantId",
                table: "mk_BHXH");

            migrationBuilder.DropIndex(
                name: "IX_mk_BangTinhLuong_TenantId",
                table: "mk_BangTinhLuong");

            migrationBuilder.DropIndex(
                name: "IX_mk_BangLuongItem_TenantId",
                table: "mk_BangLuongItem");

            migrationBuilder.DropIndex(
                name: "IX_IdmRightMapRole_TenantId",
                table: "IdmRightMapRole");

            migrationBuilder.DropIndex(
                name: "IX_IdmRight_TenantId",
                table: "IdmRight");

            migrationBuilder.DropIndex(
                name: "IX_idm_User_TenantId",
                table: "idm_User");

            migrationBuilder.DropIndex(
                name: "IX_idm_Role_TenantId",
                table: "idm_Role");

            migrationBuilder.DropIndex(
                name: "IX_fm_Search_Sample_TenantId",
                table: "fm_Search_Sample");

            migrationBuilder.DropIndex(
                name: "IX_erp_Attachment_TenantId",
                table: "erp_Attachment");

            migrationBuilder.DropIndex(
                name: "IX_cata_Province_TenantId",
                table: "cata_Province");

            migrationBuilder.DropIndex(
                name: "IX_cata_District_TenantId",
                table: "cata_District");

            migrationBuilder.DropIndex(
                name: "IX_cata_Commune_TenantId",
                table: "cata_Commune");

            migrationBuilder.DropIndex(
                name: "IX_bsd_Parameter_TenantId",
                table: "bsd_Parameter");

            migrationBuilder.DropIndex(
                name: "IX_bsd_Navigation_Map_Role_TenantId",
                table: "bsd_Navigation_Map_Role");

            migrationBuilder.DropIndex(
                name: "IX_bsd_Navigation_TenantId",
                table: "bsd_Navigation");
        }
    }
}
