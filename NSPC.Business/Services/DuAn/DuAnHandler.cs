using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using NSPC.Business.Services.ChucVu;
using NSPC.Common;
using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NSPC.Data.Data.Entity.DuAn;
using Microsoft.EntityFrameworkCore;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.DuAn
{
    public class DuAnHandler : IDuAnHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public DuAnHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<DuAnViewModel>> Create(DuAnCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                if (_dbContext.mk_DuAn.Any(x => x.MaDuAn == model.MaDuAn))
                    return Helper.CreateBadRequestResponse<DuAnViewModel>(string.Format("Mã dự án {0} đã tồn tại!", model.MaDuAn));

                var entity = _mapper.Map<mk_DuAn>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.mk_DuAn.Add(entity);

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<DuAnViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<DuAnViewModel>(ex);
            }
        }

        public async Task<Response<DuAnViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_DuAn.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<DuAnViewModel>(string.Format("Dự án không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<DuAnViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<DuAnViewModel>(ex);
            }
        }

        public async Task<Response<DuAnViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_DuAn.AsNoTracking()
                    .Include(x => x.sm_Quotation)
                    .Include(x => x.sm_Cashbook_Transaction)
                    .Include(x => x.sm_InventoryNote)
                    .Include(x => x.sm_PurchaseOrder)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<DuAnViewModel>("Mã dự án không tồn tại trong hệ thống.");

                var result = _mapper.Map<DuAnViewModel>(entity);

                return new Response<DuAnViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<DuAnViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<DuAnViewModel>>> GetPage(DuAnQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.mk_DuAn.Include(x => x.sm_Cashbook_Transaction).AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<DuAnViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<DuAnViewModel>>(ex);
            }
        }
        private Expression<Func<mk_DuAn, bool>> BuildQuery(DuAnQueryModel query)
        {
            var predicate = PredicateBuilder.New<mk_DuAn>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.MaDuAn.ToLower().Contains(query.FullTextSearch.ToLower()) || s.TenDuAn.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.MaDuAn))
                predicate.And(s => s.MaDuAn == query.MaDuAn);

            if (!string.IsNullOrEmpty(query.TenDuAn))
                predicate.And(s => s.TenDuAn.Contains(query.TenDuAn));

            return predicate;
        }

        public async Task<Response<DuAnViewModel>> Update(Guid id, DuAnCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.mk_DuAn.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<DuAnViewModel>(string.Format("Dự án không tồn tại trong hệ thống!"));

                if (_dbContext.mk_DuAn.Any(x => x.MaDuAn == model.MaDuAn && x.Id != id))
                    return Helper.CreateBadRequestResponse<DuAnViewModel>(string.Format("Mã dự án {0} đã tồn tại!", model.MaDuAn));

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<DuAnViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<DuAnViewModel>(ex);
            }
        }
    }
}
