using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.QuanLyLaiXe;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.Business
{
    public class LaiXeHandler : ILaiXeHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public LaiXeHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<Response<LaiXeViewModel>> Create(LaiXeCreateUpdateModel model)
        {
            try
            {
                if (_dbContext.sm_LaiXe.Any(x => x.Cccd == model.Cccd))
                    return Helper.CreateBadRequestResponse<LaiXeViewModel>(string.Format("CCCD {0} đã tồn tại!", model.Cccd)); 
                if (_dbContext.sm_LaiXe.Any(x => x.MaTaiXe == model.MaTaiXe))
                    return Helper.CreateBadRequestResponse<LaiXeViewModel>(string.Format("Mã tài xế đã tồn tại!", model.MaTaiXe));

                if (!_dbContext.sm_PhuongTien.Any(x => x.Id == model.IdPhuongTien))
                    return Helper.CreateBadRequestResponse<LaiXeViewModel>("Phương tiện không tồn tại!");

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = _mapper.Map<sm_LaiXe>(model);
                entity.Id = Guid.NewGuid();
                //entity.TrangThai = TrangThaiKhachHangConstants.KHACH_MOI;
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.sm_LaiXe.Add(entity);

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<LaiXeViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<LaiXeViewModel>(ex);
            }
        }

        public async Task<Response<LaiXeViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_LaiXe.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LaiXeViewModel>(string.Format("Tài xế không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<LaiXeViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<LaiXeViewModel>(ex);
            }
        }

        public async Task<Response> DeleteMany(List<Guid> ids)
        {
            try
            {
                var entity = await _dbContext.sm_LaiXe.Where(x => ids.Contains(x.Id)).ToListAsync();
                if (!entity.Any())
                    return Helper.CreateBadRequestResponse("Vui lòng chọn ít nhất 1 tài xế");
                _dbContext.RemoveRange(entity);
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse(ex);
            }
        }

        public async Task<Response<LaiXeViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_LaiXe.Include(x => x.PhuongTien).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LaiXeViewModel>("Mã lái xe không tồn tại trong hệ thống.");

                var result = _mapper.Map<LaiXeViewModel>(entity);

                return new Response<LaiXeViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<LaiXeViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<LaiXeViewModel>>> GetPage(LaiXeQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_LaiXe.Include(x => x.PhuongTien).AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<LaiXeViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<LaiXeViewModel>>(ex);
            }
        }
        private Expression<Func<sm_LaiXe, bool>> BuildQuery(LaiXeQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var predicate = PredicateBuilder.New<sm_LaiXe>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Gplx.ToLower().Contains(query.FullTextSearch.ToLower()) 
                || s.TenTaiXe.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.MaTaiXe.ToLower().Contains(query.FullTextSearch.ToLower())
                );

            if (!string.IsNullOrEmpty(query.TenTaiXe))
                predicate.And(s => s.TenTaiXe == query.TenTaiXe);

            if (!currentUser.ListRights.Contains("TAIXE." + RightActionConstants.VIEWALL))
                predicate.And(s => s.CreatedByUserId == currentUser.UserId);

            if (query.ExcludeHaveUser)
            {
                if (!query.ForUserId.HasValue)
                {
                    predicate.And(x => !x.UserId.HasValue);
                }
                else
                {
                    predicate.And(x => !x.UserId.HasValue || x.UserId == query.ForUserId);
                }
            }

            return predicate;
        }

        public async Task<Response<LaiXeViewModel>> Update(Guid id, LaiXeCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.sm_LaiXe.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LaiXeViewModel>(string.Format("Tài xế không tồn tại trong hệ thống!"));

                if (_dbContext.sm_LaiXe.Any(x => x.Cccd == model.Cccd && x.Id != id))
                    return Helper.CreateBadRequestResponse<LaiXeViewModel>(string.Format("Căn cước công dân {0} đã tồn tại!", model.Cccd));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<LaiXeViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<LaiXeViewModel>(ex);
            }
        }
    }
}
