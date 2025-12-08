using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using NSPC.Business;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.EInvoice;
using NSPC.Data.Data.Entity.JsonbEntity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.EInvoice
{
    public class EInvoiceHandler : IEInvoiceHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public EInvoiceHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<EInvoiceViewModel>> CreateAsync(EInvoiceCreateUpdateModel model, Helper.RequestUser currentUser)
        {
            try
            {
                if (model.EInvoiceItems != null && model.EInvoiceItems.Count > 0)
                {
                    // Check danh sách items không được trùng nhau
                    var checkDuplicateItems = model.EInvoiceItems.GroupBy(x => x.Name).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (checkDuplicateItems.Count > 0)
                        return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Danh sách hàng hóa, dịch vụ không được trùng nhau.");

                    // Check danh sách items số lượng phải > 0
                    var checkQuantityItems = model.EInvoiceItems.Where(x => x.Quantity <= 0).ToList();
                    if (checkQuantityItems.Count > 0)
                        return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Số lượng hàng hóa, dịch vụ không được nhỏ hơn hoặc bằng 0.");

                    // Check danh sách items đơn giá phải > 0
                    var checkUnitPriceItems = model.EInvoiceItems.Where(x => x.UnitPrice < 0).ToList();
                    if (checkUnitPriceItems.Count > 0)
                        return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Đơn giá hàng hóa, dịch vụ không được nhỏ hơn 0.");

                    // Check danh sách items số lượng phải > 0
                    var checkQuantityUnitPriceItems = model.EInvoiceItems.Where(x => x.Quantity <= 0).ToList();
                    if (checkQuantityUnitPriceItems.Count > 0)
                        return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Số lượng hàng hóa, dịch vụ không được nhỏ hơn hoặc bằng 0.");

                    #region Tính toán số thứ tự items
                    model.EInvoiceItems = model.EInvoiceItems.OrderBy(x => x.LineNumber).ToList();
                    for (int i = 0; i < model.EInvoiceItems.Count; i++)
                        model.EInvoiceItems[i].LineNumber = i + 1;
                    #endregion
                }

                if (model.EInvoiceItems == null || model.EInvoiceItems.Count == 0)
                    return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Danh sách hàng hóa, dịch vụ không được để trống.");

                // Mapping và tạo entity
                var entity = _mapper.Map<sm_EInvoice>(model);

                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.FullName;
                entity.CreatedOnDate = DateTime.Now;

                entity.Id = Guid.NewGuid();
                entity.Code = await AutoGenerateEInvoiceCode(InvoiceConstants.InvoiceCodePrefix);
                entity.PaymentStatusCode = InvoiceConstants.PaymentStatusCode.DRAFT;
                entity.PaymentStatusName = InvoiceConstants.PaymentStatusName.DRAFT;
                entity.PaymentStatusColor = InvoiceConstants.PaymentStatusColor.DRAFT;

                #region Tính toán các thông tin khác
                if (entity.EInvoiceItems != null && entity.EInvoiceItems.Count > 0)
                {
                    foreach (var item in entity.EInvoiceItems)
                    {
                        item.Id = Guid.NewGuid();
                        item.LineAmount = item.Quantity * item.UnitPrice;
                        item.VatAmount = item.LineAmount * item.VatPercent / 100;
                    }
                }
                #endregion

                entity.TotalBeforeVatAmount = entity.EInvoiceItems.Sum(x => x.LineAmount);
                entity.TotalVatAmount = entity.EInvoiceItems.Sum(x => x.VatAmount);
                entity.TotalAmount = entity.TotalBeforeVatAmount + entity.TotalVatAmount;
                //entity.TotalAmountInWords = Utils.NumberToWordsConverter.NumberToWords(entity.TotalAmount);
                entity.PaidAmount = 0;
                entity.StillInDebtAmount = entity.TotalAmount - entity.PaidAmount;

                // Tính toán thông tin VAT và lưu vào bảng sm_EInvoiceVatAnalytics
                List<sm_EInvoiceVatAnalytics> listVatAnalytics = new List<sm_EInvoiceVatAnalytics>()
                {
                    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Không kê khai thuế GTGT:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Không chịu thuế GTGT:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Thuế suất 0%:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Thuế suất 5%:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Thuế suất 8%:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Thuế suất 10%:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Thuế suất KHÁC:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id }
                };

                // Nhóm các dịch vụ theo VAT
                var groupByVatPercent = entity.EInvoiceItems.OrderBy(x => x.CreatedOnDate).GroupBy(x => x.VatPercent);
                foreach (var group in groupByVatPercent)
                {
                    decimal beforeVatAmount = group.Sum(x => x.LineAmount);
                    decimal vatAmount = group.Sum(x => x.VatAmount);
                    decimal totalPaymentAmount = beforeVatAmount + vatAmount;

                    // Xác định vị trí của dòng thuế suất cần cập nhật
                    sm_EInvoiceVatAnalytics vatRow = listVatAnalytics.FirstOrDefault(x =>
                        (group.Key == 0 && x.Synthetic == "Thuế suất 0%:") ||
                        (group.Key == 5 && x.Synthetic == "Thuế suất 5%:") ||
                        (group.Key == 8 && x.Synthetic == "Thuế suất 8%:") ||
                        (group.Key == 10 && x.Synthetic == "Thuế suất 10%:") ||
                        (group.Key != 0 && group.Key != 5 && group.Key != 8 && group.Key != 10 && x.Synthetic == "Thuế suất KHÁC:")
                    );

                    // Nếu tìm thấy dòng phù hợp, cập nhật giá trị
                    if (vatRow != null)
                    {
                        vatRow.BeforeVatAmount += beforeVatAmount;
                        vatRow.VatAmount += vatAmount;
                        vatRow.TotalPaymentAmount += totalPaymentAmount;
                    }
                }

                // Thêm dòng Tổng cộng
                var vatAnalyticsTotal = new sm_EInvoiceVatAnalytics
                {
                    Id = Guid.NewGuid(),
                    Synthetic = "Tổng cộng:",
                    BeforeVatAmount = listVatAnalytics.Sum(x => x.BeforeVatAmount),
                    VatAmount = listVatAnalytics.Sum(x => x.VatAmount),
                    TotalPaymentAmount = listVatAnalytics.Sum(x => x.TotalPaymentAmount),
                    EInvoiceId = entity.Id
                };

                listVatAnalytics.Add(vatAnalyticsTotal);
                entity.EInvoiceVatAnalytics = listVatAnalytics;

                _dbContext.sm_EInvoice.Add(entity);

                await _dbContext.SaveChangesAsync();

                var result = await GetByIdAsync(entity.Id);

                if (result.IsSuccess)
                    result.Message = "Thêm mới hóa đơn thành công.";

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<EInvoiceViewModel>(ex);
            }
        }

        public async Task<Response> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_EInvoice.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse("Hóa đơn không tồn tại");

                if (entity.PaymentStatusCode != InvoiceConstants.PaymentStatusCode.DRAFT)
                    return Helper.CreateBadRequestResponse("Hóa đơn đã được xác nhận, không thể xóa");

                _dbContext.Remove(entity);

                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse("Xóa hóa đơn thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response<EInvoiceViewModel>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_EInvoice
                    .AsNoTracking()
                    .Include(x => x.EInvoiceItems.OrderBy(x => x.LineNumber))
                    .Include(x => x.EInvoiceVatAnalytics.OrderBy(x => x.CreatedOnDate))
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<EInvoiceViewModel>("Hóa đơn không tồn tại trong hệ thống.");

                var result = _mapper.Map<EInvoiceViewModel>(entity);

                return new Response<EInvoiceViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<EInvoiceViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<EInvoiceViewModel>>> GetPageAsync(EInvoiceQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_EInvoice
                    .AsNoTracking()
                    .Include(x => x.EInvoiceItems.OrderBy(x => x.LineNumber))
                    .Include(x => x.EInvoiceVatAnalytics.OrderBy(x => x.CreatedOnDate))
                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<EInvoiceViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<EInvoiceViewModel>>(ex);
            }
        }

        public async Task<Response<EInvoiceViewModel>> UpdateAsync(Guid id, EInvoiceCreateUpdateModel model, Helper.RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_EInvoice.Include(x => x.EInvoiceItems).Include(x => x.EInvoiceVatAnalytics).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<EInvoiceViewModel>(string.Format("Hóa đơn không tồn tại trong hệ thống"));

                if (entity.PaymentStatusCode != InvoiceConstants.PaymentStatusCode.DRAFT)
                    return Helper.CreateBadRequestResponse<EInvoiceViewModel>("Hóa đơn đã được kích hoạt, không thể chỉnh sửa");

                if (model.EInvoiceItems != null && model.EInvoiceItems.Count > 0)
                {
                    // Check danh sách items không được trùng nhau
                    var checkDuplicateItems = model.EInvoiceItems.GroupBy(x => x.Name).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (checkDuplicateItems.Count > 0)
                        return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Danh sách hàng hóa, dịch vụ không được trùng nhau.");

                    // Check danh sách items số lượng phải > 0
                    var checkQuantityItems = model.EInvoiceItems.Where(x => x.Quantity <= 0).ToList();
                    if (checkQuantityItems.Count > 0)
                        return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Số lượng hàng hóa, dịch vụ không được nhỏ hơn hoặc bằng 0.");

                    // Check danh sách items đơn giá phải > 0
                    var checkUnitPriceItems = model.EInvoiceItems.Where(x => x.UnitPrice < 0).ToList();
                    if (checkUnitPriceItems.Count > 0)
                        return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Đơn giá hàng hóa, dịch vụ không được nhỏ hơn 0.");

                    // Check danh sách items số lượng phải > 0
                    var checkQuantityUnitPriceItems = model.EInvoiceItems.Where(x => x.Quantity <= 0).ToList();
                    if (checkQuantityUnitPriceItems.Count > 0)
                        return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Số lượng hàng hóa, dịch vụ không được nhỏ hơn hoặc bằng 0.");

                    #region Tính toán số thứ tự items
                    model.EInvoiceItems = model.EInvoiceItems.OrderBy(x => x.LineNumber).ToList();
                    for (int i = 0; i < model.EInvoiceItems.Count; i++)
                        model.EInvoiceItems[i].LineNumber = i + 1;
                    #endregion
                }

                if (model.EInvoiceItems == null || model.EInvoiceItems.Count == 0)
                    return Helper.CreateBadRequestResponse<EInvoiceViewModel>($"Danh sách hàng hóa, dịch vụ không được để trống.");

                // Cập nhật các thông tin
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;

                #region Tính toán các thông tin khác
                _dbContext.RemoveRange(entity.EInvoiceItems);
                entity.EInvoiceItems = new List<sm_EInvoiceItems>();

                foreach (var modelItem in model.EInvoiceItems)
                {
                    // Readd new item
                    var item = _mapper.Map<sm_EInvoiceItems>(modelItem);
                    entity.EInvoiceItems.Add(item);
                }

                if (entity.EInvoiceItems != null && entity.EInvoiceItems.Count > 0)
                {
                    foreach (var item in entity.EInvoiceItems)
                    {
                        item.Id = Guid.NewGuid();
                        item.LineAmount = item.Quantity * item.UnitPrice;
                        item.VatAmount = item.LineAmount * item.VatPercent / 100;
                        _dbContext.sm_EInvoiceItems.Add(item);
                    }
                }
                #endregion

                entity.TotalBeforeVatAmount = entity.EInvoiceItems.Sum(x => x.LineAmount);
                entity.TotalVatAmount = entity.EInvoiceItems.Sum(x => x.VatAmount);
                entity.TotalAmount = entity.TotalBeforeVatAmount + entity.TotalVatAmount;
                //entity.TotalAmountInWords = Utils.NumberToWordsConverter.NumberToWords(entity.TotalAmount);
                entity.PaidAmount = 0;
                entity.StillInDebtAmount = entity.TotalAmount - entity.PaidAmount;

                // Tính toán thông tin VAT và lưu vào bảng sm_EInvoiceVatAnalytics
                List<sm_EInvoiceVatAnalytics> listVatAnalytics = new List<sm_EInvoiceVatAnalytics>()
                {
                    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Không kê khai thuế GTGT:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Không chịu thuế GTGT:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Thuế suất 0%:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Thuế suất 5%:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Thuế suất 8%:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Thuế suất 10%:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id },
    new sm_EInvoiceVatAnalytics { Id = Guid.NewGuid(), Synthetic = "Thuế suất KHÁC:", BeforeVatAmount = 0, VatAmount = 0, TotalPaymentAmount = 0, EInvoiceId = entity.Id }
                };

                // Nhóm các dịch vụ theo VAT
                var groupByVatPercent = entity.EInvoiceItems.OrderBy(x => x.CreatedOnDate).GroupBy(x => x.VatPercent);
                foreach (var group in groupByVatPercent)
                {
                    decimal beforeVatAmount = group.Sum(x => x.LineAmount);
                    decimal vatAmount = group.Sum(x => x.VatAmount);
                    decimal totalPaymentAmount = beforeVatAmount + vatAmount;

                    // Xác định vị trí của dòng thuế suất cần cập nhật
                    sm_EInvoiceVatAnalytics vatRow = listVatAnalytics.FirstOrDefault(x =>
                        (group.Key == 0 && x.Synthetic == "Thuế suất 0%:") ||
                        (group.Key == 5 && x.Synthetic == "Thuế suất 5%:") ||
                        (group.Key == 8 && x.Synthetic == "Thuế suất 8%:") ||
                        (group.Key == 10 && x.Synthetic == "Thuế suất 10%:") ||
                        (group.Key != 0 && group.Key != 5 && group.Key != 8 && group.Key != 10 && x.Synthetic == "Thuế suất KHÁC:")
                    );

                    // Nếu tìm thấy dòng phù hợp, cập nhật giá trị
                    if (vatRow != null)
                    {
                        vatRow.BeforeVatAmount += beforeVatAmount;
                        vatRow.VatAmount += vatAmount;
                        vatRow.TotalPaymentAmount += totalPaymentAmount;
                    }
                }

                // Thêm dòng Tổng cộng
                var vatAnalyticsTotal = new sm_EInvoiceVatAnalytics
                {
                    Id = Guid.NewGuid(),
                    Synthetic = "Tổng cộng:",
                    BeforeVatAmount = listVatAnalytics.Sum(x => x.BeforeVatAmount),
                    VatAmount = listVatAnalytics.Sum(x => x.VatAmount),
                    TotalPaymentAmount = listVatAnalytics.Sum(x => x.TotalPaymentAmount),
                    EInvoiceId = entity.Id
                };

                listVatAnalytics.Add(vatAnalyticsTotal);
                entity.EInvoiceVatAnalytics = listVatAnalytics;

                _dbContext.sm_EInvoice.Update(entity);

                await _dbContext.SaveChangesAsync();

                var result = await GetByIdAsync(entity.Id);

                if (result.IsSuccess)
                    result.Message = "Chỉnh sửa hóa đơn thành công.";

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<EInvoiceViewModel>(ex);
            }
        }

        private Expression<Func<sm_EInvoice, bool>> BuildQuery(EInvoiceQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_EInvoice>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate = predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.BuyerName.ToLower().Contains(query.FullTextSearch.ToLower())
                );

            if (!string.IsNullOrEmpty(query.PaymentStatusCode))
                predicate = predicate.And(s => s.PaymentStatusCode == query.PaymentStatusCode);

            if (query.CreatedOnDateRange != null && query.CreatedOnDateRange.Count() > 0)
            {
                if (query.CreatedOnDateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.CreatedOnDateRange[0].Value.Date);

                if (query.CreatedOnDateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.CreatedOnDateRange[1].Value.Date);
            }

            return predicate;
        }

        public async Task<string> AutoGenerateEInvoiceCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_EInvoice
                    .AsNoTracking()
                    .Where(x => x.Code.Contains(code))
                    .OrderByDescending(x => x.CreatedOnDate)
                    .FirstOrDefaultAsync();

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

        public async Task<Response> ActiveAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_EInvoice.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse("Hóa đơn không tồn tại");

                if (entity.PaymentStatusCode != InvoiceConstants.PaymentStatusCode.DRAFT)
                    return Helper.CreateBadRequestResponse("Hóa đơn đã được kích hoạt, không thể kích hoạt thêm lần nữa");

                entity.PaymentStatusCode = InvoiceConstants.PaymentStatusCode.NOT_YET_PAID;
                entity.PaymentStatusName = InvoiceConstants.PaymentStatusName.NOT_YET_PAID;
                entity.PaymentStatusColor = InvoiceConstants.PaymentStatusColor.NOT_YET_PAID;

                _dbContext.Update(entity);

                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse("Kích hoạt hóa đơn thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response<EInvoiceViewModel>> PaymentHistoryAsync(Guid id, PaymentHistoryViewModel model, Helper.RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_EInvoice.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<EInvoiceViewModel>("Hóa đơn không tồn tại trong hệ thống.");

                if (entity.PaymentStatusCode == InvoiceConstants.PaymentStatusCode.DRAFT)
                    return Helper.CreateBadRequestResponse<EInvoiceViewModel>("Hóa đơn chưa được kích hoạt, không thể thực hiện thanh toán.");

                if (entity.PaymentStatusCode == InvoiceConstants.PaymentStatusCode.PAID)
                    return Helper.CreateBadRequestResponse<EInvoiceViewModel>("Hóa đơn đã được thanh toán, không thể thực hiện thanh toán lại.");

                #region Số tiền thanh toán phải lớn hơn 0, Số tiền thanh toán không được lớn hơn số tiền còn nợ
                if (model.Amount < 0)
                    return Helper.CreateBadRequestResponse<EInvoiceViewModel>("Số tiền thanh toán phải lớn hơn 0.");

                if (model.Amount == 0)
                    return Helper.CreateBadRequestResponse<EInvoiceViewModel>("Số tiền thanh toán không được bằng 0.");

                if (model.Amount > entity.StillInDebtAmount)
                    return Helper.CreateBadRequestResponse<EInvoiceViewModel>("Số tiền thanh toán không được lớn hơn số tiền còn nợ.");
                #endregion

                entity.PaidAmount += model.Amount;
                entity.StillInDebtAmount = entity.TotalAmount - entity.PaidAmount;

                switch (entity.StillInDebtAmount)
                {
                    case 0:
                        entity.PaymentStatusCode = InvoiceConstants.PaymentStatusCode.PAID;
                        entity.PaymentStatusName = InvoiceConstants.PaymentStatusName.PAID;
                        entity.PaymentStatusColor = InvoiceConstants.PaymentStatusColor.PAID;
                        break;
                    case > 0:
                        entity.PaymentStatusCode = InvoiceConstants.PaymentStatusCode.PARTIAL_PAYMENT;
                        entity.PaymentStatusName = InvoiceConstants.PaymentStatusName.PARTIAL_PAYMENT;
                        entity.PaymentStatusColor = InvoiceConstants.PaymentStatusColor.PARTIAL_PAYMENT;
                        break;
                }

                entity.ListOfPaymentHistory.Add(new jsonb_PaymentInvoice
                {
                    Id = Guid.NewGuid(),
                    PaymentMethodCode = model.PaymentMethodCode,
                    PaymentMethodName = CodeTypeCollection.Instance.FetchCode(model.PaymentMethodCode, "vn", entity.TenantId)?.Title,
                    Amount = model.Amount,
                    PaymentOnDate = model.PaymentOnDate,
                    PaymentByUserName = currentUser.FullName,
                    Note = model.Note
                });

                _dbContext.sm_EInvoice.Update(entity);

                await _dbContext.SaveChangesAsync();

                var result = await GetByIdAsync(entity.Id);

                if (result.IsSuccess)
                    result.Message = "Thanh toán hóa đơn thành công.";

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Model: {@Model}", id, model);
                return Helper.CreateExceptionResponse<EInvoiceViewModel>(ex);
            }
        }
    }
}
