using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.ChamCong;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.BangTinhLuong;
using NSPC.Data.Data.Entity.ChamCong;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.BangTinhLuong
{
    public class BangTinhLuongHandler : IBangTinhLuongHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public BangTinhLuongHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
        }
        public async Task<Response<BangTinhLuongIdCreatedModel>> Create(BangTinhLuongCreateUpdateModel model)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                if (model.BangLuongItem == null || !model.BangLuongItem.Any())
                    return Helper.CreateBadRequestResponse<BangTinhLuongIdCreatedModel>("Bảng tính lương cần ít nhất 1 danh sách");

                foreach (var item in model.BangLuongItem)
                {
                    item.Tong = item.LuongCoBan + item.CacKhoanTroCap.AnCa + item.CacKhoanTroCap.DienThoai + item.CacKhoanTroCap.TrangPhuc;
                    item.TongNLD = item.BhxhNLD + item.BhytNLD + item.BhtnNLD;
                    item.TongNSDLD = item.BhxhNSDLD + item.BhytNSDLD + item.BhtnNSDLD;
                    item.TongTatCa = item.TongNLD + item.TongNSDLD;
                }

                var entity = _mapper.Map<mk_BangTinhLuong>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;

                entity.TenBangTinhLuong = $"Bảng lương tháng {entity.ThoiGian.Month:00} năm {entity.ThoiGian.Year}";

                _dbContext.mk_BangTinhLuong.Add(entity);
                await _dbContext.SaveChangesAsync();
                var result = new BangTinhLuongIdCreatedModel
                {
                    Id = entity.Id,
                };
                return Helper.CreateSuccessResponse(result, "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<BangTinhLuongIdCreatedModel>(ex);
            }
        }

        public async Task<Response<BangTinhLuongViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_BangTinhLuong.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<BangTinhLuongViewModel>(string.Format("Bảng tính lương không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<BangTinhLuongViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<BangTinhLuongViewModel>(ex);
            }
        }

        public async Task<Response<BangTinhLuongViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_BangTinhLuong.AsNoTracking().Include(x => x.BangLuongItem.OrderBy(x => x.Order)).ThenInclude(x => x.CacKhoanTroCap).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<BangTinhLuongViewModel>("Mã bảng tính lương không tồn tại trong hệ thống.");

                var result = _mapper.Map<BangTinhLuongViewModel>(entity);

                return new Response<BangTinhLuongViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<BangTinhLuongViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<BangTinhLuongViewModel>>> GetPage(BangTinhLuongQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.mk_BangTinhLuong.AsNoTracking().Include(x => x.BangLuongItem).ThenInclude(x => x.CacKhoanTroCap).Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<BangTinhLuongViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<BangTinhLuongViewModel>>(ex);
            }
        }
        private Expression<Func<mk_BangTinhLuong, bool>> BuildQuery(BangTinhLuongQueryModel query)
        {
            var predicate = PredicateBuilder.New<mk_BangTinhLuong>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate = predicate.And(s => s.TenBangTinhLuong.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.KichHoatBangLuong.HasValue)
                predicate = predicate.And(s => s.KichHoatBangLuong == query.KichHoatBangLuong);

            return predicate;
        }

        public async Task<Response<BangTinhLuongViewModel>> Update(Guid id, BangTinhLuongCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.mk_BangTinhLuong.Include(x => x.BangLuongItem).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<BangTinhLuongViewModel>(string.Format("Bảng tính lương không tồn tại trong hệ thống!"));

                if (model.BangLuongItem == null || !model.BangLuongItem.Any())
                    return Helper.CreateBadRequestResponse<BangTinhLuongViewModel>("Bảng tính lương cần ít nhất 1 danh sách");

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                List<mk_BangLuongItem> listBangTinhLuongItem = entity.BangLuongItem.ToList();
                
                _dbContext.RemoveRange(listBangTinhLuongItem);

                _mapper.Map(model, entity);

                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                entity.TenBangTinhLuong = $"Bảng lương tháng {entity.ThoiGian.Month:00} năm {entity.ThoiGian.Year}";

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<BangTinhLuongViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<BangTinhLuongViewModel>(ex);
            }
        }

        public async Task<Response> ChangeActiveStatusAsync(Guid id, bool status, Guid byUserId)
        {
            try
            {
                var currentBangLuong = _dbContext.mk_BangTinhLuong.Where(x => x.Id == id).FirstOrDefault();
                if (currentBangLuong == null)
                    return Helper.CreateNotFoundResponse("Không tìm thấy bảng lương");

                currentBangLuong.KichHoatBangLuong = status;

                await _dbContext.SaveChangesAsync();

                if (status)
                    return Helper.CreateSuccessResponse("Kích hoạt bảng lương thành công!");
                else
                    return Helper.CreateSuccessResponse("Hủy kích hoạt bảng lương thành công.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Status: {@status}, ByUserIds: {@requestUserId}", id, status,
                    byUserId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }
    }
}
