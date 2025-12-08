using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using System.Linq.Expressions;

namespace NSPC.Business.Services.Business
{
    public class KhoHandler : IKhoHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;

        public KhoHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<KhoViewModel>> Create(KhoCreateUpdateModel model)
        {
            try
            {
                if (_dbContext.sm_Kho.Any(x => x.Ma == model.Ma))
                    return Helper.CreateBadRequestResponse<KhoViewModel>(string.Format("Mã Kho {0} đã tồn tại",
                        model.Ma));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = _mapper.Map<sm_Kho>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.sm_Kho.Add(entity);

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<KhoViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<KhoViewModel>(ex);
            }
        }

        public async Task<Response<KhoViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Kho.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<KhoViewModel>(
                        string.Format("Kho không tồn tại trong hệ thống"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<KhoViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<KhoViewModel>(ex);
            }
        }

        public async Task<Response<KhoViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Kho.Include(x => x.Customer).AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<KhoViewModel>("Mã Kho không tồn tại trong hệ thống");

                var result = _mapper.Map<KhoViewModel>(entity);
                result.KhachHang = _mapper.Map<KhachHangViewModel>(entity.Customer);
                return new Response<KhoViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<KhoViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<KhoViewModel>>> GetPage(KhoQueryModel query)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_Kho.AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<KhoViewModel>>(data);

                if (!currentUser.ListRights.Contains("KHO." + RightActionConstants.VIEWALL))
                    predicate.And(s => s.CreatedByUserId == currentUser.UserId);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<KhoViewModel>>(ex);
            }
        }

        private Expression<Func<sm_Kho, bool>> BuildQuery(KhoQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var predicate = PredicateBuilder.New<sm_Kho>(true);

            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s =>
                    s.Ma.ToLower().Contains(query.FullTextSearch.ToLower()) ||
                    s.Ten.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.LoaiKho))
                predicate.And(s => s.LoaiKho == query.LoaiKho);

            if (query.IsInitialized != null)
            {
                predicate.And(s => s.IsInitialized == query.IsInitialized);
            }

            if (query.KhachHangId.HasValue)
                predicate.And(s => s.CustomerId == query.KhachHangId.Value);
            if (query.Id.HasValue)
                predicate.And(s => s.Id == query.Id.Value);

            /*
             * Nếu không có quyền KHO.VIEWALL và query không phải là Unlimited
             * thì select ra các kho của User tạo ra + các kho Nhà máy
             */

            if (!currentUser.ListRights.Contains("KHO." + RightActionConstants.VIEWALL) && !query.UnLimeted)
                predicate.And(s => s.CreatedByUserId == currentUser.UserId || s.LoaiKho == "KHO_NHA_MAY");
            return predicate;
        }

        public async Task<Response<KhoViewModel>> Update(Guid id, KhoCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.sm_Kho.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<KhoViewModel>(
                        string.Format("Kho không tồn tại trong hệ thống"));

                if (_dbContext.sm_Kho.Any(x => x.Ma == model.Ma && x.Id != id))
                    return Helper.CreateBadRequestResponse<KhoViewModel>(string.Format("Mã Kho {0} đã tồn tại",
                        model.Ma));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<KhoViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<KhoViewModel>(ex);
            }
        }

        public async Task<Response> DeleteMany(List<Guid> ids)
        {
            try
            {
                var entity = await _dbContext.sm_Kho.Where(x => ids.Contains(x.Id)).ToListAsync();
                if (!entity.Any())
                    return Helper.CreateBadRequestResponse("Vui lòng chọn ít nhất 1 kho");
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
    }
}