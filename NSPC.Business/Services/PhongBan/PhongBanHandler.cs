using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.PhongBan;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.PhongBan
{
    public class PhongBanHandler : IPhongBanHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public PhongBanHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<Response<PhongBanViewModel>> Create(PhongBanCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                if (_dbContext.mk_PhongBan.Any(x => x.MaPhongBan == model.MaPhongBan))
                    return Helper.CreateBadRequestResponse<PhongBanViewModel>(string.Format("Mã phòng ban {0} đã tồn tại!", model.MaPhongBan));

                var entity = _mapper.Map<mk_PhongBan>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.mk_PhongBan.Add(entity);

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<PhongBanViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<PhongBanViewModel>(ex);
            }
        }

        public async Task<Response<PhongBanViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_PhongBan.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<PhongBanViewModel>(string.Format("Phòng ban không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<PhongBanViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<PhongBanViewModel>(ex);
            }
        }

        public async Task<Response<PhongBanViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_PhongBan.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<PhongBanViewModel>("Mã phòng ban không tồn tại trong hệ thống.");

                var result = _mapper.Map<PhongBanViewModel>(entity);

                return new Response<PhongBanViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<PhongBanViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<PhongBanViewModel>>> GetPage(PhongBanQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.mk_PhongBan.AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<PhongBanViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<PhongBanViewModel>>(ex);
            }
        }
        private Expression<Func<mk_PhongBan, bool>> BuildQuery(PhongBanQueryModel query)
        {
            var predicate = PredicateBuilder.New<mk_PhongBan>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.MaPhongBan.ToLower().Contains(query.FullTextSearch.ToLower()) || s.TenPhongBan.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.MaPhongBan))
                predicate.And(s => s.MaPhongBan == query.MaPhongBan);

            if (!string.IsNullOrEmpty(query.TenPhongBan))
                predicate.And(s => s.TenPhongBan.Contains(query.TenPhongBan));

            return predicate;
        }

        public async Task<Response<PhongBanViewModel>> Update(Guid id, PhongBanCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.mk_PhongBan.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<PhongBanViewModel>(string.Format("Phòng ban không tồn tại trong hệ thống!"));

                if (_dbContext.mk_PhongBan.Any(x => x.MaPhongBan == model.MaPhongBan && x.Id != id))
                    return Helper.CreateBadRequestResponse<PhongBanViewModel>(string.Format("Mã phòng ban {0} đã tồn tại!", model.MaPhongBan));

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<PhongBanViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<PhongBanViewModel>(ex);
            }
        }
    }
}
