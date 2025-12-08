using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using OfficeOpenXml;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.ChamCong;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business.Services.ChamCong
{
    public class ChamCongHandler : IChamCongHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public ChamCongHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
        }
        public async Task<Response<ChamCongIdCreatedModel>> Create(ChamCongCreateUpdateModel model)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                if (model.ListChamCong == null || !model.ListChamCong.Any())
                    return Helper.CreateBadRequestResponse<ChamCongIdCreatedModel>("Chấm công cần ít nhất 1 danh sách");

                var entity = _mapper.Map<mk_ChamCong>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;

                //if (_dbContext.mk_ChamCong.Any(x => x.TenBangChamCong == entity.TenBangChamCong))
                //    return Helper.CreateBadRequestResponse<ChamCongIdCreatedModel>(string.Format("Tên bảng chấm công đã tồn tại!", entity.TenBangChamCong));

                _dbContext.mk_ChamCong.Add(entity);
                await _dbContext.SaveChangesAsync();
                var result = new ChamCongIdCreatedModel
                {
                    Id = entity.Id,
                };
                return Helper.CreateSuccessResponse(result, "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<ChamCongIdCreatedModel>(ex);
            }
        }

        public async Task<Response<ChamCongViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_ChamCong.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<ChamCongViewModel>(string.Format("Chấm công không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<ChamCongViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<ChamCongViewModel>(ex);
            }
        }

        public async Task<Response<ChamCongViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.mk_ChamCong.AsNoTracking().Include(x => x.ListChamCong.OrderBy(x => x.Order)).ThenInclude(x => x.NgayTrongThang).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<ChamCongViewModel>("Mã chấm công không tồn tại trong hệ thống.");

                var result = _mapper.Map<ChamCongViewModel>(entity);

                foreach (var item in result.ListChamCong)
                {
                    item.NgayTrongThang = item.NgayTrongThang.OrderBy(x => x.Ngay).ToList();
                }

                return new Response<ChamCongViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<ChamCongViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<ChamCongViewModel>>> GetPage(ChamCongQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.mk_ChamCong.AsNoTracking().Include(x => x.ListChamCong).ThenInclude(x => x.NgayTrongThang).Where(predicate);
                //queryResult = queryResult.OrderBy(x => x.ListChamCong.Min(c => c.NgayTrongThang.Min(n => n.Ngay)));
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<ChamCongViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<ChamCongViewModel>>(ex);
            }
        }
        private Expression<Func<mk_ChamCong, bool>> BuildQuery(ChamCongQueryModel query)
        {
            var predicate = PredicateBuilder.New<mk_ChamCong>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate = predicate.And(s => s.TenBangChamCong.ToLower().Contains(query.FullTextSearch.ToLower())
                );

            if (query.Thang.HasValue)
                predicate = predicate.And(s => s.Thang == query.Thang);

            if (query.Nam.HasValue)
                predicate = predicate.And(s => s.Nam == query.Nam);

            if (query.KichHoatBangChamCong.HasValue)
                predicate = predicate.And(s => s.KichHoatBangChamCong == query.KichHoatBangChamCong);

            if (query.Id != null)
                predicate = predicate.And(s => s.Id == query.Id);

            return predicate;
        }

        public async Task<Response<ChamCongViewModel>> Update(Guid id, ChamCongCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.mk_ChamCong.Include(x => x.ListChamCong).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<ChamCongViewModel>(string.Format("Chấm công không tồn tại trong hệ thống!"));

                if (model.ListChamCong == null || !model.ListChamCong.Any())
                    return Helper.CreateBadRequestResponse<ChamCongViewModel>("Chấm công cần ít nhất 1 danh sách");

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                List<mk_ChamCongItem> listChamCongItem = entity.ListChamCong.ToList();
                
                _dbContext.RemoveRange(listChamCongItem);

                _mapper.Map(model, entity);

                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                //if (_dbContext.mk_ChamCong.Any(x => x.TenBangChamCong == entity.TenBangChamCong))
                //    return Helper.CreateBadRequestResponse<ChamCongViewModel>(string.Format("Tên bảng chấm công đã tồn tại!", entity.TenBangChamCong));

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<ChamCongViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<ChamCongViewModel>(ex);
            }
        }

        public async Task<Response> ChangeActiveStatusAsync(Guid id, bool status, Guid byUserId)
        {
            try
            {
                var currentBangChamCong = _dbContext.mk_ChamCong.Where(x => x.Id == id).FirstOrDefault();
                if (currentBangChamCong == null)
                    return Helper.CreateNotFoundResponse("Không tìm thấy bảng chấm công");

                currentBangChamCong.KichHoatBangChamCong = status;

                await _dbContext.SaveChangesAsync();

                if (status)
                    return Helper.CreateSuccessResponse("Kích hoạt bảng chấm công thành công!");
                else
                    return Helper.CreateSuccessResponse("Hủy kích hoạt bảng chấm công thành công.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Status: {@status}, ByUserIds: {@requestUserId}", id, status,
                    byUserId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response<List<ChamCongItemViewModel>>> Import(string path)
        {
            try
            {
                var nhanSu = _dbContext.IdmUser;

                var filePath = _staticsFolder + "/" + path;
                var listTest = new List<ChamCongItemViewModel>();

                if (string.IsNullOrEmpty(path) || !File.Exists(filePath))
                    return Helper.CreateBadRequestResponse<List<ChamCongItemViewModel>>("Invalid path or files does not exist.");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(filePath))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Where(x => x.Name == "Sheet1").FirstOrDefault();
                    var lastRow = worksheet.Dimension.Rows;
                    var lastCol = worksheet.Dimension.Columns;
                    var month = int.Parse(worksheet.Cells[3, 18].Value.ToString());
                    var year = int.Parse(worksheet.Cells[3, 20].Value.ToString());

                    for (int rowIndex = 6; rowIndex <= lastRow; rowIndex++)
                    {
                        var cell = worksheet.Cells[rowIndex, 1];
                        if (cell == null || cell.Value == null)
                            break;
                        var item = new ChamCongItemViewModel
                        {
                            MaSo = ExcelHelper.GetCellValue(worksheet, rowIndex, "Mã số", 5, 1, null),
                            HoVaTen = ExcelHelper.GetCellValue(worksheet, rowIndex, "Họ và tên", 5, 1, null),
                            ChucVu = ExcelHelper.GetCellValue(worksheet, rowIndex, "Chức vụ", 5, 1, null),
                            Date = new DateTime(year, month, 1),
                            //NgayCong = int.Parse(ExcelHelper.GetCellValue(worksheet, rowIndex, "Ngày công", 5, 1, null)),
                            //LamThemNgayThuong = int.Parse(ExcelHelper.GetCellValue(worksheet, rowIndex, "Làm thêm ngày thường", 5, 1, null)),
                            //LamThemChuNhat = int.Parse(ExcelHelper.GetCellValue(worksheet, rowIndex, "Làm thêm chủ nhật", 5, 1, null)),
                            //LamThemNgayLe = int.Parse(ExcelHelper.GetCellValue(worksheet, rowIndex, "Làm thêm ngày lễ", 5, 1, null)),
                            NgayCong = 0,
                            LamThemNgayThuong = 0,
                            LamThemChuNhat = 0,
                            LamThemNgayLe = 0,
                            Stt = rowIndex - 5,
                            NgayTrongThang = new List<NgayTrongThangViewModel>(),
                        };

                        var daysInMonth = DateTime.DaysInMonth(year, month);
                        var daysOfWeekMapping = new Dictionary<DayOfWeek, string>
                        {
                            { DayOfWeek.Monday, "T2" },
                            { DayOfWeek.Tuesday, "T3" },
                            { DayOfWeek.Wednesday, "T4" },
                            { DayOfWeek.Thursday, "T5" },
                            { DayOfWeek.Friday, "T6" },
                            { DayOfWeek.Saturday, "T7" },
                            { DayOfWeek.Sunday, "CN" }
                        };
                        for (var i = 1; i <= daysInMonth; i++)
                        {
                            var date = new DateTime(year, month, i);
                            var cong = ExcelHelper.GetCellValue(worksheet, rowIndex, i.ToString(), 5, 1, null);
                            item.NgayTrongThang.Add(new NgayTrongThangViewModel
                            {
                                Ngay = i,
                                ThuTrongTuan = daysOfWeekMapping[new DateTime(year, month, i).DayOfWeek],
                                LoaiNgay = cong == null ? "dayoff" : cong == "LE" ? "holiday" : "working",
                                Cong = cong != null && cong == "LE" ? "LE" : cong!= null && cong != "LE" ? "1" : null,
                            });
                        }

                        if (listTest.Any(x => x.MaSo == item.MaSo && x.HoVaTen == item.HoVaTen))
                            return new Response<List<ChamCongItemViewModel>>(System.Net.HttpStatusCode.BadRequest, null, string.Format("Nhân sự {0} nằm trong danh sách chấm công {1} đã bị lặp lại", item.MaSo, item.HoVaTen));

                        listTest.Add(item);
                    }
                }

                return new Response<List<ChamCongItemViewModel>>(System.Net.HttpStatusCode.OK, listTest, string.Format("Thêm vào {0} danh sách chấm công thành công", listTest.Count()));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Path: {@path}", path);
                return Helper.CreateExceptionResponse<List<ChamCongItemViewModel>>(ex);
            }
        }
    }
}
