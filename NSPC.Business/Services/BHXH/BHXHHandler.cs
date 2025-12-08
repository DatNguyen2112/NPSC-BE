using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.BHXH;
using NSPC.Data.Data.Entity.PhongBan;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.BHXH
{
    public class BHXHHandler : IBHXHHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public BHXHHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<Response<BHXHViewModel>> Create(BHXHCreateUpdateModel model)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = _mapper.Map<mk_BHXH>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.TongNguoiLaoDong = model.BHXHNguoiLaoDong + model.BHYTNguoiLaoDong + model.BHTNNguoiLaoDong;
                entity.TongNguoiSuDungLaoDong = model.BHXHNguoiSuDungLaoDong + model.BHYTNguoiSuDungLaoDong + model.BHTNNguoiSuDungLaoDong;
                entity.TongTatCa = entity.TongNguoiLaoDong + entity.TongNguoiSuDungLaoDong;
                _dbContext.mk_BHXH.Add(entity);

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<BHXHViewModel>(entity), "Thêm mới thành công");
            } catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<BHXHViewModel>(ex);
            }
        }

        public async Task<Response<BHXHViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_BHXH.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<BHXHViewModel>(string.Format("BHXH không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<BHXHViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<BHXHViewModel>(ex);
            }
        }

        public async Task<Response<BHXHViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_BHXH.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<BHXHViewModel>("BHXH không tồn tại trong hệ thống.");

                var result = _mapper.Map<BHXHViewModel>(entity);

                return new Response<BHXHViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<BHXHViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<BHXHViewModel>>> GetPage(BHXHQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.mk_BHXH.AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<BHXHViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<BHXHViewModel>>(ex);
            }
        }
        private Expression<Func<mk_BHXH, bool>> BuildQuery(BHXHQueryModel query)
        {
            var predicate = PredicateBuilder.New<mk_BHXH>(true);
            //if (!string.IsNullOrEmpty(query.FullTextSearch))
            //    predicate.And(s => s.MaPhongBan.ToLower().Contains(query.FullTextSearch.ToLower()) || s.TenPhongBan.ToLower().Contains(query.FullTextSearch.ToLower()));

            //if (!string.IsNullOrEmpty(query.MaPhongBan))
            //    predicate.And(s => s.MaPhongBan == query.MaPhongBan);

            //if (!string.IsNullOrEmpty(query.TenPhongBan))
            //    predicate.And(s => s.TenPhongBan.Contains(query.TenPhongBan));

            return predicate;
        }

        public async Task<Response<BHXHViewModel>> Update(Guid id, BHXHCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.mk_BHXH.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<BHXHViewModel>(string.Format("BHXH không tồn tại trong hệ thống!"));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.TongNguoiLaoDong = model.BHXHNguoiLaoDong + model.BHYTNguoiLaoDong + model.BHTNNguoiLaoDong;
                entity.TongNguoiSuDungLaoDong = model.BHXHNguoiSuDungLaoDong + model.BHYTNguoiSuDungLaoDong + model.BHTNNguoiSuDungLaoDong;
                entity.TongTatCa = entity.TongNguoiLaoDong + entity.TongNguoiSuDungLaoDong;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<BHXHViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<BHXHViewModel>(ex);
            }
        }
    }
}
