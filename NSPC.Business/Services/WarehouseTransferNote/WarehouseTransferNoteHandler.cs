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
using AutoMapper;
using LinqKit;
using static NSPC.Common.Helper;
using Exception = System.Exception;
using NSPC.Business.Services.CashbookTransaction;

namespace NSPC.Business.Services
{
    public class WarehouseTransferNoteHandler: IWarehouseTransferNoteHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IStockTransactionHandler _stockTransactionHandler;
        private readonly string _staticsFolder;
        private readonly IProductInventoryHandler _productInventoryHandler;
        
        public WarehouseTransferNoteHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IStockTransactionHandler stockTransactionHandler, IProductInventoryHandler productInventoryHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _stockTransactionHandler = stockTransactionHandler;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
            _productInventoryHandler = productInventoryHandler;
        }

        /// <summary>
        /// Tạo phiếu chuyển kho
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currrentUser"></param>
        /// <returns></returns>
        public async Task<Response<WarehouseTransferNoteViewModel>> Create(WarehouseTransferNoteCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                #region Validate: Mã phiếu đã tồn tại

                bool isDuplicateCode = await _dbContext.sm_WarehouseTransferNote
                    .AsNoTracking()
                    .AnyAsync(x => x.TransferNoteCode == model.TransferNoteCode);

                if (isDuplicateCode)
                {
                    return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>(
                        $"Mã {model.TransferNoteCode} đã tồn tại!");
                }

                #endregion

                List<sm_Product> allListProductUseForTransfer = new List<sm_Product>();

                #region Validated

                // Validate: Bắt buộc phải có ít nhất 1 sản phẩm
                if (model.Items == null || !model.Items.Any())
                    return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>
                        ("Vui lòng chọn ít nhất 1 sản phẩm cần chuyển");

                if (model.Items != null && model.Items.Count > 0)
                {
                    // Fill tất cả products
                    var allProductIds = model.Items.Select(x => x.ProductId).ToList();
                    allListProductUseForTransfer = await _dbContext.sm_Product.AsNoTracking()
                        .Where(x => allProductIds.Contains(x.Id))
                        .ToListAsync();
                    
                    // Validate: Không được duplicate dòng trong Product Item
                    if (model.Items.Count !=
                        model.Items.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>
                            ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: Số lượng không được nhỏ hơn 0 và null
                    if (model.Items.Any(x => x.Quantity < 0))
                        return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>(
                            "Danh sách sản phẩm không được có số lượng chuyển nhỏ hơn 0.");

                    // Validate: Số lượng không được bằng 0
                    if (model.Items.Any(x => x.Quantity == 0))
                        return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>(
                            "Vui lòng nhập số lượng sản phẩm cần chuyển.");

                    foreach (var item in model.Items)
                    {
                        var product = allListProductUseForTransfer
                            .FirstOrDefault(x => x.Id == item.ProductId);
                        if (product == null)
                            return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>
                                ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                    }
                    
                    // Validated: Kiểm tra tồn kho tại kho xuất
                    foreach (var item in model.Items)
                    {
                        var allStockTransaction =
                            await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId,
                                model.ExportWarehouseCode);
                        if (allStockTransaction.Data != null && allStockTransaction.Data.Count > 0)
                        {
                            if (allStockTransaction.Data[0].StockTransactionQuantity < item.Quantity)
                            {
                                return CreateBadRequestResponse<WarehouseTransferNoteViewModel>(
                                    $"Số lượng hàng trong kho {allStockTransaction.Data[0].WareName} không đủ để chuyển");
                            }
                        }
                        else
                        {
                            var nameProduct = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Name;
                            return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>
                                ($"Sản phẩm {nameProduct} có số lượng bán ra vượt quá số lượng có thể bán trong kho");
                        }
                    }
                }
                #endregion

                #region Validate không được trùng kho
                if (model.ExportWarehouseCode == model.ImportWarehouseCode)
                {
                    return CreateBadRequestResponse<WarehouseTransferNoteViewModel>(
                        "Kho nhập và kho xuất không được giống nhau");
                }
                #endregion
                
                #region Fill Item's Line Number
                model.Items = model.Items.OrderBy(x => x.LineNo).ToList();
                for (int i = 0; i < model.Items.Count; i++)
                    model.Items[i].LineNo = i + 1;
                #endregion
                
                // Mapping và tạo entity
                var entity = _mapper.Map<sm_WarehouseTransferNote>(model);
                entity.Id = Guid.NewGuid();

                if (model.TransferNoteCode == null)
                {
                    entity.TransferNoteCode = await GetNewCode(WarehouseTransferNoteConstants.WarehouseTransferNoteCodePrefix);
                }
                
                entity.StatusCode = WarehouseTransferNoteConstants.StatusCode.DRAFT;
                entity.CreatedOnDate = DateTime.Now;
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.ImportWarehouseName = CodeTypeCollection.Instance.FetchCode(entity.ImportWarehouseCode, "vn", currentUser.TenantId).Title;
                entity.ExportWarehouseName = CodeTypeCollection.Instance.FetchCode(entity.ExportWarehouseCode, "vn", currentUser.TenantId).Title;

                #region Xử lý Items

                if (entity.Items != null && entity.Items.Count > 0)
                {
                    foreach (var item in entity.Items)
                    {
                        var product = allListProductUseForTransfer.FirstOrDefault(x => x.Id == item.ProductId);
                        var productInventoryEntity = _dbContext.sm_ProductInventory
                            .FirstOrDefault(x => x.Id == item.ProductId && x.WarehouseCode == model.ExportWarehouseCode);
                        var allStockTransactionEntity =
                            await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId, entity.ExportWarehouseCode);

                        if (allStockTransactionEntity.Data != null && allStockTransactionEntity.Data.Count > 0)
                        {
                            if (product != null)
                            {
                                item.ProductCode = product.Code;
                                item.ProductName = product.Name;
                                item.Unit = product.Unit;

                                if (productInventoryEntity != null)
                                {
                                    if (item.Quantity > productInventoryEntity.SellableQuantity)
                                    {
                                        return CreateBadRequestResponse<WarehouseTransferNoteViewModel>(
                                            $"Số lượng điều chuyển lớn hơn số lượng có thể giao dịch trong {allStockTransactionEntity.Data[0].WareName}");
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
                
                _dbContext.sm_WarehouseTransferNote.Add(entity);
                
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Cập nhật lại số lượng có thể bán
                    foreach (var item in entity.Items)
                    {
                        var productEntity =
                            await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                        var productInventoryEntity = await _dbContext.sm_ProductInventory
                            .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseCode == entity.ExportWarehouseCode);

                        if (productInventoryEntity != null)
                        {
                            productInventoryEntity.SellableQuantity -= item.Quantity;
                            
                            if (productEntity != null)
                            {
                                productEntity.SellableQuantity -= item.Quantity;
                            }
                            
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                    #endregion
                }

                var result = await GetById(entity.Id);

                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<WarehouseTransferNoteViewModel>(result.Data, "Tạo phiếu chuyển kho thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<WarehouseTransferNoteViewModel>(ex);
            }
        }
        
        private async Task<string> GetNewCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_WarehouseTransferNote.AsNoTracking().Where(x => x.TransferNoteCode.Contains(code)).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

                if (result != null)
                {
                    var currentNum = result.TransferNoteCode.Substring(result.TransferNoteCode.Length - 3, 3);
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
        /// Cập nhật phiếu chuyển kho
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="currrentUser"></param>
        /// <returns></returns>
        public async Task<Response<WarehouseTransferNoteViewModel>> Update(Guid id,
            WarehouseTransferNoteCreateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_WarehouseTransferNote
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == id);

                // Check id không tồn tại
                if (entity == null)
                    return Helper.CreateNotFoundResponse<WarehouseTransferNoteViewModel>("Phiếu không tồn tại trong hệ thống.");

                #region Kiểm tra mã phiếu chuyển đã tồn tại

                bool isDuplicateCode = await _dbContext.sm_WarehouseTransferNote
                    .AsNoTracking()
                    .AnyAsync(x => x.TransferNoteCode == model.TransferNoteCode && x.Id != id);

                if (isDuplicateCode)
                {
                    return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>(
                        $"Mã {model.TransferNoteCode} đã tồn tại!");
                }

                #endregion

                // Check không được sửa phiếu đã hoàn thành hoặc đã hủy
                if (entity.StatusCode == WarehouseTransferNoteConstants.StatusCode.COMPLETED ||
                    entity.StatusCode == WarehouseTransferNoteConstants.StatusCode.CANCELLED)
                    return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>("Không được sửa các phiếu “Hoàn thành, Đã hủy”");

                List<sm_Product> allListProductUseForTransfer = new List<sm_Product>();

                #region Validated

                // Validate: Bắt buộc phải có ít nhất 1 sản phẩm
                if (model.Items == null || !model.Items.Any())
                    return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>
                        ("Vui lòng chọn ít nhất 1 sản phẩm cần chuyển");

                if (model.Items != null && model.Items.Count > 0)
                {
                    // Fill tất cả products
                    var allProductIds = model.Items.Select(x => x.ProductId).ToList();
                    allListProductUseForTransfer = await _dbContext.sm_Product.AsNoTracking()
                        .Where(x => allProductIds.Contains(x.Id))
                        .ToListAsync();
                    
                    // Validate: Không được duplicate dòng trong Product Item
                    if (model.Items.Count !=
                        model.Items.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>
                            ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: Số lượng không được nhỏ hơn 0 và null
                    if (model.Items.Any(x => x.Quantity < 0))
                        return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>(
                            "Danh sách sản phẩm không được có số lượng chuyển nhỏ hơn 0.");

                    // Validate: Số lượng không được bằng 0
                    if (model.Items.Any(x => x.Quantity == 0))
                        return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>(
                            "Vui lòng nhập số lượng sản phẩm cần chuyển.");

                    foreach (var item in model.Items)
                    {
                        var product = allListProductUseForTransfer
                            .FirstOrDefault(x => x.Id == item.ProductId);
                        if (product == null)
                            return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>
                                ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                    }
                    
                    // Validated: Kiểm tra tồn kho tại kho xuất
                    foreach (var item in model.Items)
                    {
                        var allStockTransaction =
                            await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId,
                                model.ExportWarehouseCode);
                        if (allStockTransaction.Data != null && allStockTransaction.Data.Count > 0)
                        {
                            if (allStockTransaction.Data[0].StockTransactionQuantity < item.Quantity)
                            {
                                return CreateBadRequestResponse<WarehouseTransferNoteViewModel>(
                                    $"Số lượng hàng trong kho {allStockTransaction.Data[0].WareName} không đủ để chuyển");
                            }
                        }
                    }
                }
                #endregion
                
                #region Fill Item's Line Number
                model.Items = model.Items.OrderBy(x => x.LineNo).ToList();
                for (int i = 0; i < model.Items.Count; i++)
                    model.Items[i].LineNo = i + 1;
                #endregion

                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.LastModifiedByUserName = currentUser.FullName;
                
                if (model.TransferNoteCode == null)
                {
                    entity.TransferNoteCode = entity.TransferNoteCode;
                }
                else
                {
                    entity.TransferNoteCode = model.TransferNoteCode;
                }
                
                entity.TransferredOnDate = model.TransferredOnDate;
                //entity.ImportWarehouseCode = model.ImportWarehouseCode;
                //entity.ExportWarehouseCode = model.ExportWarehouseCode;
                //entity.ImportWarehouseName = _dbContext.sm_CodeType
                //    .FirstOrDefault(x => x.Code == model.ImportWarehouseCode)?.Title;
                //entity.ExportWarehouseName = _dbContext.sm_CodeType
                //    .FirstOrDefault(x => x.Code == model.ExportWarehouseCode)?.Title;
                entity.Note = model.Note;

                // Xử lý Items
                _dbContext.RemoveRange(entity.Items);
                entity.Items = new List<sm_WarehouseTransferNoteItem>();

                if (model.Items != null && model.Items.Count > 0)
                {
                    foreach (var item in model.Items)
                    {
                        // Re-add new item
                        var itemEntity = _mapper.Map<sm_WarehouseTransferNoteItem>(item);
                        entity.Items.Add(itemEntity);

                        // Assign InventoryNoteId
                        itemEntity.TransferNoteID = entity.Id;
                        
                        var product = allListProductUseForTransfer.FirstOrDefault(x => x.Id == item.ProductId);
                        // var productInventoryEntity = _dbContext.sm_ProductInventory
                        //     .FirstOrDefault(x => x.Id == item.ProductId && x.WarehouseCode == model.ExportWarehouseCode);
                        var allStockTransactionEntity =
                            await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId, entity.ExportWarehouseCode);

                        if (allStockTransactionEntity.Data != null && allStockTransactionEntity.Data.Count > 0)
                        {
                            if (product != null)
                            {
                                itemEntity.ProductCode = product.Code;
                                itemEntity.ProductName = product.Name;
                                itemEntity.Unit = product.Unit;
                                
                                // if (productInventoryEntity != null)
                                // {
                                //     if (item.Quantity > productInventoryEntity.SellableQuantity)
                                //     {
                                //         return CreateBadRequestResponse<WarehouseTransferNoteViewModel>(
                                //             $"Số lượng điều chuyển lớn hơn số lượng có thể giao dịch trong {allStockTransactionEntity.Data[0].WareName}");
                                //     }
                                // }
                            }
                        }
                    }
                }
                
                await _dbContext.SaveChangesAsync();
                
                var result = await GetById(entity.Id);

                if (result.IsSuccess)
                {
                    return Helper.CreateSuccessResponse<WarehouseTransferNoteViewModel>(result.Data, "Sửa phiếu chuyển kho thành công");
                }
                else return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<WarehouseTransferNoteViewModel>(ex);
            }
        }

        /// <summary>
        /// Lấy chi tiết phiếu chuyển kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<WarehouseTransferNoteViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_WarehouseTransferNote.AsNoTracking().Include(x => x.Items.OrderBy(x => x.LineNo)).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<WarehouseTransferNoteViewModel>("Phiếu không tồn tại trong hệ thống.");

                var result = _mapper.Map<WarehouseTransferNoteViewModel>(entity);

                return new Response<WarehouseTransferNoteViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<WarehouseTransferNoteViewModel>(ex);
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
                    var entity = await _dbContext.sm_WarehouseTransferNote
                        .Include(x => x.Items)
                        .Where(x => x.Id == id).FirstOrDefaultAsync();
                    var entities = _dbContext.sm_WarehouseTransferNote.Where(x => ids.Contains(x.Id)).ToList();
                    if (entities == null | entities.Count == 0)
                        return Helper.CreateNotFoundResponse("Không tìm thấy phiếu");

                    if (entity.StatusCode == WarehouseTransferNoteConstants.StatusCode.COMPLETED ||
                        entity.StatusCode == WarehouseTransferNoteConstants.StatusCode.CANCELLED)
                        return Helper.CreateBadRequestResponse("Không được hủy các phiếu “Hoàn thành, Đã hủy”");
                    
                    foreach (var item in entity.Items)
                    {
                        var productEntity =
                            await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                        var productInventoryEntity = await _dbContext.sm_ProductInventory
                            .FirstOrDefaultAsync(x =>
                                x.ProductId == item.ProductId && x.WarehouseCode == entity.ExportWarehouseCode);

                        if (productInventoryEntity != null)
                        {
                            productInventoryEntity.SellableQuantity += item.Quantity;

                            if (productEntity != null)
                            {
                                productEntity.SellableQuantity += item.Quantity; 
                            }
                                
                            await _dbContext.SaveChangesAsync();
                        }
                    }

                    entity.StatusCode = WarehouseTransferNoteConstants.StatusCode.CANCELLED;
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
        /// Lấy danh sách phiếu chuyển kho
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Response<Pagination<WarehouseTransferNoteViewModel>>> GetPage(
            WarehouseTransferNoteQuery query)
        {
            var predicate = BuildQuery(query);

            var queryResult = _dbContext.sm_WarehouseTransferNote.AsNoTracking().Include(x => x.Items).Where(predicate);
            var data = await queryResult.GetPageAsync(query);

            var result = _mapper.Map<Pagination<WarehouseTransferNoteViewModel>>(data);

            return Helper.CreateSuccessResponse(result);
        }
        
        private Expression<Func<sm_WarehouseTransferNote, bool>> BuildQuery(WarehouseTransferNoteQuery query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            
            var predicate = PredicateBuilder.New<sm_WarehouseTransferNote>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.TransferNoteCode.ToLower().Contains(query.FullTextSearch.ToLower()));

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
            
            if (!currentUser.ListRights.Contains("WAREHOUSETRANSFERNOTE." + RightActionConstants.VIEWALL))
                predicate.And(s => s.CreatedByUserId == currentUser.UserId);

            return predicate;
        }

        /// <summary>
        /// Điều chuyển kho
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<WarehouseTransferNoteViewModel>> TransferWarehouse(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_WarehouseTransferNote
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return CreateBadRequestResponse<WarehouseTransferNoteViewModel>("Phiếu không tồn tại trong hệ thống");
                }
                
                entity.LastModifiedOnDate = DateTime.Now;
                entity.StatusCode = WarehouseTransferNoteConstants.StatusCode.COMPLETED;
                entity.TransferredByUserName = currentUser.FullName;

                if (entity.StatusCode == WarehouseTransferNoteConstants.StatusCode.COMPLETED)
                {
                    foreach (var item in entity.Items)
                    {
                        #region Tạo bản ghi XNT với kho xuất
                        var exportTransaction = new StockTransactionCreateModel()
                        {
                            ReceiptInventoryQuantity = 0,
                            ExportInventoryQuantity = item.Quantity,
                            SellableQuantity = -item.Quantity,
                            InitialStockQuantity = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)
                                ?.InitialStockQuantity - item.Quantity ?? 0,
                            ProductCode = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Code,
                            ProductName = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Name,
                            ProductId = item.ProductId,
                            WareCode = entity.ExportWarehouseCode,
                            OriginalDocumentType = OriginalDocumentTypeConstants.XUAT_KHO,
                            OriginalDocumentId = entity.Id,
                            OriginalDocumentCode = entity.TransferNoteCode,
                            Unit = item.Unit,
                            CreatedByUserId = entity.CreatedByUserId,
                            Action = ActionConstants.TRANSFER_WAREHOUSE,
                        };
                        await _stockTransactionHandler.Create(exportTransaction);
                        
                        #region Update InitialStockQuantity của sản phẩm khi xuất kho chuyển hàng
                        var productExport = await _dbContext.sm_Product.Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();

                        if (productExport == null)
                        {
                            return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>
                                ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                        }
                        else
                        {
                            productExport.InitialStockQuantity -= item.Quantity;
                            // productExport.SellableQuantity -= item.Quantity;
                            _dbContext.sm_Product.Update(productExport);
                        }
                        #endregion
                        #endregion
                        
                        #region Tạo bản ghi XNT với kho nhập
                        var importTransaction = new StockTransactionCreateModel()
                        {
                            ReceiptInventoryQuantity = item.Quantity,
                            ExportInventoryQuantity = 0,
                            // SellableQuantity = item.Quantity,
                            InitialStockQuantity = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)
                                ?.InitialStockQuantity + item.Quantity ?? 0,
                            ProductCode = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Code,
                            ProductName = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Name,
                            ProductId = item.ProductId,
                            WareCode = entity.ImportWarehouseCode,
                            OriginalDocumentType = OriginalDocumentTypeConstants.NHAP_HANG,
                            OriginalDocumentId = entity.Id,
                            OriginalDocumentCode = entity.TransferNoteCode,
                            Unit = item.Unit,
                            CreatedByUserId = entity.CreatedByUserId,
                            Action = ActionConstants.TRANSFER_WAREHOUSE,
                        };
                        
                        await _stockTransactionHandler.Create(importTransaction);

                        #region Check khi có tồn tại bản ghi có chứa importWarehouseCode thì update, còn không thì tạo bản ghi mới
                        var productInventoryEntity = _dbContext.sm_ProductInventory
                            .Where(x => x.ProductId == item.ProductId);
                        
                        if (productInventoryEntity.Any(x => x.WarehouseCode == entity.ImportWarehouseCode))
                        {
                            var productInventory =
                                await _dbContext.sm_ProductInventory.FirstOrDefaultAsync(x =>
                                    x.ProductId == item.ProductId && x.WarehouseCode == entity.ImportWarehouseCode);

                            if (productInventory != null)
                            {
                                productInventory.SellableQuantity += item.Quantity;
                                _dbContext.sm_ProductInventory.Update(productInventory);
                            }
                        }
                        else
                        {
                            await _productInventoryHandler.Create(new ProductInventoryCreateModel
                            {
                                WarehouseCode = entity.ImportWarehouseCode,
                                SellableQuantity = item.Quantity,
                                ProductId = item.ProductId,
                            }, currentUser);
                        }
                        #endregion
                        
                        #region Update InitialStockQuantity khi nhập kho hàng chuyển
                        var productImport = await _dbContext.sm_Product.Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();

                        if (productImport == null)
                        {
                            return Helper.CreateBadRequestResponse<WarehouseTransferNoteViewModel>
                                ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");
                        }
                        else
                        {
                            productImport.InitialStockQuantity += item.Quantity;
                            productImport.SellableQuantity += item.Quantity;
                            _dbContext.sm_Product.Update(productImport);
                        }
                        #endregion
                        
                        #endregion
                    }
                }

                await _dbContext.SaveChangesAsync();
                
                return Helper.CreateSuccessResponse(_mapper.Map<WarehouseTransferNoteViewModel>(entity), "Chuyển kho thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<WarehouseTransferNoteViewModel>(ex);
            }
        }
        
        /// <summary>
        /// Hàm xuất file danh sách phiếu chuyển kho excel theo query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<string>> ExportListToExcel(WarehouseTransferNoteQuery query)
        {
            try
            {
                // Tạo predicate để lọc dựa trên các tham số trong query
                var predicate = BuildQuery(query);

                // Lấy danh sách phiếu kiểm kho từ cơ sở dữ liệu dựa trên lọc và phân trang
                var warehousesTransferNote = await _dbContext.sm_WarehouseTransferNote.AsNoTracking()
                    .Include(x => x.Items)
                    .Where(predicate)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .GetPageAsync(query);

                // Kiểm tra nếu không có dữ liệu trả về trong trang hiện tại
                if (warehousesTransferNote == null || warehousesTransferNote.Content == null ||
                    warehousesTransferNote.Content.Count == 0)
                    return Helper.CreateNotFoundResponse<string>("Không có phiếu kiểm nào tồn tại trong hệ thống.");

                // Đặt tên file và đường dẫn template
                var fileName = $"danh sách phiếu điều chuyển_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}.xlsx";
                var filePath = Path.Combine(_staticsFolder, fileName);
                var templatePath = Path.Combine(_staticsFolder, "excel-template/WarehouseTransferNoteTemplate.xlsx");

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

                    foreach (var order in warehousesTransferNote.Content)
                    {
                        worksheet.Cells[startRow, 1].Value = index; // STT
                        worksheet.Cells[startRow, 2].Value = order.CreatedOnDate.ToString("dd/MM/yyyy"); // Ngày tạo
                        worksheet.Cells[startRow, 3].Value = order.TransferredOnDate?.ToString("dd/MM/yyyy"); // Ngày dự kiến chuyển
                        worksheet.Cells[startRow, 4].Value =
                            order.LastModifiedOnDate?.ToString("dd/MM/yyyy"); // Ngày chuyển kho
                        worksheet.Cells[startRow, 5].Value = order.TransferNoteCode; // Mã phiếu kiểm

                        // Kiểm tra trạng thái và gán tên trạng thái
                        string statusName;
                        switch (order.StatusCode)
                        {
                            case WarehouseTransferNoteConstants.StatusCode.DRAFT:
                                statusName = "Nháp";
                                break;
                            case WarehouseTransferNoteConstants.StatusCode.COMPLETED:
                                statusName = "Hoàn thành";
                                break;
                            case WarehouseTransferNoteConstants.StatusCode.CANCELLED:
                                statusName = "Đã hủy";
                                break;
                            default:
                                statusName = "Không xác định"; // Trường hợp mã không hợp lệ
                                break;
                        }

                        worksheet.Cells[startRow, 6].Value = statusName; // Gán tên trạng thái

                        worksheet.Cells[startRow, 7].Value = CodeTypeCollection.Instance
                            .FetchCode(order.ExportWarehouseCode, LanguageConstants.Default, order.TenantId)?.Title ?? null; // Kho xuất;
                        worksheet.Cells[startRow, 8].Value = CodeTypeCollection.Instance
                            .FetchCode(order.ImportWarehouseCode, LanguageConstants.Default, order.TenantId)?.Title ?? null; // Kho nhập;
                        worksheet.Cells[startRow, 9].Value = order.CreatedByUserName; // Người tạo
                        worksheet.Cells[startRow, 10].Value = order.TransferredByUserName; // Người cân bằng
                        worksheet.Cells[startRow, 11].Value = order.Note; // Ghi chú

                        startRow++;
                        index++;
                    }

                    int lastDataRow = startRow - 1;

                    // Thêm đường viền cho các ô đã điền dữ liệu
                    using (var range =
                           worksheet.Cells[4, 1, lastDataRow,
                               11]) // điều chỉnh cột cuối (18) tùy theo số lượng cột bạn có
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
