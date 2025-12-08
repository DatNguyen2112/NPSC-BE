using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.Cata;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.DebtTransaction;
using Serilog;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services;
using static NSPC.Common.Helper;

namespace NSPC.Business
{
    public class KhachHangHandler : IKhachHangHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        private readonly IDebtTransactionHandler _debtTransactionHandler;
        private readonly ICustomerServiceCommentHandler _customerServiceCommentHandler;

        public KhachHangHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IDebtTransactionHandler debtTransactionHandler, ICustomerServiceCommentHandler customerServiceCommentHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _debtTransactionHandler = debtTransactionHandler;
            _customerServiceCommentHandler = customerServiceCommentHandler;
        }

        public async Task<Response<KhachHangViewModel>> Create(KhachHangCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                if (_dbContext.sm_Customer.Any(x => x.Code == model.Code))
                    return Helper.CreateBadRequestResponse<KhachHangViewModel>(string.Format("Mã khách hàng {0} đã tồn tại!", model.Code));

                var entity = _mapper.Map<sm_Customer>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.TotalQuotationCount = 0;
                entity.ProvinceName = _dbContext.cata_Province.Where(x => x.ProvinceCode == model.ProvinceCode).Select(x => x.ProvinceName).FirstOrDefault();
                entity.DistrictName = _dbContext.cata_District.Where(x => x.DistrictCode == model.DistrictCode).Select(x => x.DistrictName).FirstOrDefault();
                entity.WardName = _dbContext.cata_Commune.Where(x => x.CommuneCode == model.WardCode).Select(x => x.CommuneName).FirstOrDefault();
                
                // await _customerServiceCommentHandler.Create(new CustomerServiceCommentCreateModel()
                // {
                //     CustomerId = entity.Id,
                //     IsSystemLog = true,
                //     Content = "Tạo mới khách hàng",
                // }, currentUser);
                
                _dbContext.sm_Customer.Add(entity);
                await _dbContext.SaveChangesAsync();

                #region Tạo công nợ khách hàng vào bảng DebtTransaction
                // Tạo bản ghi giao dịch nợ mặc định cho khách hàng
                var debtTransaction = new DebtTransactionCreateModel
                {
                    EntityId = entity.Id,
                    EntityCode = entity.Code,
                    EntityType = DebtTransactionEntityTypesConstants.CUSTOMER,
                    EntityName = entity.Name,
                    OriginalDocumentId = null,
                    OriginalDocumentCode = null,
                    OriginalDocumentType = null,
                    ChangeAmount = entity.DebtAmount,
                    DebtAmount = entity.DebtAmount,
                    Action = DebtTransactionActionsCodesConstants.CUSTOMER_DEBT_INIT,
                    Note = DebtTransactionNotesConstants.CUSTOMER_DEBT_INIT,
                };
                await _debtTransactionHandler.Create(debtTransaction, currentUser);
                #endregion

                return Helper.CreateSuccessResponse(_mapper.Map<KhachHangViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<KhachHangViewModel>(ex);
            }
        }

        public async Task<Response<CustomerSummaryView>> GetCustomerSummary(KhachHangQueryModel query)
        {
            try
            {
                
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<CustomerSummaryView>(ex);
            }
        }

        public async Task<Response<KhachHangViewModel>> Update(Guid id, KhachHangCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Customer
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<KhachHangViewModel>(string.Format("Khách hàng không tồn tại trong hệ thống!"));

                if (model.Code != null)
                {
                    if (_dbContext.sm_Customer.Any(x => x.Code == model.Code && x.Id != id))
                        return Helper.CreateBadRequestResponse<KhachHangViewModel>(string.Format("Mã khách hàng {0} đã tồn tại!", model.Code));
                }
                
                var province = AddressCollection.Instance.FetchProvince(model.ProvinceCode);
                var district = AddressCollection.Instance.FetchDistrict(model.DistrictCode);
                var ward = AddressCollection.Instance.FetchCommune(model.WardCode);

                // _mapper.Map(model, entity);
                if (province != null)
                {
                    entity.ProvinceCode = province.ProvinceCode;
                    entity.ProvinceName = province.ProvinceName;
                }
                if (district != null)
                {
                    entity.DistrictCode = district.DistrictCode;
                    entity.DistrictName = district.DistrictName;
                }
                if (ward != null)
                {
                    entity.WardCode = ward.CommuneCode;
                    entity.WardName = ward.CommuneName;
                }
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;

                if (model.CustomerType != null)
                {
                    entity.CustomerType = model.CustomerType;
                }
                else
                {
                    entity.CustomerType = entity.CustomerType;
                }

                if (model.CustomerSource != null)
                {
                    entity.CustomerSource = model.CustomerSource;
                }
                else
                {
                    entity.CustomerSource = entity.CustomerSource;
                }

                if (model.Address != null)
                {
                    entity.Address = model.Address;
                }
                else
                {
                    entity.Address = entity.Address;
                }

                if (model.Code != null)
                {
                    entity.Code = model.Code;
                }
                else
                {
                    entity.Code = entity.Code;
                }

                if (model.PhoneNumber != null)
                {
                    entity.PhoneNumber = model.PhoneNumber;
                }
                else
                {
                    entity.PhoneNumber = entity.PhoneNumber;
                }

                if (model.Email != null)
                {
                    entity.Email = model.Email;
                }
                else
                {
                    entity.Email = entity.Email;
                }

                if (model.Birthdate != null)
                {
                    entity.Birthdate = model.Birthdate;
                }
                else
                {
                    entity.Birthdate = entity.Birthdate;
                }

                if (model.Note != null)
                {
                    entity.Note = model.Note;
                }
                else
                { 
                    entity.Note = entity.Note;
                }

                if (model.Sex != null)
                {
                    entity.Sex = model.Sex;
                }
                else
                {
                    entity.Sex = entity.Sex;
                }

                if (model.Website != null)
                {
                    entity.Website = model.Website;
                }
                else
                {
                    entity.Website = entity.Website;
                }

                if (model.DebtAmount != 0)
                {
                    entity.DebtAmount = model.DebtAmount;
                }
                else
                {
                    entity.DebtAmount = entity.DebtAmount;
                }

                if (model.ListPersonInCharge != null)
                {
                    entity.ListPersonInCharge = model.ListPersonInCharge;
                }
                else
                {
                    entity.ListPersonInCharge = entity.ListPersonInCharge;
                }

                if (model.LinkFacebook != null)
                {
                    entity.LinkFacebook = model.LinkFacebook;
                }
                else
                {
                    entity.LinkFacebook = entity.LinkFacebook;
                }

                if (model.PaymentMethod != null)
                {
                    entity.PaymentMethod = model.PaymentMethod;
                }
                else
                {
                    entity.PaymentMethod = entity.PaymentMethod;
                }
                
                // Cập nhật TotalQuotationCount bằng cách đếm từ bảng Quotation
                entity.TotalQuotationCount = _dbContext.sm_Quotation
                    .AsNoTracking()
                    .Count(x => x.CustomerId == id);

                _dbContext.sm_Customer.Update(entity);
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<KhachHangViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<KhachHangViewModel>(ex);
            }
        }

        public async Task<Response<KhachHangViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Customer.AsNoTracking()
                    .Include(x => x.CreatedByUser)
                    .Where(x => x.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse<KhachHangViewModel>(string.Format("Khách hàng không tồn tại trong hệ thống!"));

                var result = _mapper.Map<KhachHangViewModel>(entity);

                result.InformationToCopy = $"Tên: {result.Name}\nĐT: {result.PhoneNumber}\nNhu cầu: {result.InitialRequirement}\nNguồn: {string.Join(",",result.CustomerType)}";
                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<KhachHangViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<KhachHangViewModel>>> GetPage(KhachHangQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_Customer
                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<KhachHangViewModel>>(data);

                foreach (var item in result.Content)
                {
                    if (item.LastCareOnDate.HasValue)
                        item.TotalDaysLastCare = (DateTime.Now.Date - item.LastCareOnDate.Value.Date).Days;
                }

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@query}", query);
                return Helper.CreateExceptionResponse<Pagination<KhachHangViewModel>>(ex);
            }
        }

        private Expression<Func<sm_Customer, bool>> BuildQuery(KhachHangQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var predicate = PredicateBuilder.New<sm_Customer>(true);

            if (!currentUser.IsAdmin)
            {
                predicate.And(x => x.ListPersonInCharge.Contains(currentUser.UserId.ToString()));
            }
            
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Name.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.PhoneNumber.ToLower().Contains(query.FullTextSearch.ToLower()) || s.TaxCode.ToLower().Contains(query.FullTextSearch.ToLower()));

            //if (!string.IsNullOrEmpty(query.TrangThai))
            //    predicate.And(s => s.TrangThai == query.TrangThai);
            if (!string.IsNullOrEmpty(query.Sex))
                predicate.And(s => s.Sex == query.Sex);
            if (!string.IsNullOrEmpty(query.PhoneNumber))
                predicate.And(s => s.PhoneNumber == query.PhoneNumber);
            if (!string.IsNullOrEmpty(query.Name))
                predicate.And(s => s.Name == query.Name);
            if (!string.IsNullOrEmpty(query.Code))
                predicate.And(s => s.Code == query.Code);
            if (query.IsActive.HasValue)
                predicate.And(s => s.IsActive == query.IsActive);

            if (!string.IsNullOrEmpty(query.CustomerType))
                predicate.And(s => s.CustomerType == query.CustomerType);
            
            if (!string.IsNullOrEmpty(query.CustomerGroupCode)) 
                predicate.And(s => s.CustomerGroupCode == query.CustomerGroupCode);
            
            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date);
            }

            return predicate;
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Customer.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<KhachHangViewModel>(
                        string.Format("Khách hàng không tồn tại trong hệ thống!"));

                //Chỉ xóa khi khách hàng không có báo giá
                var hasQuotes = _dbContext.sm_Quotation.AsNoTracking().Any(x => x.CustomerId == id);
                if (hasQuotes)
                    return Helper.CreateBadRequestResponse("Khách hàng đã có báo giá, không thể xóa!");

                //Chỉ xóa khi khách hàng không có chi phí
                var hasCashBooks = _dbContext.sm_Cashbook_Transaction.AsNoTracking().Any(x => x.EntityId == id);
                if (hasCashBooks)
                    return Helper.CreateBadRequestResponse("Khách hàng đã có chi phí, không thể xóa!");
                //Chỉ xóa khách hàng có công nợ = 0 trong bảng DebtTransaction
                var debtTransaction = _dbContext.sm_DebtTransaction.AsNoTracking().FirstOrDefault(x => x.EntityId == id);
                if (debtTransaction.DebtAmount != 0)
                    return Helper.CreateBadRequestResponse(string.Format("Khách hàng {0} còn nợ, không thể xóa!", entity.Name));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse("Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<KhachHangViewModel>(ex);
            }
        }

        public async Task<Response<MultipleDeleteModel>> DeleteMultiple(List<Guid> ids)
        {
            try
            {
                var customersHaveDebt = await _dbContext.sm_Customer.Where(x => x.DebtAmount == 0 && ids.Contains(x.Id))
                    .ToListAsync();
                var customersHaveNoDebt = await _dbContext.sm_Customer
                    .Where(x => x.DebtAmount != 0 && ids.Contains(x.Id)).ToListAsync();

                _dbContext.RemoveRange(customersHaveDebt);
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(new MultipleDeleteModel()
                {
                    Deleted = customersHaveDebt.Select(x => x.Name),
                    CannotDelete = customersHaveNoDebt.Select(x => x.Name),
                    Message = "Xử lý dữ liệu hoàn tất"
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<MultipleDeleteModel>(ex);
            }
        }

        public async Task<Response> CheckIfCustomerHasQuotes(Guid id)
        {
            try
            {
                var result = _dbContext.sm_Quotation.Any(x => x.CustomerId == id);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: Customer Id: {@Param}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response<KhachHangViewModel>> ChangeCustomerType(Guid id, KhachHangCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Customer.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<KhachHangViewModel>(string.Format("Khách hàng không tồn tại trong hệ thống!"));
                
                string message = string.Empty;
                
                entity.CustomerType = model.CustomerType;

                switch (model.CustomerType)
                {
                    case CustomerTypeConstants.M:
                        message = "Chuyển mối quan hệ khách hàng thành mới";
                        break;
                    case CustomerTypeConstants.ĐLH:
                        message = "Chuyển mối quan hệ khách hàng thành mới liên hệ";
                        break;
                    case CustomerTypeConstants.HTV:
                        message = "Chuyển mối quan hệ khách hàng thành hẹn tư vấn";
                        break;
                    case CustomerTypeConstants.TT:
                        message = "Chuyển mối quan hệ khách hàng thành trung thành";
                        break;
                    case CustomerTypeConstants.TC:
                        message = "Chuyển mối quan hệ khách hàng thành từ chối";
                        break;
                    case CustomerTypeConstants.K:
                        message = "Chuyển mối quan hệ khách hàng thành khác";
                        break;
                    case CustomerTypeConstants.CHĐ:
                        message = "Chuyển mối quan hệ khách hàng thành chốt hợp đồng";
                        break;
                    case CustomerTypeConstants.KPH:
                        message = "Chuyển mối quan hệ khách hàng thành không phản hồi";
                        break;
                    case CustomerTypeConstants.ĐĐP:
                        message = "Chuyển mối quan hệ khách hàng thành đàm phán";
                        break;
                    case CustomerTypeConstants.TN:
                        message = "Chuyển mối quan hệ khách hàng thành tiềm năng";
                        break;
                }

                // await _customerServiceCommentHandler.Create(new CustomerServiceCommentCreateModel()
                // {
                //     CustomerId = id,
                //     IsSystemLog = true,
                //     Content = message
                // }, currentUser);
                
                await _dbContext.SaveChangesAsync();
                
                return Helper.CreateSuccessResponse<KhachHangViewModel>(_mapper.Map<KhachHangViewModel>(entity), "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}