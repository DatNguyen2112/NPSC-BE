using AutoMapper;
using FileManagement.Data;
using Newtonsoft.Json;
using NSPC.Business.AssetLiquidationSheet;
using NSPC.Business.AutoMapper.Profiles;
using NSPC.Business.Services;
using NSPC.Business.Services.AssetAllocation;
using NSPC.Business.Services.BangTinhLuong;
using NSPC.Business.Services.BHXH;
using NSPC.Business.Services.Business;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.CauHinhNhanSu;
using NSPC.Business.Services.ChamCong;
using NSPC.Business.Services.ChucVu;
using NSPC.Business.Services.Dashboard;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Business.Services.DuAn;
using NSPC.Business.Services.Feedback;
using NSPC.Business.Services.NhaCungCap;
using NSPC.Business.Services.NhomVatTu;
using NSPC.Business.Services.PhongBan;
using NSPC.Business.Services.ProjectTemplate;
using NSPC.Business.Services.QuanLyLaiXe;
using NSPC.Business.Services.QuanLyLoaiXe;
using NSPC.Business.Services.QuanLyPhuongTien;
using NSPC.Business.Services.Quotation;
using NSPC.Business.Services.StockTransaction;
using NSPC.Business.Services.TaskNotification;
using NSPC.Business.Services.TaskUsageHistory;
using NSPC.Business.Services.TemplateStage;
using NSPC.Business.Services.VatTu;
using NSPC.Business.Services.WorkingDay;
using NSPC.Business.Services.WorkItem;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data.Entity.ActivityHistory;
using NSPC.Data.Data.Entity.Asset;
using NSPC.Data.Data.Entity.AssetAllocation;
using NSPC.Data.Data.Entity.AssetCategories;
using NSPC.Data.Data.Entity.AssetHistory;
using NSPC.Data.Data.Entity.AssetLocation;
using NSPC.Data.Data.Entity.BangTinhLuong;
using NSPC.Data.Data.Entity.BHXH;
using NSPC.Data.Data.Entity.CashbookTransaction;
using NSPC.Data.Data.Entity.CauHinhNhanSu;
using NSPC.Data.Data.Entity.ChamCong;
using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data.Entity.CodeType;
using NSPC.Data.Data.Entity.DebtTransaction;
using NSPC.Data.Data.Entity.DuAn;
using NSPC.Data.Data.Entity.Feedback;
using NSPC.Data.Data.Entity.InventoryNote;
using NSPC.Data.Data.Entity.KiemKho;
using NSPC.Data.Data.Entity.NhaCungCap;
using NSPC.Data.Data.Entity.NhomVatTu;
using NSPC.Data.Data.Entity.PhongBan;
using NSPC.Data.Data.Entity.Quotation;
using NSPC.Data.Data.Entity.StockTransaction;
using NSPC.Data.Data.Entity.VatTu;
using NSPC.Data.Data.Entity.WorkingDay;
using NSPC.Data.Entity;
using SaleManagement.Data.Data.Entity.TaskHistory;

namespace NSPC.Business.AutoMapper
{
    public class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DatabaseTableToViewModelMapping());
                cfg.AddProfile(new ViewModelToDatabaseTableMapping());
                cfg.AddProfile(new SalesOrderMapping());
                cfg.AddProfile(new PurchaseOrderMapping());
                cfg.AddProfile(new CustomerReturnMapping());
                cfg.AddProfile(new SupplierReturnMapping());
                cfg.AddProfile(new InventoryNoteMapping());
                cfg.AddProfile(new IdmRightMapping());
                cfg.AddProfile(new IdmRightMapRoleMapping());
                cfg.AddProfile(new WarehouseTransferNoteMapping());
                cfg.AddProfile(new CheckInventoryNoteMapping());
                cfg.AddProfile(new BomMapping());
                cfg.AddProfile(new CustomerServiceCommentMapping());
                cfg.AddProfile(new EInvoiceMapping());
                cfg.AddProfile(new ConstructionMapping());
                cfg.AddProfile(new MaterialRequestMapping());
                cfg.AddProfile(new AdvanceRequestMapping());
                cfg.AddProfile(new ConstructionActivityLogMapping());
                cfg.AddProfile(new FeedbackMapping());
                cfg.AddProfile(new SocialMediaMapping());
                cfg.AddProfile(new VehicleRequestMapping());
                cfg.AddProfile(new InvestorMapping());
                cfg.AddProfile(new ContractMapping());
                cfg.AddProfile(new ConstructionWeekReportMapping());
            });
        }
    }
    public class DatabaseTableToViewModelMapping : Profile
    {
        public DatabaseTableToViewModelMapping()
        {
            // Pagination
            CreateMap(typeof(Pagination<>), typeof(Pagination<>));

            // Attachment
            CreateMap<erp_Attachment, AttachmentDetailViewModel>()
                .ForMember(dest => dest.FileName, x => x.MapFrom(src => Path.GetFileName(src.FilePath)))
                .ForMember(dest => dest.FileUrl, x => x.MapFrom(src => Utils.FetchHost(src.FilePath)));
            CreateMap<erp_Attachment, AttachmentViewModel>()
                .ForMember(dest => dest.FileUrl, x => x.MapFrom(src => Utils.FetchHost(src.FilePath)));
            CreateMap<jsonb_Attachment, AttachmentDetailViewModel>()
                .ForMember(dest => dest.FileUrl, x => x.MapFrom(src => Utils.FetchHost(src.FilePath)));
            CreateMap<jsonb_Attachment, AttachmentListViewModel>()
                .ForMember(dest => dest.FileUrl, x => x.MapFrom(src => Utils.FetchHost(src.FilePath)));
            CreateMap<jsonb_Attachment, AttachmentViewModel>()
                .ForMember(dest => dest.FileUrl, x => x.MapFrom(src => Utils.FetchHost(src.FilePath)));
            CreateMap<AttachmentViewModel, jsonb_Attachment>();

            CreateMap<BsdParameter, ParameterModel>();
            CreateMap<IdmRole, RoleModel>();
            CreateMap<IdmRole, RoleUpdateModel>();
            CreateMap<idm_User, UserModel>()
                //.ForMember(dest => dest.AvatarUrl, x => x.MapFrom(src => Utils.FetchHost(src.AvatarUrl)))
                .ForMember(dest => dest.ListRole,
                    x => x.MapFrom(src => RoleCollection.Instance.GetModelFromListCode(src.RoleListCode)))
                .ForMember(dest => dest.Role,
                    x => x.MapFrom(src =>
                        RoleCollection.Instance.GetModelFromListCode(src.RoleListCode).FirstOrDefault()))
                .ForMember(dest => dest.ChucVu, x => x.MapFrom(src => src.mk_ChucVu))
                .ForMember(dest => dest.PhongBan,
                    x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.MaPhongBan, LanguageConstants.Default, src.TenantId)))
                .ForMember(dest => dest.ToThucHien,
                    x => x.MapFrom(src => CodeTypeItemsCollection.Instance.FetchItemsCode(src.MaTo, LanguageConstants.Default, src.TenantId)));
            CreateMap<idm_User, BaseUserModel>()
                .ForMember(dest => dest.ChucVu, x => x.MapFrom(src => src.mk_ChucVu))
                .ForMember(dest => dest.PhongBan,
                    x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.MaPhongBan, LanguageConstants.Default, src.TenantId)))
                .ForMember(dest => dest.ToThucHien,
                    x => x.MapFrom(src => CodeTypeItemsCollection.Instance.FetchItemsCode(src.MaTo, LanguageConstants.Default, src.TenantId)));
            CreateMap<UserRegisterRequestModel, idm_User>();
            CreateMap<Pagination<idm_User>, Pagination<UserModel>>();
            CreateMap<Pagination<idm_User>, Pagination<UserModel>>();
            CreateMap<idm_User, UserFeedBackVIew>();

            CreateMap<sm_CodeType, CodeTypeViewModel>();
            CreateMap<sm_CodeType, CodeTypeListModel>();
            CreateMap<sm_CodeType_Item, CodeTypeItemViewModel>();
            CreateMap<cata_Province, CataProvinceModel>();
            CreateMap<cata_District, CataDistrictModel>();
            CreateMap<cata_Commune, CataCommueModel>();

            //KhachHang
            CreateMap<sm_Customer, KhachHangViewModel>()
                .ForMember(dest => dest.CustomerGroup, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.CustomerGroupCode, LanguageConstants.Default, src.TenantId)))
                // .ForMember(dest => dest.LichSuChamSoc, x => x.MapFrom(src => src.sm_LichSuChamSoc))
                // .ForMember(dest => dest.CustomerServiceComment, x => x.MapFrom(src => src.sm_CustomerServiceComment.OrderByDescending(x => x.CreatedOnDate)))
                .ForMember(dest => dest.LastCareOnDate, x => x.MapFrom(src => src.LastCareOnDate)); ;
            CreateMap<sm_LichSuChamSoc, LichSuChamSocViewModel>()
                .ForMember(dest => dest.ProjectId, x => x.MapFrom(src => src.ProjectId))
                .ForMember(dest => dest.Project, x => x.MapFrom(src => src.mk_DuAn));
            //PhongBan
            CreateMap<mk_PhongBan, PhongBanViewModel>();
            //ChucVu
            CreateMap<mk_ChucVu, ChucVuViewModel>();
            //NhomVatTu
            CreateMap<mk_NhomVatTu, NhomVatTuViewModel>();
            //VatTu
            CreateMap<sm_Product, VatTuViewModel>()
              .ForMember(dest => dest.AvatarUrl,
                    x => x.MapFrom(src => src.Attachments.Where(x => x.DocType == AttachmentDocTypeConstants.Avatar_Supplies).FirstOrDefault()))
            .ForMember(dest => dest.AttachmentUrl,
                    x => x.MapFrom(src => src.Attachments.Where(x => x.DocType == AttachmentDocTypeConstants.Attach).FirstOrDefault()))
            .ForMember(dest => dest.InitialStockQuantity, x => x.MapFrom(src => src.InitialStockQuantity))
            //NẾU NHOMVATTU: NULL THÌ MAPPING KHI GET TRẢ RA OBJECT NHOMVATTU
            .ForMember(dest => dest.NhomVatTu, x => x.MapFrom(src => src.mk_NhomVatTu))
            .ForMember(dest => dest.ProductGroupName, x => x.MapFrom(src => src.mk_NhomVatTu.TenNhom));
            //DuAn
            CreateMap<mk_DuAn, DuAnViewModel>()
            .ForMember(dest => dest.BaoGia, x => x.MapFrom(src => src.sm_Quotation.Where(x => x.Status == QuotationConstants.StatusCode.CUSTOMER_APPROVED).OrderBy(x => x.CreatedOnDate)))
            .ForMember(dest => dest.Thu, x => x.MapFrom(src => src.sm_Cashbook_Transaction
                .Where(x => x.TransactionTypeCode == CashbookTransactionConstants.ReceiptVoucherType
                            && x.IsActive == StatusCashbookTransaction.COMPLETED).OrderBy(x => x.CreatedOnDate)))
            .ForMember(dest => dest.Chi, x => x.MapFrom(src => src.sm_Cashbook_Transaction
                .Where(x => x.TransactionTypeCode == CashbookTransactionConstants.PaymentVoucherType
                            && x.IsActive == StatusCashbookTransaction.COMPLETED).OrderBy(x => x.CreatedOnDate)))
            .ForMember(dest => dest.PhieuNhapKho, x => x.MapFrom(src => src.sm_InventoryNote
                .Where(x => x.TypeCode == InventoryNoteConstants.TypeCode.InventoryImport && x.StatusCode == InventoryNoteConstants.StatusCode.COMPLETED).OrderBy(x => x.CreatedOnDate)))
            .ForMember(dest => dest.PhieuXuatKho, x => x.MapFrom(src => src.sm_InventoryNote
                .Where(x => x.TypeCode == InventoryNoteConstants.TypeCode.InventoryExport && x.StatusCode == InventoryNoteConstants.StatusCode.COMPLETED).OrderBy(x => x.CreatedOnDate)));
            //NhaCungCap
            CreateMap<sm_Supplier, NhaCungCapViewModel>()
                .ForMember(dest => dest.SupplierGroup,
                x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.SupplierGroupCode, LanguageConstants.Default, src.TenantId)));
            //ThuChi
            //SearchSample
            CreateMap<fm_Search_Sample, SearchSampleModel>()
           .ForMember(dest => dest.Query, x => x.MapFrom(src => JsonConvert.DeserializeObject<PurchaseOrderQueryModel>(src.QueryJsonString)));

            CreateMap<sm_Cashbook_Transaction, ListCashbookTransaction>();

            //KiemKho
            CreateMap<mk_KiemKho, KiemKhoViewModel>();
            CreateMap<mk_KiemKho, KiemKhoDetailViewModel>()
                .ForMember(dest => dest.StockInvetories, x => x.MapFrom(src => src.Sm_Stock_Transactions));

            CreateMap<sm_Stock_Transaction, StockInventoryViewModel>();
            CreateMap<ThongKeTonKhoViewModel, ThongKeTonKhoViewModel>()
                .ForMember(dest => dest.TenKho, x => x.MapFrom(x => CodeTypeCollection.Instance.FetchCode(x.MaKho, "vn", x.TenantId).Title));
            //NgayNghi
            CreateMap<mk_WorkingDay, WorkingDayViewModel>();
            //BHXH
            CreateMap<mk_BHXH, BHXHViewModel>();
            //ChamCongItem
            CreateMap<mk_ChamCongItem, ChamCongItemViewModel>();
            //ChamCong
            CreateMap<mk_ChamCong, ChamCongViewModel>()
            .ForMember(dest => dest.ListChamCong, x => x.MapFrom(src => src.ListChamCong));
            //NgayTrongThang
            CreateMap<NgayTrongThang, NgayTrongThangViewModel>();
            //BangLuongItem
            CreateMap<mk_BangLuongItem, BangTinhLuongItemViewModel>();
            //BangLuong
            CreateMap<mk_BangTinhLuong, BangTinhLuongViewModel>()
            .ForMember(dest => dest.BangLuongItem, x => x.MapFrom(src => src.BangLuongItem));
            //NgayTrongThang
            CreateMap<mk_CacKhoanTroCap, CacKhoanTroCapViewModel>();
            //CauHinhNhanSu
            CreateMap<mk_CauHinhNhanSu, CauHinhNhanSuViewModel>()
                .ForMember(dest => dest.Id, x => x.MapFrom(src => src.idm_User.Id))
                .ForMember(dest => dest.ChucVu, x => x.MapFrom(src => src.idm_User.mk_ChucVu))
                .ForMember(dest => dest.Ma, x => x.MapFrom(src => src.idm_User.Ma))
                .ForMember(dest => dest.TenNhanSu, x => x.MapFrom(src => src.idm_User.Name))
                // .ForMember(dest => dest.PhongBan, x => x.MapFrom(src => src.idm_User.mk_PhongBan))
                .ForMember(dest => dest.CreatedByUserName, x => x.MapFrom(src => src.idm_User.CreatedByUserName));
            //Cashbook_Transaction
            CreateMap<sm_Cashbook_Transaction, CashbookTransactionViewModel>()
                .ForMember(dest => dest.PaymentMethodName, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.PaymentMethodCode, "vn", src.TenantId).Title))
                .ForMember(dest => dest.PaymentMethodCode, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.PaymentMethodCode, "vn", src.TenantId).Code))
                .ForMember(dest => dest.Construction,
                    x => x.MapFrom(src => src.sm_Construction))
                .ForMember(dest => dest.AdvanceRequest,
                    x => x.MapFrom(src => src.sm_AdvanceRequest))
                .ForMember(dest => dest.ProjectName, x => x.MapFrom(src => src.Mk_DuAn.TenDuAn));
            CreateMap<sm_Cashbook_Transaction, CashbookTransactionViewModelInProject>();
            //QuotationItem
            CreateMap<sm_QuotationItem, QuotationItemViewModel>();
            CreateMap<sm_QuotationItem, QuotationViewModelInProject>();
            //StockTransaction
            CreateMap<sm_Stock_Transaction, StockTransactionViewModel>();
            CreateMap<sm_Stock_Transaction, StockHistoryViewModel>();
            //ProductInventory
            CreateMap<sm_ProductInventory, ProductInventoryViewModel>();
            CreateMap<sm_DebtTransaction, DebtTransactionReportViewModel>();

            //Quotation
            CreateMap<sm_Quotation, QuotationViewModel>()
            .ForMember(dest => dest.QuotationItem, x => x.MapFrom(src => src.QuotationItem))
            .ForMember(dest => dest.PaymentMethodName, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.PaymentMethodCode, "vn", src.TenantId).Title))
            .ForMember(dest => dest.Customer, x => x.MapFrom(src => src.sm_Customer));
            CreateMap<sm_Quotation, QuotationViewModelInProject>();
            CreateMap<sm_Customer, KhachHangViewModelInQuotation>();
            // ActivityHistory
            CreateMap<sm_ActiviyHisroty, ActivityHistoryViewModel>();

            // ActivityLogHistory
            CreateMap<sm_IssueActivityLog, IssueActivityLogViewModel>();
            // Assset
            CreateMap<sm_Asset, AssetViewModel>()
                .ForMember(dest => dest.AssetTypeName, opt => opt.MapFrom(src => src.AssetType != null ? src.AssetType.Name : ""))
                .ForMember(dest => dest.AssetLocationName, opt => opt.MapFrom(src => src.AssetLocation != null ? src.AssetLocation.Name : ""))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : ""))
                .ForMember(dest => dest.MeasureUnitName, opt => opt.MapFrom(src => src.MeasureUnit != null ? src.MeasureUnit.Name : ""))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.DepreciationUnit, opt => opt.MapFrom(src => src.DepreciationUnit.ToString()))
                .ForMember(dest => dest.AllowedOperations, opt => opt.MapFrom(src => AssetHandler.GetAllowedOperations(src.Status)));
            CreateMap<sm_AssetMaintenanceSheet, AssetMaintenanceSheetViewModel>()
                .ForMember(dest => dest.AssetName, x => x.MapFrom(src => src.Asset != null ? src.Asset.Name : ""))
                .ForMember(dest => dest.Status, x => x.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.MaintenanceType, x => x.MapFrom(src => src.MaintenanceType.ToString()))
                .ForMember(dest => dest.PerformerName, x => x.MapFrom(src => src.Performer != null ? src.Performer.Name : ""));
            CreateMap<sm_AssetLiquidationSheet, AssetLiquidationSheetViewModel>()
                .ForMember(dest => dest.LiquidatorName, x => x.MapFrom(src => src.Liquidator != null ? src.Liquidator.Name : ""))
                .ForMember(dest => dest.AssetName, x => x.MapFrom(src => src.Asset != null ? src.Asset.Name : ""));
            CreateMap<sm_AssetLocation, AssetLocationViewModel>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null))
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children));
            CreateMap<sm_AssetGroup, AssetGroupViewModel>();
            CreateMap<sm_AssetType, AssetTypeViewModel>()
                .ForMember(dest => dest.AssetGroupName, opt => opt.MapFrom(src => src.AssetGroup != null ? src.AssetGroup.Name : null));
            CreateMap<sm_MeasureUnit, MeasureUnitViewModel>();
            CreateMap<sm_AssetUsageHistory, AssetUsageHistoryViewModel>()
                .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.Name : ""))
                .ForMember(dest => dest.AssetCode, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.Code : ""))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location != null ? src.Location.Name : ""))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : ""))
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Name : src.CreatedByUserName))
                .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => src.Operation.ToString()))
                .ForMember(dest => dest.AssetStatus, opt => opt.MapFrom(src => src.AssetStatus.ToString()));
            CreateMap<sm_AssetAllocation, AssetAllocationViewModel>()
                .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.Name : ""))
                .ForMember(dest => dest.FromLocationName, opt => opt.MapFrom(src => src.FromLocation != null ? src.FromLocation.Name : ""))
                .ForMember(dest => dest.ToLocationName, opt => opt.MapFrom(src => src.ToLocation != null ? src.ToLocation.Name : ""))
                .ForMember(dest => dest.FromUserName, opt => opt.MapFrom(src => src.FromUser != null ? src.FromUser.Name : ""))
                .ForMember(dest => dest.ToUserName, opt => opt.MapFrom(src => src.ToUser != null ? src.ToUser.Name : ""));
            CreateMap<sm_Asset, AssetDetailViewModel>()
                .IncludeBase<sm_Asset, AssetViewModel>();
            //DebtTransaction
            CreateMap<sm_DebtTransaction, DebtTransactionViewModel>();
            CreateMap<sm_SalesOrder, DashboardForDataRevenueModel>()
                .ForMember(dest => dest.Code, x => x.MapFrom(src => src.OrderCode))
                .ForMember(dest => dest.Total, x => x.MapFrom(src => src.Total))
                .ForMember(dest => dest.Time, x => x.MapFrom(src => src.LastModifiedOnDate.Value.Date.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.Note, x => x.MapFrom(src => src.Note));

            CreateMap<Idm_Application, ApplicationModel>();
            CreateMap<Idm_Tenant, TenantModel>()
                .ForMember(dest => dest.Logo, x => x.MapFrom(src => src.Attachments.FirstOrDefault(x => x.DocType == AttachmentDocTypeConstants.Logo)))
                .ForMember(dest => dest.WebUrl, x => x.MapFrom(src => "http://" + src.SubDomain + "." + Utils.GetConfig("Authentication:Domain")));
            CreateMap<BsdNavigation, BsdNavigation>()
                .ForMember(dest => dest.Id, x => x.Ignore());
            CreateMap<Pagination<sm_AssetUsageHistory>, Pagination<AssetUsageHistoryViewModel>>();

            //Task Management
            CreateMap<sm_TaskManagement, sm_TaskManagement>()
                .ForMember(dest => dest.Id, x => x.Ignore());
            CreateMap<sm_TaskManagement, TaskManagementViewModel>()
                .ForMember(dest => dest.Assignees, x => x.MapFrom(src => src.sm_TaskManagementAssignees))
                .ForMember(dest => dest.Histories, x => x.MapFrom(src => src.sm_TaskManagementHistories))
                .ForMember(dest => dest.MileStones, x => x.MapFrom(src => src.sm_TaskManagementMileStones))
                .ForMember(dest => dest.Comments, x => x.MapFrom(src => src.sm_TaskManagementComments));
            //.ForMember(dest => dest.Type, x => x.MapFrom(src => CodeTypeCollection.Instance.FetchCode(src.Type, "vn", src.TenantId)));
            CreateMap<sm_TaskManagementAssignee, TaskManagementAssigneeViewModel>()
                .ForMember(dest => dest.UserName, x => x.MapFrom(src => src.idm_User.Name));

            CreateMap<sm_TaskManagementComment, TaskManagementCommentViewModel>()
                .ForMember(dest => dest.UserName, x => x.MapFrom(src => src.idm_User.Name));
            CreateMap<sm_TaskManagementHistory, TaskManagementHistoryViewModel>();
            CreateMap<sm_TaskManagementMileStone, TaskManagementMileStoneViewModel>();

            // Feedback
            CreateMap<sm_Feedback, FeedbackViewModel>();
            CreateMap<sm_Feedback, FeedbackCreateModel>();

            // IssueManagement
            CreateMap<sm_IssueManagement, sm_IssueManagement>();
            CreateMap<sm_IssueManagement, IssueManagementViewModel>()
                 .ForMember(dest => dest.User, x => x.MapFrom(src => src.Idm_User))
                 .ForMember(dest => dest.Construction, x => x.MapFrom(src => src.sm_Construction));
            CreateMap<sm_IssueManagement, IssueManagementCreateUpdateModel>();

            //PhuongTien
            CreateMap<sm_PhuongTien, PhuongTienViewModel>()
                .ForMember(dest => dest.TaiXe, x => x.MapFrom(src => src.TaiXe.TenTaiXe))
                .ForMember(dest => dest.IdTaiXe, x => x.MapFrom(src => src.TaiXe.Id));
            //LaiXe
            CreateMap<sm_LaiXe, LaiXeViewModel>()
                .ForMember(dest => dest.PhuongTien, x => x.MapFrom(src => src.PhuongTien.Model));
            //Kho
            CreateMap<sm_Kho, KhoViewModel>()
                .ForMember(dest => dest.KhachHang, x => x.Ignore())
                .ForMember(dest => dest.DiaChiFull,
                    x => x.MapFrom(src =>
                        Utils.TransformAddress(src.DiaChi, src.CommuneName, src.DistrictName, src.ProvinceName)));
            // Loại xe
            CreateMap<sm_LoaiXe, LoaiXeViewModel>();
            // Teamplate dự ấn
            CreateMap<sm_ProjectTemplate, ProjectTemplateViewModel>();
            CreateMap<sm_TemplateStage, TemplateStageViewModel>();
            // Công việc
            CreateMap<sm_Task, TaskViewModel>();
            CreateMap<sm_SubTask, SubTaskViewModel>();
            CreateMap<sm_TaskExecutor, TaskExecutorViewModel>();
            CreateMap<sm_TaskApprover, TaskApproverViewModel>();
            CreateMap<sm_SubTaskExecutor, SubTaskExecutorViewModel>();
            CreateMap<sm_TaskUsageHistory, TaskUsageHistoryViewModel>();
            CreateMap<sm_TaskNotification, TaskNotificationViewModel>()
                .ForMember(
                    dest => dest.Content,
                    x => x.MapFrom((src, dest) => TaskNotificationHandler.RenderNotificationContent(dest))
                );
            CreateMap<sm_TaskComment, TaskCommentViewModel>().ForMember(dest => dest.AvatarUrl, x => x.MapFrom(src => src.CreatedByUser.AvatarUrl));
            
            // Công việc
            CreateMap<sm_TaskPersonal, TaskPersonalViewModel>()
                .ForMember(dest => dest.TaskTypeModel,
                    x => x.MapFrom(src =>
                        CodeTypeCollection.Instance.FetchCode(src.TaskType, "vn", src.TenantId)))
                .ForMember(dest => dest.SubTaskPersonals,
                    x => x.MapFrom(src => src.SubTasksPersonal));
            CreateMap<sm_SubTaskPersonal, SubTaskPersonalViewModel>();
        }
    }
    public class ViewModelToDatabaseTableMapping : Profile
    {
        public ViewModelToDatabaseTableMapping()
        {
            CreateMap<ParameterCreateModel, BsdParameter>();
            // Attachments
            CreateMap<AttachmentCreateUpdateModel_V1, erp_Attachment>();

            // Profile

            CreateMap<AttachmentCreateUpdateModel_V1, erp_Attachment>();
            CreateMap<CodeTypeCreateUpdateModel, sm_CodeType>();
            CreateMap<CodeTypeItemCreateUpdateModel, sm_CodeType_Item>();

            //KhachHang
            CreateMap<KhachHangCreateUpdateModel, sm_Customer>();
            CreateMap<LichSuChamSocCreateUpdateModel, sm_LichSuChamSoc>();
            //PhongBan
            CreateMap<PhongBanCreateUpdateModel, mk_PhongBan>();
            //ChucVu
            CreateMap<ChucVuCreateUpdateModel, mk_ChucVu>();
            //NhomVatTu
            CreateMap<NhomVatTuCreateUpdateModel, mk_NhomVatTu>();
            //VatTu
            CreateMap<VatTuCreateUpdateModel, sm_Product>()
            .ForMember(dest => dest.InitialStockQuantity, x => x.MapFrom(src => src.InitialStockQuantity));
            //DuAn
            CreateMap<DuAnCreateUpdateModel, mk_DuAn>();
            //NhaCungCap
            CreateMap<NhaCungCapCreateUpdateModel, sm_Supplier>();
            //KiemKho
            CreateMap<KiemKhoCreateUpdateModel, mk_KiemKho>()
                .ForMember(dest => dest.Sm_Stock_Transactions, x => x.MapFrom(src => src.StockInvetories));
            CreateMap<StockTransactionCreateModel, sm_Stock_Transaction>();
            //BHXH
            CreateMap<BHXHCreateUpdateModel, mk_BHXH>();
            //ChamCongItem
            CreateMap<ChamCongItemCreateUpdateModel, mk_ChamCongItem>()
            .ForMember(dest => dest.NgayTrongThang, x => x.MapFrom(src => src.NgayTrongThang));
            //ChamCong
            CreateMap<ChamCongCreateUpdateModel, mk_ChamCong>()
            .ForMember(dest => dest.ListChamCong, x => x.MapFrom(src => src.ListChamCong));
            //NgayTrongThang
            CreateMap<NgayTrongThangCreateUpdateModel, NgayTrongThang>();
            //BangTinhLuongItem
            CreateMap<BangTinhLuongItemCreateUpdateModel, mk_BangLuongItem>()
            .ForMember(dest => dest.CacKhoanTroCap, x => x.MapFrom(src => src.CacKhoanTroCap));
            //BangTinhLuong
            CreateMap<BangTinhLuongCreateUpdateModel, mk_BangTinhLuong>()
            .ForMember(dest => dest.BangLuongItem, x => x.MapFrom(src => src.BangLuongItem));
            //CacKhoanTroCap
            CreateMap<CacKhoanTroCapCreateUpdateModel, mk_CacKhoanTroCap>();
            //CauHinhNhanSu
            CreateMap<CauHinhNhanSuCreateUpdateModel, mk_CauHinhNhanSu>();
            //Cashbook_Transaction
            CreateMap<CashbookTransactionCreateUpdateModel, sm_Cashbook_Transaction>();
            //QuotationItem
            CreateMap<QuotationItemCreateUpdateModel, sm_QuotationItem>();
            // Assset
            CreateMap<AssetCreateUpdateModel, sm_Asset>()
                .ForMember(dest => dest.DepreciationUnit, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.DepreciationUnit) ? DepreciationUnit.Month :
                    Enum.Parse<DepreciationUnit>(src.DepreciationUnit)));
            CreateMap<AssetMaintenanceSheetCreateUpdateModel, sm_AssetMaintenanceSheet>();
            CreateMap<AssetLiquidationSheetCreateUpdateModel, sm_AssetLiquidationSheet>();
            CreateMap<AssetLocationCreateUpdateModel, sm_AssetLocation>();
            CreateMap<AssetGroupCreateUpdateModel, sm_AssetGroup>();
            CreateMap<AssetTypeCreateUpdateModel, sm_AssetType>();
            CreateMap<MeasureUnitCreateUpdateModel, sm_MeasureUnit>();
            //Quotation
            CreateMap<QuotationCreateUpdateModel, sm_Quotation>()
            .ForMember(dest => dest.QuotationItem, x => x.MapFrom(src => src.QuotationItem));

            CreateMap<ApplicationCreateUpdateModel, Idm_Application>();
            CreateMap<TenantUpdateModel, Idm_Tenant>();
            CreateMap<TenantCreateModel, Idm_Tenant>();
            CreateMap<ListCashbookTransaction, sm_Cashbook_Transaction>();


            //Task Management
            CreateMap<TaskManagementCreateUpdateModel, sm_TaskManagement>();
            CreateMap<TaskManagementAssigneeCreateUpdateModel, sm_TaskManagementAssignee>();
            CreateMap<TaskManagementCommentCreateUpdateModel, sm_TaskManagementComment>();
            CreateMap<TaskManagementHistoryCreateUpdateModel, sm_TaskManagementHistory>();
            CreateMap<TaskManagementMileStoneCreateUpdateModel, sm_TaskManagementMileStone>();
            // Feedback
            CreateMap<FeedbackCreateModel, sm_Feedback>();
            CreateMap<FeedbackViewModel, sm_Feedback>();
            // IssueManagement
            CreateMap<IssueManagementViewModel, sm_IssueManagement>();
            CreateMap<IssueManagementCreateUpdateModel, sm_IssueManagement>();

            // IssueActivityLog
            CreateMap<IssueActivityLogViewModel, sm_IssueActivityLog>();
            CreateMap<IssueActivityLogCreateModel, sm_IssueActivityLog>();
            // Phương tiện
            CreateMap<PhuongTienCreateUpdateModel, sm_PhuongTien>();
            //Lái xe
            CreateMap<LaiXeCreateUpdateModel, sm_LaiXe>();
            //kho
            CreateMap<KhoCreateUpdateModel, sm_Kho>()
                .ForMember(dest => dest.DiaChi, x => x.MapFrom(src => src.DiaChi.Trim()));
            // Loại xe
            CreateMap<LoaiXeCreateUpdateModel, sm_LoaiXe>();
            // Teamplate dự án
            CreateMap<ProjectTemplateCreateUpdateModel, sm_ProjectTemplate>().ForMember(dest => dest.TemplateStages, x => x.MapFrom(src => src.TemplateStages));
            CreateMap<TemplateStageCreateUpdateModel, sm_TemplateStage>();
            // Công việc
            CreateMap<TaskCreateUpdateModel, sm_Task>();
            CreateMap<SubTaskCreateUpdateModel, sm_SubTask>();
            CreateMap<TaskCommentCreateModel, sm_TaskComment>();

            CreateMap<TaskPersonalCreateModel, sm_TaskPersonal>();
            CreateMap<SubTaskPersonalCreateModel, sm_SubTaskPersonal>();
        }
    }
}
