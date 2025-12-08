using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.KiemKho;
using NSPC.Data.Data.Entity.XuatNhapTon;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public class ThongKeHandler : IThongKeHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;

        public ThongKeHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
        }

        public async Task<Response<Pagination<ThongKeTonKhoViewModel>>> GetPage(ThongKeTonKhoQueryModel query)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var predicate = BuildQuery(query);

                var iquery = _dbContext.mk_XuatNhapTon.Include(x=>x.Sm_Product).Where(predicate);

                var queryResult = iquery.GroupBy(x => new { x.MaVatTu, x.TenVatTu, x.IdVatTu, x.MaKho }).Select(x => new ThongKeTonKhoViewModel()
                {
                    IdVatTu = x.Key.IdVatTu,
                    MaVatTu = x.Key.MaVatTu,
                    TenVatTu = x.Key.TenVatTu,
                    MaKho = x.Key.MaKho,     
                    DonViTinh = x.FirstOrDefault().DonViTinh,
                    SoLuongNhap = x.Where(x => x.LoaiXuatNhapTon == LoaiXuatNhapTonConstants.NHAP_KHO).Sum(x => x.SoLuong),
                    GiaTriNhap = x.Where(x => x.LoaiXuatNhapTon == LoaiXuatNhapTonConstants.NHAP_KHO).Sum(x => x.DonGia * x.SoLuong),
                    SoLuongXuat = x.Where(x => x.LoaiXuatNhapTon == LoaiXuatNhapTonConstants.XUAT_KHO).Sum(x => x.SoLuong),
                    GiaTriXuat = x.Where(x => x.LoaiXuatNhapTon == LoaiXuatNhapTonConstants.XUAT_KHO).Select(x => x.DonGia * x.SoLuong).Sum(x => x),
                    TenantId = currentUser.TenantId,
                });

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<ThongKeTonKhoViewModel>>(data);

                return Helper.CreateSuccessResponse(result);

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@query}", query);
                return Helper.CreateExceptionResponse<Pagination<ThongKeTonKhoViewModel>>(ex);
            }
        }

        private Expression<Func<mk_XuatNhapTon, bool>> BuildQuery(ThongKeTonKhoQueryModel query)
        {
            var predicate = PredicateBuilder.New<mk_XuatNhapTon>(true);
            predicate.And(x => x.LoaiXuatNhapTon == LoaiXuatNhapTonConstants.NHAP_KHO || x.LoaiXuatNhapTon == LoaiXuatNhapTonConstants.XUAT_KHO);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.TenVatTu.ToLower().Contains(query.FullTextSearch.ToLower()) || s.MaVatTu.ToLower().Contains(query.FullTextSearch.ToLower()) || s.MaKho.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (!string.IsNullOrEmpty(query.MaKho))
                predicate.And(s => s.MaKho == query.MaKho);

            if (query.DateRange != null && query.DateRange.Count() == 2)
            {
                predicate.And(s => s.CreatedOnDate.Date >= query.DateRange[0].Date && s.CreatedOnDate.Date <= query.DateRange[1].Date);
            }

            if (!string.IsNullOrEmpty(query.Loai))
                predicate.And(s => s.Sm_Product.Type == query.Loai);

            return predicate;
        }
    }
}
