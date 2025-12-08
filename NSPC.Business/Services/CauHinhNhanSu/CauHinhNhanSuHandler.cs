using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.PhongBan;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.CauHinhNhanSu;
using NSPC.Data.Data.Entity.ChucVu;
using NSPC.Data.Data.Entity.PhongBan;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.CauHinhNhanSu
{
    public class CauHinhNhanSuHandler : ICauHinhNhanSuHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public CauHinhNhanSuHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        //public async Task<Response<CauHinhNhanSuViewModel>> Create(CauHinhNhanSuCreateUpdateModel model)
        //{
        //    try
        //    {
        //        //if (_dbContext.mk_CauHinhNhanSu.Any(x => x.Ma == model.Ma))
        //        //    return Helper.CreateBadRequestResponse<CauHinhNhanSuViewModel>(string.Format("Mã cấu hình nhân sự {0} đã tồn tại!", model.Ma));

        //        var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

        //        var entity = _mapper.Map<mk_CauHinhNhanSu>(model);
        //        entity.Id = Guid.NewGuid();
        //        entity.CreatedByUserId = currentUser.UserId;
        //        entity.CreatedByUserName = currentUser.UserName;
        //        entity.CreatedOnDate = DateTime.Now;
        //        _dbContext.mk_CauHinhNhanSu.Add(entity);

        //        await _dbContext.SaveChangesAsync();
        //        return Helper.CreateSuccessResponse(_mapper.Map<CauHinhNhanSuViewModel>(entity), "Thêm mới thành công");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        Log.Information("Params: Model: {@model}", model);
        //        return Helper.CreateExceptionResponse<CauHinhNhanSuViewModel>(ex);
        //    }
        //}

        //public async Task<Response<CauHinhNhanSuViewModel>> Delete(Guid id)
        //{
        //    try
        //    {
        //        var entity = await _dbContext.mk_CauHinhNhanSu.FirstOrDefaultAsync(x => x.Id == id);
        //        if (entity == null)
        //            return Helper.CreateNotFoundResponse<CauHinhNhanSuViewModel>(string.Format("Cấu hình nhân sự không tồn tại trong hệ thống!"));

        //        _dbContext.Remove(entity);
        //        await _dbContext.SaveChangesAsync();

        //        return Helper.CreateSuccessResponse(_mapper.Map<CauHinhNhanSuViewModel>(entity), "Xóa thành công");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        Log.Information("Params: Model: {@id}", id);
        //        return Helper.CreateExceptionResponse<CauHinhNhanSuViewModel>(ex);
        //    }
        //}

        public async Task<Response> SeedCauHinh()
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var listUser = await _dbContext.IdmUser.ToListAsync();
                var listAllCauHinh = await _dbContext.mk_CauHinhNhanSu.ToListAsync();
                var listCauHinh = new List<mk_CauHinhNhanSu>();
                foreach (var user in listUser)
                {
                    if(!listAllCauHinh.Any(x => x.IdUser == user.Id))
                    {
                        var cauHinh = new mk_CauHinhNhanSu
                        {
                            //Id = Guid.NewGuid(),
                            LuongCoBan = 0,
                            AnCa = 0,
                            DienThoai = 0,
                            TrangPhuc = 0,
                            IdUser = user.Id,
                        };
                        listCauHinh.Add(cauHinh);
                    }
                }
                await _dbContext.AddRangeAsync(listCauHinh);
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("Seed cấu hình nhân sự thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse(ex);
            }
        }   

        public async Task<Response<CauHinhNhanSuViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_CauHinhNhanSu.Include(x => x.idm_User).ThenInclude(x => x.mk_ChucVu).Include(x => x.idm_User).AsNoTracking().FirstOrDefaultAsync(x => x.idm_User.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<CauHinhNhanSuViewModel>("Mã cấu hình nhân sự không tồn tại trong hệ thống.");

                var result = _mapper.Map<CauHinhNhanSuViewModel>(entity);

                return new Response<CauHinhNhanSuViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<CauHinhNhanSuViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<CauHinhNhanSuViewModel>>> GetPage(CauHinhNhanSuQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.mk_CauHinhNhanSu.Include(x => x.idm_User).ThenInclude(x => x.mk_ChucVu).Include(x => x.idm_User).AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<CauHinhNhanSuViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<CauHinhNhanSuViewModel>>(ex);
            }
        }
        private Expression<Func<mk_CauHinhNhanSu, bool>> BuildQuery(CauHinhNhanSuQueryModel query)
        {
            var predicate = PredicateBuilder.New<mk_CauHinhNhanSu>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.idm_User.Ma.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.idm_User.Name.ToLower().Contains(query.FullTextSearch.ToLower())
                //|| s.idm_User.mk_ChucVu.TenChucVu.ToLower().Contains(query.FullTextSearch.ToLower())
                //|| s.idm_User.mk_PhongBan.TenPhongBan.ToLower().Contains(query.FullTextSearch.ToLower())
                );
            if (!string.IsNullOrEmpty(query.ChucVu))
                predicate = predicate.And(s => s.idm_User.mk_ChucVu.TenChucVu == query.ChucVu);
            // if (!string.IsNullOrEmpty(query.PhongBan))
            //     predicate = predicate.And(s => s.idm_User.mk_PhongBan.TenPhongBan == query.PhongBan);

            return predicate;
        }

        public async Task<Response<CauHinhNhanSuViewModel>> Update(Guid id, CauHinhNhanSuCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.mk_CauHinhNhanSu.FirstOrDefaultAsync(x => x.idm_User.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<CauHinhNhanSuViewModel>(string.Format("Cấu hình nhân sự không tồn tại trong hệ thống!"));

                //if (_dbContext.mk_CauHinhNhanSu.Any(x => x.Ma == model.Ma && x.Id != id))
                //    return Helper.CreateBadRequestResponse<CauHinhNhanSuViewModel>(string.Format("Mã cấu hình nhân sự {0} đã tồn tại!", model.Ma));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<CauHinhNhanSuViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<CauHinhNhanSuViewModel>(ex);
            }
        }
    }
}
