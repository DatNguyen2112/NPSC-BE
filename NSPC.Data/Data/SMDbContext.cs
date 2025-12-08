using FileManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using NSPC.Common;
using NSPC.Data.Data.Entity.ActivityHistory;
using NSPC.Data.Data.Entity.AdvanceRequest;
using NSPC.Data.Data.Entity.Asset;
using NSPC.Data.Data.Entity.AssetAllocation;
using NSPC.Data.Data.Entity.AssetCategories;
using NSPC.Data.Data.Entity.AssetHistory;
using NSPC.Data.Data.Entity.AssetIncident;
using NSPC.Data.Data.Entity.AssetLocation;
using NSPC.Data.Data.Entity.BangTinhLuong;
using NSPC.Data.Data.Entity.BHXH;
using NSPC.Data.Data.Entity.Bom;
using NSPC.Data.Data.Entity.CashbookTransaction;
using NSPC.Data.Data.Entity.CauHinhNhanSu;
using NSPC.Data.Data.Entity.ChamCong;
using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data.Entity.CodeType;
using NSPC.Data.Data.Entity.Contract;
using NSPC.Data.Data.Entity.DebtTransaction;
using NSPC.Data.Data.Entity.DuAn;
using NSPC.Data.Data.Entity.EInvoice;
using NSPC.Data.Data.Entity.Feedback;
using NSPC.Data.Data.Entity.InventoryNote;
using NSPC.Data.Data.Entity.KiemKho;
using NSPC.Data.Data.Entity.NguyenVatLieu;
using NSPC.Data.Data.Entity.NhaCungCap;
using NSPC.Data.Data.Entity.NhomVatTu;
using NSPC.Data.Data.Entity.PhongBan;
//using NSPC.Data.Data.Entity.QuanLyKho;
using NSPC.Data.Data.Entity.Quotation;
using NSPC.Data.Data.Entity.StockTransaction;
using NSPC.Data.Data.Entity.VatTu;
using NSPC.Data.Data.Entity.VehicleRequest;
using NSPC.Data.Data.Entity.WorkingDay;
using NSPC.Data.Data.Entity.XuatNhapTon;
using NSPC.Data.Entity;
using OpenData.Common;
using SaleManagement.Data.Data.Entity.TaskHistory;
using System.Linq.Expressions;
using NSPC.Data.Data.Entity.Configuration;

namespace NSPC.Data.Data
{
    public class SMDbContext : DbContext
    {

        private string connectionString;

        private static readonly ILoggerFactory loggerFactory = new LoggerFactory(new[] {
              new DebugLoggerProvider()
        });
        private readonly ITenantProvider _tenantProvider;
        public Guid? TenantId;
        public SMDbContext()
        {
            connectionString = Utils.GetConfig("ConnectionStrings:PostgreSQLDatabase");
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public SMDbContext(ITenantProvider tenantProvider)
        {
            connectionString = Utils.GetConfig("ConnectionStrings:PostgreSQLDatabase");
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            _tenantProvider = tenantProvider;
            TenantId = tenantProvider.GetTenantId();
        }
        public virtual DbSet<BsdParameter> BsdParameter { get; set; }
        public virtual DbSet<BsdNavigation> BsdNavigation { get; set; }
        public virtual DbSet<BsdNavigationMapRole> BsdNavigationMapRole { get; set; }
        public virtual DbSet<bsd_KeyValue> bsd_KeyValue { get; set; }

        public virtual DbSet<erp_Attachment> erp_Attachment { get; set; }

        // Notification
        public virtual DbSet<sm_Notification> sm_Notification { get; set; }
        public virtual DbSet<sm_Notification_Template> sm_Notification_Template { get; set; }
        public virtual DbSet<sm_Notification_Template_Translation> sm_Notification_Template_Translation
        {
            get;
            set;
        }

        // IDM
        public virtual DbSet<idm_User> IdmUser { get; set; }

        public virtual DbSet<IdmRole> IdmRole { get; set; }

        public virtual DbSet<IdmRight> IdmRight { get; set; }
        public virtual DbSet<IdmRightMapRole> IdmRightMapRole { get; set; }

        // Email
        public virtual DbSet<sm_Email_Subscribe> sm_Email_Subscribe { get; set; }
        public virtual DbSet<sm_Email_Verification> sm_Email_Verifications { get; set; }
        public virtual DbSet<sm_Email_Template> sm_Email_Templates { get; set; }
        public virtual DbSet<sm_Email_Template_Translation> sm_Email_Template_Translations { get; set; }

        // Cata
        public virtual DbSet<cata_District> cata_District { get; set; }
        public virtual DbSet<cata_Province> cata_Province { get; set; }
        public virtual DbSet<cata_Commune> cata_Commune { get; set; }
        // Code Type
        public virtual DbSet<sm_CodeType> sm_CodeType { get; set; }
        public virtual DbSet<sm_CodeType_Item> sm_CodeType_Item { get; set; }
        public virtual DbSet<sm_CodeType_Translation> sm_CodeType_Translation { get; set; }

        public virtual DbSet<sm_ActiviyHisroty> sm_ActiviyHisroty { get; set; }

        public virtual DbSet<sm_Customer> sm_Customer { get; set; }

        // SanPham
        public virtual DbSet<sm_Product> sm_Product { get; set; }
        public virtual DbSet<sm_LichSuChamSoc> sm_LichSuChamSoc { get; set; }

        //PhongBan
        public virtual DbSet<mk_PhongBan> mk_PhongBan { get; set; }
        //ChucVu
        public virtual DbSet<mk_ChucVu> mk_ChucVu { get; set; }
        //NhomVatTu
        public virtual DbSet<mk_NhomVatTu> mk_NhomVatTu { get; set; }
        //NguyenVatLieu
        public virtual DbSet<mk_NguyenVatLieu> mk_NguyenVatLieu { get; set; }
        //Bom
        public virtual DbSet<mk_Bom> mk_Bom { get; set; }
        //VatTu
        //public virtual DbSet<mk_VatTu> mk_VatTu { get; set; }
        //DuAn
        public virtual DbSet<mk_DuAn> mk_DuAn { get; set; }
        //NhaCungCap
        public virtual DbSet<sm_Supplier> sm_Supplier { get; set; }

        // Nhap hang
        public virtual DbSet<sm_PurchaseOrder> sm_PurchaseOrder { get; set; }
        public virtual DbSet<sm_PurchaseOrderItem> sm_PurchaseOrderItem { get; set; }
        //ThuChi
        public virtual DbSet<mk_XuatNhapTon> mk_XuatNhapTon { get; set; }
        public virtual DbSet<mk_KiemKho> mk_KiemKho { get; set; }

        //SearchSample
        public virtual DbSet<fm_Search_Sample> fm_Search_Sample { get; set; }

        //NgayNghi
        public virtual DbSet<mk_WorkingDay> mk_WorkingDay { get; set; }
        //BHXH
        public virtual DbSet<mk_BHXH> mk_BHXH { get; set; }
        //ChamCong
        public virtual DbSet<mk_ChamCong> mk_ChamCong { get; set; }
        //BangLuong
        public virtual DbSet<mk_BangTinhLuong> mk_BangTinhLuong { get; set; }
        //CauHinhNhanSu
        public virtual DbSet<mk_CauHinhNhanSu> mk_CauHinhNhanSu { get; set; }
        //Cashbook_Transaction
        public virtual DbSet<sm_Cashbook_Transaction> sm_Cashbook_Transaction { get; set; }
        //Stock_Transaction
        public virtual DbSet<sm_Stock_Transaction> sm_Stock_Transaction { get; set; }
        //PurchaseOrder
        public virtual DbSet<sm_SalesOrder> sm_SalesOrder { get; set; }
        //PurchaseOrderItem
        public virtual DbSet<sm_SalesOrderItem> sm_SalesOrderItem { get; set; }
        //BaoGia
        public virtual DbSet<sm_Quotation> sm_Quotation { get; set; }
        public virtual DbSet<sm_QuotationItem> Sm_QuotationItems { get; set; }
        //DebtTransaction
        public virtual DbSet<sm_DebtTransaction> sm_DebtTransaction { get; set; }
        // Configuration
        public virtual DbSet<sm_Configuration> sm_Configuration { get; set; }

        //OrderReturn
        public virtual DbSet<sm_Return_Order> sm_Return_Order { get; set; }

        //OrderReturnItem
        public virtual DbSet<sm_Return_Order_Item> sm_Return_Order_Item { get; set; }

        // InventoryNote
        public virtual DbSet<sm_InventoryNote> sm_InventoryNote { get; set; }
        public virtual DbSet<sm_InventoryNoteItem> sm_InventoryNoteItem { get; set; }

        // InventoryCheckNote
        public virtual DbSet<sm_InventoryCheckNote> sm_InventoryCheckNote { get; set; }

        // InventoryCheckNoteItem
        public virtual DbSet<sm_InventoryCheckNoteItems> sm_InventoryCheckNoteItems { get; set; }

        // WarehouseTransferNote
        public virtual DbSet<sm_WarehouseTransferNote> sm_WarehouseTransferNote { get; set; }

        // InventoryCheckNoteItem
        public virtual DbSet<sm_WarehouseTransferNoteItem> sm_WarehouseTransferNoteItem { get; set; }

        // ProductInventory (Dùng để check số lượng có thể bán)
        public virtual DbSet<sm_ProductInventory> sm_ProductInventory { get; set; }

        // Bom
        public virtual DbSet<sm_Bom> sm_Bom { get; set; }
        // Meterials
        public virtual DbSet<sm_Materials> sm_Materials { get; set; }

        // CustomerServiceComment
        public virtual DbSet<sm_CustomerServiceComment> sm_CustomerServiceComment { get; set; }

        // Asset Management
        public virtual DbSet<sm_Asset> sm_Asset { get; set; }
        public virtual DbSet<sm_AssetLocation> sm_AssetLocation { get; set; }
        public virtual DbSet<sm_AssetMaintenanceSheet> sm_AssetMaintenanceSheet { get; set; }
        public virtual DbSet<sm_AssetLiquidationSheet> sm_AssetLiquidationSheet { get; set; }
        public virtual DbSet<sm_AssetGroup> sm_AssetGroup { get; set; }
        public virtual DbSet<sm_AssetType> sm_AssetType { get; set; }
        public virtual DbSet<sm_MeasureUnit> sm_MeasureUnit { get; set; }
        public virtual DbSet<sm_AssetUsageHistory> sm_AssetUsageHistory { get; set; }
        public virtual DbSet<sm_AssetIncident> sm_AssetIncident { get; set; }
        public virtual DbSet<sm_AssetAllocation> sm_AssetAllocation { get; set; }

        // EInvoice (Hóa đơn điện tử)
        public virtual DbSet<sm_EInvoice> sm_EInvoice { get; set; }
        public virtual DbSet<sm_EInvoiceItems> sm_EInvoiceItems { get; set; }
        public virtual DbSet<sm_EInvoiceVatAnalytics> sm_EInvoiceVatAnalytics { get; set; }
        // Tenant
        public virtual DbSet<Idm_Tenant> Idm_Tenants { get; set; }
        //   public virtual DbSet<Idm_Application> Idm_Applications { get; set; }

        // Construction
        public virtual DbSet<sm_Construction> sm_Construction { get; set; }

        // Construction Activity Log 
        public virtual DbSet<sm_ConstructionActivityLog> sm_ConstructionActivityLog { get; set; }

        // Contract
        public virtual DbSet<sm_Contract> sm_Contract { get; set; }

        // Material Request
        public virtual DbSet<sm_MaterialRequest> sm_MaterialRequest { get; set; }

        // Material Request Item
        public virtual DbSet<sm_MaterialRequestItem> sm_MaterialRequestItem { get; set; }

        // Construction Item
        public virtual DbSet<sm_ConstructionItems> sm_ConstructionItems { get; set; }
        // Feedback
        public virtual DbSet<sm_Feedback> sm_Feedback { get; set; }

        // Yêu cầu tạm ứng
        public virtual DbSet<sm_AdvanceRequest> sm_AdvanceRequest { get; set; }
        public virtual DbSet<sm_AdvanceRequestItems> sm_AdvanceRequestItems { get; set; }

        // Task Management
        public virtual DbSet<sm_TaskManagement> sm_TaskManagement { get; set; }
        public virtual DbSet<sm_TaskManagementAssignee> sm_TaskManagementAssignee { get; set; }
        public virtual DbSet<sm_TaskManagementComment> sm_TaskManagementComment { get; set; }
        public virtual DbSet<sm_TaskManagementHistory> sm_TaskManagementHistory { get; set; }
        public virtual DbSet<sm_TaskManagementMileStone> sm_TaskManagementMileStone { get; set; }

        // Phương tiện
        public virtual DbSet<sm_PhuongTien> sm_PhuongTien { get; set; }
        public virtual DbSet<sm_Kho> sm_Kho { get; set; }
        public virtual DbSet<sm_LaiXe> sm_LaiXe { get; set; }
        public virtual DbSet<sm_LoaiXe> sm_LoaiXe { get; set; }

        // Vehicle Request
        public virtual DbSet<sm_VehicleRequest> sm_VehicleRequest { get; set; }

        // Social Media
        public virtual DbSet<sm_SocialMediaPost> sm_SocialMediaPost { get; set; }
        public virtual DbSet<sm_Comments> sm_Comments { get; set; }
        public virtual DbSet<sm_CommentItems> sm_CommentItems { get; set; }
        // sm_SocialPost
        public virtual DbSet<sm_SocialPost> sm_SocialPost { get; set; }
        // sm_SocialComment
        public virtual DbSet<sm_SocialComment> sm_SocialComment { get; set; }

        // Vướng mắc
        public virtual DbSet<sm_IssueManagement> sm_IssueManagement { get; set; }


        // Lịch sử vướng mắc
        public virtual DbSet<sm_IssueActivityLog> sm_IssueActivityLog { get; set; }

        // Template dự án
        public virtual DbSet<sm_ProjectTemplate> sm_ProjectTemplate { get; set; }
        public virtual DbSet<sm_TemplateStage> sm_TemplateStage { get; set; }

        // Execution teams (Tổ thực hiện)
        public virtual DbSet<sm_ExecutionTeams> sm_ExecutionTeams { get; set; }

        // Chủ đầu tư/ Loại chủ đầu tư
        public virtual DbSet<sm_Investor> sm_Investor { get; set; }
        public virtual DbSet<sm_InvestorType> sm_InvestorType { get; set; }

        // Báo cáo tuần trong dự án
        public virtual DbSet<sm_ConstructionWeekReport> sm_ConstructionWeekReport { get; set; }

        // Quản lý công việc
        public virtual DbSet<sm_Task> sm_Task { get; set; }
        public virtual DbSet<sm_SubTask> sm_SubTask { get; set; }
        public virtual DbSet<sm_TaskApprover> sm_TaskApprover { get; set; }
        public virtual DbSet<sm_TaskExecutor> sm_TaskExecutor { get; set; }
        public virtual DbSet<sm_TaskUsageHistory> sm_TaskUsageHistory { get; set; }
        public virtual DbSet<sm_SubTaskExecutor> sm_SubTaskExecutor { get; set; }
        public virtual DbSet<sm_TaskComment> sm_TaskComment { get; set; }
        public virtual DbSet<sm_TaskNotification> sm_TaskNotification { get; set; }
        
        public virtual DbSet<sm_TaskPersonal> sm_TaskPersonal { get; set; }
        public virtual DbSet<sm_SubTaskPersonal> sm_SubTaskPersonal { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(connectionString);
                optionsBuilder.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
            }
            optionsBuilder.UseLoggerFactory(loggerFactory).EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("AssetSequence")
                .StartsAt(1)
                .IncrementsBy(1);
            modelBuilder.Entity<sm_Asset>()
                .Property(x => x.Code)
                .HasDefaultValueSql("CONCAT('AN-', LPAD(NEXTVAL('\"AssetSequence\"')::text, 5, '0'))");

            modelBuilder.HasSequence<int>("AssetMaintenanceSheetSequence")
                .StartsAt(1)
                .IncrementsBy(1);
            modelBuilder.Entity<sm_AssetMaintenanceSheet>()
                .Property(x => x.Code)
                .HasDefaultValueSql("CONCAT('MSN-', LPAD(NEXTVAL('\"AssetMaintenanceSheetSequence\"')::text, 5, '0'))");

            modelBuilder.HasSequence<int>("AssetLiquidationSheetSequence")
                .StartsAt(1)
                .IncrementsBy(1);
            modelBuilder.Entity<sm_AssetLiquidationSheet>()
                .Property(x => x.Code)
                .HasDefaultValueSql("CONCAT('LSN-', LPAD(NEXTVAL('\"AssetLiquidationSheetSequence\"')::text, 5, '0'))");

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Cascade;

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                 .SelectMany(t => t.GetProperties())
                 .Where
                 (p
                   => p.ClrType == typeof(DateTime)
                      || p.ClrType == typeof(DateTime?)
                 ))
            {
                property.SetColumnType("timestamp without time zone");
            }
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {

                var entityClass = entityType.ClrType;
                var tenantProperty = entityClass.GetProperty("TenantId");

                if (tenantProperty != null && tenantProperty.PropertyType == typeof(Guid?))
                {
                    var parameter = Expression.Parameter(entityClass, "e");
                    var property = Expression.Property(parameter, tenantProperty);
                    var tenant = Expression.Constant(TenantId, typeof(Guid?));

                    var filter = Expression.Lambda(Expression.Equal(property, tenant), parameter);
                    modelBuilder.Entity(entityClass).HasQueryFilter(filter);
                }
            }
        }

        public override int SaveChanges()
        {
            SetTenantId();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetTenantId();
            return base.SaveChangesAsync(cancellationToken);
        }


        private void SetTenantId()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    var entity = entry.Entity;
                    var tenantProperty = entity.GetType().GetProperty("TenantId");

                    if (tenantProperty != null && tenantProperty.PropertyType == typeof(Guid?))
                    {
                        var currentValue = (Guid?)tenantProperty.GetValue(entity);
                        if (!currentValue.HasValue || currentValue == Guid.Empty)
                        {
                            tenantProperty.SetValue(entity, TenantId);
                        }
                    }
                }
            }
        }
    }


    public class TenantModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
        {
            var tenantContext = context as SMDbContext;
            return (context.GetType(), tenantContext?.TenantId, designTime);
        }
    }
}
