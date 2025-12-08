using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.ChucVu;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data.Entity.NhomVatTu;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.NhomVatTu
{
    public class NhomVatTuHandler : INhomVatTuHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public NhomVatTuHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<Response<NhomVatTuViewModel>> Create(NhomVatTuCreateUpdateModel model)
        {
            try
            {
                if (_dbContext.mk_NhomVatTu.Any(x => x.MaNhom == model.MaNhom))
                    return Helper.CreateBadRequestResponse<NhomVatTuViewModel>(string.Format("Mã nhóm vật tư {0} đã tồn tại!", model.MaNhom));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = _mapper.Map<mk_NhomVatTu>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.mk_NhomVatTu.Add(entity);

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<NhomVatTuViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<NhomVatTuViewModel>(ex);
            }
        }

        public async Task<Response<NhomVatTuViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_NhomVatTu.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<NhomVatTuViewModel>(string.Format("Nhóm vật tư không tồn tại trong hệ thống"));

                var vatTu = await _dbContext.sm_Product.FirstOrDefaultAsync(x => x.ProductGroupId == id);
                if (vatTu != null)
                    return Helper.CreateNotFoundResponse<NhomVatTuViewModel>(string.Format("Nhóm vật tư đã tồn tại vật tư không thể xóa"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<NhomVatTuViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<NhomVatTuViewModel>(ex);
            }
        }

        public async Task<Response<NhomVatTuViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_NhomVatTu.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<NhomVatTuViewModel>("Mã nhóm vật tư không tồn tại trong hệ thống.");

                var result = _mapper.Map<NhomVatTuViewModel>(entity);

                return new Response<NhomVatTuViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<NhomVatTuViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<NhomVatTuViewModel>>> GetPage(NhomVatTuQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.mk_NhomVatTu.AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<NhomVatTuViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<NhomVatTuViewModel>>(ex);
            }
        }
        private Expression<Func<mk_NhomVatTu, bool>> BuildQuery(NhomVatTuQueryModel query)
        {
            var predicate = PredicateBuilder.New<mk_NhomVatTu>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.MaNhom.ToLower().Contains(query.FullTextSearch.ToLower()) || s.TenNhom.ToLower().Contains(query.FullTextSearch.ToLower()) || s.GhiChu.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.MaNhom))
                predicate.And(s => s.MaNhom == query.MaNhom);

            if (!string.IsNullOrEmpty(query.TenNhom))
                predicate.And(s => s.TenNhom == query.TenNhom);

            if (!string.IsNullOrEmpty(query.GhiChu))
                predicate.And(s => s.GhiChu.Contains(query.GhiChu));

            return predicate;
        }

        public async Task<Response<NhomVatTuViewModel>> Update(Guid id, NhomVatTuCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.mk_NhomVatTu.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<NhomVatTuViewModel>(string.Format("Nhóm vật tư không tồn tại trong hệ thống!"));

                if (_dbContext.mk_NhomVatTu.Any(x => x.MaNhom == model.MaNhom && x.Id != id))
                    return Helper.CreateBadRequestResponse<NhomVatTuViewModel>(string.Format("Mã nhóm vật tư {0} đã tồn tại!", model.MaNhom));
                
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<NhomVatTuViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<NhomVatTuViewModel>(ex);
            }
        }
    }
}
