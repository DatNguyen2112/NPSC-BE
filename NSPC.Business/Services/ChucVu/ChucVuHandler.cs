using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data.Entity.PhongBan;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.ChucVu
{
    public class ChucVuHandler : IChucVuHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public ChucVuHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<Response<ChucVuViewModel>> Create(ChucVuCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                if (_dbContext.mk_ChucVu.Any(x => x.MaChucVu == model.MaChucVu))
                    return Helper.CreateBadRequestResponse<ChucVuViewModel>(string.Format("Mã chức vụ {0} đã tồn tại!", model.MaChucVu));

                var entity = _mapper.Map<mk_ChucVu>(model);
                entity.Id = Guid.NewGuid();
                //entity.TrangThai = TrangThaiKhachHangConstants.KHACH_MOI;
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.mk_ChucVu.Add(entity);

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<ChucVuViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<ChucVuViewModel>(ex);
            }
        }

        public async Task<Response<ChucVuViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_ChucVu.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<ChucVuViewModel>(string.Format("Chức vụ không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<ChucVuViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<ChucVuViewModel>(ex);
            }
        }

        public async Task<Response<ChucVuViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_ChucVu.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<ChucVuViewModel>("Mã chức vụ không tồn tại trong hệ thống.");

                var result = _mapper.Map<ChucVuViewModel>(entity);

                return new Response<ChucVuViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<ChucVuViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<ChucVuViewModel>>> GetPage(ChucVuQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.mk_ChucVu.AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<ChucVuViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<ChucVuViewModel>>(ex);
            }
        }
        private Expression<Func<mk_ChucVu, bool>> BuildQuery(ChucVuQueryModel query)
        {
            var predicate = PredicateBuilder.New<mk_ChucVu>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.MaChucVu.ToLower().Contains(query.FullTextSearch.ToLower()) || s.TenChucVu.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.MaChucVu))
                predicate.And(s => s.MaChucVu == query.MaChucVu);

            if (!string.IsNullOrEmpty(query.TenChucVu))
                predicate.And(s => s.TenChucVu.Contains(query.TenChucVu));

            return predicate;
        }

        public async Task<Response<ChucVuViewModel>> Update(Guid id, ChucVuCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.mk_ChucVu.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<ChucVuViewModel>(string.Format("Chức vụ không tồn tại trong hệ thống!"));

                if (_dbContext.mk_ChucVu.Any(x => x.MaChucVu == model.MaChucVu && x.Id != id))
                    return Helper.CreateBadRequestResponse<ChucVuViewModel>(string.Format("Mã chức vụ {0} đã tồn tại!", model.MaChucVu));

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<ChucVuViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<ChucVuViewModel>(ex);
            }
        }
    }
}
