using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.ChucVu;
using NSPC.Business.Services.StockTransaction;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data.Entity.NguyenVatLieu;
using NSPC.Data.Data.Entity.StockTransaction;
using NSPC.Data.Data.Entity.VatTu;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.VatTu
{
    public class VatTuHandler : IVatTuHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;

        private readonly IAttachmentHandler _attachmentHandler;
        private readonly IStockTransactionHandler _stockTransactionHandler;
        private readonly IProductInventoryHandler _productInventoryHandler;

        public VatTuHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IAttachmentHandler attachmentHandler, IStockTransactionHandler stockTransactionHandler, IProductInventoryHandler productInventoryHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _attachmentHandler = attachmentHandler;
            _stockTransactionHandler = stockTransactionHandler;
            _productInventoryHandler = productInventoryHandler;
        }
        public async Task<Response<VatTuViewModel>> Create(VatTuCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                if (_dbContext.sm_Product.Any(x => x.Code == model.Code))
                    return Helper.CreateBadRequestResponse<VatTuViewModel>(string.Format("Mã vật tư {0} đã tồn tại!", model.Code));
                if (model.PurchaseUnitPrice < 0)
                    return Helper.CreateBadRequestResponse<VatTuViewModel>(string.Format("Giá nhập phải lớn hơn 0"));
                if (model.SellingUnitPrice < 0)
                    return Helper.CreateBadRequestResponse<VatTuViewModel>(string.Format("Giá bán phải lớn hơn 0"));
                if (model.InitialStockQuantity < 0)
                {
                    return Helper.CreateBadRequestResponse<VatTuViewModel>(
                        string.Format("Giá trị tồn kho không được nhỏ hơn 0"));
                }

                #region Validate ton kho khong duoc am
                if (model.ListWareCodes != null && model.ListWareCodes.Count > 0)
                {
                    if (model.ListWareCodes.Any(x => x.InitialStockQuantity < 0))
                    {
                        return Helper.CreateBadRequestResponse<VatTuViewModel>(string.Format("Khởi tạo tồn kho không được âm"));
                    }
                }
                #endregion

                var entity = _mapper.Map<sm_Product>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.ProductGroupId = model.ProductGroupId;
                entity.CreatedOnDate = DateTime.Now;
                entity.ListWareCodes = model.ListWareCodes;
                entity.SellableQuantity = model.ListWareCodes.Sum(x => x.SellableQuantity);

                #region Add List Warecodes Array 
                if (model.ListWareCodes != null && model.ListWareCodes.Count != 0)
                {
                    foreach (var item in model.ListWareCodes)
                    {
                        await _stockTransactionHandler.Create(new StockTransactionCreateModel
                        {
                            Id = item.Id,
                            ProductCode = entity.Code,
                            ProductName = entity.Name,
                            ProductId = entity.Id,
                            Unit = entity.Unit,
                            CreatedByUserId = entity.CreatedByUserId,
                            ReceiptInventoryQuantity = item.InitialStockQuantity,
                            ExportInventoryQuantity = 0,
                            InitialStockQuantity = item.InitialStockQuantity,
                            // SellableQuantity = item.InitialStockQuantity,
                            WareCode = item.WareCode,
                            Action = ActionConstants.INITIALIZE,
                            UnitPrice = entity.PurchaseUnitPrice,
                            OriginalDocumentType = OriginalDocumentTypeConstants.NHAP_HANG,
                        });
                        
                        // Tạo bản ghi ProductInventory
                        await _productInventoryHandler.Create(new ProductInventoryCreateModel
                        {
                            WarehouseCode = item.WareCode,
                            SellableQuantity = item.InitialStockQuantity,
                            ProductId = entity.Id,
                        }, currentUser);
                    }
                }
                else
                {
                    await _stockTransactionHandler.Create(new StockTransactionCreateModel
                    {
                        ProductCode = entity.Code,
                        ProductName = entity.Name,
                        ProductId = entity.Id,
                        CreatedByUserId = entity.CreatedByUserId,
                        ReceiptInventoryQuantity = 0,
                        ExportInventoryQuantity = 0,
                        InitialStockQuantity = 0,
                        // SellableQuantity = 0,
                        Action = ActionConstants.INITIALIZE,
                        UnitPrice = entity.PurchaseUnitPrice,
                        OriginalDocumentType = OriginalDocumentTypeConstants.NHAP_HANG,
                    });
                    
                    // Tạo bản ghi ProductInventory
                    await _productInventoryHandler.Create(new ProductInventoryCreateModel
                    {
                        SellableQuantity = 0,
                        ProductId = entity.Id,
                    }, currentUser);
                }
                #endregion
                
                #region Tính toán tổng SL có thể bán và Tồn tổng của tất cả các kho
                entity.SellableQuantity = model.ListWareCodes.Sum(x => x.SellableQuantity);
                entity.InitialStockQuantity = model.ListWareCodes.Sum(x => x.InitialStockQuantity);
                #endregion

                _dbContext.sm_Product.Add(entity);

                await _dbContext.SaveChangesAsync();
                
                if (model.Attachments != null)
                {
                    /* Attachment Process */
                    await processAttachment(entity.Id, model.Attachments);
                }
                
                return Helper.CreateSuccessResponse(_mapper.Map<VatTuViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<VatTuViewModel>(ex);
            }
        }

        public async Task<Response<VatTuViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<VatTuViewModel>(string.Format("Vật tư không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<VatTuViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<VatTuViewModel>(ex);
            }
        }

        public async Task<Response> DeleteMultiple(List<Guid> ids)
        {
            try
            {
                var entities = _dbContext.sm_Product.Where(x => ids.Contains(x.Id)).ToList();
                if (entities == null || entities.Count == 0)
                    return Helper.CreateBadRequestResponse("Các sản phẩm vừa chọn đã bị xóa hoặc không tồn tại trong cơ sở dữ liệu");

                _dbContext.RemoveRange(entities);
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse(ex);

            }
        }

        public async Task<Response<VatTuViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Product.AsNoTracking().Include(x => x.mk_NhomVatTu).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<VatTuViewModel>("Vật tư không tồn tại trong hệ thống!");

                var result = _mapper.Map<VatTuViewModel>(entity);

                return new Response<VatTuViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<VatTuViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<VatTuViewModel>>> GetPage(VatTuQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_Product.Include(x => x.mk_NhomVatTu).AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);
                var result = _mapper.Map<Pagination<VatTuViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<VatTuViewModel>>(ex);
            }
        }
        private Expression<Func<sm_Product, bool>> BuildQuery(VatTuQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            
            var predicate = PredicateBuilder.New<sm_Product>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Type.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.Type))
                predicate.And(s => s.Type == query.Type);
            if (!string.IsNullOrEmpty(query.TenVatTu))
                predicate.And(s => s.Name == query.TenVatTu); 
            if (!string.IsNullOrEmpty(query.MaVatTu))
                predicate.And(s => s.Code == query.MaVatTu);
            if (query.IsOrder.HasValue)
            {
                predicate.And(s => s.IsOrder == query.IsOrder);

            }
            if (query.IsActive.HasValue)
            {
                predicate.And(s => s.IsActive == query.IsActive);

            }

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date);

            }
            
            if (!currentUser.ListRights.Contains("VATTU." + RightActionConstants.VIEWALL))
                predicate.And(s => s.CreatedByUserId == currentUser.UserId);
            

            return predicate;
        }

        public async Task<Response<VatTuViewModel>> Update(Guid id, VatTuCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<VatTuViewModel>(string.Format("Vật tư không tồn tại trong hệ thống!"));
                if (_dbContext.sm_Product.Any(x => x.Code == model.Code && x.Id != id))
                    return Helper.CreateBadRequestResponse<VatTuViewModel>(string.Format("Mã vật tư {0} đã tồn tại!", model.Code));
                if (model.PurchaseUnitPrice < 0)
                    return Helper.CreateBadRequestResponse<VatTuViewModel>(string.Format("Giá nhập phải lớn hơn 0"));
                if (model.SellingUnitPrice < 0)
                    return Helper.CreateBadRequestResponse<VatTuViewModel>(string.Format("Giá bán phải lớn hơn 0"));
                if (model.InitialStockQuantity < 0)
                {
                    return Helper.CreateBadRequestResponse<VatTuViewModel>(
                        string.Format("Giá trị tồn kho không được nhỏ hơn 0"));
                }

                #region Validate ton kho khong duoc am
                if (model.ListWareCodes != null && model.ListWareCodes.Count > 0)
                {
                    if (model.ListWareCodes.Any(x => x.InitialStockQuantity < 0))
                    {
                        return Helper.CreateBadRequestResponse<VatTuViewModel>(string.Format("Khởi tạo tồn kho không được âm"));
                    }
                }
                #endregion
                
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.Type = model.Type;
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Code = model.Code;
                entity.ProductGroupId = model.ProductGroupId;
                entity.Unit = model.Unit;
                entity.PurchaseUnitPrice = model.PurchaseUnitPrice;
                entity.SellingUnitPrice = model.SellingUnitPrice;
                entity.IsVATApplied = model.IsVATApplied;
                entity.ExportVATPercent = model.ExportVATPercent;
                entity.ImportVATPercent = model.ImportVATPercent;
                entity.IsOrder = model.IsOrder;
                entity.Barcode = model.Barcode;
                entity.SellableQuantity = model.ListWareCodes.Sum(x => x.SellableQuantity);

                if (entity.ListWareCodes != null && entity.ListWareCodes.Count > 0)
                {
                    entity.InitialStockQuantity = entity.ListWareCodes.Sum(x => x.InitialStockQuantity);
                }
                else
                {
                    var stockTransactionEntity =
                        await _dbContext.sm_Stock_Transaction
                            .OrderByDescending(x => x.CreatedOnDate)
                            .FirstOrDefaultAsync(x => x.ProductId == entity.Id);
                    
                    var allStockTransactionEntity =
                        await _dbContext.sm_Stock_Transaction
                            .OrderByDescending(x => x.CreatedOnDate)
                            .Where(x => x.ProductId == entity.Id)
                            .ToListAsync();

                    if (allStockTransactionEntity != null && stockTransactionEntity != null)
                    {
                        entity.InitialStockQuantity = stockTransactionEntity.InitialStockQuantity ?? 0;
                    }
                }
                 
                

                await _dbContext.SaveChangesAsync();
                
                if (model.Attachments != null && model.Attachments.Count() > 0 )
                {
                    /* Attachment Process */
                    await processAttachment(entity.Id, model.Attachments);
                }
                else
                {
                    entity.Attachments = new List<jsonb_Attachment> {};
                    await _dbContext.SaveChangesAsync();
                }
                return Helper.CreateSuccessResponse(_mapper.Map<VatTuViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<VatTuViewModel>(ex);
            }
        }
        private async Task processAttachment(Guid vatTuId, List<jsonb_Attachment> attachments)
        {
            try
            {
                var attachmentListId = attachments.Select(x => x.Id).ToList();
                // Process attachments
                if (attachmentListId.Count > 0)
                {
                    var newVatTu =
                        await _dbContext.sm_Product.Where(x => x.Id == vatTuId).FirstOrDefaultAsync();
                    var allAttachments = await _dbContext.erp_Attachment.Where(x => attachmentListId.Contains(x.Id))
                        .ToListAsync();

                    foreach (var att in allAttachments)
                    {
                        // UpdatePaid entity
                        att.EntityId = newVatTu.Id;
                        att.EntityType = attachments.Where(x => x.Id == att.Id).FirstOrDefault()?.DocType;
                        att.Description = attachments.Where(x => x.Id == att.Id).FirstOrDefault()?.Description;

                        // Move files to new folder
                        var moveFileResult = _attachmentHandler.MoveEntityAttachment(att.DocType, att.EntityType,
                            newVatTu.Id, att.FilePath, newVatTu.CreatedOnDate);
                        if (moveFileResult.IsSuccess)
                            att.FilePath = moveFileResult.Data;
                    }

                    if (allAttachments != null && allAttachments.Count() > 0)
                        newVatTu.Attachments = allAttachments.Select(x => new jsonb_Attachment
                        {
                            Description = x.Description,
                            DocType = x.DocType,
                            FilePath = x.FilePath,
                            //Name = x.Name,
                            Id = x.Id
                        }).ToList();
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }
    }
}
