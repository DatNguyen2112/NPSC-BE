using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.QuanLyPhuongTien;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using System.Linq.Expressions;

namespace NSPC.Business.Services.Business
{
    public class PhuongTienHandler : IPhuongTienHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public PhuongTienHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<Response<PhuongTienViewModel>> Create(PhuongTienCreateUpdateModel model)
        {
            try
            {
                if (_dbContext.sm_PhuongTien.Any(x => x.BienSoXe == model.BienSoXe))
                    return Helper.CreateBadRequestResponse<PhuongTienViewModel>(string.Format("Biển số xe {0} đã tồn tại!", model.BienSoXe));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = _mapper.Map<sm_PhuongTien>(model);
                var khoXe = new sm_Kho
                {
                    Ma = model.BienSoXe,
                    Ten = model.Model,
                    LoaiKho = "KHO_XE",
                    CreatedByUserId = currentUser.UserId,
                };
                _dbContext.sm_Kho.Add(khoXe);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.WarehouseId = khoXe.Id;
                _dbContext.sm_PhuongTien.Add(entity);

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<PhuongTienViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<PhuongTienViewModel>(ex);
            }
        }

        public async Task<Response<PhuongTienViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_PhuongTien.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<PhuongTienViewModel>(string.Format("Phương tiện này không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<PhuongTienViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<PhuongTienViewModel>(ex);
            }
        }

        public async Task<Response> DeleteMany(List<Guid> ids)
        {
            try
            {
                var entity = await _dbContext.sm_PhuongTien.Where(x => ids.Contains(x.Id)).ToListAsync();
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

        public async Task<Response<PhuongTienViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_PhuongTien.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<PhuongTienViewModel>("Mã phương tiện không tồn tại trong hệ thống.");

                var result = _mapper.Map<PhuongTienViewModel>(entity);

                return new Response<PhuongTienViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<PhuongTienViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<PhuongTienViewModel>>> GetPage(PhuongTienQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_PhuongTien
                    //.Include(x => x.TaiXe)
                    .Include(x => x.LoaiXe)
                    .AsNoTracking()
                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<PhuongTienViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<PhuongTienViewModel>>(ex);
            }
        }
        private Expression<Func<sm_PhuongTien, bool>> BuildQuery(PhuongTienQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_PhuongTien>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.BienSoXe.ToLower().Contains(query.FullTextSearch.ToLower()) || s.BienSoXe.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.IsKhongTaiXe.HasValue && query.IsKhongTaiXe.Value == true)
            {
                predicate.And(s => s.TaiXe == null || s.TaiXe.IdPhuongTien == query.IdTaiXe);
            }


            if (!currentUser.ListRights.Contains("XE." + RightActionConstants.VIEWALL))
                predicate.And(s => s.CreatedByUserId == currentUser.UserId);
            return predicate;
        }

        public async Task<Response<PhuongTienViewModel>> Update(Guid id, PhuongTienCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.sm_PhuongTien.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<PhuongTienViewModel>(string.Format("Phương tiện không tồn tại trong hệ thống!"));

                if (_dbContext.sm_PhuongTien.Any(x => x.BienSoXe == model.BienSoXe && x.Id != id))
                    return Helper.CreateBadRequestResponse<PhuongTienViewModel>(string.Format("Biển số {0} đã tồn tại!", model.BienSoXe));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<PhuongTienViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<PhuongTienViewModel>(ex);
            }
        }
    }
}
