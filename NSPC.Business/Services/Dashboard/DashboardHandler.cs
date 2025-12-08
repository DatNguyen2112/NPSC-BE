using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Entity;
using Serilog;

namespace NSPC.Business.Services.Dashboard;

public class DashboardHandler : IDashboardHandler
{
    private readonly SMDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public DashboardHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<Response<DashboardViewModel>> RevenueDashboard(DashboardFilter objectFilter)
    {
        try
        {
            var typeList = new List<string>();
            var amountList = new List<decimal>();
            switch (objectFilter.Type)
            {
                case "time":
                    if (objectFilter.DateRange != null)
                    {
                        var dateRangeInDays = (objectFilter.DateRange[1] - objectFilter.DateRange[0])?.TotalDays;

                        if (dateRangeInDays <= 30)
                        {
                            typeList = await _dbContext.sm_SalesOrder
                                .Include(c => c.sm_Customer)
                                .Where(x => x.LastModifiedOnDate.HasValue &&
                                            x.LastModifiedOnDate.Value.Date >= objectFilter.DateRange[0] &&
                                            x.LastModifiedOnDate.Value.Date <= objectFilter.DateRange[1] &&
                                            x.IsReturned == false)
                                .Select(x => x.LastModifiedOnDate.Value.Date.ToString("dd/MM/yyyy"))
                                .Distinct()
                                .ToListAsync();
                        }
                        else
                        {
                            typeList = await _dbContext.sm_SalesOrder
                                .Include(c => c.sm_Customer)
                                .Where(x => x.LastModifiedOnDate.HasValue &&
                                            x.LastModifiedOnDate.Value.Date >= objectFilter.DateRange[0] &&
                                            x.LastModifiedOnDate.Value.Date <= objectFilter.DateRange[1] &&
                                            x.IsReturned == false)
                                .GroupBy(x => new { x.LastModifiedOnDate.Value.Year, x.LastModifiedOnDate.Value.Month })
                                .Select(g => g.Key)
                                .Select(g => new DateTime(g.Year, g.Month, 1).ToString("MM/yyyy"))
                                .ToListAsync();
                        }
                    }
                    else
                    {
                        typeList = await _dbContext.sm_SalesOrder
                            .Include(c => c.sm_Customer)
                            .Where(x => x.LastModifiedOnDate.HasValue &&
                                        x.IsReturned == false)
                            .Select(x => x.LastModifiedOnDate.Value.Date.ToString("dd/MM/yyyy"))
                            .Distinct()
                            .ToListAsync();
                    }

                    foreach (var type in typeList)
                    {
                        if (objectFilter.DateRange != null)
                        {
                            var dateRangeInDays = (objectFilter.DateRange[1] - objectFilter.DateRange[0])?.TotalDays;
                            if (dateRangeInDays <= 30)
                            {
                                var typeDate = DateTime.ParseExact(type, "dd/MM/yyyy", null);
                                var totalAmount = await _dbContext.sm_SalesOrder
                                    .Where(x => x.LastModifiedOnDate.HasValue &&
                                                x.LastModifiedOnDate.Value.Date == typeDate &&
                                                x.ExportStatusCode == "XUAT_KHO" &&
                                                x.IsReturned == false)
                                    .SumAsync(x => x.Total);
                                amountList.Add(totalAmount);
                            }
                            else
                            {
                                var typeMonthYear = DateTime.ParseExact(type, "MM/yyyy", null);
                                var totalAmount = await _dbContext.sm_SalesOrder
                                    .Where(x => x.LastModifiedOnDate.HasValue &&
                                                x.LastModifiedOnDate.Value.Year == typeMonthYear.Year &&
                                                x.LastModifiedOnDate.Value.Month == typeMonthYear.Month &&
                                                x.ExportStatusCode == "XUAT_KHO" &&
                                                x.IsReturned == false)
                                    .SumAsync(x => x.Total);
                                amountList.Add(totalAmount);
                            }
                        }
                        else
                        {
                            var typeDate = DateTime.ParseExact(type, "dd/MM/yyyy", null);
                            var totalAmount = await _dbContext.sm_SalesOrder
                                .Where(x => x.LastModifiedOnDate.HasValue &&
                                            x.LastModifiedOnDate.Value.Date == typeDate &&
                                            x.IsReturned == false)
                                .SumAsync(x => x.Total);
                            amountList.Add(totalAmount);
                        }
                    }

                    break;
                case "staff":
                    typeList = await _dbContext.sm_SalesOrder
                        .Include(c => c.sm_Customer)
                        .Where(x => x.CreatedByUserName != null &&
                                    x.IsReturned == false)
                        .Select(x => x.CreatedByUserName)
                        .Distinct()
                        .ToListAsync();
                    foreach (var type in typeList)
                    {
                        var totalAmount = await _dbContext.sm_SalesOrder
                            .Where(x => x.CreatedByUserName == type && x.ExportStatusCode == "XUAT_KHO" &&
                                        x.IsReturned == false)
                            .SumAsync(x => x.Total);
                        amountList.Add(totalAmount);
                    }

                    break;
                case "product":
                    typeList = await _dbContext.sm_SalesOrderItem
                        .Include(c => c.sm_SalesOrder)
                        .ThenInclude(c => c.sm_Customer)
                        .Where(c => c.ProductName != null && c.sm_SalesOrder.IsReturned == false)
                        .Select(x => x.ProductName)
                        .Distinct()
                        .ToListAsync();
                    foreach (var type in typeList)
                    {
                        var totalAmount = await _dbContext.sm_SalesOrderItem
                            .Where(x => x.ProductName == type && x.sm_SalesOrder.ExportStatusCode == "XUAT_KHO" && x.sm_SalesOrder.IsReturned == false)
                            .SumAsync(x => x.sm_SalesOrder.Total);
                        amountList.Add(totalAmount);
                    }

                    break;
                case "group_customer":
                    typeList = await _dbContext.sm_CodeType.Where(x => x.Type.Equals("CustomerGroup"))
                        .Select(x => x.Code)
                        .ToListAsync();
                    foreach (var type in typeList)
                    {
                        var totalAmount = await _dbContext.sm_SalesOrder
                            .Where(x => x.sm_Customer.CustomerGroupCode == type && x.ExportStatusCode == "XUAT_KHO")
                            .SumAsync(x => x.Total);
                        amountList.Add(totalAmount);
                    }

                    break;
                case "ware":
                    typeList = await _dbContext.sm_SalesOrder
                        .Include(c => c.sm_Customer)
                        .Where(x => x.WareName != null &&
                                    x.IsReturned == false)
                        .Select(x => x.WareName)
                        .Distinct()
                        .ToListAsync();
                    foreach (var type in typeList)
                    {
                        var totalAmount = await _dbContext.sm_SalesOrder
                            .Where(x => x.WareName.Equals(type) && x.ExportStatusCode == "XUAT_KHO" &&
                                        x.IsReturned == false)
                            .SumAsync(x => x.Total);
                        amountList.Add(totalAmount);
                    }

                    break;
            }

            var dashboard = new DashboardViewModel()
            {
                MetaObjects = typeList,
                DataObjects = new
                {
                    Type = "Bar",
                    Name = "Dashboard revenue",
                    Data = amountList
                }
            };
            return new Response<DashboardViewModel>(dashboard);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            return Helper.CreateExceptionResponse<DashboardViewModel>(ex);
        }
    }

    public async Task<Response<DashboardViewModel>> Top5CustomerWithTotalAmountDashboard(DashboardFilter objectFilter)
    {
        try
        {
            var entity = _dbContext.sm_Customer.AsQueryable();
            if (objectFilter.DateRange != null)
            {
                entity = _dbContext.sm_Customer
                    .Where(x => x.LastModifiedOnDate.HasValue &&
                                x.LastModifiedOnDate.Value.Date >= objectFilter.DateRange[0] &&
                                x.LastModifiedOnDate.Value.Date <= objectFilter.DateRange[1])
                    .OrderByDescending(x => x.ExpenseAmount)
                    .Take(5);
            }
            else
            {
                entity = _dbContext.sm_Customer
                    .OrderByDescending(x => x.ExpenseAmount)
                    .Take(5);
            }

            var queryItems = await entity.ToListAsync();

            var dashboard = new DashboardViewModel()
            {
                MetaObjects = queryItems.Select(c => c.Name).ToList(),
                DataObjects = new
                {
                    Type = "Bar",
                    Name = "Top 5 customer expenses",
                    Data = queryItems.Select(c => c.ExpenseAmount).ToList()
                }
            };
            return new Response<DashboardViewModel>(dashboard);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            return Helper.CreateExceptionResponse<DashboardViewModel>(ex);
        }
    }

    public async Task<Response<DashboardViewModel>> ReturnedOrdersDashboard(DashboardFilter objectFilter)
    {
        try
        {
            var entity = _dbContext.sm_Return_Order.AsQueryable();
            if (objectFilter.DateRange != null)
            {
                entity = _dbContext.sm_Return_Order
                    .Where(x => x.EntityTypeCode == "customer" && x.LastModifiedOnDate.HasValue &&
                                x.LastModifiedOnDate.Value.Date >= objectFilter.DateRange[0] &&
                                x.LastModifiedOnDate.Value.Date <= objectFilter.DateRange[1] &&
                                x.StatusCode.Equals("returned"));
            }
            else
            {
                entity = _dbContext.sm_Return_Order
                    .Where(x => x.EntityTypeCode.Equals("customer") && x.StatusCode.Equals("returned"));
            }

            var returnOrders = await entity.ToListAsync();
            var dailyReturnOrderCounts = new List<DailyReturnOrderCountModel>();
            if (objectFilter.DateRange != null)
            {
                var dateRangeInDays = (objectFilter.DateRange[1] - objectFilter.DateRange[0])?.TotalDays;
                if (dateRangeInDays <= 30)
                {
                    dailyReturnOrderCounts = returnOrders
                        .GroupBy(x => new
                            { Date = x.CreatedOnDate.ToString("dd/MM/yyyy"), OrderCode = x.OriginalDocumentCode })
                        .Select(group => new DailyReturnOrderCountModel
                        {
                            Date = group.Key.Date,
                            OrderCode = group.Key.OrderCode,
                        })
                        .ToList();
                }
                else
                {
                    dailyReturnOrderCounts = returnOrders
                        .GroupBy(x => new
                            { Date = x.CreatedOnDate.ToString("MM/yyyy"), OrderCode = x.OriginalDocumentCode })
                        .Select(group => new DailyReturnOrderCountModel
                        {
                            Date = group.Key.Date,
                            OrderCode = group.Key.OrderCode,
                        })
                        .ToList();
                }
            }
            else
            {
                dailyReturnOrderCounts = returnOrders
                    .GroupBy(x => new
                        { Date = x.CreatedOnDate.ToString("dd/MM/yyyy"), OrderCode = x.OriginalDocumentCode })
                    .Select(group => new DailyReturnOrderCountModel
                    {
                        Date = group.Key.Date,
                        OrderCode = group.Key.OrderCode,
                    })
                    .ToList();
            }

            var result = dailyReturnOrderCounts
                .GroupBy(order => order.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    Count = group.Select(order => order.OrderCode).Distinct().Count()
                })
                .ToList();
            var dashboard = new DashboardViewModel()
            {
                MetaObjects = result.Select(c => c.Date),
                DataObjects = result.Select(c => c.Count)
            };
            return new Response<DashboardViewModel>(dashboard);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            return Helper.CreateExceptionResponse<DashboardViewModel>(ex);
        }
    }

    public async Task<Response<DashboardViewModel>> TotalReturnedAmountDashBoard(DashboardFilter objectFilter)
    {
        try
        {
            decimal returnOrder = 0;
            int totalAmount = 0;
            if (objectFilter.DateRange != null)
            {
                returnOrder = _dbContext.sm_Return_Order
                    .Where(x => x.StatusCode.Equals("returned") && x.LastModifiedOnDate.HasValue &&
                                x.LastModifiedOnDate.Value.Date >= objectFilter.DateRange[0] &&
                                x.LastModifiedOnDate.Value.Date <= objectFilter.DateRange[1] &&
                                x.OriginalDocumentCode.StartsWith("SON") &&
                                x.EntityTypeCode.Equals("customer"))
                    .AsNoTracking()
                    .Sum(x => x.RefundSubTotal);
                totalAmount = await _dbContext.sm_SalesOrder
                    .Include(c => c.sm_Customer)
                    .Where(x => x.ExportStatusCode.Equals("XUAT_KHO") && x.LastModifiedOnDate.HasValue &&
                                x.LastModifiedOnDate.Value.Date >= objectFilter.DateRange[0] &&
                                x.LastModifiedOnDate.Value.Date <= objectFilter.DateRange[1])
                    .AsNoTracking()
                    .CountAsync();
            }
            else
            {
                returnOrder = _dbContext.sm_Return_Order
                    .Where(x => x.StatusCode.Equals("returned") &&
                                x.OriginalDocumentCode.StartsWith("SON") &&
                                x.EntityTypeCode.Equals("customer"))
                    .AsNoTracking()
                    .Sum(x => x.RefundSubTotal);
                totalAmount = await _dbContext.sm_SalesOrder
                    .Include(c => c.sm_Customer)
                    .Where(x => x.ExportStatusCode.Equals("XUAT_KHO"))
                    .AsNoTracking()
                    .CountAsync();
            }


            var groupStatusOrder = await _dbContext.sm_Return_Order
                .Where(x => x.LastModifiedOnDate.HasValue &&
                            x.LastModifiedOnDate.Value.Date >= objectFilter.DateRange[0] &&
                            x.LastModifiedOnDate.Value.Date <= objectFilter.DateRange[1])
                .GroupBy(x => new
                    { Type = x.EntityTypeCode, OrderCode = x.OriginalDocumentCode, Status = x.StatusCode })
                .Select(x => new { x.Key.Type, x.Key.OrderCode, x.Key.Status })
                .Distinct()
                .ToListAsync();

            int percent = 0;
            if (totalAmount == 0)
            {
                percent = 0;
            }
            else
            {
                percent = groupStatusOrder.Count(c =>
                              c.Type == "customer" && c.OrderCode.StartsWith("SON") &&
                              c.Status.Equals("returned")) * 100 /
                          totalAmount;
            }

            var dashboard = new DashboardViewModel()
            {
                DataObjects = new
                {
                    returnedValue = returnOrder,
                    returnedPercent = percent,
                }
            };
            return new Response<DashboardViewModel>(dashboard);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            return Helper.CreateExceptionResponse<DashboardViewModel>(ex);
        }
    }

    public async Task<Response<DashboardViewModel>> OrderStatusDashBoard()
    {
        try
        {
            var metaStatuses = new Dictionary<string, string>()
            {
                { "CHUA_THANH_TOAN", "Chưa thanh toán" },
                { "THANH_TOAN_MOT_NUA", "Thanh toán một phần" },
                { "DA_THANH_TOAN", "Thanh toán toàn bộ" },
                { "finalized", "Đang giao dịch" },
                { "completed", "Hoàn thành" },
                { "cancelled", "Đã huỷ" }
            };

            var statusCodes = new[]
            {
                "CHUA_THANH_TOAN", "THANH_TOAN_MOT_NUA", "DA_THANH_TOAN", "finalized",
                "completed", "cancelled"
            };

            var orderStatusCounts = await _dbContext.sm_SalesOrder
                .GroupBy(x => new
                {
                    Id = x.Id,
                    PaymentStatus = x.PaymentStatusCode,
                    Status = x.StatusCode
                })
                .Select(g => new
                {
                    Id = g.Key.Id,
                    PaymentStatus = g.Key.PaymentStatus,
                    Status = g.Key.Status,
                    Count = g.Count()
                })
                .ToListAsync();

            var statusCounts = new Dictionary<string, int>
            {
                { "CHUA_THANH_TOAN", 0 },
                { "THANH_TOAN_MOT_NUA", 0 },
                { "DA_THANH_TOAN", 0 },
                { "finalized", 0 },
                { "completed", 0 },
                { "cancelled", 0 }
            };

            foreach (var statusCount in orderStatusCounts)
            {
                if (statusCodes.Contains(statusCount.PaymentStatus))
                    statusCounts[statusCount.PaymentStatus] += statusCount.Count;
                if (statusCodes.Contains(statusCount.Status))
                    statusCounts[statusCount.Status] += statusCount.Count;
            }

            var orderStatuses = new DashboardForOrderStatusModel
            {
                Type = "bar",
                Name = "Thống kê hàng theo trạng thái",
                Data = new List<int>
                {
                    statusCounts["CHUA_THANH_TOAN"],
                    statusCounts["THANH_TOAN_MOT_NUA"],
                    statusCounts["DA_THANH_TOAN"],
                    statusCounts["finalized"],
                    statusCounts["completed"],
                    statusCounts["cancelled"]
                }
            };
            var dashboard = new DashboardViewModel
            {
                MetaObjects = metaStatuses.Values.ToList(),
                DataObjects = orderStatuses
            };

            return new Response<DashboardViewModel>(dashboard);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            return Helper.CreateExceptionResponse<DashboardViewModel>(ex);
        }
    }

    public async Task<Response<DashboardViewModel>> QuantityValueQuotationDashBoard()
    {
        try
        {
            var metaStatuses = new Dictionary<string, string>()
            {
                { "PENDING_APPROVAL", "Chờ duyệt" },
                { "CUSTOMER_APPROVED", "Khách hàng duyệt" },
                { "INTERNAL_APPROVAL", "Duyệt nội bộ" },
                { "DRAFT", "Nháp" },
                { "CANCEL", "Đã huỷ" }
            };
            var statusCodes = new[]
            {
                "PENDING_APPROVAL", "CUSTOMER_APPROVED", "INTERNAL_APPROVAL", "DRAFT", "CANCEL"
            };

            var orderStatusCounts = await _dbContext.sm_Quotation
                .GroupBy(x => new { Status = x.Status })
                .Select(group => new
                {
                    Status = group.Key.Status,
                    Count = group.Count(),
                })
                .ToListAsync();
            var orderStatusAmount = await _dbContext.sm_Quotation
                .GroupBy(x => new { Id = x.Id, Status = x.Status, Amount = x.TotalAmount })
                .Select(group => new
                {
                    Id = group.Key.Id,
                    Status = group.Key.Status,
                    Amount = group.Key.Amount,
                })
                .ToListAsync();
            var statusCounts = new Dictionary<string, int>
            {
                { "PENDING_APPROVAL", 0 },
                { "CUSTOMER_APPROVED", 0 },
                { "INTERNAL_APPROVAL", 0 },
                { "DRAFT", 0 },
                { "CANCEL", 0 },
            };

            var statusAmount = new Dictionary<string, decimal>
            {
                { "PENDING_APPROVAL", 0 },
                { "CUSTOMER_APPROVED", 0 },
                { "INTERNAL_APPROVAL", 0 },
                { "DRAFT", 0 },
                { "CANCEL", 0 },
            };

            foreach (var statusCount in orderStatusCounts)
            {
                if (statusCodes.Contains(statusCount.Status))
                {
                    statusCounts[statusCount.Status] = statusCount.Count;
                }
            }

            foreach (var statusAmountItem in orderStatusAmount)
            {
                if (statusCodes.Contains(statusAmountItem.Status))
                {
                    statusAmount[statusAmountItem.Status] += statusAmountItem.Amount;
                }
            }

            var orderStatuses = new DashboardForOrderStatusModel
            {
                Type = "bar",
                Name = "Số lượng",
                Data = new List<int>
                {
                    statusCounts["PENDING_APPROVAL"],
                    statusCounts["CUSTOMER_APPROVED"],
                    statusCounts["INTERNAL_APPROVAL"],
                    statusCounts["DRAFT"],
                    statusCounts["CANCEL"],
                }
            };
            var orderStatusesAmount = new DashboardForOrderStatusModel
            {
                Type = "line",
                Name = "Tổng tiền",
                Data = new List<decimal>
                {
                    statusAmount["PENDING_APPROVAL"],
                    statusAmount["CUSTOMER_APPROVED"],
                    statusAmount["INTERNAL_APPROVAL"],
                    statusAmount["DRAFT"],
                    statusAmount["CANCEL"],
                }
            };
            var dashboard = new DashboardViewModel
            {
                MetaObjects = metaStatuses.Values.ToList(),
                DataObjects = new
                {
                    Count = orderStatuses,
                    Amount = orderStatusesAmount
                }
            };

            return new Response<DashboardViewModel>(dashboard);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            return Helper.CreateExceptionResponse<DashboardViewModel>(ex);
        }
    }

    public async Task<Response<DashboardViewModel>> QuantityPercentQuotationDashBoard(DashboardFilter objectFilter)
    {
        try
        {
            int totalQuotation = 0;
            int quotationExist = 0;
            if (objectFilter.DateRange != null)
            {
                totalQuotation = await _dbContext.sm_Quotation.Where(x => x.LastModifiedOnDate.HasValue &&
                                                                          x.LastModifiedOnDate.Value.Date >=
                                                                          objectFilter.DateRange[0] &&
                                                                          x.LastModifiedOnDate.Value.Date <=
                                                                          objectFilter.DateRange[1]).CountAsync();
                quotationExist = await _dbContext.sm_SalesOrder.Where(x => x.LastModifiedOnDate.HasValue &&
                                                                           x.LastModifiedOnDate.Value.Date >=
                                                                           objectFilter.DateRange[0] &&
                                                                           x.LastModifiedOnDate.Value.Date <=
                                                                           objectFilter.DateRange[1] &&
                                                                           x.ExportStatusCode.Equals("XUAT_KHO") &&
                                                                           x.QuotationId != null)
                    .Select(x => x.QuotationId).Distinct().CountAsync();
            }
            else
            {
                totalQuotation = await _dbContext.sm_Quotation.CountAsync();
                quotationExist = await _dbContext.sm_SalesOrder.Where(x => x.ExportStatusCode.Equals("XUAT_KHO"))
                    .Select(x => x.QuotationId).Distinct().CountAsync();
            }

            int percentage = 0;
            if (totalQuotation == 0)
            {
                percentage = 0;
            }
            else
            {
                percentage = quotationExist * 100 / totalQuotation;
            }

            var dashboard = new DashboardViewModel
            {
                DataObjects = new
                {
                    Quantity = totalQuotation,
                    Percent = percentage
                }
            };

            return new Response<DashboardViewModel>(dashboard);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            return Helper.CreateExceptionResponse<DashboardViewModel>(ex);
        }
    }

    public async Task<Response<DashboardViewModel>> TotalAmountDashboard(DashboardFilter objectFilter)
    {
        try
        {
            decimal? totalAll = 0;
            decimal? totalExportAll = 0;
            if (objectFilter.DateRange != null)
            {
                totalAll = _dbContext.sm_SalesOrder
                    .Include(c => c.sm_Customer)
                    .Where(x => x.LastModifiedOnDate.HasValue &&
                                x.LastModifiedOnDate.Value.Date >=
                                objectFilter.DateRange[0] &&
                                x.LastModifiedOnDate.Value.Date <=
                                objectFilter.DateRange[1])
                    .Where(x => x.ExportStatusCode.Equals("XUAT_KHO"))
                    .AsNoTracking().Sum(c => c.Total);
                decimal returnOrder = _dbContext.sm_Return_Order
                    .Where(x => x.StatusCode.Equals("returned") && x.LastModifiedOnDate.HasValue &&
                                x.LastModifiedOnDate.Value.Date >= objectFilter.DateRange[0] &&
                                x.LastModifiedOnDate.Value.Date <= objectFilter.DateRange[1] &&
                                x.OriginalDocumentCode.StartsWith("SON") &&
                                x.EntityTypeCode.Equals("customer"))
                    .AsNoTracking()
                    .Sum(x => x.RefundSubTotal);
                totalAll -= returnOrder;
                totalExportAll = _dbContext.sm_SalesOrder
                    .Include(c => c.sm_Customer)
                    .Where(x => x.LastModifiedOnDate.HasValue &&
                                x.LastModifiedOnDate.Value.Date >=
                                objectFilter.DateRange[0] &&
                                x.LastModifiedOnDate.Value.Date <=
                                objectFilter.DateRange[1])
                    .Where(x => x.ExportStatusCode.Equals("XUAT_KHO"))
                    .AsNoTracking().Sum(c => c.PaidAmount);
            }
            else
            {
                totalAll = _dbContext.sm_SalesOrder
                    .Include(c => c.sm_Customer)
                    .AsNoTracking().Sum(c => c.Total);
                decimal returnOrder = _dbContext.sm_Return_Order
                    .Where(x => x.StatusCode.Equals("returned") &&
                                x.OriginalDocumentCode.StartsWith("SON") &&
                                x.EntityTypeCode.Equals("customer"))
                    .AsNoTracking()
                    .Sum(x => x.RefundSubTotal);
                totalAll -= returnOrder;
                totalExportAll = _dbContext.sm_SalesOrder
                    .Include(c => c.sm_Customer)
                    .Where(x => x.ExportStatusCode.Equals("XUAT_KHO"))
                    .AsNoTracking().Sum(c => c.PaidAmount);
            }

            var dashboard = new DashboardViewModel()
            {
                DataObjects = new
                {
                    Total = totalAll,
                    TotalExport = totalExportAll
                }
            };
            return new Response<DashboardViewModel>(dashboard);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            return Helper.CreateExceptionResponse<DashboardViewModel>(ex);
        }
    }

    public async Task<Response<DashboardViewModel>> Top5CustomersInDebtDashboard(DashboardFilter objectFilter)
    {
        try
        {
            var entity = new List<sm_Customer>();
            if (objectFilter.DateRange != null)
            {
                entity = await _dbContext.sm_Customer
                    .Where(x => x.LastModifiedOnDate.HasValue &&
                                x.LastModifiedOnDate.Value.Date >=
                                objectFilter.DateRange[0] &&
                                x.LastModifiedOnDate.Value.Date <=
                                objectFilter.DateRange[1])
                    .OrderByDescending(c => c.DebtAmount)
                    .Take(5)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                entity = await _dbContext.sm_Customer
                    .OrderByDescending(c => c.DebtAmount)
                    .Take(5)
                    .AsNoTracking()
                    .ToListAsync();
            }

            var dashboard = new DashboardViewModel()
            {
                MetaObjects = entity.Select(c => c.Name).ToList(),
                DataObjects = entity.Select(c => c.DebtAmount).ToList()
            };
            return new Response<DashboardViewModel>(dashboard);
        }
        catch (Exception ex)
        {
            Log.Error(ex, string.Empty);
            return Helper.CreateExceptionResponse<DashboardViewModel>(ex);
        }
    }
}