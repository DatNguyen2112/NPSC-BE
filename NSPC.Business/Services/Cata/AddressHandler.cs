using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSPC.Common;
using NSPC.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSPC.Common;
using Code = System.Net.HttpStatusCode;
namespace NSPC.Business
{
    public class AddressHandler : IAddressHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _baseUrl;
        public AddressHandler(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _baseUrl = _configuration.GetValue<string>("NewVnpost:Url:BaseUrl");
        }
        public async Task<Response> GetPhuong(AddressQueryModel filter)
        {
            try
            {
                var result = new List<CataCommueModel>();
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var repo = unitOfWork.GetRepository<cata_Commune>();
                    var query = from s in repo.GetAll()
                                select s;

                    if (filter.ParentId != null)
                    {
                        query = from s in query
                                where s.DistrictCode == filter.ParentId
                                select s;
                    }
                    else
                    {
                        return new ResponseError(Code.BadRequest, "Thiếu điều kiện lọc");
                    }

                    query = query.OrderBy(s => s.DistrictCode).ThenBy(x => x.CommuneName);
                    if (!string.IsNullOrWhiteSpace(filter.FullTextSearch))
                        result = await _mapper.ProjectTo<CataCommueModel>(query).Where(x => x.WardName_VP.ToLower().Contains(filter.FullTextSearch.ToLower())).ToListAsync();
                    else
                        result = await _mapper.ProjectTo<CataCommueModel>(query).ToListAsync();

                    var provinceName = "";
                    if (result != null && result.Count > 0)
                    {
                        var province = (from s in unitOfWork.GetRepository<cata_Province>().GetAll()
                                        where s.ProvinceCode == filter.ParentId
                                        select s.ProvinceName).FirstOrDefault();
                        provinceName = province ?? province;
                    }

                    var districtName = "";
                    if (result != null && result.Count > 0)
                    {
                        var province = (from s in unitOfWork.GetRepository<cata_District>().GetAll()
                                        where s.DistrictCode == filter.ParentId
                                        select s.DistrictName).FirstOrDefault();
                        districtName = province ?? province;
                    }

                    foreach (var item in result)
                    {
                        item.CommuneFullName = item.CommuneName + ", " + districtName + ", " + provinceName;
                    }

                    // result = AutoMapperUtils.AutoMap<List<cata_DanhMucPhuong>, List<DanhMucPhuongModel>>(qr);
                }
                return new Response<List<CataCommueModel>>(result);
            }
            catch (Exception ex)
            {
                Log.Error("", ex);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }

        public async Task<Response> GetHuyen(AddressQueryModel filter)
        {
            try
            {
                var result = new List<CataDistrictModel>();
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var repo = unitOfWork.GetRepository<cata_District>();
                    var query = from s in repo.GetAll()
                                select s;

                    if (filter.ParentId != null)
                    {
                        query = from s in query
                                where s.ProvinceCode == filter.ParentId
                                select s;
                    }
                    else
                    {
                        return new ResponseError(Code.BadRequest, "Thiếu điều kiện lọc");
                    }
                    

                    query = query.OrderBy(s => s.DistrictName);
                    if (!string.IsNullOrWhiteSpace(filter.FullTextSearch))
                        result = await _mapper.ProjectTo<CataDistrictModel>(query).Where(x=>x.DistrictName_VP.ToLower().Contains(filter.FullTextSearch.ToLower())).ToListAsync();
                    else 
                        result = await _mapper.ProjectTo<CataDistrictModel>(query).ToListAsync();

                    var provinceName = "";
                    if (result != null && result.Count > 0)
                    {
                        var province = (from s in unitOfWork.GetRepository<cata_Province>().GetAll()
                                        where s.ProvinceCode == filter.ParentId
                                        select s.ProvinceName).FirstOrDefault();
                        provinceName = province ?? province;
                    }
                    

                    foreach (var item in result)
                    {
                        item.DistrictFullName = provinceName;
                    }

                    // result = AutoMapperUtils.AutoMap<List<cata_DanhMucHuyen>, List<DanhMucHuyenModel>>(qr);
                }
                return new Response<List<CataDistrictModel>>(result);
            }
            catch (Exception ex)
            {
                Log.Error("", ex);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }

        public async Task<Response> GetTinh(AddressQueryModel filter)
        {
            try
            {
                var result = new List<CataProvinceModel>();
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var repo = unitOfWork.GetRepository<cata_Province>();
                    var query = from s in repo.GetAll()
                                select s;
                    

                    query = query.OrderByDescending(x => x.ProvinceCode.Equals(10) || x.ProvinceCode.Equals(70)).ThenBy(x => x.ProvinceName);
                    if (!string.IsNullOrWhiteSpace(filter.FullTextSearch))
                        result = await _mapper.ProjectTo<CataProvinceModel>(query).Where(x => x.ProvinceName.ToLower().Contains(filter.FullTextSearch.ToLower())).ToListAsync();
                    else 
                        result = await _mapper.ProjectTo<CataProvinceModel>(query).ToListAsync();
                }
                return new Response<List<CataProvinceModel>>(result);
            }
            catch (Exception ex)
            {
                Log.Error("", ex);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }

        #region UpdatePaid Province
        private Response validateProviceUpdateModel(float provinceId, ProvinceUpdateModel model)
        {
            try
            {
                //await Task.Delay(1);
                if ((model.DelayShipToDate.HasValue && model.DelayShipFromDate.HasValue) || (model.StopShipFromDate.HasValue && model.StopShipToDate.HasValue))
                {
                    // Ensure fromdate is less than todate
                    if ((model.DelayShipFromDate.HasValue && model.DelayShipFromDate <= model.DelayShipToDate) ||
                            (model.StopShipFromDate.HasValue && model.StopShipFromDate <= model.StopShipToDate))
                        return new Response(Code.OK, "Validated");
                }
                else if (!model.DelayShipToDate.HasValue && !model.DelayShipFromDate.HasValue && !model.StopShipFromDate.HasValue && !model.StopShipToDate.HasValue)
                {
                    return new Response(Code.OK, "Validated");
                }

                return new Response(Code.BadRequest, "Thông tin ngày tháng không hợp lệ.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private cata_Province updateProvince(cata_Province province, ProvinceUpdateModel model)
        {
           // province.ShippingStatus = ShippingStatus.Normal;
            province.DelayShipFromDate = model.DelayShipFromDate;
            province.StopShipFromDate = model.StopShipFromDate;
            province.DelayShipToDate = model.DelayShipToDate == null ? model.DelayShipToDate : model.DelayShipToDate.Value.EndOfDay();
            province.StopShipToDate = model.StopShipToDate == null ? model.StopShipToDate : model.StopShipToDate.Value.EndOfDay();

            // Change status of province
        /*    if (province.DelayShipFromDate.HasValue && province.DelayShipFromDate.Value <= DateTime.Now && province.DelayShipToDate.Value >= DateTime.Now)
                province.ShippingStatus = ShippingStatus.Delay;

            if (province.StopShipFromDate.HasValue && province.StopShipFromDate.Value <= DateTime.Now && province.StopShipToDate.Value >= DateTime.Now)
                province.ShippingStatus = ShippingStatus.Stop;*/

            return province;
        }

        public async Task<Response> UpdateProvince(float provinceId, ProvinceUpdateModel model)
        {
            try
            {
                var validateResult = validateProviceUpdateModel(provinceId, model);
                if (validateResult.Code != Code.OK)
                    return validateResult;

                var provinceRepo = _unitOfWork.GetRepository<cata_Province>();
                var province = await provinceRepo.GetAll().Where(x => x.ProvinceCode.Equals(provinceId)).FirstOrDefaultAsync();
                if (province == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy tỉnh hiện tại");

                province = updateProvince(province, model);
                await _unitOfWork.SaveAsync();
                return new Response(Code.OK, "Cập nhật thông tin tỉnh thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }

        public async Task<Response> UpdateProvinceMany(List<ProvinceUpdateModel> model)
        {
            try
            {
                var provinceRepo = _unitOfWork.GetRepository<cata_Province>();
                var districtRepo = _unitOfWork.GetRepository<cata_District>();

                var listProvinceId = model.Select(x => x.ProvinceId).ToList();

                var provinceEntities = await provinceRepo.GetAll().Where(x => listProvinceId.Contains(x.ProvinceCode)).ToListAsync();
                if (provinceEntities.Count == 0)
                    return new ResponseError(Code.NotFound, "Không tìm thấy tỉnh");

                for (var i = 0; i < provinceEntities.Count; i++)
                {
                    var updateModel = model.First(x => x.ProvinceId.Equals(provinceEntities[i].ProvinceCode));

                    // Validate
                    var validateResult = validateProviceUpdateModel(provinceEntities[i].ProvinceCode, updateModel);
                    if (validateResult.Code != Code.OK)
                        return validateResult;

                    // UpdatePaid
                    provinceEntities[i] = updateProvince(provinceEntities[i], updateModel);
                }    
                await _unitOfWork.SaveAsync();
                return new Response(Code.OK, "Cập nhật thông tin tỉnh thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.OK, ex.Message);
            }
        }
        #endregion

        #region UpdatePaid District

        private Response validateDistrictUpdateModel(DistrictUpdateModel model)
        {
            try
            {
                //await Task.Delay(1);
                if ((model.DelayShipToDate.HasValue && model.DelayShipFromDate.HasValue) || (model.StopShipFromDate.HasValue && model.StopShipToDate.HasValue))
                {
                    if ((model.DelayShipFromDate.HasValue && model.DelayShipFromDate <= model.DelayShipToDate) ||
                            (model.StopShipFromDate.HasValue && model.StopShipFromDate <= model.StopShipToDate))
                        return new Response(Code.OK, "Validated");
                }
                else if (!model.DelayShipToDate.HasValue && !model.DelayShipFromDate.HasValue && !model.StopShipFromDate.HasValue && !model.StopShipToDate.HasValue)
                {
                    return new Response(Code.OK, "Validated");
                }

                return new Response(Code.BadRequest, "Thông tin ngày tháng không hợp lệ.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private cata_District _updateDistrictData(cata_District district, DistrictUpdateModel model)
        {
           // district.ShippingStatus = ShippingStatus.Normal;
            district.DelayShipFromDate = model.DelayShipFromDate;
            district.StopShipFromDate = model.StopShipFromDate;
            district.DelayShipToDate = model.DelayShipToDate == null ? model.DelayShipToDate : model.DelayShipToDate.Value.EndOfDay();
            district.StopShipToDate = model.StopShipToDate == null ? model.StopShipToDate : model.StopShipToDate.Value.EndOfDay();

            // Change status of district rightaway
          /*  if (district.DelayShipFromDate.HasValue && district.DelayShipFromDate.Value <= DateTime.Now && district.DelayShipToDate.Value >= DateTime.Now)
                district.ShippingStatus = ShippingStatus.Delay;
            if (district.StopShipFromDate.HasValue && district.StopShipFromDate.Value <= DateTime.Now && district.StopShipToDate.Value >= DateTime.Now)
                district.ShippingStatus = ShippingStatus.Stop;*/

            return district;
        }


        public async Task<Response> UpdateDistrict(float districtId, DistrictUpdateModel model)
        {
            try
            {
                var provinceRepo = _unitOfWork.GetRepository<cata_Province>();
                var districtRepo = _unitOfWork.GetRepository<cata_District>();
                var district = await districtRepo.GetAll().Where(x => x.DistrictCode == districtId).FirstOrDefaultAsync();
                if (district == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy huyện hiện tại");

                // Validate
                var validateResult = validateDistrictUpdateModel(model);
                if (validateResult.Code != Code.OK)
                    return validateResult;

                // UpdatePaid
                district = _updateDistrictData(district, model);
                await _unitOfWork.SaveAsync();

                await _updateProviceShippingStatistic(district.ProvinceCode);

                return new Response(Code.OK, "Cập nhật thông tin huyện thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }

        public async Task<Response> UpdateDistrictMany(List<DistrictUpdateModel> models)
        {
            try
            {
                var provinceRepo = _unitOfWork.GetRepository<cata_Province>();
                var districtRepo = _unitOfWork.GetRepository<cata_District>();
                var listDistrictId = models.Select(x => x.DistrictId).ToList();
                var districtEntities = await districtRepo.GetAll().Where(x => listDistrictId.Contains(x.DistrictCode)).ToListAsync();
                if (districtEntities == null)
                    return new ResponseError(Code.NotFound, "Không tìm thấy huyện cần cập nhật.");

                // Validate
                foreach (var model in models)
                {
                    // Validate
                    var validateResult = validateDistrictUpdateModel(model);
                    if (validateResult.Code != Code.OK)
                        return validateResult;
                }

                foreach (var district in districtEntities)
                {
                    var updateModel = models.First(x => x.DistrictId.Equals(district.DistrictCode));
                    _updateDistrictData(district, updateModel);
                }
                await _unitOfWork.SaveAsync();

                var provinceIds = districtEntities.Select(x => x.ProvinceCode).Distinct().ToList();
                foreach (var provinceId in provinceIds)
                {
                    await _updateProviceShippingStatistic(provinceId);
                }
                return new Response(Code.OK, "Cập nhật thông tin huyện thành công");

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return new ResponseError(Code.InternalServerError, ex.Message);
            }
        }

        private async Task _updateProviceShippingStatistic(float provinceId)
        {
            var province = await _unitOfWork.GetRepository<cata_Province>().GetAll().Where(x => x.ProvinceCode.Equals(provinceId)).FirstOrDefaultAsync();
            var districtRepo = _unitOfWork.GetRepository<cata_District>();
            /*var totalDelayedDistrict = await districtRepo.GetAll().CountAsync(x => x.ProvinceCode.Equals(province.MaTinh) && x.ShippingStatus.Equals(ShippingStatus.Delay));
            var totalStoppedDistrict = await districtRepo.GetAll().CountAsync(x => x.ProvinceCode.Equals(province.MaTinh) && x.ShippingStatus.Equals(ShippingStatus.Stop));
            province.TotalDistrictShipDelay = totalDelayedDistrict;
            province.TotalDistrictShipStop = totalStoppedDistrict;*/
            await _unitOfWork.SaveAsync();
        }


        #endregion


        public async Task<string> GetNotifyMessage()
        {
            var html = await ParameterCollection.Instance.GetValue(ParamConstants.MAINTENANCE_TOP_ANNOUNCEMENT);

            var tinhRepo = _unitOfWork.GetRepository<cata_District>();
            var huyenRepo = _unitOfWork.GetRepository<cata_District>();


           /* var listDelayProvince = await tinhRepo.GetAll().Where(x => x.ShippingStatus.Equals(ShippingStatus.Delay)).ToListAsync();

            var listDelayDistrict = await huyenRepo.GetAll().Where(x => x.ShippingStatus.Equals(ShippingStatus.Delay)).ToListAsync();

            var extendHtml = GenerateHtml(listDelayProvince, listDelayDistrict);

            html = html.Replace("DANH_SACH_SHIP_CHAM", extendHtml);

            var listStoppedProvince = await tinhRepo.GetAll().Where(x => x.ShippingStatus.Equals(ShippingStatus.Stop)).ToListAsync();

            var listStoppedDistrict = await huyenRepo.GetAll().Where(x => x.ShippingStatus.Equals(ShippingStatus.Stop)).ToListAsync();

            extendHtml = GenerateHtml(listStoppedProvince, listStoppedDistrict);

            html = html.Replace("DANH_SACH_KHONG_SHIP", extendHtml);*/

            return html;
        }

        private string GenerateHtml(List<cata_Province> listProvince, List<cata_District> listDistrict)
        {
            if (listProvince.Count > 0)
            {
                var extendHtml = "<ul>";

                foreach (var pro in listProvince)
                {
                    var listChildDistrict = listDistrict?.Where(x => x.ProvinceCode.Equals(pro.ProvinceCode)).ToList();

                    extendHtml += "<li>" + pro.ProvinceName + "</li>";

                    if (listChildDistrict != null && listChildDistrict.Count > 0)
                    {
                        extendHtml += "<ul>";

                        foreach (var dis in listChildDistrict)
                        {
                            extendHtml += "<li>" + dis.DistrictName + "</li>";
                        }
                        extendHtml += "</ul>";
                    }
                }

                var listMaTinh = listProvince.Select(x => x.ProvinceCode).ToList();
                var listRemainingDistrict = listDistrict?.Where(x => !listMaTinh.Contains(x.ProvinceCode)).ToList();

                extendHtml += "<ul>";

                foreach (var item in listRemainingDistrict)
                {
                    extendHtml += "<li>" + item.DistrictName + "</li>";
                }

                extendHtml += "</ul>";

                extendHtml += "</ul>";

                return extendHtml;


            }
            return null;
        }

        public class ProvinceResponse
        {
            public string MaTinhThanh { get; set; }
            public string TenTinhThanh { get; set; }
        }

        public class ProvinceCompareResponse
        {
            public string ProvinceName { get; set; }
            public string ProvinceCode { get; set; }
            public string Status { get; set; }
        }

        public class DistrictResponse
        {
            public string MaQuanHuyen { get; set; }
            public string TenQuanHuyen { get; set; }
            public string MaTinhThanh { get; set; }
        }

        public class DistrictCompareResponse
        {
            public string DistrictName { get; set; }
            public string DistrictCode { get; set; }
            public string ProvinceName { get; set; }
            public string ProvinceCode { get; set; }
            public string Status { get; set; }
        }

        public class CommuneResponse
        {
            public string TenPhuongXa { get; set; }
            public string MaPhuongXa { get; set; }
            public string MaQuanHuyen { get; set; }
        }

        public class CommuneCompareResponse
        {
            public string DistrictName { get; set; }
            public string DistrictCode { get; set; }
            public string CommuneName { get; set; }
            public string CommuneCode { get; set; }
            public string Status { get; set; }
        }
    }
}