/*using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.ChucVu;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data.Entity.SanPham;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.SanPham
{
    public class SanPhamHandler : ISanPhamHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public SanPhamHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<Response<SanPhamViewModel>> Create(SanPhamCreateUpdateModel model)
        {
            try
            {
                if (_dbContext.mk_SanPham.Any(x => x.MaSanPham == model.MaSanPham))
                    return Helper.CreateBadRequestResponse<SanPhamViewModel>(string.Format("Mã sản phẩm {0} đã tồn tại!", model.MaSanPham));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = _mapper.Map<mk_SanPham>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.mk_SanPham.Add(entity);

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<SanPhamViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<SanPhamViewModel>(ex);
            }
        }

        public async Task<Response<SanPhamViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_SanPham.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<SanPhamViewModel>(string.Format("Sản phẩm không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<SanPhamViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<SanPhamViewModel>(ex);
            }
        }

        public async Task<Response<SanPhamViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_SanPham.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<SanPhamViewModel>("Mã sản phẩm không tồn tại trong hệ thống.");

                var result = _mapper.Map<SanPhamViewModel>(entity);

                return new Response<SanPhamViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<SanPhamViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<SanPhamViewModel>>> GetPage(SanPhamQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.mk_SanPham.AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<SanPhamViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<SanPhamViewModel>>(ex);
            }
        }
        private Expression<Func<mk_SanPham, bool>> BuildQuery(SanPhamQueryModel query)
        {
            var predicate = PredicateBuilder.New<mk_SanPham>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.MaSanPham.ToLower().Contains(query.FullTextSearch.ToLower()) || s.TenSanPham.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.MaSanPham))
                predicate.And(s => s.MaSanPham == query.MaSanPham);

            if (!string.IsNullOrEmpty(query.TenSanPham))
                predicate.And(s => s.TenSanPham.Contains(query.TenSanPham));

            return predicate;
        }

        public async Task<Response<SanPhamViewModel>> Update(Guid id, SanPhamCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.mk_SanPham.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<SanPhamViewModel>(string.Format("Sản phẩm không tồn tại trong hệ thống!"));

                if (_dbContext.mk_SanPham.Any(x => x.MaSanPham == model.MaSanPham && x.Id != id))
                    return Helper.CreateBadRequestResponse<SanPhamViewModel>(string.Format("Mã sản phẩm {0} đã tồn tại!", model.MaSanPham));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<SanPhamViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<SanPhamViewModel>(ex);
            }
        }
    }
}
*/