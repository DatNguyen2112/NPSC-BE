using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.QuanLyLoaiXe;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using System.Linq.Expressions;

namespace NSPC.Business.Services.Business
{
    public class LoaiXeHandler : ILoaiXeHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public LoaiXeHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<Response<LoaiXeViewModel>> Create(LoaiXeCreateUpdateModel model)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = _mapper.Map<sm_LoaiXe>(model);
                //var khoXe = new sm_LoaiXe
                //{
                //    //Ma = model.BienSoXe,
                //    TenLoaiXe = model.TenLoaiXe,
                //    MoTa = model.MoTa,
                //    CreatedByUserId = currentUser.UserId,
                //}
                //_dbContext.sm_LoaiXe.Add(khoXe);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                //entity.WarehouseId = khoXe.Id;
                _dbContext.sm_LoaiXe.Add(entity);

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<LoaiXeViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<LoaiXeViewModel>(ex);
            }
        }

        public async Task<Response<LoaiXeViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_LoaiXe.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LoaiXeViewModel>(string.Format("Phương tiện này không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<LoaiXeViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<LoaiXeViewModel>(ex);
            }
        }

        public async Task<Response> DeleteMany(List<Guid> ids)
        {
            try
            {
                var entity = await _dbContext.sm_LoaiXe.Where(x => ids.Contains(x.Id)).ToListAsync();
                if (!entity.Any())
                    return Helper.CreateBadRequestResponse("Vui lòng chọn ít nhất 1 phương tiện");
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

        public async Task<Response<LoaiXeViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_LoaiXe.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LoaiXeViewModel>("Mã phương tiện không tồn tại trong hệ thống.");

                var result = _mapper.Map<LoaiXeViewModel>(entity);

                return new Response<LoaiXeViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<LoaiXeViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<LoaiXeViewModel>>> GetPage(LoaiXeQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_LoaiXe.AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<LoaiXeViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<LoaiXeViewModel>>(ex);
            }
        }
        private Expression<Func<sm_LoaiXe, bool>> BuildQuery(LoaiXeQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_LoaiXe>(true);
            //if (!string.IsNullOrEmpty(query.FullTextSearch))
            //    predicate.And(s => s.BienSoXe.ToLower().Contains(query.FullTextSearch.ToLower()) || s.BienSoXe.ToLower().Contains(query.FullTextSearch.ToLower()));

            //if (query.IsKhongTaiXe.HasValue && query.IsKhongTaiXe.Value == true)
            //{
            //    predicate.And(s => s.TaiXe == null || s.TaiXe.IdLoaiXe == query.IdTaiXe);
            //}


            //if (!currentUser.ListRights.Contains("XE." + RightActionConstants.VIEWALL))
            //    predicate.And(s => s.CreatedByUserId == currentUser.UserId);
            return predicate;
        }

        public async Task<Response<LoaiXeViewModel>> Update(Guid id, LoaiXeCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.sm_LoaiXe.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LoaiXeViewModel>(string.Format("Loại xe không tồn tại trong hệ thống!"));

                //if (_dbContext.sm_LoaiXe.Any(x => x.BienSoXe == model.BienSoXe && x.Id != id))
                //    return Helper.CreateBadRequestResponse<LoaiXeViewModel>(string.Format("Biển số {0} đã tồn tại!", model.BienSoXe));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<LoaiXeViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<LoaiXeViewModel>(ex);
            }
        }
    }
}
