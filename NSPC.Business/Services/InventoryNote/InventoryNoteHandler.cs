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
using NSPC.Data;
using static NSPC.Common.Helper;
using NSPC.Business.Services.ConstructionActitvityLog;

namespace NSPC.Business.Services.InventoryNote
{
    public class InventoryNoteHandler : IInventoryNoteHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IStockTransactionHandler _stockTransactionHandler;
        private readonly string _staticsFolder;
        private readonly IProductInventoryHandler _productInventoryHandler;
        private readonly IConstructionActivityLogHandler _constructionActivityLogHandler;
        public InventoryNoteHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IStockTransactionHandler stockTransactionHandler, IProductInventoryHandler productInventoryHandler, IConstructionActivityLogHandler constructionActivityLogHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _stockTransactionHandler = stockTransactionHandler;
            _productInventoryHandler = productInventoryHandler;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
            _constructionActivityLogHandler = constructionActivityLogHandler;
        }
        /// <summary>
        /// Create Inventory Note
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<InventoryNoteViewModel>> Create(InventoryNoteCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                // Check mã phiếu đã tồn tại với các phiếu khác hay chưa
                if (_dbContext.sm_InventoryNote.Any(x => x.Code == model.Code))
                    return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                        (string.Format("Mã phiếu {0} đã tồn tại!", model.Code));

                List<sm_Product> allInventoryNoteProducts = new List<sm_Product>();

                #region Validate
                
                // Validate MaterialRequestId
                if (model.MaterialRequestId != null)
                {
                    if (!await _dbContext.sm_MaterialRequest.AnyAsync(x => x.Id == model.MaterialRequestId))
                    {
                        return Helper.CreateNotFoundResponse<InventoryNoteViewModel>
                            ($"Id ${model.MaterialRequestId} không tồn tại");
                    }
                }

                // Validate Product Item Model
                if (model.InventoryNoteItems != null && model.InventoryNoteItems.Count > 0)
                {
                    // Fill tất cả products
                    var allProductIds = model.InventoryNoteItems.Select(x => x.ProductId).ToList();
                    allInventoryNoteProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Không được duplicate dòng trong Product Item
                    if (model.InventoryNoteItems.Count !=
                        model.InventoryNoteItems.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.InventoryNoteItems.Any(x => x.Quantity <= 0))
                        return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn hoặc bằng 0.");

                    foreach (var item in model.InventoryNoteItems)
                    {
                        var product = allInventoryNoteProducts
                                            .Where(x => x.Id == item.ProductId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");

                        if (product.InitialStockQuantity < item.Quantity && model.TypeCode == InventoryNoteConstants.TypeCode.InventoryExport)
                            return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không đủ tồn kho");

                        if (product.IsOrder == false && model.TypeCode == InventoryNoteConstants.TypeCode.InventoryExport)
                            return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không cho phép bán");
                    }
                }
                #endregion

                #region Check số lượng có thể bán và tồn trong kho của sản phẩm
                if (model.TypeCode == InventoryNoteConstants.TypeCode.InventoryExport)
                {
                    foreach (var item in model.InventoryNoteItems)
                    {
                        var allStockTransactions =
                            await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId, model.InventoryCode);
                        var productInventoryEntity = await _dbContext.sm_ProductInventory.FirstOrDefaultAsync(x =>
                            x.ProductId == item.ProductId && x.WarehouseCode == model.InventoryCode);
                        
                        if (allStockTransactions.Data != null && allStockTransactions.Data.Count > 0)
                        {
                            if (productInventoryEntity != null)
                            {
                                if (productInventoryEntity.SellableQuantity < item.Quantity)
                                    return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                        ($"Sản phẩm {allStockTransactions.Data[0].ProductName} có số lượng bán ra vượt quá số lượng có thể bán trong kho");
                            } 
                            
                            if (allStockTransactions.Data[0].StockTransactionQuantity < item.Quantity)
                                return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ($"Sản phẩm {allStockTransactions.Data[0].ProductName} không đủ tồn trong kho");
                        }
                        else
                        {
                            var nameProduct = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Name;
                            return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                ($"Sản phẩm {nameProduct} có số lượng bán ra vượt quá số lượng có thể bán trong kho");
                        }
                    }
                    #endregion
                }

                #region Fill Item's Line Number
                model.InventoryNoteItems = model.InventoryNoteItems.OrderBy(x => x.LineNumber).ToList();
                for (int i = 0; i < model.InventoryNoteItems.Count; i++)
                    model.InventoryNoteItems[i].LineNumber = i + 1;
                #endregion

                // Mapping và tạo entity
                var entity = _mapper.Map<sm_InventoryNote>(model);
                entity.Id = Guid.NewGuid();

                if (model.Code == null)
                {
                    entity.Code = await GetNewCodeInventoryNote(model.TypeCode);
                }
                
                if (model.EntityTypeCode == "employee")
                {
                    entity.EntityName = _dbContext.IdmUser.FirstOrDefault(x => x.Id == model.EntityId).Name;
                }
                
                entity.StatusCode = InventoryNoteConstants.StatusCode.DRAFT;
                entity.StatusName = InventoryNoteConstants.StatusName.DRAFT;
                entity.InventoryName = CodeTypeCollection.Instance.FetchCode(entity.InventoryCode, "vn", currentUser.TenantId).Title;
                entity.EntityTypeName = CodeTypeCollection.Instance.FetchCode(entity.EntityTypeCode, "vn",currentUser.TenantId).Title;
                entity.TransactionTypeName = CodeTypeCollection.Instance.FetchCode(entity.TransactionTypeCode, "vn",currentUser.TenantId).Title;
                entity.CreatedOnDate = DateTime.Now;
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.TotalQuantity = model.InventoryNoteItems.Sum(x => x.Quantity);
                

                _dbContext.sm_InventoryNote.Add(entity);
                
                // await _dbContext.SaveChangesAsync();
                
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    if (entity.TypeCode == InventoryNoteConstants.TypeCode.InventoryExport)
                    {
                        #region Cập nhật lại số lượng có thể bán
                        foreach (var item in entity.InventoryNoteItems)
                        {
                            var productEntity =
                                await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                            var productInventoryEntity =
                                await _dbContext.sm_ProductInventory.FirstOrDefaultAsync(x =>
                                    x.ProductId == item.ProductId && x.WarehouseCode == entity.InventoryCode);

                            if (productInventoryEntity != null)
                            {
                                productInventoryEntity.SellableQuantity -= item.Quantity;
                            }

                            if (productEntity != null)
                            {
                                productEntity.SellableQuantity -= item.Quantity;
                            }
                            
                            await _dbContext.SaveChangesAsync();
                        }
                        #endregion
                        
                        #region Log lại hoạt động thêm mới phiếu xuất kho vào bảng sm_ConstructionActivityLog
                        await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                        {
                            Description = "đã tạo phiếu xuất kho",
                            CodeLinkDescription = $"{entity.Code}",
                            OrderId = entity.Id,
                            ConstructionId = entity.ConstructionId,
                        }, currentUser);
                        #endregion
                    }
                }

                var result = await GetById(entity.Id);
                return result.IsSuccess
                            ? Helper.CreateSuccessResponse<InventoryNoteViewModel>(result.Data, "Thêm mới thành công")
                            : result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<InventoryNoteViewModel>(ex);
            }
        }
        
        public async Task<Response<InventoryNoteViewModel>> CreateInventoryExportAutomatic(InventoryNoteCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                // Check mã phiếu đã tồn tại với các phiếu khác hay chưa
                if (_dbContext.sm_InventoryNote.Any(x => x.Code == model.Code))
                    return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                        (string.Format("Mã phiếu {0} đã tồn tại!", model.Code));

                List<sm_Product> allInventoryNoteProducts = new List<sm_Product>();

                #region Validate

                // Validate Product Item Model
                if (model.InventoryNoteItems != null && model.InventoryNoteItems.Count > 0)
                {
                    // Fill tất cả products
                    var allProductIds = model.InventoryNoteItems.Select(x => x.ProductId).ToList();
                    allInventoryNoteProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Không được duplicate dòng trong Product Item
                    if (model.InventoryNoteItems.Count !=
                        model.InventoryNoteItems.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: giá nhập và số lượng không được nhỏ hơn 0
                    if (model.InventoryNoteItems.Any(x => x.Quantity <= 0))
                        return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn hoặc bằng 0.");

                    foreach (var item in model.InventoryNoteItems)
                    {
                        var product = allInventoryNoteProducts
                                            .Where(x => x.Id == item.ProductId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");

                        // if (product.InitialStockQuantity < item.Quantity && model.TypeCode == InventoryNoteConstants.TypeCode.InventoryExport)
                        //     return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                        //             ($"Danh sách sản phẩm có sản phẩm {product.Name} không đủ tồn kho");

                        if (product.IsOrder == false && model.TypeCode == InventoryNoteConstants.TypeCode.InventoryExport)
                            return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không cho phép bán");
                    }
                }
                #endregion

                // #region Check số lượng có thể bán và tồn trong kho của sản phẩm
                // if (model.TypeCode == InventoryNoteConstants.TypeCode.InventoryExport)
                // {
                //     foreach (var item in model.InventoryNoteItems)
                //     {
                //         var allStockTransactions =
                //             await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId, model.InventoryCode);
                //         var productInventoryEntity = await _dbContext.sm_ProductInventory.FirstOrDefaultAsync(x =>
                //             x.ProductId == item.ProductId && x.WarehouseCode == model.InventoryCode);
                //         
                //         if (allStockTransactions.Data != null && allStockTransactions.Data.Count > 0)
                //         {
                //             if (productInventoryEntity != null)
                //             {
                //                 if (productInventoryEntity.SellableQuantity < item.Quantity)
                //                     return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                //                         ($"Sản phẩm {allStockTransactions.Data[0].ProductName} có số lượng bán ra vượt quá số lượng có thể bán trong kho");
                //             } 
                //             
                //             if (allStockTransactions.Data[0].StockTransactionQuantity < item.Quantity)
                //                 return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                //                     ($"Sản phẩm {allStockTransactions.Data[0].ProductName} không đủ tồn trong kho");
                //         }
                //         else
                //         {
                //             var nameProduct = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Name;
                //             return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                //                 ($"Sản phẩm {nameProduct} có số lượng bán ra vượt quá số lượng có thể bán trong kho");
                //         }
                //     }
                // }
                // #endregion

                #region Fill Item's Line Number
                model.InventoryNoteItems = model.InventoryNoteItems.OrderBy(x => x.LineNumber).ToList();
                for (int i = 0; i < model.InventoryNoteItems.Count; i++)
                    model.InventoryNoteItems[i].LineNumber = i + 1;
                #endregion

                // Mapping và tạo entity
                var entity = _mapper.Map<sm_InventoryNote>(model);
                entity.Id = Guid.NewGuid();

                if (model.Code == null)
                {
                    entity.Code = await GetNewCodeInventoryNote(model.TypeCode);
                }
                
                entity.StatusCode = InventoryNoteConstants.StatusCode.DRAFT;
                entity.StatusName = InventoryNoteConstants.StatusName.DRAFT;
                entity.InventoryName = CodeTypeCollection.Instance.FetchCode(entity.InventoryCode, "vn", currentUser.TenantId).Title;
                entity.EntityTypeName = CodeTypeCollection.Instance.FetchCode(entity.EntityTypeCode, "vn", currentUser.TenantId).Title;
                entity.TransactionTypeName = CodeTypeCollection.Instance.FetchCode(entity.TransactionTypeCode, "vn",currentUser.TenantId).Title;
                entity.CreatedOnDate = DateTime.Now;
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.TotalQuantity = model.InventoryNoteItems.Sum(x => x.Quantity);

                _dbContext.sm_InventoryNote.Add(entity);
                
                var createResult = await _dbContext.SaveChangesAsync();
                
                if (createResult > 0)
                {
                    #region Log lại hoạt động thêm mới phiếu xuất kho vào bảng sm_ConstructionActivityLog
                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã tạo phiếu xuất kho",
                        CodeLinkDescription = $"{entity.Code}",
                        OrderId = entity.Id,
                        ConstructionId = entity.ConstructionId,
                    }, currentUser);
                    #endregion
                }

                var result = await GetById(entity.Id);
                return result.IsSuccess
                            ? Helper.CreateSuccessResponse<InventoryNoteViewModel>(result.Data, "Thêm mới thành công")
                            : result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<InventoryNoteViewModel>(ex);
            }
        }

        /// <summary>
        /// Method update inventory note by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<InventoryNoteViewModel>> Update(Guid id, InventoryNoteCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                // Check mã phiếu đã tồn tại với các phiếu khác hay chưa
                if (_dbContext.sm_InventoryNote.Any(x => x.Code == model.Code && x.Id != id))
                    return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                        (string.Format("Mã phiếu {0} đã tồn tại!", model.Code));

                List<sm_Product> allInventoryNoteProducts = new List<sm_Product>();

                // Select và validate entity
                var entity = await _dbContext.sm_InventoryNote
                    .Include(x => x.InventoryNoteItems)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<InventoryNoteViewModel>
                        ($"Phiếu không tồn tại trong hệ thống, id: {id}");

                // Validate: Chỉ cho phép sửa phiếu ở trạng thái Nháp
                if (entity.StatusCode != InventoryNoteConstants.StatusCode.DRAFT)
                    return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                        ("Chỉ cho phép sửa phiếu ở trạng thái Nháp");

                #region Validate
                
                // Validate MaterialRequestId
                if (model.MaterialRequestId != null)
                {
                    if (!await _dbContext.sm_MaterialRequest.AnyAsync(x => x.Id == model.MaterialRequestId))
                    {
                        return Helper.CreateNotFoundResponse<InventoryNoteViewModel>
                            ($"Id ${model.MaterialRequestId} không tồn tại");
                    }
                }
                
                //// Validate Project
                //if (!await _dbContext.mk_DuAn.AnyAsync(x => x.Id == model.ProjectId))
                //{
                //    return Helper.CreateNotFoundResponse<InventoryNoteViewModel>
                //        ($"Project Id: {model.ProjectId} not found.");
                //}

                if (model.InventoryNoteItems != null && model.InventoryNoteItems.Count > 0)
                {
                    // Fill tất cả products
                    var allProductIds = model.InventoryNoteItems.Select(x => x.ProductId).ToList();
                    allInventoryNoteProducts = await _dbContext.sm_Product.AsNoTracking()
                                            .Where(x => allProductIds.Contains(x.Id))
                                            .ToListAsync();

                    // Validate: Không được duplicate dòng trong Product Item
                    if (model.InventoryNoteItems.Count !=
                        model.InventoryNoteItems.Select(x => x.ProductId).Distinct().Count())
                        return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ("Danh sách sản phẩm không được có dòng trùng nhau.");

                    // Validate: Số lượng không được nhỏ hơn 0
                    if (model.InventoryNoteItems.Any(x => x.Quantity <= 0))
                        return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ("Danh sách sản phẩm không được có số lượng nhỏ hơn 0.");

                    foreach (var item in model.InventoryNoteItems)
                    {
                        var product = allInventoryNoteProducts
                                            .Where(x => x.Id == item.ProductId)
                                            .FirstOrDefault();
                        if (product == null)
                            return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm không tồn tại, id: {item.ProductId}");

                        if (product.InitialStockQuantity < item.Quantity)
                            return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không đủ tồn kho");

                        if (product.IsOrder == false)
                            return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                    ($"Danh sách sản phẩm có sản phẩm {product.Name} không cho phép bán");
                    }
                }
                #endregion
                
                // #region Check số lượng có thể bán và tồn trong kho của sản phẩm
                // if (model.TypeCode == InventoryNoteConstants.TypeCode.InventoryExport)
                // {
                //     foreach (var item in model.InventoryNoteItems)
                //     {
                //         var allStockTransactions =
                //             await _stockTransactionHandler.GetByIdAndWareCode(item.ProductId, model.InventoryCode);
                //         var productInventoryEntity = await _dbContext.sm_ProductInventory.FirstOrDefaultAsync(x =>
                //             x.ProductId == item.ProductId && x.WarehouseCode == model.InventoryCode);
                //
                //         if (allStockTransactions.Data != null && allStockTransactions.Data.Count > 0)
                //         {
                //             if (productInventoryEntity != null)
                //             {
                //                 if (productInventoryEntity.SellableQuantity < item.Quantity)
                //                     return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                //                         ($"Sản phẩm {allStockTransactions.Data[0].ProductName} có số lượng bán ra vượt quá số lượng có thể bán trong kho");
                //             }
                //
                //             if (allStockTransactions.Data[0].StockTransactionQuantity < item.Quantity)
                //                 return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                //                     ($"Sản phẩm {allStockTransactions.Data[0].ProductName} không đủ tồn trong kho");
                //         }
                //         else
                //         {
                //             var nameProduct = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.Name;
                //             return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                //                 ($"Sản phẩm {nameProduct} có số lượng bán ra vượt quá số lượng có thể bán trong kho");
                //         }
                //     }
                //     #endregion
                // }

                #region Validate Status Code (trạng thái phiếu)
                // Không cập nhật Original Document
                // Nếu phiếu ở trạng thái hoàn thành không cho cập nhật
                if (entity.StatusCode == InventoryNoteConstants.StatusCode.COMPLETED)
                    return Helper.CreateBadRequestResponse<InventoryNoteViewModel>
                                        ("Phiếu đã hoàn thành, không thể cập nhật.");
                // Chỉ cho phép sửa Code, Note khi phiếu ở trạng thái CANCELED
                if (entity.StatusCode == InventoryNoteConstants.StatusCode.CANCELLED)
                {
                    model.Code = entity.Code;
                    model.Note = entity.Note;
                }

                // Cho phép sửa hết khi phiếu ở trạng thái DRAFT
                if (entity.StatusCode == InventoryNoteConstants.StatusCode.DRAFT)
                {
                    entity.LastModifiedByUserId = currentUser.UserId;
                    entity.LastModifiedByUserName = $"{currentUser.FullName} ({currentUser.UserName})";
                    entity.LastModifiedOnDate = DateTime.Now;

                    if (model.Code == null)
                    {
                        entity.Code = entity.Code;
                    }
                    else
                    {
                        entity.Code = model.Code;
                    }
                    
                    if (model.EntityTypeCode == "employee")
                    {
                        entity.EntityName = _dbContext.IdmUser.FirstOrDefault(x => x.Id == model.EntityId).Name;
                    }

                    entity.EntityId = model.EntityId;
                    entity.EntityCode = model.EntityCode;
                    entity.EntityName = model.EntityName;
                    entity.EntityTypeCode = model.EntityTypeCode;
                    entity.EntityTypeName = CodeTypeCollection.Instance.FetchCode(entity.EntityTypeCode, "vn", entity.TenantId).Title;
                    entity.TransactionTypeCode = model.TransactionTypeCode;
                    entity.TransactionTypeName = CodeTypeCollection.Instance.FetchCode(entity.TransactionTypeCode, "vn", entity.TenantId).Title;
                    entity.TransactionDate = model.TransactionDate;
                    entity.InventoryCode = model.InventoryCode;
                    entity.InventoryName = CodeTypeCollection.Instance.FetchCode(entity.InventoryCode, "vn", entity.TenantId).Title;
                    entity.ProjectId = model.ProjectId;
                    entity.ProjectName = model.ProjectName;
                    entity.Note = model.Note;
                    entity.TotalQuantity = model.InventoryNoteItems.Sum(x => x.Quantity);
                    entity.ContractId = model.ContractId;
                    entity.ConstructionId = model.ConstructionId;

                    #region Fill Item's Line Number
                    model.InventoryNoteItems = model.InventoryNoteItems.OrderBy(x => x.LineNumber).ToList();
                    for (int i = 0; i < model.InventoryNoteItems.Count; i++)
                        model.InventoryNoteItems[i].LineNumber = i + 1;
                    #endregion

                    // Remove Old Inventory Note Item -> Re-add
                    _dbContext.RemoveRange(entity.InventoryNoteItems);
                    entity.InventoryNoteItems = new List<sm_InventoryNoteItem>();

                    #region Điền thông tin Product
                    // Fill Product Data
                    foreach (var modelItem in model.InventoryNoteItems)
                    {
                        // Re-add new item
                        var item = _mapper.Map<sm_InventoryNoteItem>(modelItem);
                        entity.InventoryNoteItems.Add(item);

                        // Assign InventoryNoteId
                        item.InventoryNoteId = entity.Id;

                        // Fill product data
                        var product = allInventoryNoteProducts
                                        .Where(x => x.Id == modelItem.ProductId)
                                        .FirstOrDefault();

                        item.ProductCode = product.Code;
                        item.ProductName = product.Name;
                        item.Unit = product.Unit;
                    }
                    #endregion

                    foreach (var item in entity.InventoryNoteItems)
                    {
                        _dbContext.sm_InventoryNoteItem.Add(item);
                    }
                }
                #endregion
                
                _dbContext.sm_InventoryNote.Update(entity);
                
                await _dbContext.SaveChangesAsync();
                
                var result = await GetById(entity.Id);

                return result.IsSuccess
                            ? Helper.CreateSuccessResponse<InventoryNoteViewModel>(result.Data, "Chỉnh sửa thành công")
                            : result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Model: {@model}", id, model);
                return Helper.CreateExceptionResponse<InventoryNoteViewModel>(ex);
            }
        }

        /// <summary>
        /// Method get page inventory note by query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Response<Pagination<InventoryNoteViewModel>>> GetPage(InventoryNoteQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_InventoryNote
                    .AsNoTracking()
                    .Include(x => x.InventoryNoteItems.OrderBy(x => x.LineNumber))
                    .Include(x => x.sm_Contract)
                    .Include(x => x.sm_Construction)
                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<InventoryNoteViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<InventoryNoteViewModel>>(ex);
            }
        }

        /// <summary>
        /// Method delete multiple inventory note by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<Response> DeleteMultipleAsync(List<Guid> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _dbContext.sm_InventoryNote.Where(x => x.Id == id).FirstOrDefaultAsync();
                    var entities = _dbContext.sm_InventoryNote.Where(x => ids.Contains(x.Id)).ToList();
                    if (entities == null | entities.Count == 0)
                        return Helper.CreateNotFoundResponse("Không tìm thấy phiếu");

                    if (entity.StatusCode == InventoryNoteConstants.StatusCode.COMPLETED ||
                        entity.StatusCode == InventoryNoteConstants.StatusCode.CANCELLED)
                        return Helper.CreateBadRequestResponse("Không được xóa các phiếu “Hoàn thành, Đã hủy”");

                    _dbContext.RemoveRange(entities);
                }
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(string.Format("Xóa {0} phiếu thành công.", ids.Count));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, ByUserIds: {@requestUserId}");
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        /// <summary>
        /// Method get inventory note by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<InventoryNoteViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_InventoryNote
                    .AsNoTracking()
                    .Include(x => x.InventoryNoteItems.OrderBy(x => x.LineNumber))
                    .Include(x => x.sm_Contract)
                    .Include(x => x.sm_Construction)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<InventoryNoteViewModel>("Phiếu không tồn tại trong hệ thống.");

                var result = _mapper.Map<InventoryNoteViewModel>(entity);

                return new Response<InventoryNoteViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<InventoryNoteViewModel>(ex);
            }
        }

        /// <summary>
        /// Method get new code inventory note
        /// </summary>
        /// <param name="defaultPrefix"></param>
        /// <returns></returns>
        private async Task<string> GetNewCodeInventoryNote(string type)
        {
            try
            {
                var prefixCode = "";
                if (type == InventoryNoteConstants.TypeCode.InventoryImport)
                {
                    prefixCode = InventoryNoteConstants.Code.PrefixInventoryImport;
                }
                else prefixCode = InventoryNoteConstants.Code.PrefixInventoryExport;

                var code = prefixCode + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_InventoryNote.AsNoTracking().Where(s => s.Code.Contains(code)).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();


                if (result != null)
                {
                    var currentNum = result.Code.Substring(result.Code.Length - 3, 3);
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
        /// Method build query for get page inventory note
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private Expression<Func<sm_InventoryNote, bool>> BuildQuery(InventoryNoteQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            
            var predicate = PredicateBuilder.New<sm_InventoryNote>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.EntityTypeName.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.EntityTypeCodes != null && query.EntityTypeCodes.Count() > 0)
            {
                predicate.And(x => query.EntityTypeCodes.Contains(x.EntityTypeCode));
            }

            if (!string.IsNullOrEmpty(query.TypeCode))
            {
                predicate.And(s => s.TypeCode == query.TypeCode);
            }

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

            if (query.TypeCode == InventoryNoteConstants.TypeCode.InventoryImport)
            {
                if (currentUser.ListRights.Contains("INVENTORYIMPORT." + RightActionConstants.VIEWALL))
                    predicate.And(s => s.CreatedByUserId == currentUser.UserId);
            }
            else
            {
                if (currentUser.ListRights.Contains("INVENTORYEXPORT." + RightActionConstants.VIEWALL))
                    predicate.And(s => s.CreatedByUserId == currentUser.UserId);
            }
            
            if (query.ConstructionId.HasValue)
            {
                predicate.And(x => x.ConstructionId == query.ConstructionId);
            }
            
            return predicate;
        }

        /// <summary>
        /// Method cancel multiple inventory note by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<Response> CancelMultipleAsync(List<Guid> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var entity = await _dbContext.sm_InventoryNote.Where(x => x.Id == id)
                        .Include(x => x.InventoryNoteItems)
                        .FirstOrDefaultAsync();
                    var entities = _dbContext.sm_InventoryNote.Where(x => ids.Contains(x.Id)).ToList();
                    if (entities == null | entities.Count == 0)
                        return Helper.CreateNotFoundResponse("Không tìm thấy phiếu");

                    if (entity.StatusCode == InventoryNoteConstants.StatusCode.COMPLETED ||
                        entity.StatusCode == InventoryNoteConstants.StatusCode.CANCELLED)
                        return Helper.CreateBadRequestResponse("Không được hủy các phiếu “Hoàn thành, Đã hủy”");
                    
                    if (entity.TypeCode == InventoryNoteConstants.TypeCode.InventoryExport)
                    {
                        foreach (var item in entity.InventoryNoteItems)
                        {
                            var productEntity =
                                await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                            var productInventoryEntity = await _dbContext.sm_ProductInventory
                                .FirstOrDefaultAsync(x =>
                                    x.ProductId == item.ProductId && x.WarehouseCode == entity.InventoryCode);

                            if (productInventoryEntity != null)
                            {
                                productInventoryEntity.SellableQuantity += item.Quantity;
                            }
                            
                            if (productEntity != null)
                            {
                                productEntity.SellableQuantity += item.Quantity;
                            }
                            
                            await _dbContext.SaveChangesAsync();
                        }
                    }

                    entity.StatusCode = InventoryNoteConstants.StatusCode.CANCELLED;
                    entity.StatusName = InventoryNoteConstants.StatusName.CANCELLED;
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
        /// // Method import/export inventory by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<Response> InventoryTransactionAsync(List<Guid> ids, RequestUser currentUser)
        {
            try
            {
                var isInventoryImportType = true;
                
                List<jsonb_HistoryProcess> allHistoryProcess = new List<jsonb_HistoryProcess>();

                var entities = _dbContext.sm_InventoryNote.Where(x => ids.Contains(x.Id)).ToList();
                if (entities == null | entities.Count == 0)
                    return Helper.CreateNotFoundResponse("Không tìm thấy phiếu");

                foreach (var id in ids)
                {
                    var entity = await _dbContext.sm_InventoryNote.Include(x => x.InventoryNoteItems).Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (entity.StatusCode == InventoryNoteConstants.StatusCode.COMPLETED ||
                        entity.StatusCode == InventoryNoteConstants.StatusCode.CANCELLED)
                        return Helper.CreateBadRequestResponse(
                            entity.TypeCode == InventoryNoteConstants.TypeCode.InventoryImport
                                ? "Không được nhập các phiếu “Hoàn thành, Đã hủy”"
                                : "Không được xuất các phiếu “Hoàn thành, Đã hủy”"
                        );

                    entity.StatusCode = InventoryNoteConstants.StatusCode.COMPLETED;
                    entity.StatusName = InventoryNoteConstants.StatusName.COMPLETED;

                    if (entity.TypeCode == InventoryNoteConstants.TypeCode.InventoryImport)
                    {
                        // Message = "Nhập phiếu thành công";
                        isInventoryImportType = true;

                        // Create Stock Transaction
                        if (entity.InventoryNoteItems.Count != 0)
                        {
                            foreach (var item in entity.InventoryNoteItems)
                            {
                                var newProduct = new StockTransactionCreateModel
                                {
                                    ProductId = item.ProductId,
                                    ProductCode = item.ProductCode,
                                    ProductName = item.ProductName,
                                    // SellableQuantity = item.Quantity,
                                    InitialStockQuantity = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.InitialStockQuantity + item.Quantity,
                                    WareCode = entity.InventoryCode,
                                    OriginalDocumentCode = entity.Code,
                                    OriginalDocumentType = OriginalDocumentTypeConstants.NHAP_HANG,
                                    UnitPrice = 0,
                                    Unit = item.Unit,
                                    ReceiptInventoryQuantity = item.Quantity,
                                    ExportInventoryQuantity = 0,
                                    Action = ActionConstants.RECEIPT_ORDER,
                                    CreatedByUserId = entity.CreatedByUserId,
                                };
                                await _stockTransactionHandler.Create(newProduct);

                                // Update product quantity in sm_Product

                                var product = await _dbContext.sm_Product.Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                                var productInventoryByIdAndWareCode = await _dbContext.sm_ProductInventory
                                    .Where(x => x.ProductId == item.ProductId && x.WarehouseCode == entity.InventoryCode)
                                    .FirstOrDefaultAsync();
                                var productInventory = await _dbContext.sm_ProductInventory
                                    .Where(x => x.ProductId == item.ProductId)
                                    .FirstOrDefaultAsync();
                                
                                if (product == null)
                                {
                                    return Helper.CreateNotFoundResponse("Sản phẩm không tồn tại trong hệ thống");
                                }
                                product.InitialStockQuantity += item.Quantity;
                                product.SellableQuantity += item.Quantity;
                                _dbContext.sm_Product.Update(product);
                                
                                #region Check khi có tồn tại bản ghi có chứa wareCode thì update, còn không thì tạo bản ghi mới
                                var productInventoryEntity = _dbContext.sm_ProductInventory
                                    .Where(x => x.ProductId == item.ProductId);
                        
                                if (productInventoryEntity.Any(x => x.WarehouseCode == entity.InventoryCode))
                                {
                                    if (productInventoryByIdAndWareCode != null)
                                    {
                                        productInventoryByIdAndWareCode.SellableQuantity += item.Quantity;
                                        _dbContext.sm_ProductInventory.Update(productInventoryByIdAndWareCode);
                                    }
                                    else
                                    {
                                        productInventory.SellableQuantity += item.Quantity;
                                        productInventory.WarehouseCode = entity.InventoryCode;
                                        _dbContext.sm_ProductInventory.Update(productInventory);
                                    }
                                }
                                else
                                {
                                    await _productInventoryHandler.Create(new ProductInventoryCreateModel
                                    {
                                        WarehouseCode = entity.InventoryCode,
                                        SellableQuantity = item.Quantity,
                                        ProductId = item.ProductId,
                                    }, currentUser);
                                }
                                #endregion
                            }
                        }
                    }

                    if (entity.TypeCode == InventoryNoteConstants.TypeCode.InventoryExport)
                    {
                        // Message = "Xuất phiếu thành công";
                        isInventoryImportType = false;

                        // Create Stock Transaction
                        if (entity.InventoryNoteItems.Count != 0)
                        {
                            foreach (var item in entity.InventoryNoteItems)
                            {
                                var newProduct = new StockTransactionCreateModel
                                {
                                    ProductId = item.ProductId,
                                    ProductCode = item.ProductCode,
                                    ProductName = item.ProductName,
                                    WareCode = entity.InventoryCode,
                                    InitialStockQuantity = _dbContext.sm_Product.FirstOrDefault(x => x.Id == item.ProductId)?.InitialStockQuantity - item.Quantity,
                                    OriginalDocumentCode = entity.Code,
                                    OriginalDocumentType = OriginalDocumentTypeConstants.XUAT_KHO,
                                    UnitPrice = 0,
                                    Unit = item.Unit,
                                    ReceiptInventoryQuantity = 0,
                                    ExportInventoryQuantity = item.Quantity,
                                    Action = ActionConstants.EXPORT_ORDER,
                                    CreatedByUserId = entity.CreatedByUserId,
                                };
                                await _stockTransactionHandler.Create(newProduct);

                                // Update product quantity in sm_Product

                                var product = await _dbContext.sm_Product.Where(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                                if (product == null)
                                {
                                    return Helper.CreateNotFoundResponse("Sản phẩm không tồn tại trong hệ thống");
                                }
                                product.InitialStockQuantity -= item.Quantity;
                                // product.SellableQuantity -= item.Quantity;
                                _dbContext.sm_Product.Update(product);
                            }
                        }
                        
                        #region Xuất phiếu thành công sẽ cập nhật lại lịch sử xử lý và trạng thái trong yêu cầu vật tư
                        var materialRequestEntity =
                            await _dbContext.sm_MaterialRequest.FirstOrDefaultAsync(x => x.Id == entity.MaterialRequestId);

                        if (materialRequestEntity != null)
                        {
                            foreach (var item in materialRequestEntity.ListHistoryProcess)
                            {
                                allHistoryProcess.Add(item);
                            }

                            var newHistoryProcess = new jsonb_HistoryProcess()
                            {
                                UserName =  currentUser.FullName,
                                Description = "đã xuất kho thành công từ yêu cầu",
                                CreatedOnDate =  DateTime.Now,
                            };

                            allHistoryProcess.Add(newHistoryProcess);
                            materialRequestEntity.ListHistoryProcess = allHistoryProcess;
                            materialRequestEntity.StatusCode = MaterialRequestConstants.StatusCode.COMPLETED;
                            _dbContext.sm_MaterialRequest.Update(materialRequestEntity);
                        }
                        #endregion
                    }
                    await _dbContext.SaveChangesAsync();
                }
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(string.Format("{0} {1} phiếu thành công.", isInventoryImportType ? "Nhập" : "Xuất", ids.Count));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, ByUserIds: {@requestUserId}");
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        /// <summary>
        /// Method create InventoryNote when export sales order completed by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response> CreateExportSalesOrder(Guid id, RequestUser currentUser)
        {
            try
            {
                var salesOrderEntity = await _dbContext.sm_SalesOrder.AsNoTracking()
                    .Include(x => x.sm_Construction)
                    .Include(x => x.sm_Contract)
                    .Include(x => x.SalesOrderItems)
                    .Include(x => x.sm_Customer)
                    .Include(x => x.mk_DuAn)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (salesOrderEntity == null)
                    return Helper.CreateNotFoundResponse("Đơn bán hàng không tồn tại trong hệ thống.");

                // Validation: Only allow creating an export warehouse receipt for orders with the status "XUAT_KHO"
                if (salesOrderEntity.ExportStatusCode == OriginalDocumentTypeConstants.XUAT_KHO)
                {
                    var newInventoryNote = new InventoryNoteCreateUpdateModel
                    {
                        EntityId = salesOrderEntity.CustomerId,
                        ProjectId = salesOrderEntity.ProjectId,
                        EntityCode = salesOrderEntity.sm_Customer.Code,
                        EntityName = salesOrderEntity.sm_Customer.Name,
                        EntityTypeCode = InventoryNoteConstants.EntityTypeCode.CUSTOMER,
                        EntityTypeName = InventoryNoteConstants.EntityTypeName.CUSTOMER,
                        ConstructionId =  salesOrderEntity.ConstructionId,
                        ContractId = salesOrderEntity.ContractId,
                        OriginalDocumentId = salesOrderEntity.Id,
                        OriginalDocumentCode = salesOrderEntity.OrderCode,
                        OriginalDocumentType = InventoryNoteConstants.OriginalDocumentType.SALES_ORDER,
                        TransactionTypeCode = InventoryNoteConstants.TransactionTypeCode.SALES_GOODS_EXPORT,
                        TransactionDate = salesOrderEntity.LastModifiedOnDate,
                        InventoryCode = salesOrderEntity.WareCode,
                        Note = salesOrderEntity.Note,
                        TypeCode = InventoryNoteConstants.TypeCode.InventoryExport,
                        InventoryNoteItems = salesOrderEntity.SalesOrderItems.Select(x => new InventoryNoteItemCreateUpdateModel
                        {
                            LineNumber = x.LineNo,
                            ProductId = x.ProductId,
                            ProductCode = x.ProductCode,
                            ProductName = x.ProductName,
                            Unit = x.Unit,
                            Quantity = x.Quantity,
                            Note = x.Note,
                        }).ToList(),
                        CreatedByUserId = salesOrderEntity.CreatedByUserId,
                    };

                    // Create Inventory Note
                    var result = await CreateInventoryExportAutomatic(newInventoryNote, currentUser);

                    var inventoryNoteFirst = _dbContext.sm_InventoryNote.Where(x => x.Id == result.Data.Id).FirstOrDefault();
                    if (inventoryNoteFirst != null)
                    {
                        inventoryNoteFirst.StatusCode = InventoryNoteConstants.StatusCode.COMPLETED;
                        inventoryNoteFirst.StatusName = InventoryNoteConstants.StatusName.COMPLETED;
                    }
                }

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: id: {@id}", id);
                return Helper.CreateExceptionResponse<InventoryNoteViewModel>(ex);
            }
        }

        /// <summary>
        /// Create InventoryNote when import purchase order completed by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response> CreateImportPurchaseOrder(Guid id, RequestUser currentUser)
        {
            try
            {
                var purchaseOrderEntity = await _dbContext.sm_PurchaseOrder.AsNoTracking()
                    .Include(x => x.sm_Construction)
                    .Include(x => x.sm_Contract)
                    .Include(x => x.Items)
                    .Include(x => x.sm_Supplier)
                    .Include(x => x.mk_DuAn)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (purchaseOrderEntity == null)
                    return Helper.CreateNotFoundResponse("Đơn nhập hàng không tồn tại trong hệ thống.");

                // Validation: Only allow creating a warehouse receipt for orders with the status "NHAP_HANG"
                if (purchaseOrderEntity.ImportStatusCode == OriginalDocumentTypeConstants.NHAP_HANG)
                {
                    var newInventoryNote = new InventoryNoteCreateUpdateModel
                    {
                        EntityId = purchaseOrderEntity.SupplierId,
                        ProjectId = purchaseOrderEntity.ProjectId,
                        EntityCode = purchaseOrderEntity.sm_Supplier.Code,
                        EntityName = purchaseOrderEntity.sm_Supplier.Name,
                        ConstructionId = purchaseOrderEntity.ConstructionId,
                        ContractId = purchaseOrderEntity.ContractId,
                        EntityTypeCode = InventoryNoteConstants.EntityTypeCode.SUPPLIER,
                        EntityTypeName = InventoryNoteConstants.EntityTypeName.SUPPLIER,
                        OriginalDocumentId = purchaseOrderEntity.Id,
                        OriginalDocumentCode = purchaseOrderEntity.OrderCode,
                        OriginalDocumentType = InventoryNoteConstants.OriginalDocumentType.PURCHASE_ORDER,
                        TransactionTypeCode = InventoryNoteConstants.TransactionTypeCode.PURCHASE_GOODS_IMPORT,
                        TransactionDate = purchaseOrderEntity.LastModifiedOnDate,
                        InventoryCode = purchaseOrderEntity.WareCode,
                        Note = purchaseOrderEntity.Note,
                        TypeCode = InventoryNoteConstants.TypeCode.InventoryImport,
                        InventoryNoteItems = purchaseOrderEntity.Items.Select(x => new InventoryNoteItemCreateUpdateModel
                        {
                            LineNumber = x.LineNo,
                            ProductId = x.ProductId,
                            ProductCode = x.ProductCode,
                            ProductName = x.ProductName,
                            Unit = x.Unit,
                            Quantity = x.Quantity,
                            Note = x.Note
                        }).ToList(),
                        CreatedByUserId = purchaseOrderEntity.CreatedByUserId,
                    };

                    // Create Inventory Note
                    var result = await Create(newInventoryNote, currentUser);

                    var inventoryNoteFirst = _dbContext.sm_InventoryNote.Where(x => x.Id == result.Data.Id).FirstOrDefault();
                    if (inventoryNoteFirst != null)
                    {
                        inventoryNoteFirst.StatusCode = InventoryNoteConstants.StatusCode.COMPLETED;
                        inventoryNoteFirst.StatusName = InventoryNoteConstants.StatusName.COMPLETED;
                    }
                }

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: id: {@id}", id);
                return Helper.CreateExceptionResponse<InventoryNoteViewModel>(ex);
            }
        }

        public async Task<Response> CreateCustomerReturn(Guid id, RequestUser currentUser)
        {
            try
            {
                var customerReturnEntity = await _dbContext.sm_Return_Order.AsNoTracking()
                    .Include(x => x.sm_Construction)
                    .Include(x => x.sm_Contract)
                    .Include(x => x.OrderItems)
                    .Include(x => x.mk_DuAn)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (customerReturnEntity == null)
                    return Helper.CreateNotFoundResponse("Đơn trả hàng khách hàng không tồn tại trong hệ thống.");

                // Validation: Only allow creating a warehouse receipt for orders with the status "returned"
                if (customerReturnEntity.StatusCode == OrderReturnConstants.StatusCode.RETURNED &&
                    customerReturnEntity.EntityTypeCode == OrderReturnConstants.EntityTypeCode.CUSTOMER)
                {
                    var newInventoryNote = new InventoryNoteCreateUpdateModel
                    {
                        EntityId = customerReturnEntity.EntityId,
                        ProjectId = customerReturnEntity.ProjectId,
                        EntityCode = customerReturnEntity.EntityCode,
                        EntityName = customerReturnEntity.EntityName,
                        ConstructionId = customerReturnEntity.ConstructionId,
                        ContractId =  customerReturnEntity.ContractId,
                        EntityTypeCode = InventoryNoteConstants.EntityTypeCode.CUSTOMER,
                        EntityTypeName = InventoryNoteConstants.EntityTypeName.CUSTOMER,
                        OriginalDocumentId = customerReturnEntity.Id,
                        OriginalDocumentCode = customerReturnEntity.OrderCode,
                        OriginalDocumentType = InventoryNoteConstants.OriginalDocumentType.CUSTOMER_RETURN,
                        TransactionTypeCode = InventoryNoteConstants.TransactionTypeCode.RETURN_GOODS_IMPORT,
                        TransactionDate = customerReturnEntity.LastModifiedOnDate,
                        InventoryCode = customerReturnEntity.WareCode,
                        Note = customerReturnEntity.Note,
                        TypeCode = InventoryNoteConstants.TypeCode.InventoryImport,
                        InventoryNoteItems = customerReturnEntity.OrderItems.Select(x => new InventoryNoteItemCreateUpdateModel
                        {
                            LineNumber = x.LineNo,
                            ProductId = x.ProductId,
                            ProductCode = x.ProductCode,
                            ProductName = x.ProductName,
                            Unit = x.Unit,
                            Quantity = x.InitialQuantity,
                            Note = null
                        }).ToList(),
                        CreatedByUserId = customerReturnEntity.CreatedByUserId,
                    };

                    // Create Inventory Note
                    var result = await Create(newInventoryNote, currentUser);

                    var inventoryNoteFirst = _dbContext.sm_InventoryNote.Where(x => x.Id == result.Data.Id).FirstOrDefault();
                    if (inventoryNoteFirst != null)
                    {
                        inventoryNoteFirst.StatusCode = InventoryNoteConstants.StatusCode.COMPLETED;
                        inventoryNoteFirst.StatusName = InventoryNoteConstants.StatusName.COMPLETED;
                    }
                }

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: id: {@id}", id);
                return Helper.CreateExceptionResponse<InventoryNoteViewModel>(ex);
            }
        }

        public async Task<Response> CreateSupplierReturn(Guid id, RequestUser currentUser)
        {
            try
            {
                var supplierReturnEntity = await _dbContext.sm_Return_Order.AsNoTracking()
                    .Include(x => x.sm_Construction)
                    .Include(x => x.sm_Contract)
                    .Include(x => x.OrderItems)
                    .Include(x => x.mk_DuAn)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (supplierReturnEntity == null)
                    return Helper.CreateNotFoundResponse("Đơn trả hàng nhà cung cấp không tồn tại trong hệ thống.");

                // Validation: Only allow creating a warehouse receipt for orders with the status "returned"
                if (supplierReturnEntity.StatusCode == OrderReturnConstants.StatusCode.RETURNED
                    && supplierReturnEntity.EntityTypeCode == OrderReturnConstants.EntityTypeCode.SUPPLIER)
                {
                    var newInventoryNote = new InventoryNoteCreateUpdateModel
                    {
                        EntityId = supplierReturnEntity.EntityId,
                        ProjectId = supplierReturnEntity.ProjectId,
                        EntityCode = supplierReturnEntity.EntityCode,
                        EntityName = supplierReturnEntity.EntityName,
                        ConstructionId =  supplierReturnEntity.ConstructionId,
                        ContractId =  supplierReturnEntity.ContractId,
                        EntityTypeCode = InventoryNoteConstants.EntityTypeCode.SUPPLIER,
                        EntityTypeName = InventoryNoteConstants.EntityTypeName.SUPPLIER,
                        OriginalDocumentId = supplierReturnEntity.Id,
                        OriginalDocumentCode = supplierReturnEntity.OrderCode,
                        OriginalDocumentType = InventoryNoteConstants.OriginalDocumentType.SUPPLIER_RETURN,
                        TransactionTypeCode = InventoryNoteConstants.TransactionTypeCode.RETURN_GOODS_EXPORT,
                        TransactionDate = supplierReturnEntity.LastModifiedOnDate,
                        InventoryCode = supplierReturnEntity.WareCode,
                        Note = supplierReturnEntity.Note,
                        TypeCode = InventoryNoteConstants.TypeCode.InventoryExport,
                        InventoryNoteItems = supplierReturnEntity.OrderItems.Select(x => new InventoryNoteItemCreateUpdateModel
                        {
                            LineNumber = x.LineNo,
                            ProductId = x.ProductId,
                            ProductCode = x.ProductCode,
                            ProductName = x.ProductName,
                            Unit = x.Unit,
                            Quantity = x.InitialQuantity,
                            Note = null
                        }).ToList(),
                        CreatedByUserId = supplierReturnEntity.CreatedByUserId,
                    };
                 
                    // Create Inventory Note
                    var result = await CreateInventoryExportAutomatic(newInventoryNote, currentUser);

                    if (!result.IsSuccess)
                    {
                        return Helper.CreateBadRequestResponse(result.Message);
                    }

                    var inventoryNoteFirst = _dbContext.sm_InventoryNote.Where(x => x.Id == result.Data.Id).FirstOrDefault();
                    if (inventoryNoteFirst != null)
                    {
                        inventoryNoteFirst.StatusCode = InventoryNoteConstants.StatusCode.COMPLETED;
                        inventoryNoteFirst.StatusName = InventoryNoteConstants.StatusName.COMPLETED;
                    }
                }

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: id: {@id}", id);
                return Helper.CreateExceptionResponse<InventoryNoteViewModel>(ex);
            }
        }

        /// <summary>
        /// Export Excel List of Inventory Notes (either import or export)
        /// </summary>
        /// <param name="type">Loại phiếu, có thể là "InventoryImport" hoặc "InventoryExport"</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<string>> ExportExcelList(string type)
        {
            try
            {
                // Kiểm tra loại phiếu hợp lệ
                if (type != InventoryNoteConstants.TypeCode.InventoryImport && type != InventoryNoteConstants.TypeCode.InventoryExport)
                {
                    return Helper.CreateBadRequestResponse<string>("Loại phiếu không hợp lệ.");
                }

                // Kiểm tra loại phiếu (nhập hoặc xuất)
                bool isInventoryImport = type == InventoryNoteConstants.TypeCode.InventoryImport;

                // Lấy danh sách phiếu nhập hoặc phiếu xuất từ cơ sở dữ liệu
                var inventoryNotes = await _dbContext.sm_InventoryNote.AsNoTracking()
                    .Include(x => x.InventoryNoteItems)
                    .Where(x => x.TypeCode == (isInventoryImport
                        ? InventoryNoteConstants.TypeCode.InventoryImport
                        : InventoryNoteConstants.TypeCode.InventoryExport))
                    .ToListAsync();

                if (inventoryNotes == null || inventoryNotes.Count == 0)
                    return Helper.CreateNotFoundResponse<string>("Không có phiếu nào tồn tại trong hệ thống.");

                // Đặt tên file và đường dẫn template dựa trên loại phiếu
                var fileName = isInventoryImport
                    ? $"InventoryImport_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                    : $"InventoryExport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(_staticsFolder, fileName);

                // Kiểm tra template path
                var templatePath = Path.Combine(_staticsFolder, isInventoryImport
                    ? "excel-template/InventoryImportTemplate.xlsx"
                    : "excel-template/InventoryExportTemplate.xlsx");

                if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                    return Helper.CreateBadRequestResponse<string>("Không tìm thấy file template");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // Mở file template excel và điền dữ liệu vào file
                using (var package = new ExcelPackage(new FileInfo(templatePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Giả sử dữ liệu trong worksheet đầu tiên

                    // Đặt độ rộng cố định cho cột A (cột STT) là 8
                    worksheet.Column(1).Width = 8;

                    // Điền tiêu đề bảng vào Excel (giả sử bắt đầu từ hàng thứ 4)
                    int startRow = 4;
                    int index = 1;

                    foreach (var note in inventoryNotes)
                    {
                        worksheet.Cells[startRow, 1].Value = index; // STT
                        worksheet.Cells[startRow, 2].Value = note.CreatedOnDate.ToString("dd/MM/yyyy"); // Ngày tạo
                        worksheet.Cells[startRow, 3].Value = note.TransactionDate?.ToString("dd/MM/yyyy"); // Ngày nhập/xuất kho
                        worksheet.Cells[startRow, 4].Value = note.LastModifiedOnDate?.ToString("dd/MM/yyyy"); // Ngày cập nhật
                        worksheet.Cells[startRow, 5].Value = CodeTypeCollection.Instance.FetchCode(note.TransactionTypeCode, "vn", note.TenantId)?.Title ?? null; // Loại phiếu
                        worksheet.Cells[startRow, 6].Value = note.Code; // Mã phiếu
                        worksheet.Cells[startRow, 7].Value = note.StatusName; // Trạng thái
                        worksheet.Cells[startRow, 8].Value = note.TotalQuantity;
                        worksheet.Cells[startRow, 9].Value = CodeTypeCollection.Instance.FetchCode(note.InventoryCode, "vn", note.TenantId)?.Title ?? null; // Kho
                        worksheet.Cells[startRow, 10].Value = note.CreatedByUserName; // Người tạo
                        worksheet.Cells[startRow, 11].Value = note.EntityCode; // Mã đối tượng
                        worksheet.Cells[startRow, 12].Value = note.EntityName; // Tên đối tượng
                        worksheet.Cells[startRow, 13].Value = note.EntityTypeName; // Nhóm đối tượng
                        worksheet.Cells[startRow, 14].Value = note.OriginalDocumentCode; // Chứng từ gốc
                        worksheet.Cells[startRow, 15].Value = note.ProjectName; // Tên dự án
                        worksheet.Cells[startRow, 16].Value = note.Note; // Ghi chú

                        startRow++;
                        index++;
                    }

                    // Thêm đường viền cho các ô đã điền dữ liệu
                    int lastDataRow = startRow - 1; // Hàng cuối cùng có dữ liệu
                    using (var range = worksheet.Cells[4, 1, lastDataRow, 16]) // điều chỉnh cột cuối (16) tùy theo số lượng cột bạn có
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

                    // Auto-fit các cột từ cột thứ hai đến cột cuối cùng
                    worksheet.Cells[1, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].AutoFitColumns();

                    // Lưu file Excel đã điền dữ liệu
                    await package.SaveAsAsync(new FileInfo(filePath));
                }

                return Helper.CreateSuccessResponse<string>(filePath, "Xuất file thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<string>(ex);
            }
        }

        /// <summary>
        /// Xuất danh sách phiếu kho thành file Excel dựa trên phân trang và lọc của trang hiện tại
        /// </summary>
        /// <param name="type">Loại phiếu, có thể là "inventory_import" hoặc "inventory_export"</param>
        /// <param name="query">Các tham số lọc và phân trang</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<string>> ExportExcelListCurrentPage(string type, InventoryNoteQueryModel query)
        {
            try
            {
                // Kiểm tra loại phiếu hợp lệ
                if (type != InventoryNoteConstants.TypeCode.InventoryImport && type != InventoryNoteConstants.TypeCode.InventoryExport)
                {
                    return Helper.CreateBadRequestResponse<string>("Loại phiếu không hợp lệ.");
                }

                // Xác định loại phiếu (nhập hoặc xuất)
                bool isInventoryImport = type == InventoryNoteConstants.TypeCode.InventoryImport;

                // Tạo predicate để lọc dựa trên các tham số trong query
                var predicate = BuildQuery(query);

                // Lấy danh sách phiếu nhập hoặc phiếu xuất từ cơ sở dữ liệu dựa trên lọc và phân trang
                var inventoryNotes = await _dbContext.sm_InventoryNote.AsNoTracking()
                    .Include(x => x.InventoryNoteItems)
                    .Where(x => x.TypeCode == (isInventoryImport
                        ? InventoryNoteConstants.TypeCode.InventoryImport
                        : InventoryNoteConstants.TypeCode.InventoryExport))
                    .Where(predicate)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .GetPageAsync(query);

                // Kiểm tra nếu không có dữ liệu trả về trong trang hiện tại
                if (inventoryNotes == null || inventoryNotes.Content == null || inventoryNotes.Content.Count == 0)
                    return Helper.CreateNotFoundResponse<string>("Không có phiếu nào tồn tại trong hệ thống.");

                // Đặt tên file và đường dẫn template dựa trên loại phiếu
                var fileName = isInventoryImport
                    ? $"InventoryImport_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                    : $"InventoryExport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                var filePath = Path.Combine(_staticsFolder, fileName);

                // Xác định đường dẫn template dựa trên loại phiếu
                var templatePath = Path.Combine(_staticsFolder, isInventoryImport
                    ? "excel-template/InventoryImportTemplate.xlsx"
                    : "excel-template/InventoryExportTemplate.xlsx");

                if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                    return Helper.CreateBadRequestResponse<string>("Không tìm thấy file template");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Mở template Excel và điền dữ liệu vào file
                using (var package = new ExcelPackage(new FileInfo(templatePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Giả sử dữ liệu nằm trong worksheet đầu tiên

                    // Đặt độ rộng cố định cho cột A (cột STT) là 8
                    worksheet.Column(1).Width = 8;

                    // Điền dữ liệu vào bảng Excel (giả sử bắt đầu từ hàng thứ 4)
                    int startRow = 4;
                    int index = 1;

                    foreach (var note in inventoryNotes.Content)
                    {
                        worksheet.Cells[startRow, 1].Value = index; // STT
                        worksheet.Cells[startRow, 2].Value = note.CreatedOnDate.ToString("dd/MM/yyyy"); // Ngày tạo
                        worksheet.Cells[startRow, 3].Value = note.TransactionDate?.ToString("dd/MM/yyyy"); // Ngày nhập/xuất kho
                        worksheet.Cells[startRow, 4].Value = note.LastModifiedOnDate?.ToString("dd/MM/yyyy"); // Ngày cập nhật
                        worksheet.Cells[startRow, 5].Value = note.TransactionTypeName; // Loại phiếu 
                        worksheet.Cells[startRow, 6].Value = note.Code; // Mã phiếu
                        worksheet.Cells[startRow, 7].Value = note.StatusName; // Trạng thái
                        worksheet.Cells[startRow, 8].Value = note.InventoryNoteItems.Sum(x => x.Quantity); // Tổng số lượng
                        worksheet.Cells[startRow, 9].Value = note.InventoryName; // Kho
                        worksheet.Cells[startRow, 10].Value = note.CreatedByUserName; // Người tạo
                        worksheet.Cells[startRow, 11].Value = note.EntityCode; // Mã đối tượng
                        worksheet.Cells[startRow, 12].Value = note.EntityName; // Tên đối tượng
                        worksheet.Cells[startRow, 13].Value = note.EntityTypeName; // Nhóm đối tượng
                        worksheet.Cells[startRow, 14].Value = note.OriginalDocumentCode; // Chứng từ gốc
                        worksheet.Cells[startRow, 15].Value = note.ProjectName; // Tên dự án
                        worksheet.Cells[startRow, 16].Value = note.Note; // Ghi chú

                        startRow++;
                        index++;
                    }

                    // Xóa các dòng thừa sau khi điền dữ liệu
                    int lastDataRow = startRow - 1;
                    int totalRows = worksheet.Dimension.End.Row;
                    if (totalRows > lastDataRow)
                    {
                        worksheet.DeleteRow(lastDataRow + 1, totalRows - lastDataRow);
                    }

                    // Tự động điều chỉnh kích thước các cột từ cột thứ hai đến cột cuối cùng
                    worksheet.Cells[1, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].AutoFitColumns();

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
