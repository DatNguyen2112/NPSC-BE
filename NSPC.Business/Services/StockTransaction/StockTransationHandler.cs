using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.StockTransaction;
using Serilog;
using System.Linq.Expressions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using NSPC.Business.Services.VatTu;
using NSPC.Data.Migrations;
using Exception = System.Exception;

namespace NSPC.Business.Services.StockTransaction
{
    public class StockTransationHandler : IStockTransactionHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;

        public StockTransationHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
        }

        public async Task<Response<Pagination<StockTransactionViewModel>>> GetPage(StockTransactionQueryModel query)
        {
           
            try
            {
                var predicate = BuildQuery(query);
                var iquery = _dbContext.sm_Stock_Transaction.AsNoTracking().Include(x => x.Sm_Product).Where(predicate);
                
                var startDate = query.DateRange[0].Value.Date;
                var endDate = query.DateRange[1].Value.Date;
                
                var queryResult = iquery.GroupBy(x => new { x.ProductCode, x.ProductName, x.ProductId}).Select(x => new StockTransactionViewModel()
                {
                    ProductId = x.Key.ProductId,
                    ProductCode = x.Key.ProductCode,
                    ProductName = x.Key.ProductName,
                    WareCode = x.FirstOrDefault().WareCode,
                    WareName = _dbContext.sm_CodeType
                        .FirstOrDefault(s => s.Code == x.FirstOrDefault().WareCode).Title,
                    
                    Unit = _dbContext.sm_Product.FirstOrDefault(s => s.Id == x.Key.ProductId).Unit,
                    UnitPrice = _dbContext.sm_Product.FirstOrDefault(s => s.Id == x.Key.ProductId).PurchaseUnitPrice,
                    
                    ReceiptInventoryQuantity = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG 
                            || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.SALES_RETURN 
                        ).Sum(x => x.ReceiptInventoryQuantity),
                    ExportInventoryQuantity = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.BAN_HANG 
                        || x.OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO 
                        || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.PURCHASE_RETURN).Sum(x => x.ExportInventoryQuantity),
                    
                    OpeningInventoryAmount = _dbContext.sm_Stock_Transaction.AsNoTracking().Where(s => s.CreatedOnDate.Date < startDate && s.ProductId == x.Key.ProductId).Sum(x => (x.ReceiptInventoryQuantity - x.ExportInventoryQuantity) * x.UnitPrice),
                    OpeningInventoryQuantity = _dbContext.sm_Stock_Transaction.AsNoTracking().Where(s => s.CreatedOnDate.Date < startDate && s.ProductId == x.Key.ProductId).Sum(x =>  (x.ReceiptInventoryQuantity - x.ExportInventoryQuantity)),
                    // ClosingInventoryQuantity = _dbContext.sm_Stock_Transaction.AsNoTracking().Where(x => x.CreatedOnDate <= endDate).Sum(x =>  (x.ReceiptInventoryQuantity - x.ExportInventoryQuantity)),
                    // ClosingInventoryAmount = _dbContext.sm_Stock_Transaction.AsNoTracking().Where(x => x.CreatedOnDate <= endDate).Sum(x =>  (x.ReceiptInventoryQuantity - x.ExportInventoryQuantity) * x.UnitPrice),
                    
                    #region Tổng số lượng nhập trong kỳ
                    TotalImportQuantity = x.Where(x => (x.CreatedOnDate.Date >= startDate && x.CreatedOnDate.Date <= endDate) && (x.OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG 
                            || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.SALES_RETURN 
                        )).Sum(x => x.ReceiptInventoryQuantity),
                    TotalImportAmount = x.Where(x => (x.CreatedOnDate.Date >= startDate && x.CreatedOnDate.Date <= endDate) && (x.OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG 
                            || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.SALES_RETURN 
                        )).Sum(x => x.ReceiptInventoryQuantity * x.UnitPrice),
                    #endregion
                    
                    // SellableQuantity = x
                    //     .Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.SALES_RETURN ||  x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.PURCHASE_RETURN || x.OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO)
                    //     .Sum(x => x.SellableQuantity),

                    #region Tổng số lượng xuất trong kỳ
                    TotalExportAmount = x.Where(x => (x.CreatedOnDate.Date >= startDate &&
                                                      x.CreatedOnDate.Date <= endDate) && ((x.OriginalDocumentType == OriginalDocumentTypeConstants.BAN_HANG 
                        || x.OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO 
                        || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.PURCHASE_RETURN))).Sum(x => x.ExportInventoryQuantity * x.UnitPrice),
                    TotalExportQuantity = x.Where(x => (x.CreatedOnDate.Date >= startDate &&
                                                        x.CreatedOnDate.Date <= endDate) && ((x.OriginalDocumentType == OriginalDocumentTypeConstants.BAN_HANG 
                        || x.OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO 
                        || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.PURCHASE_RETURN))).Sum(x => x.ExportInventoryQuantity),
                    #endregion
                    
                    InventoryIncreased = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG).Sum(x => x.InventoryIncreased),
                    InventoryDecreased = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO).Sum(x => x.InventoryDecreased),
                });
                
              var data = await queryResult.GetPageAsync(query);

              var result = _mapper.Map<Pagination<StockTransactionViewModel>>(data);

                return Helper.CreateSuccessResponse<Pagination<StockTransactionViewModel>>(result);

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@query}", query);
                return Helper.CreateExceptionResponse<Pagination<StockTransactionViewModel>>(ex);
            }

        }
        
        /// <summary>
        /// Lấy ra lịch sử kho của sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response<List<StockHistoryViewModel>>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Stock_Transaction.Where(x => x.ProductId == id).OrderByDescending(x => x.CreatedOnDate).ToListAsync();

                List<StockHistoryViewModel> result = new List<StockHistoryViewModel>();
                
                if (entity != null)
                {
                    // Tách các bản ghi có action là khởi tạo tồn ban đầu
                    var initialStockRecords = entity
                        .Where(x => x.Action == ActionConstants.INITIALIZE) // Lọc theo action khởi tạo
                        .OrderByDescending(x => x.CreatedOnDate)
                        .ToList();

                    if (initialStockRecords != null)
                    {
                        // Tổng hợp các giá trị của các bản ghi khởi tạo tồn ban đầu
                        var aggregatedInitialStockRecord = new StockHistoryViewModel
                        {
                            ProductId = id,
                            Action = ActionConstants.INITIALIZE,
                            CreatedOnDate = initialStockRecords != null ? initialStockRecords.FirstOrDefault().CreatedOnDate : null,
                            ReceiptInventoryQuantity = initialStockRecords.Sum(x => x.ReceiptInventoryQuantity),
                            ReceiptInventoryAmount = initialStockRecords.Sum(x => x.ReceiptInventoryAmount),
                            ExportInventoryQuantity = initialStockRecords.Sum(x => x.ExportInventoryAmount),
                            ExportInventoryAmount = initialStockRecords.Sum(x => x.ExportInventoryAmount),
                            InitialStockQuantity = initialStockRecords.Sum(x => x.InitialStockQuantity),
                        };
                
                        // Lấy danh sách các bản ghi khác không phải khởi tạo
                        var otherRecords = entity
                            .Where(x => x.Action != ActionConstants.INITIALIZE)
                            .Select(x => _mapper.Map<StockHistoryViewModel>(x))
                            .ToList();

                        // Kết hợp bản ghi tổng hợp và các bản ghi khác
                        result = new List<StockHistoryViewModel>(otherRecords);
                        result.Add(aggregatedInitialStockRecord);
                    }
                }
                
                // Trả về danh sách kết quả
                return new Response<List<StockHistoryViewModel>>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<List<StockHistoryViewModel>>(ex);
            }
        }
        
        /// <summary>
        /// Lấy ra thao tác , lịch sử của từng kho
        /// </summary>
        public async Task<Response<List<StockHistoryViewModel>>> GetStockHistoryOfEachWareHouse(StockTransactionQueryModel query) 
        {
            try
            {
                var predicate = BuildQueryTransaction(query);
                var entityList = await _dbContext.sm_Stock_Transaction.Where(predicate.And(s => query.WaresCode.Contains(s.WareCode) && query.ProductIds.Contains(s.ProductId))).OrderBy(x => x.CreatedOnDate).ToListAsync();
                
                // Khởi tạo biến đầu vào để lưu giá trị
                decimal? totalStockTransactionQuantity = 0M;
                
                // Select từng phần tử
                var result = entityList.Select(x =>
                {
                    totalStockTransactionQuantity += x.ReceiptInventoryQuantity - x.ExportInventoryQuantity;

                    return new StockHistoryViewModel
                    {
                        ProductId = x.ProductId,
                        Action = x.Action,
                        ReceiptInventoryQuantity = x.ReceiptInventoryQuantity,
                        ExportInventoryQuantity = x.ExportInventoryQuantity,
                        CreatedOnDate = x.CreatedOnDate,
                        InitialStockQuantity = totalStockTransactionQuantity,
                        OriginalDocumentCode = x.OriginalDocumentCode,
                        OriginalDocumentId = x.OriginalDocumentId,
                        InventoryDecreased = x.InventoryDecreased,
                        InventoryIncreased = x.InventoryIncreased,
                        WareCode = x.WareCode,
                    };
                }).Reverse().ToList();
                
                // Trả về danh sách kết quả
                return new Response<List<StockHistoryViewModel>>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<List<StockHistoryViewModel>>(ex);
            }
        }
        
        private Expression<Func<sm_Stock_Transaction, bool>> BuildQueryTransaction(StockTransactionQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_Stock_Transaction>(true);
            return predicate;
        }
        
        /// <summary>
        /// Lấy ra tồn kho của sản phẩm để check tồn kho và số lượng có thể bán 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wareCode"></param>
        /// <returns></returns>
        public async Task<Response<List<StockTransactionViewModel>>> GetByIdAndWareCode(Guid id, string wareCode)
        {
            try
            {
                var entityList = await _dbContext.sm_Stock_Transaction.Where(x => x.ProductId == id && x.WareCode == wareCode).OrderByDescending(x => x.CreatedOnDate).ToListAsync();
                
                var entityStockTransaction = await _dbContext.sm_CodeType.Where(x => x.Code == wareCode).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();
                
                // var entitySalesOrder = await _dbContext.sm_SalesOrder.Where(x => x.WareCode == wareCode).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

                var result = entityList.GroupBy(x => new { x.ProductCode, x.ProductName, x.ProductId, x.WareCode }).Select(
                    x => new StockTransactionViewModel()
                    {
                        ProductId = x.Key.ProductId,
                        ProductCode = x.Key.ProductCode,
                        ProductName = x.Key.ProductName,
                        WareCode = x.Key.WareCode,
                        WareName = entityStockTransaction.Title,
                        ExportInventoryAmount = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.BAN_HANG || x.OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.PURCHASE_RETURN).Sum(x => x.ExportInventoryAmount),
                        ExportInventoryQuantity = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.BAN_HANG || x.OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.PURCHASE_RETURN).Sum(x => x.ExportInventoryQuantity),
                        ReceiptInventoryAmount = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.SALES_RETURN).Sum(x => x.ReceiptInventoryAmount),
                        ReceiptInventoryQuantity = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.SALES_RETURN).Sum(x => x.ReceiptInventoryQuantity),
                        // SellableQuantity = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.SALES_RETURN ||  x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.PURCHASE_RETURN || x.OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO).Sum(x => x.SellableQuantity),
                    });
                
                return new Response<List<StockTransactionViewModel>>(_mapper.Map<List<StockTransactionViewModel>>(result));

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<List<StockTransactionViewModel>>(ex);
            }
        }
        
        /// <summary>
        /// Lấy ra tồn kho của sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wareCode"></param>
        /// <returns></returns>
        public async Task<Response<List<StockTransactionViewModel>>> GetByWareCode(string wareCode)
        {
            try
            {
                var entityList = await _dbContext.sm_Stock_Transaction.Where(x => x.WareCode == wareCode).OrderByDescending(x => x.CreatedOnDate).ToListAsync();
                
                var entityStockTransaction = await _dbContext.sm_CodeType.Where(x => x.Code == wareCode).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();
                
                var result = entityList.GroupBy(x => new { x.ProductCode, x.ProductName, x.ProductId, x.WareCode }).Select(
                    x => new StockTransactionViewModel()
                    {
                        ProductId = x.Key.ProductId,
                        ProductCode = x.Key.ProductCode,
                        ProductName = x.Key.ProductName,
                        WareCode = x.Key.WareCode,
                        WareName = entityStockTransaction.Title,
                        SellableQuantity = _dbContext.sm_ProductInventory
                            .Where(s => s.ProductId == x.Key.ProductId && s.WarehouseCode == wareCode).Sum(x => x.SellableQuantity),
                        ExportInventoryAmount = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.BAN_HANG || x.OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.PURCHASE_RETURN).Sum(x => x.ExportInventoryAmount),
                        ExportInventoryQuantity = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.BAN_HANG || x.OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.PURCHASE_RETURN).Sum(x => x.ExportInventoryQuantity),
                        ReceiptInventoryAmount = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.SALES_RETURN).Sum(x => x.ReceiptInventoryAmount),
                        ReceiptInventoryQuantity = x.Where(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.SALES_RETURN).Sum(x => x.ReceiptInventoryQuantity),
                    });
                
                return new Response<List<StockTransactionViewModel>>(_mapper.Map<List<StockTransactionViewModel>>(result));

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<List<StockTransactionViewModel>>(ex);
            }
        }

        private Expression<Func<sm_Stock_Transaction, bool>> BuildQuery(StockTransactionQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_Stock_Transaction>(true);
            predicate.And(x => x.OriginalDocumentType == OriginalDocumentTypeConstants.NHAP_HANG || x.OriginalDocumentType == OriginalDocumentTypeConstants.BAN_HANG || x.OriginalDocumentType == OriginalDocumentTypeConstants.KIEM_KHO || x.OriginalDocumentType == OriginalDocumentTypeConstants.XUAT_KHO || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.SALES_RETURN || x.OriginalDocumentType == OrderReturnConstants.OriginalDocumentType.PURCHASE_RETURN);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.ProductName.ToLower().Contains(query.FullTextSearch.ToLower()) || s.ProductCode.ToLower().Contains(query.FullTextSearch.ToLower()) || s.WareCode.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.WaresCode != null && query.WaresCode.Count() > 0)
                predicate.And(s => query.WaresCode.Contains(s.WareCode));

            // Lọc theo DateRange
            if (query.DateRange != null && query.DateRange.Any())
            {
                var startDate = query.DateRange[0]?.Date;
                var endDate = query.DateRange[1]?.Date;

                if (startDate.HasValue && endDate.HasValue)
                {
                    // Điều kiện lọc ngoài DateRange
                    predicate.And(x => x.CreatedOnDate.Date >= startDate || x.CreatedOnDate.Date <= endDate);
                }
            }

            if (!string.IsNullOrEmpty(query.Type))
                predicate.And(s => s.Sm_Product.Type == query.Type);

            if (query.ProductIds != null && query.ProductIds.Count() > 0)
                predicate.And(s => query.ProductIds.Contains(s.ProductId));

          
            return predicate;
        }

        public async Task<Response<StockTransactionViewModel>> Create(StockTransactionCreateModel model)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var stockTransaction = new sm_Stock_Transaction
                {
                    Id = Guid.NewGuid(),
                    ProductCode = model.ProductCode,
                    ProductName = model.ProductName,
                    ProductId = model.ProductId,
                    ReceiptInventoryQuantity = model.ReceiptInventoryQuantity,
                    ReceiptInventoryAmount = model.ReceiptInventoryQuantity * model.UnitPrice ?? 0,
                    InitialStockQuantity = model.InitialStockQuantity,
                    ExportInventoryQuantity = model.ExportInventoryQuantity,
                    ExportInventoryAmount= model.ExportInventoryQuantity * model.UnitPrice ?? 0,
                    OriginalDocumentCode = model.OriginalDocumentCode,
                    ClosingInventoryQuantity = model.ClosingInventoryQuantity,
                    WareCode = model.WareCode,
                    OriginalDocumentType = model.OriginalDocumentType,
                    OriginalDocumentId = model.OriginalDocumentId,
                    StockTransactionQuantity = model.StockTransactionQuantity,
                    // SellableQuantity = model.SellableQuantity,
                    InventoryIncreased = model.InventoryIncreased,
                    InventoryDecreased = model.InventoryDecreased,
                    //OpeningInventoryQuantity = model.OpeningInventoryQuantity,
                    UnitPrice = model.UnitPrice,
                    CreatedOnDate = DateTime.Now,
                    CreatedByUserId = model.CreatedByUserId,
                    Unit = model.Unit,
                    Action = model.Action,
                };

                _dbContext.sm_Stock_Transaction.Add(stockTransaction);

                await _dbContext.SaveChangesAsync();

                return new Response<StockTransactionViewModel>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<StockTransactionViewModel>(ex);
            }
        }
        
        /// <summary>
        /// Hàm xuất file danh sách báo cáo xuất nhập tồn excel theo query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Response<string>> ExportListToExcel(StockTransactionQueryModel query)
        {
            try
            {
                // Tạo predicate để lọc dựa trên các tham số trong query
                var predicate = BuildQuery(query);

                // Lấy danh sách báo cáo xuất nhập tồn
                var stockTransactionList = await GetPage(query);
                if (stockTransactionList == null || stockTransactionList.Data.Content.Count == 0)
                    return Helper.CreateNotFoundResponse<string>("Không có báo cáo xuất nhập tồn nào tồn tại trong hệ thống.");

                // Đặt tên file và đường dẫn template
                var fileName = $"danh sách xuất nhập tồn_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}.xlsx";
                var filePath = Path.Combine(_staticsFolder, fileName);
                var templatePath = Path.Combine(_staticsFolder, "excel-template/StockTransactionTemplate.xlsx");

                if (!File.Exists(templatePath))
                    return Helper.CreateBadRequestResponse<string>("Không tìm thấy file template");

                var currentDate = DateTime.Now;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(templatePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    // Gán Ngày hiện tại vào dòng 6 cột 6
                    worksheet.Cells[6, 6].Value = $"Bắc Ninh, ngày {currentDate:dd} tháng {currentDate:MM} năm {currentDate:yyyy}";
                    worksheet.Cells[6, 6].Style.Font.Italic = true; // In nghiêng

                    // Gán DateRange theo query vào Excel
                    worksheet.Cells[11, 4].Value = query.DateRange[0].Value.ToString("dd/MM/yyyy");
                    worksheet.Cells[11, 6].Value = query.DateRange[1].Value.ToString("dd/MM/yyyy");

                    // Lấy tên kho từ mảng mã kho (query.WaresCode)
                    List<string> warehouseNames = new List<string>();
                    if (query.WaresCode != null && query.WaresCode.Any())
                    {
                        warehouseNames = await _dbContext.sm_CodeType
                            .Where(w => query.WaresCode.Contains(w.Code))
                            .Select(w => w.Title)
                            .ToListAsync();
                    }

                    // Gán tên Kho vào ô cụ thể
                    worksheet.Cells[13, 4].Value = warehouseNames.Any()
                        ? string.Join(", ", warehouseNames)
                        : "Tất cả";

                    int startRow = 16; // Dữ liệu bắt đầu từ hàng 16
                    int index = 1;

                    foreach (var stockTransaction in stockTransactionList.Data.Content)
                    {
                        worksheet.Cells[startRow, 1].Value = index; // STT
                        worksheet.Cells[startRow, 2].Value = stockTransaction.ProductName; // Sản phẩm
                        worksheet.Cells[startRow, 3].Value = stockTransaction.Unit; // Đơn vị tính
                        worksheet.Cells[startRow, 4].Value = stockTransaction.OpeningInventoryQuantity; // Số lượng tồn đầu kỳ
                        worksheet.Cells[startRow, 5].Value = stockTransaction.ClosingInventoryQuantity; // Số lượng tồn cuối kỳ
                        worksheet.Cells[startRow, 6].Value = stockTransaction.TotalImportQuantity; // Số lượng nhập trong kỳ
                        worksheet.Cells[startRow, 7].Value = stockTransaction.TotalExportQuantity; // Số lượng xuất trong kỳ

                        startRow++;
                        index++;
                    }

                    int lastDataRow = startRow - 1;

                    // Thêm đường viền cho các ô đã điền dữ liệu
                    using (var range = worksheet.Cells[16, 1, lastDataRow, 7]) // điều chỉnh cột cuối (8) tùy theo số lượng cột bạn có
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    // Giữ nguyên các dòng cố định phía dưới
                    int footerStartRow = lastDataRow + 4; // Đặt các dòng footer bắt đầu từ dưới bảng dữ liệu

                    worksheet.Cells[footerStartRow, 7].Value = $"Bắc Ninh, ngày {currentDate:dd} tháng {currentDate:MM} năm {currentDate:yyyy}";
                    worksheet.Cells[footerStartRow, 7].Style.Font.Italic = true; // In nghiêng
                    worksheet.Cells[footerStartRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa

                    worksheet.Cells[footerStartRow + 2, 2].Value = "Người phê duyệt";
                    worksheet.Cells[footerStartRow + 2, 2].Style.Font.Bold = true; // In đậm
                    worksheet.Cells[footerStartRow + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa

                    worksheet.Cells[footerStartRow + 2, 7].Value = "Người lập biểu";
                    worksheet.Cells[footerStartRow + 2, 7].Style.Font.Bold = true; // In đậm
                    worksheet.Cells[footerStartRow + 2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa

                    worksheet.Cells[footerStartRow + 3, 2].Value = "(Ký, ghi rõ họ tên)";
                    worksheet.Cells[footerStartRow + 3, 2].Style.Font.Italic = true; // In nghiêng
                    worksheet.Cells[footerStartRow + 3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa

                    worksheet.Cells[footerStartRow + 3, 7].Value = "(Ký, ghi rõ họ tên)";
                    worksheet.Cells[footerStartRow + 3, 7].Style.Font.Italic = true; // In nghiêng
                    worksheet.Cells[footerStartRow + 3, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa

                    // Tự động điều chỉnh kích thước các cột từ cột thứ hai đến cột cuối cùng
                    worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].AutoFitColumns();

                    await package.SaveAsAsync(new FileInfo(filePath));
                    return Helper.CreateSuccessResponse<string>(filePath, "Xuất file thành công");
                }
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
