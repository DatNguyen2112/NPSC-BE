using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Business.Services.Quotation;
using NSPC.Business.Services.StockTransaction;
using NSPC.Business.Services.VatTu;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.InventoryNote;
using NSPC.Data.Data.Entity.Quotation;
using NSPC.Data.Data.Entity.VatTu;
using NSPC.Data.Entity;
using Serilog;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public class InventoryCheckNoteHandler : IInventoryCheckNoteHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IStockTransactionHandler _stockTransactionHandler;
        private readonly string _staticsFolder;

        public InventoryCheckNoteHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IStockTransactionHandler stockTransactionHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _stockTransactionHandler = stockTransactionHandler;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
        }

        /// <summary>
        /// Tạo phiếu kiểm
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<InventoryCheckNoteViewModel>> Create(InventoryCheckNoteCreateUpdateModel model,
            RequestUser currentUser)
        {
            try
            {
                // Validate: Mã phiếu kiểm đã tồn tại
                var checkCode = await _dbContext.sm_InventoryCheckNote.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.OrderCode == model.OrderCode);
                if (checkCode != null)
                    return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>
                        (string.Format("Mã {0} đã tồn tại!", model.OrderCode));

                List<sm_Product> allInventoryNoteCheckItems = new List<sm_Product>();

                #region Validated

                // Validate when Items is Empty
                if (model.Items == null)
                {
                    return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>("Vui lòng chọn sản phẩm");
                }
                
                if (model.Items != null && model.Items.Count > 0)
                {
                    // Fill tất cả products
                    var allProductIds = model.Items.Select(x => x.ProductId).ToList();
                    allInventoryNoteCheckItems = await _dbContext.sm_Product.AsNoTracking()
                        .Where(x => allProductIds.Contains(x.Id))
                        .ToListAsync();

                    // Validate: Không được duplicate dòng trong Product Item
                    if (model.Items.Count !=
                        model.Items.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>
                            ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.Items.Any(x => x.ActualQuantity < 0))
                        return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>
                            ("Danh sách sản phẩm không được có tồn thực tế nhỏ hơn 0.");

                    foreach (var item in model.Items)
                    {
                        var product = allInventoryNoteCheckItems
                            .FirstOrDefault(x => x.Id == item.ProductId);
                        if (product == null)
                            return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>
                                ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                    }

                }

                #endregion

                #region Fill Item's Line Number

                model.Items = model.Items.OrderBy(x => x.LineNo).ToList();
                for (int i = 0; i < model.Items.Count; i++)
                    model.Items[i].LineNo = i + 1;

                #endregion

                // Mapping và tạo entity
                var entity = _mapper.Map<sm_InventoryCheckNote>(model);
                entity.Id = Guid.NewGuid();
                
                if (model.OrderCode == null)
                {
                    entity.OrderCode = await GetNewCode(InventoryCheckNoteConstants.InventoryCheckNoteCodePrefix);
                }

                entity.StatusCode = InventoryCheckNoteConstants.StatusCode.DRAFT;
                entity.CreatedOnDate = DateTime.Now;
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.WareName = _dbContext.sm_CodeType.FirstOrDefault(x => x.Code == model.WareCode)?.Title;

                #region Tính chênh lệch giữa tồn thực tế và tồn kho

                if (entity.Items != null && entity.Items.Count > 0)
                {
                    foreach (var item in entity.Items)
                    {
                        var product = allInventoryNoteCheckItems.FirstOrDefault(x => x.Id == item.ProductId);
                        var allStockTransaction =
                            await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId, model.WareCode);
                        var productInventoryEntity = _dbContext.sm_ProductInventory
                            .FirstOrDefault(x => x.ProductId == item.ProductId && x.WarehouseCode == entity.WareCode); 

                        if (allStockTransaction.Data != null && allStockTransaction.Data.Count > 0)
                        {
                            if (product != null)
                            {
                                item.RecordedQuantity = allStockTransaction.Data[0].StockTransactionQuantity ?? 0;
                                item.Unit = product.Unit;
                                item.ProductName = product.Name;
                                item.ProductCode = product.Code;
                                item.DifferenceQuantity = item.ActualQuantity -
                                    allStockTransaction.Data[0].StockTransactionQuantity ?? 0;

                                if (item.DifferenceQuantity < 0 || item.DifferenceQuantity > 0)
                                {
                                    item.DifferenceType = InventoryCheckNoteConstants.DifferenceType.MISS_MATCHED;
                                }
                                else if (item.DifferenceQuantity == 0)
                                {
                                    item.DifferenceType = InventoryCheckNoteConstants.DifferenceType.MATCHED;
                                }

                                if (productInventoryEntity != null)
                                {
                                    if (item.DifferenceQuantity < 0 && -item.DifferenceQuantity > productInventoryEntity.SellableQuantity)
                                    {
                                        return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>($"Số lượng lệch âm đang vượt quá số lượng có thể giao dịch trong " +
                                            $"{allStockTransaction.Data[0].WareName}, vui lòng kiểm tra lại");
                                    }
                                }
                                
                            }
                        }
                        else
                        {
                            if (product != null)
                            {
                                item.RecordedQuantity = 0;
                                item.Unit = product.Unit;
                                item.ProductName = product.Name;
                                item.ProductCode = product.Code;
                                item.DifferenceQuantity = item.ActualQuantity - 0;

                                if (item.DifferenceQuantity < 0 || item.DifferenceQuantity > 0)
                                {
                                    item.DifferenceType = InventoryCheckNoteConstants.DifferenceType.MISS_MATCHED;
                                }
                                else if (item.DifferenceQuantity == 0)
                                {
                                    item.DifferenceType = InventoryCheckNoteConstants.DifferenceType.MATCHED;
                                }

                                if (productInventoryEntity != null)
                                {
                                    if (item.DifferenceQuantity < 0 && -item.DifferenceQuantity > productInventoryEntity.SellableQuantity)
                                    {
                                        return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>($"Số lượng lệch âm đang vượt quá số lượng có thể giao dịch trong " +
                                            $"{allStockTransaction.Data[0].WareName}, vui lòng kiểm tra lại");
                                    }
                                }
                                
                            }
                        }
                    }
                }

                #endregion

                _dbContext.sm_InventoryCheckNote.Add(entity);

                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Cập nhật lại số lượng có thể bán
                    foreach (var item in entity.Items)
                    {
                        var productEntity =
                            await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                        var productInventoryEntity = await _dbContext.sm_ProductInventory
                            .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseCode == entity.WareCode);

                        if (productInventoryEntity != null)
                        {
                            if (item.DifferenceQuantity < 0)
                            {
                                productInventoryEntity.SellableQuantity += item.DifferenceQuantity;

                                if (productEntity != null)
                                {
                                    productEntity.SellableQuantity += item.DifferenceQuantity;
                                }
                            }
                                
                            await _dbContext.SaveChangesAsync();
                        }
                        
                    }
                    #endregion
                }

                var result = await GetById(entity.Id);

                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<InventoryCheckNoteViewModel>(result.Data,
                        "Tạo phiếu kiểm thành công");
                }
                else return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<InventoryCheckNoteViewModel>(ex);
            }
        }

        /// <summary>
        /// Update Phiếu kiểm 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<InventoryCheckNoteViewModel>> Update(Guid id,
            InventoryCheckNoteCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                List<sm_Product> allInventoryNoteCheckItems = new List<sm_Product>();

                var entity = await _dbContext.sm_InventoryCheckNote
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return Helper.CreateNotFoundResponse<InventoryCheckNoteViewModel>
                        ($"Phiếu không tồn tại trong hệ thống, id: {id}");
                }

                if (entity.StatusCode == InventoryCheckNoteConstants.StatusCode.COMPLETED || 
                    entity.StatusCode == InventoryCheckNoteConstants.StatusCode.CANCELLED)
                {
                    return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>
                     ("Không được sửa các phiếu “Hoàn thành, Đã hủy”");
                }

                #region Validated và cho sửa phiếu nháp
                
                // Validate when Items is Empty
                if (model.Items == null && model.Items.Count() == 0)
                {
                    return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>("Vui lòng chọn sản phẩm");
                }

                if (model.Items != null && model.Items.Count > 0)
                {
                    // Fill tất cả products
                    var allProductIds = model.Items.Select(x => x.ProductId).ToList();
                    allInventoryNoteCheckItems = await _dbContext.sm_Product.AsNoTracking()
                        .Where(x => allProductIds.Contains(x.Id))
                        .ToListAsync();

                    // Validate: Không được duplicate dòng trong Product Item
                    if (model.Items.Count !=
                        model.Items.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>
                            ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: tồn thực tế không được nhỏ hơn 0
                    if (model.Items.Any(x => x.ActualQuantity < 0))
                        return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>
                            ("Danh sách sản phẩm không được có tồn thực tế nhỏ hơn 0.");

                    foreach (var item in model.Items)
                    {
                        var product = allInventoryNoteCheckItems
                            .FirstOrDefault(x => x.Id == item.ProductId);
                        if (product == null)
                            return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>
                                ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                    }

                }

                #endregion

                // Update các trường cần thiết
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = $"{currentUser.FullName} ({currentUser.UserName})";
                entity.LastModifiedOnDate = DateTime.Now;

                if (model.OrderCode == null)
                {
                    entity.OrderCode = entity.OrderCode;
                }
                else
                {
                    entity.OrderCode = model.OrderCode;
                }

                entity.WareCode = model.WareCode;
                entity.WareName = CodeTypeCollection.Instance.FetchCode(entity.WareCode, "vn", entity.TenantId).Title;
                entity.CheckDate = model.CheckDate;
                entity.Note = model.Note;
                entity.Tag = model.Tag;

                #region Fill Item's Line Number

                model.Items = model.Items.OrderBy(x => x.LineNo).ToList();
                for (int i = 0; i < model.Items.Count; i++)
                    model.Items[i].LineNo = i + 1;

                #endregion

                // Remove Old Inventory Note Item -> Re-add
                _dbContext.RemoveRange(entity.Items);
                entity.Items = new List<sm_InventoryCheckNoteItems>();

                #region Tính chênh lệch giữa tồn thực tế và tồn kho

                if (model.Items != null && model.Items.Count > 0)
                {
                    foreach (var item in model.Items)
                    {
                        // Re-add new item
                        var itemEntity = _mapper.Map<sm_InventoryCheckNoteItems>(item);
                        entity.Items.Add(itemEntity);

                        // Assign InventoryNoteId
                        itemEntity.CheckInventoryNoteId = entity.Id;

                        var product = allInventoryNoteCheckItems.FirstOrDefault(x => x.Id == item.ProductId);
                        var allStockTransaction =
                            await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId, model.WareCode);
                        var productInventoryEntity = _dbContext.sm_ProductInventory
                            .FirstOrDefault(x => x.ProductId == item.ProductId && x.WarehouseCode == model.WareCode);

                        if (allStockTransaction.Data != null && allStockTransaction.Data.Count > 0)
                        {
                            if (product != null)
                            {
                                itemEntity.RecordedQuantity = allStockTransaction.Data[0].StockTransactionQuantity ?? 0;
                                itemEntity.Unit = product.Unit;
                                itemEntity.ProductName = product.Name;
                                itemEntity.ProductCode = product.Code;
                                itemEntity.DifferenceQuantity = itemEntity.ActualQuantity -
                                    allStockTransaction.Data[0].StockTransactionQuantity ?? 0;

                                if (itemEntity.DifferenceQuantity < 0 || itemEntity.DifferenceQuantity > 0)
                                {
                                    itemEntity.DifferenceType = InventoryCheckNoteConstants.DifferenceType.MISS_MATCHED;
                                }
                                else if (itemEntity.DifferenceQuantity == 0)
                                {
                                    itemEntity.DifferenceType = InventoryCheckNoteConstants.DifferenceType.MATCHED;
                                }

                                // if (productInventoryEntity != null)
                                // {
                                //     if (itemEntity.DifferenceQuantity < 0 && -itemEntity.DifferenceQuantity > productInventoryEntity.SellableQuantity)
                                //     {
                                //         return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>($"Số lượng lệch âm đang vượt quá số lượng có thể giao dịch trong " +
                                //             $"{allStockTransaction.Data[0].WareName}, vui lòng kiểm tra lại");
                                //     }
                                // }
                                
                            }
                        }
                        else
                        {
                            if (product != null)
                            {
                                itemEntity.RecordedQuantity = 0;
                                itemEntity.Unit = product.Unit;
                                itemEntity.ProductName = product.Name;
                                itemEntity.ProductCode = product.Code;
                                itemEntity.DifferenceQuantity = itemEntity.ActualQuantity - 0;

                                if (itemEntity.DifferenceQuantity < 0 || itemEntity.DifferenceQuantity > 0)
                                {
                                    itemEntity.DifferenceType = InventoryCheckNoteConstants.DifferenceType.MISS_MATCHED;
                                }
                                else if (itemEntity.DifferenceQuantity == 0)
                                {
                                    itemEntity.DifferenceType = InventoryCheckNoteConstants.DifferenceType.MATCHED;
                                }
                                
                                // if (productInventoryEntity != null)
                                // {
                                //     if (itemEntity.DifferenceQuantity < 0 && -itemEntity.DifferenceQuantity > productInventoryEntity.SellableQuantity)
                                //     {
                                //         return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>($"Số lượng lệch âm đang vượt quá số lượng có thể giao dịch trong " +
                                //             $"{allStockTransaction.Data[0].WareName}, vui lòng kiểm tra lại");
                                //     }
                                // }
                            }
                        }
                    }
                }

                #endregion
                
                await _dbContext.SaveChangesAsync();

                var result = await GetById(entity.Id);
                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<InventoryCheckNoteViewModel>(result.Data,
                        "Sửa đơn kiểm hàng thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<InventoryCheckNoteViewModel>(ex);
            }
        }

        private async Task<string> GetNewCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_InventoryCheckNote.AsNoTracking()
                    .Where(x => x.OrderCode.Contains(code)).OrderByDescending(x => x.CreatedOnDate)
                    .FirstOrDefaultAsync();

                if (result != null)
                {
                    var currentNum = result.OrderCode.Substring(result.OrderCode.Length - 3, 3);
                    var currentNumInt = int.Parse(currentNum) + 1;
                    var stringResult = "";
                    if (currentNumInt < 10)
                    {
                        stringResult = "00" + currentNumInt;
                    }
                    else if (currentNumInt >= 10 && currentNumInt < 100)
                    {
                        stringResult = "0" + currentNumInt;
                    }
                    else
                    {
                        stringResult = currentNumInt.ToString();
                    }

                    return code + stringResult;
                }
                else
                {
                    return code + "001";
                }
            }
            catch (Exception ex)
            {
                Log.Error("", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lấy chi tiết phiếu kiểm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<InventoryCheckNoteViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_InventoryCheckNote.AsNoTracking()
                    .Include(x => x.Items.OrderBy(x => x.LineNo)).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<InventoryCheckNoteViewModel>(
                        "Phiếu không tồn tại trong hệ thống.");

                var result = _mapper.Map<InventoryCheckNoteViewModel>(entity);

                return new Response<InventoryCheckNoteViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<InventoryCheckNoteViewModel>(ex);
            }
        }

        /// <summary>
        /// Hủy nhiều phiếu
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<Response> CancelMultipleAsync(List<Guid> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _dbContext.sm_InventoryCheckNote
                        .Include(x => x.Items)
                        .Where(x => x.Id == id).FirstOrDefaultAsync();
                    var entities = _dbContext.sm_InventoryCheckNote.Where(x => ids.Contains(x.Id)).ToList();
                    if (entities == null | entities.Count == 0)
                        return Helper.CreateNotFoundResponse("Không tìm thấy phiếu");

                    if (entity.StatusCode == InventoryCheckNoteConstants.StatusCode.COMPLETED ||
                        entity.StatusCode == InventoryCheckNoteConstants.StatusCode.CANCELLED)
                        return Helper.CreateBadRequestResponse("Không được hủy các phiếu “Hoàn thành, Đã hủy”");
                    
                    foreach (var item in entity.Items)
                    {
                        var productEntity =
                            await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                        var productInventoryEntity =
                            await _dbContext.sm_ProductInventory.FirstOrDefaultAsync(x =>
                                x.ProductId == item.ProductId && x.WarehouseCode == entity.WareCode);

                        if (productInventoryEntity != null)
                        {
                            if (item.DifferenceQuantity < 0)
                            {
                                productInventoryEntity.SellableQuantity += -item.DifferenceQuantity;
                            }
                                
                            if (productEntity != null)
                            {
                                if (item.DifferenceQuantity < 0)
                                {
                                    productEntity.SellableQuantity += -item.DifferenceQuantity; 
                                }
                                    
                            }
                                
                            await _dbContext.SaveChangesAsync();
                        }
                    }

                    entity.StatusCode = InventoryCheckNoteConstants.StatusCode.CANCELLED;
                }

                await _dbContext.SaveChangesAsync();
               
                return Helper.CreateSuccessResponse(string.Format("Hủy {0} phiếu thành công.", ids.Count));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, ByUserIds: {@requestUserId}");
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        /// <summary>
        /// Lấy ra danh sách bản ghi
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Response<Pagination<InventoryCheckNoteViewModel>>> GetPage(InventoryCheckNoteQuery query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_InventoryCheckNote.AsNoTracking().Include(x => x.Items)
                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<InventoryCheckNoteViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<InventoryCheckNoteViewModel>>(ex);
            }
        }

        private Expression<Func<sm_InventoryCheckNote, bool>> BuildQuery(InventoryCheckNoteQuery query)
        {
            var predicate = PredicateBuilder.New<sm_InventoryCheckNote>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.OrderCode.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.StatusCode))
            {
                predicate.And(s => s.StatusCode == query.StatusCode);
            }

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date);
            }

            return predicate;
        }

        /// <summary>
        /// Cân bằng kho 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<InventoryCheckNoteViewModel>> BalanceInventory(Guid id, RequestUser currentUser)
        {
            try
            {
                // Loai phieu (Nhap/Xuat) xac dinh bang truong SL Chenh lech
                string OriginalDocumentType = string.Empty;

                // ReceiptInventoryQty
                decimal ReceiptInventoryQty = 0;

                // ExportInventoryQty
                decimal ExportInventoryQty = 0;

                var entity = await _dbContext.sm_InventoryCheckNote.Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>(
                        "Phiếu không tồn tại trong hệ thống");
                }

                entity.StatusCode = InventoryCheckNoteConstants.StatusCode.COMPLETED;

                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = $"{currentUser.FullName} ({currentUser.UserName})";
                entity.LastModifiedOnDate = DateTime.Now;
                entity.BalancedByUserName = currentUser.FullName;

                if (entity.StatusCode == InventoryCheckNoteConstants.StatusCode.COMPLETED)
                {
                    if (entity.Items != null && entity.Items.Count > 0)
                    {
                        foreach (var item in entity.Items)
                        {
                            #region Tinh toan gia tri XNT luu vao bien ReceiptInventoryQty and ExportInventoryQty

                            if (item.ActualQuantity > item.RecordedQuantity)
                            {
                                ReceiptInventoryQty = item.ActualQuantity - item.RecordedQuantity;
                                ExportInventoryQty = item.ActualQuantity - item.RecordedQuantity;
                            }
                            else
                            {
                                ReceiptInventoryQty = item.RecordedQuantity - item.ActualQuantity;
                                ExportInventoryQty = item.RecordedQuantity - item.ActualQuantity;
                            }

                            #endregion

                            #region Gan trang thai vao bien OriginalDocumentType

                            if (item.DifferenceQuantity > 0)
                            {
                                OriginalDocumentType = OriginalDocumentTypeConstants.NHAP_HANG;
                            }
                            else if (item.DifferenceQuantity < 0)
                            {
                                OriginalDocumentType = OriginalDocumentTypeConstants.XUAT_KHO;
                            }

                            #endregion

                            #region Tao ban ghi XNT

                            await _stockTransactionHandler.Create(new StockTransactionCreateModel
                            {
                                ReceiptInventoryQuantity =
                                    OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG
                                        ? ReceiptInventoryQty
                                        : 0,
                                ExportInventoryQuantity = OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO
                                    ? ExportInventoryQty
                                    : 0,
                                SellableQuantity = 0,
                                InitialStockQuantity = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)
                                    ?.InitialStockQuantity + item.DifferenceQuantity ?? 0,
                                ProductCode = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Code,
                                ProductName = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Name,
                                ProductId = item.ProductId,
                                WareCode = entity.WareCode,
                                OriginalDocumentType = OriginalDocumentType,
                                OriginalDocumentId = entity.Id,
                                OriginalDocumentCode = entity.OrderCode,
                                InventoryIncreased = item.DifferenceQuantity > 0 ? item.DifferenceQuantity : 0,
                                InventoryDecreased = item.DifferenceQuantity < 0 ? item.DifferenceQuantity : 0,
                                Unit = item.Unit,
                                CreatedByUserId = entity.CreatedByUserId,
                                Action = ActionConstants.INVENTORY_BALANCE,
                            });

                            #endregion

                            #region Update InitialStockQuantity

                            var product = await _dbContext.sm_Product.Where(x => x.Id == item.ProductId)
                                .FirstOrDefaultAsync();
                            var productInventory = await _dbContext.sm_ProductInventory.FirstOrDefaultAsync(x =>
                                x.ProductId == item.ProductId && x.WarehouseCode == entity.WareCode);

                            if (product == null)
                            {
                                return Helper.CreateBadRequestResponse<InventoryCheckNoteViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                            }
                            else
                            {
                                product.InitialStockQuantity += item.DifferenceQuantity;

                                if (item.DifferenceQuantity > 0)
                                {
                                    product.SellableQuantity += item.DifferenceQuantity;
                                }
                                _dbContext.sm_Product.Update(product);

                                if (productInventory != null)
                                {
                                    if (item.DifferenceQuantity > 0)
                                    {
                                        productInventory.SellableQuantity += item.DifferenceQuantity;
                                        _dbContext.sm_ProductInventory.Update(productInventory);
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                }

                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<InventoryCheckNoteViewModel>(entity),
                    "Cân bằng kho thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<InventoryCheckNoteViewModel>(ex);
            }
        }

        /// <summary>
        /// Hàm xuất file danh sách phiếu kiểm kho excel theo query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<string>> ExportListToExcel(InventoryCheckNoteQuery query)
        {
            try
            {
                // Tạo predicate để lọc dựa trên các tham số trong query
                var predicate = BuildQuery(query);

                // Lấy danh sách phiếu kiểm kho từ cơ sở dữ liệu dựa trên lọc và phân trang
                var inventoriesCheckNote = await _dbContext.sm_InventoryCheckNote.AsNoTracking()
                    .Include(x => x.Items)
                    .Where(predicate)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .GetPageAsync(query);

                // Kiểm tra nếu không có dữ liệu trả về trong trang hiện tại
                if (inventoriesCheckNote == null || inventoriesCheckNote.Content == null ||
                    inventoriesCheckNote.Content.Count == 0)
                    return Helper.CreateNotFoundResponse<string>("Không có phiếu kiểm nào tồn tại trong hệ thống.");

                // Đặt tên file và đường dẫn template
                var fileName = $"danh sách phiếu kiểm_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}.xlsx";
                var filePath = Path.Combine(_staticsFolder, fileName);
                var templatePath = Path.Combine(_staticsFolder, "excel-template/InventoryCheckNoteTemplate.xlsx");

                if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                    return Helper.CreateBadRequestResponse<string>("Không tìm thấy file template");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Mở template Excel và điền dữ liệu vào file
                using (var package = new ExcelPackage(new FileInfo(templatePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Sử dụng worksheet đầu tiên

                    // Đặt độ rộng cố định cho cột A (cột STT) là 8
                    worksheet.Column(1).Width = 8;

                    // Điền dữ liệu vào bảng Excel (giả sử bắt đầu từ hàng thứ 4)
                    int startRow = 4;
                    int index = 1;

                    foreach (var order in inventoriesCheckNote.Content)
                    {
                        worksheet.Cells[startRow, 1].Value = index; // STT
                        worksheet.Cells[startRow, 2].Value = order.CreatedOnDate.ToString("dd/MM/yyyy"); // Ngày tạo
                        worksheet.Cells[startRow, 3].Value = order.CheckDate?.ToString("dd/MM/yyyy"); // Ngày kiểm
                        worksheet.Cells[startRow, 4].Value =
                            order.LastModifiedOnDate?.ToString("dd/MM/yyyy"); // Ngày cân bằng
                        worksheet.Cells[startRow, 5].Value = order.OrderCode; // Mã phiếu kiểm

                        // Kiểm tra trạng thái và gán tên trạng thái
                        string statusName;
                        switch (order.StatusCode)
                        {
                            case InventoryCheckNoteConstants.StatusCode.DRAFT:
                                statusName = "Nháp";
                                break;
                            case InventoryCheckNoteConstants.StatusCode.COMPLETED:
                                statusName = "Hoàn thành";
                                break;
                            case InventoryCheckNoteConstants.StatusCode.CANCELLED:
                                statusName = "Đã hủy";
                                break;
                            default:
                                statusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }

                        worksheet.Cells[startRow, 6].Value = statusName; // Gán tên trạng thái

                        worksheet.Cells[startRow, 7].Value = CodeTypeCollection.Instance
                            .FetchCode(order.WareCode, LanguageConstants.Default, order.TenantId)?.Title ?? null; // Kho;
                        worksheet.Cells[startRow, 8].Value = order.CreatedByUserName; // Người tạo
                        worksheet.Cells[startRow, 9].Value = order.BalancedByUserName; // Người cân bằng
                        worksheet.Cells[startRow, 10].Value = order.Note; // Ghi chú

                        startRow++;
                        index++;
                    }

                    int lastDataRow = startRow - 1;

                    // Thêm đường viền cho các ô đã điền dữ liệu
                    using (var range =
                           worksheet.Cells[4, 1, lastDataRow,
                               10]) // điều chỉnh cột cuối (18) tùy theo số lượng cột bạn có
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    // Xóa các dòng thừa sau khi điền dữ liệu
                    int totalRows = worksheet.Dimension.End.Row;
                    if (totalRows > lastDataRow)
                    {
                        worksheet.DeleteRow(lastDataRow + 1, totalRows - lastDataRow);
                    }

                    // Tự động điều chỉnh kích thước các cột từ cột thứ hai đến cột cuối cùng
                    worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].AutoFitColumns();

                    // Lưu file Excel đã điền dữ liệu
                    await package.SaveAsAsync(new FileInfo(filePath));
                }

                return Helper.CreateSuccessResponse<string>(filePath, "Xuất file thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<string>(ex);
            }
        }
    }
}
