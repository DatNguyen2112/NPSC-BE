using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Common
{
    public class Constants
    {
    }

    public static class ClaimConstants
    {
        public static string USER_ID = "x-userId";
        public static string APP_ID = "x-appId";
        public static string USER_NAME = "x-userName";
        public static string FULL_NAME = "x-fullName";
        public static string ROLES = "x-roles";
        public static string RIGHTS = "x-rights";
        public static string LEVEL = "x-level";
        public static string ISSUED_AT = "x-iat";
        public static string EXPIRES_AT = "x-exp";
        public static string CHANNEL = "x-channel";
        public static string PHONE = "x-phone";
        public static string LANGUAGE = "x-language";
        public static string CURRENCY = "x-currency";
        public static string PARTNER_ID = "x-partner-id";
        public static string TENANT_ID = "x-tenantId";
    }

    public static class LanguageConstants
    {
        public const string Default = "vn";
        public const string English = "en";
        public const string Vietnamese = "vn";

        public static string[] AvailableLanguages = { English, Vietnamese };
    }
    public class SendProposalTypeConstants
    {
        public static string FARMER = "FARMER";
        public static string ORDERER = "ORDERER";
        public static string KT = "KT";
        public static string CSKH = "CSKH";
    }
    public class GenderConstants
    {
        public static string MALE = "MALE";
        public static string FEMALE = "FEMALE";
    }
    public class UserStatusConstants
    {
        public static string BUSY = "BUSY";
        public static string FREE = "FREE";
    }
    public static class EmailSubscriptionConstants
    {
        public const string Email_Subscribed = "subscribed";
        public const string Email_Unsubscribed = "unsubscribed";
    }

    public static class CashbookTransactionConstants
    {
        public const string ProjectType = "project";
        public const string ReceiptVoucherType = "THU";
        public const string ReceiptVoucherPrefix = "RVN-";
        public const string PaymentVoucherType = "CHI";
        public const string PaymentVoucherPrefix = "PVN-";
        
        public const string InitialCashbookTransaction = "INITIAL_TRANSACTION";
        public const string InitialCashbookTransactionPrefix = "INT-";

        public const string AUTO_RECEIPT_PURPOSE = "auto_receipt"; // Phiếu thu tự động
        public const string AUTO_PAYMENT_PURPOSE = "auto_payment"; // Phiếu chi tự động
        public const string CancelStatusName = "Đã hủy";
        public const string CompletedStatusName = "Hoàn thành";
        public const string WaitTransferStatusName = "Chờ thanh toán";
        // Có thay đổi công nợ đối tượng nộp/nhận (IsDebt)
        public const string IsDebtChanged = "Có";
        public const string IsDebtUnChanged = "Không";
    }

    public class AttachmentDocTypeConstants
    {
        // Post
        public const string Post_Thumbnail = "post_thumb";
        public const string Post_Cover = "post_cover";

        // Farmer
        public const string Farmer_Avatar = "farmer_avatar";
        public const string Farmer_NationalId = "farmer_nationalid";
        public const string Farmer_Medical_Degree = "farmer_medical_degree";
        public const string Farmer_Previous_Customer = "farmer_previous_customer";

        // Clinic: gồm ảnh đại diện, ảnh người đứng đầu, giấy phép hoạt động và hình ảnh trang thiết bị
        public const string Clinic_Avatar = "clinic_avatar";
        public const string Clinic_Supervisor_Avatar = "clinic_supervisor_avatar";
        public const string Clinic_Supervisor_NationalId = "clinic_supervisor_nationalid";
        public const string Clinic_Supervisor_Degree = "clinic_supervisor_degree";
        public const string Clinic_License = "clinic_license";
        public const string Clinic_Equipment = "clinic_equipment";

        // Medical record
        public const string Medrec_Film_Before = "medrec_film_before";
        public const string Medrec_Img_Before = "medrec_img_before";

        // Treatment Diary 
        public const string Tredia_Film_After = "tredia_img_after";
        public const string Tredia_Img_After = "tredia_film_after";
        public const string Tredia_Film_Before = "tredia_film_before";
        public const string Tredia_Img_Before = "tredia_img_before";

        public const string OrdererClaim = "orderer_claim_file";

        public const string WalletWithdrawal = "withdrawal";

        public const string DataImage = "image";
        public const string DataImage1 = "image1";
        public const string DataImage2 = "image2";
        public const string DataImage3 = "image3";

        // customer
        public const string Customer_Portrait = "PORTRAIT";
        public const string Customer_FrontIdentification = "FRONTIDENTIFICATION";
        public const string Customer_BackIdentification = "BACKIDENTIFICATION";
        public const string Customer_Other = "OTHER";
        //CustomerService
        public const string Customer_Service_Image = "CUSTOMERSERVICEIMAGE";
        public const string Customer_Service_Invoice = "CUSTOMERSERVICEINVOICE";

        //Avatar_Supplies
        public const string Avatar_Supplies = "avatar_supplies";
        //Attach
        public const string Attach = "attach";
        //Ảnh đại diện
        public const string Avatar = "avatar";
        //Logo
        public const string Logo = "logo";

        public const string Task = "task";

    }
    public class AttachmentEntityTypeConstants
    {
        public const string PreExecutionImage = "PRE_EXECUTION_IMAGE";
        public const string PreExecutionFilm = "PRE_EXECUTION_FILM";
        public const string AfterExecutionImage = "AFTER_EXECUTION_IMAGE";
        public const string AfterExecutionFilm = "AFTER_EXECUTION_FILM";
        public const string Post = "post";
        public const string Order = "order";
        public const string Farmer = "farmer";
        public const string Clinic = "clinic";
        public const string MedicalRecord = "medical_record";
        public const string OrdererClaim = "orderer_claim";
        public const string TreatmentDiary = "treatment_diary";
        public const string Wallet = "wallet";
        public const string Data = "data";
        public const string Customer = "customer";
        public const string CustomerService = "c_service";
        public const string PaidHistory = "c_service_paid";
        public const string Avatar_Supplies = "avatar_supplies";
        public const string Attach = "attach";
        public const string Avatar = "avatar";
        public const string Tenant = "tenant";
        public const string Task = "task";
    }
    public static class NotificationTypeConstants
    {
        public const string Profile_Approved = "profile.approved";
        public const string Profile_Rejected = "profile.rejected";
        public const string Commission_Received = "commission.received";
        public const string Proposal_Received = "proposal.received";
        public const string Message_Received = "message.received";
        public const string Offer_Received = "offer.received";
        public const string Offer_Rejected = "offer.rejected";
        public const string New_GlobalEvent = "new.globalevent";
        public const string Order = "ORDER";
        public const string ProfileFarmer = "PROFILE_FARMER";
        public const string User = "USER";
        public static string[] AvailableNotificationTypes =
            { Profile_Approved, Profile_Rejected, Commission_Received, Proposal_Received, Offer_Received, Offer_Rejected, Message_Received, New_GlobalEvent, Order,ProfileFarmer,User };
    }
    public static class StatusCashbookTransaction
    {
        public const string COMPLETED = "COMPLETED";
        public const string CANCELED = "CANCELED";
        public const string WAIT_TRANSFER = "WAIT_TRANSFER";
    }
    public static class IssueManagementConstants
    {
        public const string IssueCodePrefix = "IN";
    }
   
    public static class StatusIssue
    {
        public const string COMPLETED = "COMPLETED";
        public const string CANCELED = "CANCELED";
        public const string WAIT_PROCESSING = "WAIT_PROCESSING";
    }
    public static class CodeTypeConstants
    {
        public const string Loai_khach_hang = "LOAI_KHACH_HANG";
        public const string Kho = "KHO";
        public const string Don_Vi_Tinh = "DON_VI_TINH";
        public const string Purpose_Receipt = "purposeReceipt"; // Loại phiếu thu
        public const string Expenditure_Purpose = "EXPENDITURE_PURPOSE"; // Loại phiếu chi
        public const string Payment_Method = "PAYMENT_METHOD"; // Phương thức thanh toán
        public const string PaymentGroup = "PaymentGroup"; // Nhóm người nộp
        public const string ActionCode = "ActionCode"; // Thao tác khi thực hiện nhập hàng, xuất hàng, cân bằng kho
        public const string ReturnedReasonCode = "ReturnedReasonCode"; // Lý do trả hàng
        public const string InventoryImportType = "InventoryImportType";  // Loại phiếu nhập kho
        public const string InventoryExportType = "InventoryExportType";  // Loại phiếu xuất kho
        public const string CustomerGroup = "CustomerGroup"; // Nhóm khách hàng
        public const string SupplierGroup = "SupplierGroup"; // Nhóm nhà cung cấp
        public const string VATList = "VATList"; // Danh sách thuế
        public const string InventoryCheckNoteReason = "InventoryCheckNoteReason"; // Lý do kiểm hàng
        public const string AssetType = "ASSET_TYPE"; // Loại tài sản
        public const string AssetGroup = "ASSET_GROUP"; // Nhóm tài sản
        public const string CustomerType = "CustomerType"; // Loại khách hàng
        public const string CustomerSource = "CustomerSource"; // Nguồn khách hàng
        public const string VatGTGT = "VAT_GTGT"; // Thuế % GTGT
        public const string TaskStatus = "TASK_STATUS"; // Trạng thái công việc
        public const string TaskType = "TASK_TYPE"; // Loại công việc
        
        // EVN
        public const string VoltageType = "VOLTAGE_TYPE"; // Loại cấp điện áp
        public const string OwnerType = "OWNER_TYPE"; // Loại chủ đầu tư
        public const string ConsultService =  "CONSULT_SERVICE"; // Loại dịch vụ tư vấn
        public const string ProcessTemplate =  "PROCESS_TEMPLATE"; // Mẫu quy trình
        public const string Investor =  "INVESTOR"; // Chủ đầu tư
        public const string OrganizationStructure = "ORGANIZATION_STRUCTURE"; // Phòng ban
    }
    public class FileTypeConstants
    {
        public const string Image = "IMAGE";
        public const string Doc = "DOC";
        public const string Text = "TEXT";
        public const string Other = "OTHER";
        public const string Archive = "ARCHIVE";
        public const string Audio = "AUDIO";
        public const string Video = "VIDEO";

        public static string GetFileType(string ext)
        {
            var imageExts = new string[] { ".jpg", ".jpeg", ".bmp", ".gif", ".apng", ".pjpg", ".png", ".svg", ".webp", ".tiff" };
            var docExts = new string[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf" };
            var archiveExts = new string[] { ".rar", ".zip", ".7zip", ".tar" };
            var audioExts = new string[] { ".mp3", ".wav" };
            var videoExts = new string[] { ".mp4", ".mkv" };
            var textExts = new string[] { ".log", ".txt" };

            if (imageExts.Contains(ext))
                return Image;
            if (textExts.Contains(ext))
                return Text;
            if (docExts.Contains(ext))
                return Doc;
            if (archiveExts.Contains(ext))
                return Archive;
            if (audioExts.Contains(ext))
                return Audio;
            if (videoExts.Contains(ext))
                return Video;
            else
                return Other;
        }
    }
    public class FileStatusConstants
    {
        public const string New = "new";
        public const string InUse = "inuse";
        public const string Delete_Confirmed = "delete_confirmed";
        public const string Deleted = "deleted";
    }
    public class RoleConstants
    {
        public static Guid AdmIdmRoleId => new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid IdAdmin => new Guid("00000031-0000-0000-0000-2333ec9adfe9");
        public static Guid NormalUserRoleId => new Guid("a0aba7cc-4fe2-451d-8e6a-f285b5137b1b");

        public const string NormalUser = "USER";
        public const string NND = "NND";

        public const int NormalUserLevel = 1;
        public const string SuperAdminRoleCode = "SUPER_ADMIN";
        public const string AdminRoleCode = "ADMIN";
        public const string PermissionFullControlRoleCode = "PER_FC";
        public const string PostFullControlRoleCode = "PST_FC";
        public const string LeaveFullControlRoleCode = "LVE_FC";
        public const string WorkDayFullControlRoleCode = "WKD_FC";
        public const string EmployeeFullControlRoleCode = "EMP_FC";
        public const string StaffCode = "STAFF";
        public const string NPComposerCode = "COM_NP";
        public const string NPApproverCode = "APP_NP";
        public const string NPPublisherCode = "PUB_NP";
        public const string NPAdminCode = "ADM_NP";

        public const string FarmerSideRoleCode = "FARMER";
        public const string OrderSideRoleCode = "ORDERER";
        public const string CSKHRoleCode = "CSKH";
        public const string KTRoleCode = "KT";
        public const int SuperAdministratorLevel = 100;
        public const int AdministratorLevel = 0;
        public const int PermissionFullControlLevel = 2;
        public const int PostFullControlLevel = 2;
        public const int LeaveFullControlLevel = 2;
        public const int WorkDayFullControlLevel = 2;
        public const int EmployeeFullControlLevel = 2;

        public const int NPComposerLevel = 3;
        public const int NPApproverLevel = 3;
        public const int NPPublisherLevel = 3;
        public const int NPAdminLevel = 3;
        public const int FarmerSideLevel = 1;
        public const int OrderSideLevel = 2;
    }

    public class AppConstants
    {
        public static string EnvironmentName = "production";
        public static Guid HO_APP => new Guid("00000000-0000-0000-0000-000000000001");
    }

    public class UserConstants
    {
        public static Guid AdministratorUserId => new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid GuestUserId => new Guid("00000000-0000-0000-0000-000000000002");
    }

    public static class ParamConstants
    {
        public const string VNPOST_DEFAULT_SHIPPING_PRICE = "VNPOST_DEFAULT_SHIPPING_PRICE";
        public const string VNPOST_TEST_ENABLED = "VNPOST_TEST_ENABLED";
        public const string SHIPPING_DEFAULT_COMPANY = "SHIPPING_DEFAULT_COMPANY";
        public const string SHOPEE_SHIPPING_CARRIERS = "SHOPEE_SHIPPING_CARRIERS";
        public const string VNPOST_MAX_FAST_DELIVERY_WEIGHT = "VNPOST_MAX_FAST_DELIVERY_WEIGHT";

        public const string VIETTEL_DEFAULT_SHIPPING_PRICE = "VIETTEL_DEFAULT_SHIPPING_PRICE";
        public const string VIETTEL_TEST_ENABLED = "VIETTEL_TEST_ENABLED";
        public const string VIETTEL_MAX_FAST_DELIVERY_WEIGHT = "VIETTEL_MAX_FAST_DELIVERY_WEIGHT";
        public const string VIETTEL_SENDER_PROVINCE_ID = "VIETTEL_SENDER_PROVINCE_ID";
        public const string VIETTEL_SENDER_DISTRICT_ID = "VIETTEL_SENDER_DISTRICT_ID";
        public const string VIETTEL_SECRET_KEY = "VIETTEL_SECRET_KEY";
        public const string MAINTENANCE_ORDER_CREATE = "MAINTENANCE_ORDER_CREATE";
        public const string PAYMENT_REQUEST_INVOICE_FEE = "PAYMENT_REQUEST_INVOICE_FEE";
        public const string REGISTER_CODE_TIMEOUT = "REGISTER_CODE_TIMEOUT";
        public const string REGISTER_CODE_ENABLED = "REGISTER_CODE_ENABLED";

        public const string WALLET_DEPOSIT_SUGGESTED_AMOUNTS = "WALLET_DEPOSIT_SUGGESTED_AMOUNTS";
        public const string WALLET_DEPOSIT_MIN_AMOUNT = "WALLET_DEPOSIT_MIN_AMOUNT";

        public const string WALLET_DEPOSIT_BANK_NUMBER = "WALLET_DEPOSIT_BANK_NUMBER";
        public const string WALLET_DEPOSIT_BANK_NAME = "WALLET_DEPOSIT_BANK_NAME";
        public const string WALLET_DEPOSIT_BANK_OWNER = "WALLET_DEPOSIT_BANK_OWNER";

        public const string VERSION_OLD_IOS = "VERSION_OLD_IOS";
        public const string VERSION_OUTDATED_IOS = "VERSION_OUTDATED_IOS";
        public const string VERSION_OLD_ANDROID = "VERSION_OLD_ANDROID";
        public const string VERSION_OUTDATED_ANDROID = "VERSION_OUTDATED_ANDROID";

        public const string MAINTENANCE_TOP_ANNOUNCEMENT = "MAINTENANCE_TOP_ANNOUNCEMENT";
        public const string SITEMAP_POST_NUMBER = "SITEMAP_POST_NUMBER";
        public const string VIEW_PROFILE_WITHOUT_LOGIN = "VIEW_PROFILE_WITHOUT_LOGIN";
        public const string INITIAL_SITE_VIEWS = "INITIAL_SITE_VIEWS";
        public const string INITIAL_TOTAL_DEAL = "INITIAL_TOTAL_DEAL";


        public const string PLATFORM_FEE = "PLATFORM_FEE";
        public const string PEAK_HOUR_FARMER_PERCENTAGE = "PEAK_HOUR_FARMER_PERCENTAGE";
        public const string PEAK_HOUR_FEE_AMOUNT = "PEAK_HOUR_FEE_AMOUNT";
        public const string NUMBER_OF_DAYS_RECEIVE_MONEY = "NUMBER_OF_DAYS_RECEIVE_MONEY";
        public const string DISTRIBUTE_AGAIN_MINUTE = "DISTRIBUTE_AGAIN_MINUTE";
    }

    public static class TrangThaiKhachHangConstants
    {
        public const string KHACH_MOI = "KHACH_MOI";
        public const string KHACH_DA_CHOT = "KHACH_MOI";
        public const string KHACH_CAN_CHAM_SOC = "KHACH_CAN_CHAM_SOC";
    }

    public static class LichSuChamSocConstants
    {
        public const string NOW = "NOW"; // Mới tạo
        public const string PENDING = "PENDING"; // Đang thực hiện
        public const string COMPLETED = "COMPLETED"; // Hoàn thành

        public const string CONFIRMED_TASK_COMPLETED = "Xác nhận công việc hoàn thành";
        public const string RESTORED_TASK = "Khôi phục công việc";

        public static class CodePrefix
        {
            public const string DATE_CUSTOMER_SERVICE_CODE = "AN-";
            public const string HISTORY_CUSTOMER_SERVICE_CODE = "CN-";
        }

        public static class Type
        {
            public const string DATE_CUSTOMER_SERVICE = "DATE_CUSTOMER_SERVICE";
            public const string HISTORY_CUSTOMER_SERVICE = "HISTORY_CUSTOMER_SERVICE";
        }
    }

    public static class CustomerTypeConstants
    {
        public const string M = "M";
        public const string TN = "TN";
        public const string HTV = "HTV";
        public const string ĐLH = "ĐLH";
        public const string ĐĐP = "ĐĐP";
        public const string TC = "TC";
        public const string TT = "TT";
        public const string KPH = "KPH";
        public const string CHĐ = "CHĐ";
        public const string K = "K";
    }

    public static class LoaiXuatNhapTonConstants
    {
        public const string NHAP_KHO = "NHAP_KHO";
        public const string XUAT_KHO = "XUAT_KHO";
        public const string TON_KHO = "TON_KHO";
    }

    public static class KiemKhoConstants
    {
        public const string DANG_KIEM_KHO = "DANG_KIEM_KHO";
        public const string DA_CAN_BANG = "DA_CAN_BANG";
    }

    public static class TrangThaiPhieuNhapXuatConstants
    {
        public const string NHAP = "NHAP";
        public const string CHO_DUYET = "CHO_DUYET";
        public const string DA_DUYET = "DA_DUYET";
        public const string DA_TU_CHOI = "DA_TU_CHOI";
        public const string DA_HOAN_THANH = "DA_HOAN_THANH";
    }

    public static class RecieveStatus
    {
        public const string DA_NHAP = "DA_NHAP";
        public const string CHUA_NHAP = "CHUA_NHAP";
    }

    public static class PurchaseOrderStatusConstants
    {
        public const string PENDING_ORDER = "ĐANG_GIAO_DICH";
        public const string REJECT_ORDER = "DA_HUY";
        public const string FULLFILLED_ORDER = "HOAN_THANH";
        public const string AUTO_RECEIPT = "auto_receipt"; // Phiếu thu tự động
    }

    public static class SaleOrderStatusConstants
    {
        public const string PENDING_ORDER = "ĐANG_GIAO_DICH";
        public const string REJECT_ORDER = "DA_HUY";
        public const string FULLFILLED_ORDER = "HOAN_THANH";
        public const string AUTO_PAYMENT = "auto_payment"; // Phiếu chi tự động
    }
    public static class SalesOrderConstants
    {
        /// <summary>
        /// Constants cho Discount Type
        /// </summary>
        public static class DiscountType
        {
            /// <summary>
            /// Discount bằng Tỉ lệ %
            /// </summary>
            public const string Percent = "percent";

            /// <summary>
            /// Discount bằng số tiền trực tiếp
            /// </summary>
            public const string Value = "value";
        }

        public static class StatusCode
        {
            public const string FINALIZED = "finalized";
            public const string CANCELLED = "cancelled";
            public const string COMPLETED = "completed";
        }

        public static class ExportStatusCode
        {
            public const string CHUA_XUAT = "CHUA_XUAT";
        }

        public static StatusViewModel FetchStatus(string statusCode)
        {
            var status = new StatusViewModel();
            status.Code = statusCode;
            switch (statusCode)
            {
                case StatusCode.FINALIZED:
                    status.Name = "Đang giao dịch";
                    break;
                case StatusCode.COMPLETED:
                    status.Name = "Hoàn thành";
                    break;
                case StatusCode.CANCELLED:
                    status.Name = "Đã hủy";
                    break;
                default:
                    break;
            }
            return status;
        }

        public const string OrderCodePrefix = "SON-";

    }

    public static class PurchaseOrderConstants
    {
        /// <summary>
        /// Constants cho Discount Type
        /// </summary>
        public static class DiscountType
        {
            /// <summary>
            /// Discount bằng Tỉ lệ %
            /// </summary>
            public const string Percent = "percent";

            /// <summary>
            /// Discount bằng số tiền trực tiếp
            /// </summary>
            public const string Value = "value";
        }

        public static class StatusCode
        {
            public const string FINALIZED = "finalized";
            public const string CANCELLED = "cancelled";
            public const string COMPLETED = "completed";
        }

        public static StatusViewModel FetchStatus(string statusCode)
        {
            var status = new StatusViewModel();
            status.Code = statusCode;
            switch (statusCode)
            {
                case StatusCode.FINALIZED:
                    status.Name = "Đang giao dịch";
                    break;
                case StatusCode.COMPLETED:
                    status.Name = "Hoàn thành";
                    break;
                case StatusCode.CANCELLED:
                    status.Name = "Đã hủy";
                    break;
                default:
                    break;
            }
            return status;
        }

        public const string OrderCodePrefix = "PON-";

        public const string NHAP_HANG = "NHAP_HANG";
        public const string CHUA_NHAP = "CHUA_NHAP";
    }

    public class StatusViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string BackgroundColor { get; set; }
        public string ForeColor { get; set; }

    }
    public static class PaymentStatusConstants
    {
        public const string CHUA_THANH_TOAN = "CHUA_THANH_TOAN";
        public const string DA_THANH_TOAN = "DA_THANH_TOAN";
        public const string THANH_TOAN_MOT_NUA = "THANH_TOAN_MOT_NUA";
    }

    public static class OriginalDocumentTypeConstants
    {
        public const string NHAP_HANG = "NHAP_HANG";
        public const string BAN_HANG = "BAN_HANG";
        public const string KIEM_KHO = "KIEM_KHO";
        public const string XUAT_KHO = "XUAT_KHO";
    }

    public static class ActionConstants
    {
        public const string EXPORT_ORDER = "export_order";
        public const string RECEIPT_ORDER = "receipt_order";
        public const string INITIALIZE = "Khởi tạo tồn ban đầu";
        public const string INVENTORY_BALANCE = "inventory_balance";
        public const string TRANSFER_WAREHOUSE = "transfer_warehouse";
    }

    public static class PrefixPurchaseOrderConstants
    {
        public const string PURCHASE_ORDER = "PON-";
    }


    public static class PrefixConstants
    {
        public const string QUOTATION_PREFIX = "QT-";
        public const string CUSTOMER_PREFIX = "CUST-";
    }

    public static class EntityTypeCodeConstants
    {
        public const string CUSTOMER = "customer";
        public const string SUPPLIER = "supplier";
    }
    public static class EntityTypeNameConstants
    {
        public const string CUSTOMER = "Khách hàng";
        public const string SUPPLIER = "Nhà cung cấp";
    }

    public static class DebtTransactionActionsCodesConstants
    {
        public const string RECEIPT_VOUCHER_CREATE = "receipt_voucher_create";
        public const string PAYMENT_VOUCHER_CREATE = "payment_voucher_create";
        public const string INVENTORY_EXPORT = "inventory_export";
        public const string INVENTORY_IMPORT = "inventory_import";
        public const string RECEIPT_VOUCHER_CANCEL = "receipt_voucher_cancel";
        public const string PAYMENT_VOUCHER_CANCEL = "payment_voucher_cancel";
        public const string RETURN_INVENTORY_EXPORT = "return_inventory_export";
        public const string RETURN_INVENTORY_IMPORT = "return_inventory_import";
        public const string CUSTOMER_DEBT_INIT = "customer_debt_init";
        public const string SUPPLIER_DEBT_INIT = "supplier_debt_init";
    }

    public static class DebtTransactionNotesConstants
    {
        public const string RECEIPT_VOUCHER_CREATE = "Tạo phiếu thu";
        public const string PAYMENT_VOUCHER_CREATE = "Tạo phiếu chi";
        public const string INVENTORY_EXPORT = "Bán hàng";
        public const string INVENTORY_IMPORT = "Nhập hàng";
        public const string RECEIPT_VOUCHER_CANCEL = "Hủy phiếu thu";
        public const string PAYMENT_VOUCHER_CANCEL = "Hủy phiếu chi";
        public const string RETURN_INVENTORY_EXPORT = "Khách trả hàng";
        public const string RETURN_INVENTORY_IMPORT = "Trả hàng nhà cung cấp";
        public const string CUSTOMER_DEBT_INIT = "Khởi tạo công nợ ban đầu khách hàng";
        public const string SUPPLIER_DEBT_INIT = "Khởi tạo công nợ ban đầu nhà cung cấp";
    }

    public static class DebtTransactionOriginalDocumentTypesConstants
    {
        public const string RECEIPT_VOUCHER = "receipt_voucher";
        public const string PAYMENT_VOUCHER = "payment_voucher";
        public const string SALES_ORDER = "sales_order";
        public const string PURCHASE_ORDER = "purchase_order";
        public const string CUSTOMER_RETURN = "customer_return";
        public const string SUPPLIER_RETURN = "supplier_return";
    }

    public static class DebtTransactionEntityTypesConstants
    {
        public const string CUSTOMER = "customer";
        public const string SUPPLIER = "supplier";
    }
    public static class CashbookTransactionEntityTypeCodeConstants
    {
        public const string CUSTOMER = "customer";
        public const string SUPPLIER = "supplier";
    }

    public static class OriginDocumentCashbookTransactionConstants
    {
        public const string SALES_ORDER = "sales_order";
        public const string PURCHASE_ORDER = "purchase_order";
        public const string SALES_RETURN = "sales_return"; // Nhập kho khách trả hàng
        public const string PURCHASE_RETURN = "purchase_return"; // Xuất kho trả hàng NCC
    }

    public static class OrderReturnConstants
    {
        public static class StatusCode
        {
            public const string RETURNING = "returning"; // Chưa nhận
            public const string RETURNED = "returned"; // Đã nhận
            public const string CANCELLED = "cancelled"; // Đã hủy
        }

        public static class RefundStatusCode
        {
            public const string PAID = "paid"; // Đã hoàn tiền
            public const string UNPAID = "unpaid"; // Chưa hoàn tiền
            public const string PARTIAL = "partial"; // Hoàn tiền một nửa
        }

        public static class EntityTypeCode
        {
            public const string CUSTOMER = "customer";
            public const string SUPPLIER = "supplier";
        }

        public static class EntityTypeName
        {
            public const string KHACH_HANG = "khách hàng";
            public const string NHA_CUNG_CAP = "nhà cung cấp";
        }

        public static class ActionCode
        {
            public const string CUSTOMER_RETURN = "customer_return";
            public const string SUPPLIER_RETURN = "supplier_return";
        }

        public static class OriginalDocumentType
        {
            public const string PURCHASE_RETURN = "purchase_return";
            public const string SALES_RETURN = "sales_return";
        }

        public const string CustomerReturnOrderCodePrefix = "SRN-";
        public const string SupplierReturnOrderCodePrefix = "PRN-";
    }

    public static class InventoryNoteConstants
    {
        public static class StatusCode
        {
            public const string DRAFT = "DRAFT"; // Nháp
            public const string COMPLETED = "COMPLETED"; // Hoàn thành
            public const string CANCELLED = "CANCELLED"; // Đã hủy
        }

        public static class StatusName
        {
            public const string DRAFT = "Nháp"; // Nháp
            public const string COMPLETED = "Hoàn thành"; // Hoàn thành
            public const string CANCELLED = "Đã hủy"; // Đã hủy
        }

        public static class Code
        {
            public const string PrefixInventoryImport = "IIN-";
            public const string PrefixInventoryExport = "IEN-";
        }

        public static class TypeCode
        {
            public const string InventoryImport = "inventory_import";
            public const string InventoryExport = "inventory_export";
        }

        public static class EntityTypeCode
        {
            public const string CUSTOMER = "customer";
            public const string SUPPLIER = "supplier";
        }

        public static class EntityTypeName
        {
            public const string CUSTOMER = "Khách hàng";
            public const string SUPPLIER = "Nhà cung cấp";
        }

        public static class TransactionTypeCode
        {
            public const string SALES_GOODS_EXPORT = "sales_goods_export";
            public const string PURCHASE_GOODS_IMPORT = "purchase_goods_import";
            public const string RETURN_GOODS_IMPORT = "return_goods_import";
            public const string RETURN_GOODS_EXPORT = "return_goods_export";
        }

        public static class OriginalDocumentType
        {
            public const string SALES_ORDER = "sales_order";
            public const string PURCHASE_ORDER = "purchase_order";
            public const string CUSTOMER_RETURN = "customer_return";
            public const string SUPPLIER_RETURN = "supplier_return";
        }
    }

    public static class QuotationConstants
    {
        public static class StatusCode
        {
            public const string DRAFT = "DRAFT"; // Nháp
            public const string PENDING_APPROVAL = "PENDING_APPROVAL"; // Chờ duyệt
            public const string INTERNAL_APPROVAL = "INTERNAL_APPROVAL"; // Duyệt nội bộ
            public const string CUSTOMER_APPROVED = "CUSTOMER_APPROVED"; // Khách hàng duyệt
            public const string CANCELLED = "CANCELLED"; // Hủy
        }

        public static class StatusName
        {
            public const string DRAFT = "Nháp"; // Nháp
            public const string PENDING_APPROVAL = "Chờ duyệt"; // Chờ duyệt
            public const string INTERNAL_APPROVAL = "Duyệt nội bộ"; // Duyệt nội bộ
            public const string CUSTOMER_APPROVED = "Khách hàng duyệt"; // Khách hàng duyệt
            public const string CANCELLED = "Hủy"; // Hủy
        }

        public static class TypeCode
        {
            public const string Quotation_Material = "QuotationMaterial"; // Báo giá vật tư
            public const string Quotation_Product = "QuotationProduct"; // Báo giá sản phẩm
        }

        public static class TypeName
        {
            public const string Quotation_Material = "Báo giá vật tư"; // Báo giá vật tư
            public const string Quotation_Product = "Báo giá sản phẩm"; // Báo giá sản phẩm
        }

        /// <summary>
        /// Constants cho Discount Type
        /// </summary>
        public static class UnitPriceDiscountType
        {
            /// <summary>
            /// Discount bằng Tỉ lệ %
            /// </summary>
            public const string Percent = "percent";

            /// <summary>
            /// Discount bằng số tiền trực tiếp
            /// </summary>
            public const string Value = "value";
        }

        /// <summary>
        /// Constants cho Discount Type
        /// </summary>
        public static class DiscountType
        {
            /// <summary>
            /// Discount bằng Tỉ lệ %
            /// </summary>
            public const string Percent = "percent";

            /// <summary>
            /// Discount bằng số tiền trực tiếp
            /// </summary>
            public const string Value = "value";
        }
    }

    public static class RightActionConstants
    {
        public const string VIEW = "VIEW";
        public const string ADD = "ADD";
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE";
        public const string VIEWALL = "VIEWALL";
    }

    public static class InventoryCheckNoteConstants
    {
        public static class StatusCode
        {
            public const string DRAFT = "DRAFT"; // Nháp
            public const string COMPLETED = "COMPLETED"; // Hoàn thành
            public const string CANCELLED = "CANCELLED"; // Đã hủy
        }

        public static class DifferenceType
        {
            public const string MATCHED = "matched"; // Khớp
            public const string MISS_MATCHED = "miss_matched"; // Lệch
        }

        public static class ActionCode
        {
            public const string INVENTORY_BALANCE = "inventory_balance";
        }

        public const string InventoryCheckNoteCodePrefix = "ICN-";
    }

    public static class WarehouseTransferNoteConstants
    {
        public static class StatusCode
        {
            public const string DRAFT = "DRAFT"; // Nháp
            public const string COMPLETED = "COMPLETED"; // Hoàn thành
            public const string CANCELLED = "CANCELLED"; // Đã hủy
        }

        public static class ActionCode
        {
            public const string WAREHOUSE_TRANSFER = "warehouse_transfer";
        }

        public const string WarehouseTransferNoteCodePrefix = "ITN-";
    }

    public static class BomConstants
    {
        public static class Code
        {
            public const string PREFIX = "BOM-";
        }
    }

    public static class InvoiceConstants
    {
        public static class PaymentStatusCode
        {
            public const string DRAFT = "DRAFT"; // Nháp
            public const string NOT_YET_PAID = "NOT_YET_PAID"; // Chưa thanh toán
            public const string PARTIAL_PAYMENT = "PARTIAL_PAYMENT"; // Thanh toán một phần
            public const string PAID = "PAID"; // Đã thanh toán
        }

        public static class PaymentStatusName
        {
            public const string DRAFT = "Nháp"; // Nháp
            public const string NOT_YET_PAID = "Chưa thanh toán"; // Chưa thanh toán
            public const string PARTIAL_PAYMENT = "Thanh toán một phần"; // Thanh toán một phần
            public const string PAID = "Đã thanh toán"; // Đã thanh toán
        }

        public static class PaymentStatusColor
        {
            public const string DRAFT = "default"; // Màu xám nhạt - Nháp
            public const string NOT_YET_PAID = "red"; // Màu đỏ nhạt - Chưa thanh toán
            public const string PARTIAL_PAYMENT = "blue"; // Màu xanh dương nhạt - Thanh toán một phần
            public const string PAID = "green"; // Màu xanh lá nhạt - Đã thanh toán
        }

        public static class InvoiceTypeCode
        {
            public const string CONTRACT_INVOICE = "CONTRACT_INVOICE"; // Hóa đơn hợp đồng
            public const string ELECTRICITY_WATER_INVOICE = "ELECTRICITY_WATER_INVOICE"; // hóa đơn điện nước
            public const string MERCHANT_INVOICE = "MERCHANT_INVOICE"; // hóa đơn tiểu thương
        }

        public const string InvoiceCodePrefix = "INV-";

        public static class PaymentMethod
        {
            public const string WALLET = "wallet";
            public const string CASH = "cash";
            public const string TRANSFER = "bank";
            public const string OTHER = "other";

        }
    }

    public static class TenantStatusConstant
    {
        public const string ACTIVE = "ACTIVE";
        public const string INACTIVE = "INACTIVE";

    }

    public static class VehicleRequestHistoryAction
    {
        public const string CREATE = "CREATE";
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE";
        public const string SUBMIT = "SUBMIT";
        public const string APPROVE = "APPROVE";
        public const string REJECT = "REJECT";
        public const string SUBMIT_SHARING = "SUBMIT_SHARING";
        public const string APPROVE_SHARING = "APPROVE_SHARING";
    }

    public static class ContractHistoryAction
    {
        public const string CREATE = "CREATE";
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE";
    }

    public static class ConstructionConstants
    {
        public static class ActionType
        {
            public const string TASK = "TASK";
            public const string WEEK_REPORT = "WEEK_REPORT";
            public const string ISSUE_MANAGEMENT = "ISSUE_MANAGEMENT";
            public const string CONTRACT = "CONTRACT";
            public const string CONSTRUCTION = "CONSTRUCTION";
        }
        
        public static class StatusCode
        {
            public const string IS_DESIGNING = "IS_DESIGNING"; // Mới tạo
            public const string AUTHOR_SUPERVISOR = "AUTHOR_SUPERVISOR"; // Đang thực hiện
        }
        
        public static class ExecutionStatusCode
        {
            public const string APPROVED = "APPROVED"; // Mới tạo
            public const string IN_PROGRESS = "IN_PROGRESS"; // Đang thực hiện
        }
        
        public static class DocumentStatusCode
        {
            public const string NOT_APPROVE = "NOT_APPROVE"; // Mới tạo
            public const string APPROVED = "APPROVED"; // Đang thực hiện
        }

        public static class PriorityCode
        {
            public const string LEVEL_1 = "1"; // Mới tạo
            public const string LEVEL_2 = "2"; // Đang thực hiện
            public const string LEVEL_3 = "3"; // Đang thực hiện
        }

        public static class ConstructionWeekReportStatusCode
        {
            public const string RIGHT_ON_PLAN =  "RIGHT_ON_PLAN"; // Đúng kế hoạch
            public const string BEHIND_SCHEDULE =  "BEHIND_SCHEDULE"; // Chậm kế hoạch
            public const string OVER_SCHEDULE = "OVER_SCHEDULE"; // Vượt kế hoạch
        }

        public static class PrefixCode
        {
            public const string ConstructionCode = "PN-";
            public const string WeekReportCode = "RN-";
        }

        public static class ConstructionPermissionCode
        {
            public const string EXPORTIMPORTFILE = "EXPORTIMPORTFILE";
            
            // // Phân quyên công việc trong dự án
            // public const string UPDATETASK = "UPDATETASK";
            // public const string ADDTASK = "ADDTASK";
            // public const string APPROVETASK =  "APPROVETASK";
            // public const string SENDAPPROVETASK = "SENDAPPROVETASK";
            //
            // // Phân quyền vướng mắc trong dự án
            // public const string ADDISSUE = "ADDISSUE";
            // public const string UPDATEISSUE = "UPDATEISSUE";
            // public const string DELETEISSUE = "DELETEISSUE";
            // public const string REOPENISSUE = "REOPENISSUE";
            // public const string PROCESSISSUE =  "PROCESSISSUE";
        }
        
        public static StatusViewModel FetchStatus(string statusCode)
        {
            var status = new StatusViewModel();
            status.Code = statusCode;
            switch (statusCode)
            {
                case StatusCode.IS_DESIGNING:
                    status.Name = "Đang thiết kế";
                    break;
                case StatusCode.AUTHOR_SUPERVISOR:
                    status.Name = "Giám sát tác giả";
                    break;
                case ExecutionStatusCode.APPROVED:
                    status.Name = "Đã phê duyệt";
                    break;
                case ExecutionStatusCode.IN_PROGRESS:
                    status.Name = "Đang thực hiện";
                    break;
                case DocumentStatusCode.NOT_APPROVE:
                    status.Name = "Chưa phê duyệt";
                    break; 
                case PriorityCode.LEVEL_1:
                    status.Name = "Cấp 1";
                    break;
                case PriorityCode.LEVEL_2:
                    status.Name = "Cấp 2";
                    break;
                case PriorityCode.LEVEL_3:
                    status.Name = "Cấp 3";
                    break; 
                case ConstructionWeekReportStatusCode.RIGHT_ON_PLAN:
                    status.Name = "Đúng kế hoạch";
                    break;
                case ConstructionWeekReportStatusCode.BEHIND_SCHEDULE:
                    status.Name = "Chậm kế hoạch";
                    break;
                case ConstructionWeekReportStatusCode.OVER_SCHEDULE:
                    status.Name = "Vượt kế hoạch";
                    break; 
                default:
                    break;
            }
            return status;
        }
        
        public static StatusViewModel FetchCode(string statusName)
        {
            var status = new StatusViewModel();
            status.Name = statusName;
            switch (statusName)
            {
                case "Đang thiết kế":
                    status.Code = StatusCode.IS_DESIGNING;
                    break;
                case "Giám sát tác giả":
                    status.Code = StatusCode.AUTHOR_SUPERVISOR;
                    break;
                case "Đã phê duyệt":
                    status.Code = ExecutionStatusCode.APPROVED;
                    break;
                case "Đang thực hiện":
                    status.Code = ExecutionStatusCode.IN_PROGRESS;
                    break;
                case "Chưa phê duyệt":
                    status.Code = DocumentStatusCode.NOT_APPROVE;
                    break; 
                case "Cấp 1":
                    status.Code = PriorityCode.LEVEL_1;
                    break;
                case "Cấp 2":
                    status.Code = PriorityCode.LEVEL_2;
                    break;
                case "Cấp 3":
                    status.Code = PriorityCode.LEVEL_3;
                    break; 
                case "Đúng kế hoạch":
                    status.Code = ConstructionWeekReportStatusCode.RIGHT_ON_PLAN;
                    break;
                case "Chậm kế hoạch":
                    status.Code = ConstructionWeekReportStatusCode.BEHIND_SCHEDULE;
                    break; 
                default:
                    break;
            }
            return status;
        }
    }
    
    public static class MaterialRequestConstants
    {
        public static class StatusCode
        {
            public const string DRAFT = "DRAFT"; // Nháp
            public const string APPROVE = "APPROVE"; // Đã duyệt
            public const string PENDING_APPROVE =  "PENDING_APPROVE"; // Chờ duyệt
            public const string REJECT  = "REJECT"; // Từ chối
            public const string COMPLETED = "COMPLETED"; // Hoàn thành
        }

        public const string PrefixCode = "MRN-";
        
        public static StatusViewModel FetchStatus(string statusCode)
        {
            var status = new StatusViewModel();
            status.Code = statusCode;
            switch (statusCode)
            {
                case StatusCode.DRAFT:
                    status.Name = "Nháp";
                    break;
                case StatusCode.REJECT:
                    status.Name = "Từ chối";
                    break;
                case StatusCode.APPROVE:
                    status.Name = "Đã duyệt";
                    break;
                case StatusCode.PENDING_APPROVE:
                    status.Name = "Chờ duyệt";
                    break;
                case StatusCode.COMPLETED:
                    status.Name = "Hoàn thành";
                    break;
                default:
                    break;
            }
            return status;
        }
    }


    public class TaskManagementConstant
    {
        public static class TaskManagementConstantStatus
        {           
            public const string DRAFT = "DRAFT"; 
            public const string FINISHED = "FINISHED"; 
            public const string INPROGRESS = "INPROGRESS"; 
            public const string PAUSED = "PAUSED"; 
        }

        public static class TaskManagementConstantLabel
        {
            public const string EXTREME = "EXTREME";
            public const string HARD = "HARD";
            public const string NORMAL = "NORMAL";
            public const string EASY = "EASY";
        }

    }

    public static class ContractConstants
    {
        public static class StatusCode
        {
            public const string NEW = "NEW"; // Mới ký kết
            public const string IN_PROGRESS = "IN_PROGRESS"; // Đang thực hiện
            public const string COMPLETED = "COMPLETED"; // Đã hủy
            public const string CANCELLED = "CANCELLED"; // Đã thanh toán
        }

        public static class StatusName
        {
            public const string NEW = "Mới ký kết"; // Mới ký kết
            public const string IN_PROGRESS = "Đang thực hiện"; // Đang thực hiện
            public const string COMPLETED = "Hoàn thành"; // Hoàn thành
            public const string CANCELLED = "Đã hủy"; // Đã hủy
        }

        public static class StatusColor
        {
            public const string NEW = "orange"; // Màu xám nhạt - Mới ký kết
            public const string IN_PROGRESS = "blue"; // Màu xanh dương nhạt - Đang thực hiện
            public const string COMPLETED = "green"; // Màu xanh lá nhạt - Hoàn thành
            public const string CANCELLED = "red"; // Màu đỏ nhạt - Đã hủy
        }

        public static class DocumentTypeCode
        {
            public const string CONTRACT = "CONTRACT"; // Hợp đồng
            public const string APPENDIX = "APPENDIX"; // Phụ lục
        }

        public static class DocumentTypeName
        {
            public const string CONTRACT = "Hợp đồng"; // Hợp đồng
            public const string APPENDIX = "Phụ lục"; // Phụ lục
        }

        public static class DocumentTypeColor
        {
            public const string CONTRACT = "cyan"; // Màu xanh dương nhạt - Hợp đồng
            public const string APPENDIX = "blue"; // Màu xanh dương nhạt - Phụ lục
        }
    }

    public static class AdvanceRequestConstants
    {
        public static class PriorityLevelCode
        {
            public const string HIGH = "HIGH"; // Cao
            public const string MEDIUM = "MEDIUM"; // Trung bình
            public const string LOW = "LOW"; // Thấp
        }

        public static class PriorityLevelName
        {
            public const string HIGH = "Cao"; // Cao
            public const string MEDIUM = "Trung bình"; // Trung bình
            public const string LOW = "Thấp"; // Thấp
        }

        public static class PriorityLevelColor
        {
            public const string HIGH = "red"; // Màu đỏ - Cao
            public const string MEDIUM = "orange"; // Màu cam - Trung bình
            public const string LOW = "green"; // Màu xanh lá - Thấp
        }

        public static class StatusCode
        {
            public const string DRATF = "DRAFT"; // Nháp
            public const string PENDING_APPROVAL = "PENDING_APPROVAL"; // Chờ duyệt
            public const string APPROVED = "APPROVED"; // Đã duyệt
            public const string REJECTED = "REJECTED"; // Từ chối
            public const string COMPLETED = "COMPLETED"; // Hoàn thành
        }

        public static class StatusName
        {
            public const string DRATF = "Nháp"; // Nháp
            public const string PENDING_APPROVAL = "Chờ duyệt"; // Chờ duyệt
            public const string APPROVED = "Đã duyệt"; // Đã duyệt
            public const string REJECTED = "Từ chối"; // Từ chối
            public const string COMPLETED = "Hoàn thành"; // Hoàn thành
        }

        public static class StatusColor
        {
            public const string DRATF = "default"; // Màu xám nhạt - Nháp
            public const string PENDING_APPROVAL = "orange"; // Màu cam nhạt - Chờ duyệt
            public const string APPROVED = "blue"; // Màu xanh lá nhạt - Đã duyệt
            public const string REJECTED = "red"; // Màu đỏ nhạt - Từ chối
            public const string COMPLETED = "green"; // Màu xanh dương nhạt - Hoàn thành
        }

        public const string AdvanceRequestCodePrefix = "ARN-";
    }
}