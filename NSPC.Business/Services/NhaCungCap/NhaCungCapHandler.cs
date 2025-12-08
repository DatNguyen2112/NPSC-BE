using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.Cata;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.NhaCungCap;
using NSPC.Data.Data.Entity.PhongBan;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.NhaCungCap
{
    public class NhaCungCapHandler : INhaCungCapHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        private readonly IDebtTransactionHandler _debtTransactionHandler;
        public NhaCungCapHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IDebtTransactionHandler debtTransactionHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _debtTransactionHandler = debtTransactionHandler;
        }

        public async Task<Response<NhaCungCapViewModel>> Create(NhaCungCapCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                if (_dbContext.sm_Supplier.Any(x => x.Code == model.Code))
                    return Helper.CreateBadRequestResponse<NhaCungCapViewModel>(string.Format("Mã nhà cung cấp {0} đã tồn tại!", model.Code));

                var entity = _mapper.Map<sm_Supplier>(model);
                entity.Id = Guid.NewGuid();
                entity.Email = model.Email;
                entity.Description = model.Description;
                entity.ProvinceName = _dbContext.cata_Province.Where(x => x.ProvinceCode == model.ProvinceCode).Select(x => x.ProvinceName).FirstOrDefault();
                entity.DistrictName = _dbContext.cata_District.Where(x => x.DistrictCode == model.DistrictCode).Select(x => x.DistrictName).FirstOrDefault();
                entity.WardName = _dbContext.cata_Commune.Where(x => x.CommuneCode == model.WardCode).Select(x => x.CommuneName).FirstOrDefault();
                entity.IsActive = model.IsActive;

                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.ListAccountBanking = model.ListAccountBanking;
                
                _dbContext.sm_Supplier.Add(entity);
                await _dbContext.SaveChangesAsync();

                #region Tạo công nợ mặc định trong bảng DebtTransaction
                // Tạo bản ghi giao dịch nợ mặc định cho nhà cung cấp nếu có nợ
                var debtTransaction = new DebtTransactionCreateModel
                {
                    EntityId = entity.Id,
                    EntityCode = entity.Code,
                    EntityType = DebtTransactionEntityTypesConstants.SUPPLIER,
                    EntityName = entity.Name,
                    OriginalDocumentId = null,
                    OriginalDocumentCode = null,
                    OriginalDocumentType = null,
                    ChangeAmount = entity.TotalDebtAmount,
                    DebtAmount = entity.TotalDebtAmount,
                    Action = DebtTransactionActionsCodesConstants.SUPPLIER_DEBT_INIT,
                    Note = DebtTransactionNotesConstants.SUPPLIER_DEBT_INIT,
                };
                await _debtTransactionHandler.Create(debtTransaction, currentUser);
                #endregion

                return Helper.CreateSuccessResponse(_mapper.Map<NhaCungCapViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<NhaCungCapViewModel>(ex);
            }
        }

        public async Task<Response<NhaCungCapViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Supplier.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<NhaCungCapViewModel>(string.Format("Nhà cung cấp không tồn tại trong hệ thống!"));

                // Chỉ xóa nhà cung cấp không có trong bảng DebtTransaction
                var debtTransaction = _dbContext.sm_DebtTransaction.AsNoTracking().FirstOrDefault(x => x.EntityId == id);
                if (debtTransaction.DebtAmount != 0)
                    return Helper.CreateBadRequestResponse<NhaCungCapViewModel>(string.Format("Nhà cung cấp {0} còn nợ, không thể xóa!", entity.Name));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<NhaCungCapViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<NhaCungCapViewModel>(ex);
            }
        }

        public async Task<Response<MultipleDeleteModel>> DeleteMultiple(List<Guid> ids)
        {
            try
            {
                var suppliersHaveDebt = await _dbContext.sm_Supplier.Where(x => x.TotalDebtAmount == 0 && ids.Contains(x.Id))
                    .ToListAsync();
                var suppliersHaveNoDebt = await _dbContext.sm_Supplier
                    .Where(x => x.TotalDebtAmount != 0 && ids.Contains(x.Id)).ToListAsync();

                _dbContext.RemoveRange(suppliersHaveDebt);
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(new MultipleDeleteModel()
                {
                    Deleted = suppliersHaveDebt.Select(x => x.Name),
                    CannotDelete = suppliersHaveNoDebt.Select(x => x.Name),
                    Message = "Xử lý dữ liệu hoàn tất"
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<MultipleDeleteModel>(ex);
            }  
        }

        public async Task<Response<NhaCungCapViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Supplier.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<NhaCungCapViewModel>("Mã nhà cung cấp không tồn tại trong hệ thống.");

                var result = _mapper.Map<NhaCungCapViewModel>(entity);

                return new Response<NhaCungCapViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<NhaCungCapViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<NhaCungCapViewModel>>> GetPage(NhaCungCapQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_Supplier.AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<NhaCungCapViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<NhaCungCapViewModel>>(ex);
            }
        }

        private Expression<Func<sm_Supplier, bool>> BuildQuery(NhaCungCapQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_Supplier>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower()) || s.PhoneNumber.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.Code))
                predicate.And(s => s.Code == query.Code);
           

            if (!string.IsNullOrEmpty(query.Name))
                predicate.And(s => s.Name.Contains(query.Name));
            if (query.IsActive.HasValue)
                predicate.And(s => s.IsActive == query.IsActive);
            
            if (!string.IsNullOrEmpty(query.SupplierGroupCode)) 
                predicate.And(s => s.SupplierGroupCode == query.SupplierGroupCode);

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date);

            }

            return predicate;
        }

        public async Task<Response<NhaCungCapViewModel>> Update(Guid id, NhaCungCapCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Supplier.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<NhaCungCapViewModel>(string.Format("Phòng ban không tồn tại trong hệ thống!"));

                if (_dbContext.sm_Supplier.Any(x => x.Code == model.Code && x.Id != id))
                    return Helper.CreateBadRequestResponse<NhaCungCapViewModel>(string.Format("Mã nhà cung cấp {0} đã tồn tại!", model.Code));
                var province = AddressCollection.Instance.FetchProvince(model.ProvinceCode);
                var district = AddressCollection.Instance.FetchDistrict(model.DistrictCode);
                var ward = AddressCollection.Instance.FetchCommune(model.WardCode);

                _mapper.Map(model, entity);
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
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.ListAccountBanking = model.ListAccountBanking;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<NhaCungCapViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<NhaCungCapViewModel>(ex);
            }
        }
    }
}
