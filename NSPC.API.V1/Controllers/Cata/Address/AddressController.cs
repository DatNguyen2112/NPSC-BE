using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;
using System.Net;
using NSPC.Business;
using NSPC.Common;

namespace NSPC.API
{
    /// <inheritdoc />
    /// <summary>
    /// Module tham số hệ thống
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/bsd/address")]
    [ApiExplorerSettings(GroupName = "Admin Address")]
    [AllowAnonymous]
    public class AddressController : ControllerBase
    {
        private readonly IAddressHandler _addressHandler;

        public AddressController(IAddressHandler parameterHandler)
        {
            _addressHandler = parameterHandler;
        }

        #region CRUD

        /// <summary>
        /// Lấy về theo bộ loc
        /// </summary>
        /// <param name="page">Số thứ tự trang tìm kiếm</param>
        /// <param name="size">Số bản ghi giới hạn một trang</param>
        /// <param name="filter">Thông tin lọc nâng cao (Object Json)</param>
        /// <param name="sort">Thông tin sắp xếp (Array Json)</param>
        /// <returns></returns>
        /// <remarks>
        ///  *filter*
        ///  ....
        ///  *sort*
        ///  ....
        /// </remarks>
        /// <response code="200">Thành công</response>
        [HttpGet, Route("/api/v1/tinh")]
        [ProducesResponseType(typeof(ResponsePagination<CataProvinceModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilterTinhAsync([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            // Call service
            var filterObject = JsonConvert.DeserializeObject<AddressQueryModel>(filter);
            filterObject.Sort = sort != null ? sort : filterObject.Sort;

            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _addressHandler.GetTinh(filterObject);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về theo bộ loc
        /// </summary>
        /// <param name="page">Số thứ tự trang tìm kiếm</param>
        /// <param name="size">Số bản ghi giới hạn một trang</param>
        /// <param name="filter">Thông tin lọc nâng cao (Object Json)</param>
        /// <param name="sort">Thông tin sắp xếp (Array Json)</param>
        /// <returns></returns>
        /// <remarks>
        ///  *filter*
        ///  ....
        ///  *sort*
        ///  ....
        /// </remarks>
        /// <response code="200">Thành công</response>
        [HttpGet, Route("/api/v1/huyen")]
        [ProducesResponseType(typeof(ResponsePagination<CataDistrictModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilterHuyenAsync([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            // Call service
            var filterObject = JsonConvert.DeserializeObject<AddressQueryModel>(filter);
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _addressHandler.GetHuyen(filterObject);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về theo bộ loc
        /// </summary>
        /// <param name="page">Số thứ tự trang tìm kiếm</param>
        /// <param name="size">Số bản ghi giới hạn một trang</param>
        /// <param name="filter">Thông tin lọc nâng cao (Object Json)</param>
        /// <param name="sort">Thông tin sắp xếp (Array Json)</param>
        /// <returns></returns>
        /// <remarks>
        ///  *filter*
        ///  ....
        ///  *sort*
        ///  ....
        /// </remarks>
        /// <response code="200">Thành công</response>
        [HttpGet, Route("/api/v1/phuong")]
        [ProducesResponseType(typeof(ResponsePagination<CataCommueModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilterPhuongAsync([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            // Call service
            var filterObject = JsonConvert.DeserializeObject<AddressQueryModel>(filter);
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _addressHandler.GetPhuong(filterObject);
            // Hander response
            return Helper.TransformData(result);
        }

        [Authorize, Allow(RoleConstants.AdminRoleCode)]
        [HttpPut("/api/v1/tinh/{provinceId}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProvince(float provinceId, ProvinceUpdateModel model)
        {
            var requestInfo = Helper.GetRequestInfo(Request);

            // Validate request

            // Call service
            var result = await _addressHandler.UpdateProvince(provinceId, model);
            // Hander response
            return Helper.TransformData(result);
        }

        [Authorize, Allow(RoleConstants.AdminRoleCode)]
        [HttpPut("/api/v1/huyen/{districtId}")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDistrict(float districtId, DistrictUpdateModel model)
        {
            var requestInfo = Helper.GetRequestInfo(Request);

            // Call service
            var result = await _addressHandler.UpdateDistrict(districtId, model);
            // Hander response
            return Helper.TransformData(result);
        }

        [Authorize, Allow(RoleConstants.AdminRoleCode)]
        [HttpPut("/api/v1/tinh/many")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProvinceMany(float provinceId, List<ProvinceUpdateModel> model)
        {
            var requestInfo = Helper.GetRequestInfo(Request);

            // Call service
            var result = await _addressHandler.UpdateProvinceMany(model);
            // Hander response
            return Helper.TransformData(result);
        }

        [Authorize, Allow(RoleConstants.AdminRoleCode)]
        [HttpPut("/api/v1/huyen/many")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDistrictMany(List<DistrictUpdateModel> model)
        {
            var requestInfo = Helper.GetRequestInfo(Request);

            // Call service
            var result = await _addressHandler.UpdateDistrictMany(model);
            // Hander response
            return Helper.TransformData(result);
        }

        #endregion CRUD

/*
        [HttpGet("get-province")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProvince()
        {

            var result = await _addressHandler.GetProvince();
            // Hander response
            return Helper.TransformData(result);
        }*/

  /*      [HttpGet("get-district")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDistrict()
        {
            var result = await _addressHandler.GetDistrict();
            // Hander response
            return Helper.TransformData(result);
        }*/
/*        [HttpGet("get-commune")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommune()
        {
            var result = await _addressHandler.GetCommune();
            // Hander response
            return Helper.TransformData(result);
        }*/

        /// <summary>
        /// Notify
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("notify"), AllowAnonymous]
        public async Task<ActionResult<Response>> Notify()
        {
            // Get Token Info

            var html = await _addressHandler.GetNotifyMessage(); 

            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = html
            };
        }


        //[HttpGet]
        //[AllowAnonymous]
        //[Route("viettelpost")]
        //public async Task ViettelPost()
        //{
        //    var adminApiEndpointHandler = new ApiEndpointHandler<Ward_VP, Ward_VP>();


        //    using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
        //    {

        //        var districtRepo = unitOfWork.GetRepository<cata_DanhMucHuyen>();
        //        var communeRepo = unitOfWork.GetRepository<cata_DanhMucPhuong>();
        //        var listDistrict = districtRepo.GetAll().ToList();
        //        var fakeId = -1;
        //        foreach (var item in listDistrict)
        //        {
        //            var uri = "https://partner.viettelpost.vn/v2/categories/listWards?districtId=" + item.DistrictId_VP;
        //            var addressResponse = await adminApiEndpointHandler.GetList(uri);
        //            var data = addressResponse.Data;
        //            //Format data

        //            foreach (var ward in data)
        //            {
        //                ward.WARDS_NAME = ward.WARDS_NAME.Replace("  ", " ");
        //                ward.VTPAsciiName = ward.WARDS_NAME.ToLower().Replace("phường", "").Replace("xã", "").Replace("tt", "").Replace("thị trấn", "").Trim();
        //                ward.VTPAsciiName = Utils.RemoveVietnameseSign(ward.VTPAsciiName);
        //            }

        //            var listCommune = communeRepo.GetAll().Where(x => x.DistrictCode.Equals(item.DistrictCode) && x.WardId_VP == 0).ToList();
        //            foreach (var commune in listCommune)
        //            {
        //                var communeVP = data.Where(x => x.VTPAsciiName.Contains(Utils.RemoveVietnameseSign(commune.CommuneName.Replace("  ", " ").ToLower().Replace("phường", "").Replace("xã", "").Replace("tt", "").Replace("thị trấn", "").Replace("số", "").Trim()))).FirstOrDefault();
        //                if (communeVP == null)
        //                {

        //                }
        //                else
        //                {
        //                    commune.WardId_VP = communeVP.WARDS_ID;
        //                    commune.WardName_VP = communeVP.WARDS_NAME;
        //                    commune.DistrictId_VP = item.DistrictId_VP;
        //                    communeVP.IsUpdate = true;
        //                }
        //            }

        //            var listAdd = data.Where(x => !x.IsUpdate).ToList();
        //            foreach (var commune in listAdd)
        //            {
        //                var entity = new cata_DanhMucPhuong()
        //                {
        //                    CommuneCode = fakeId,
        //                    WardId_VP = commune.WARDS_ID,
        //                    WardName_VP = commune.WARDS_NAME,
        //                    DistrictId_VP = item.DistrictId_VP
        //                };
        //                fakeId--;
        //                communeRepo.Add(entity);
        //            }

        //        }
        //        var modifieds = (unitOfWork.DataContext.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).ToList()).Select(x => x.Entity).ToList();
        //        foreach (var item in modifieds)
        //        {
        //            System.IO.File.AppendAllText(@"D:\file.json", JsonConvert.SerializeObject(item) + "," + Environment.NewLine);
        //        }
        //        var result = unitOfWork.Save();
        //    }


        //}

        //[HttpGet]
        //[AllowAnonymous]
        //[Route("viettelpostupdate")]

        //public async Task ViettelPostUpdate()
        //{
        //    var adminApiEndpointHandler = new ApiEndpointHandler<Ward_VP, Ward_VP>();


        //    using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
        //    {
        //        var reg = new Regex("[*'\",_&#^@]");
        //        var districtRepo = unitOfWork.GetRepository<cata_DanhMucHuyen>();
        //        var communeRepo = unitOfWork.GetRepository<cata_DanhMucPhuong>();

        //        #region MAPPING DATA
        //        //var listCommune = communeRepo.GetAll().Where(x => x.WardId_VP == 0).ToList();
        //        //var listNotMappedVTP = communeRepo.GetAll().Where(x => x.CommuneCode < 0).ToList();

        //        //foreach (var item in listCommune)
        //        //{
        //        //    var formatedVNPName = item.CommuneName.Replace("  ", " ").ToLower().Replace("phường", string.Empty).Replace("xã", string.Empty).Replace("tt", string.Empty).Replace("thị trấn", string.Empty).Replace("số", string.Empty).Trim();
        //        //    formatedVNPName = reg.Replace(formatedVNPName, string.Empty);
        //        //    formatedVNPName = Utils.RemoveVietnameseSign(formatedVNPName);
        //        //    var communeVTP = listNotMappedVTP.Where(x => Utils.RemoveVietnameseSign(reg.Replace(x.WardName_VP.Replace("  ", " ").ToLower().Replace("phường", string.Empty).Replace("xã", string.Empty).Replace("tt", string.Empty).Replace("thị trấn", string.Empty).Replace("số", string.Empty).Trim(), string.Empty)).Contains(formatedVNPName)).FirstOrDefault();
        //        //    if (communeVTP != null)
        //        //    {
        //        //        item.WardId_VP = communeVTP.WardId_VP;
        //        //        item.WardName_VP = communeVTP.WardName_VP;
        //        //        item.DistrictId_VP = communeVTP.DistrictId_VP;
        //        //    }

        //        //}

        //        //var modifieds = (unitOfWork.DataContext.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).ToList()).Select(x => x.Entity).ToList();
        //        //foreach (var item in modifieds)
        //        //{
        //        //    System.IO.File.AppendAllText(@"D:\map.json", JsonConvert.SerializeObject(item) + "," + Environment.NewLine);
        //        //}
        //        #endregion
        //        #region DELETE MAPPED DATA
        //        var listNotMappedVTP = communeRepo.GetAll().Where(x => x.CommuneCode < 0).ToList();

        //        foreach (var item in listNotMappedVTP)
        //        {
        //            var updatedWard = communeRepo.GetAll().Where(x => x.WardId_VP.Equals(item.WardId_VP) && x.CommuneCode > 0).FirstOrDefault();
        //            if (updatedWard != null)
        //            {
        //                communeRepo.Delete(x => x.WardId_VP == item.WardId_VP && x.CommuneCode < 0);
        //            }
        //        }

        //        var modifieds = (unitOfWork.DataContext.ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted).ToList()).Select(x => x.Entity).ToList();
        //        foreach (var item in modifieds)
        //        {
        //            System.IO.File.AppendAllText(@"D:\delete.json", JsonConvert.SerializeObject(item) + "," + Environment.NewLine);
        //        }
        //        #endregion

        //        #region FETCH DATA WITH VTP DISTRICT ID
        //        //var uri = "https://partner.viettelpost.vn/v2/categories/listWards?districtId=" + 310;
        //        //var addressResponse = await adminApiEndpointHandler.GetList(uri);
        //        //var data = addressResponse.Data;
        //        //var fakeId = -3800;
        //        //foreach (var item in data)
        //        //{
        //        //    var entity = new cata_DanhMucPhuong()
        //        //    {
        //        //        CommuneCode = fakeId,
        //        //        WardId_VP = item.WARDS_ID,
        //        //        WardName_VP = item.WARDS_NAME,
        //        //        DistrictId_VP = item.DISTRICT_ID
        //        //    };
        //        //    fakeId--;
        //        //    communeRepo.Add(entity);
        //        //}

        //        //var modifieds = (unitOfWork.DataContext.ChangeTracker.Entries().Where(x => x.State == EntityState.Added).ToList()).Select(x => x.Entity).ToList();
        //        //foreach (var item in modifieds)
        //        //{
        //        //    System.IO.File.AppendAllText(@"D:\fetch.json", JsonConvert.SerializeObject(item) + "," + Environment.NewLine);
        //        //}
        //        #endregion


        //        var result = unitOfWork.Save();
        //    }

        //}

        //[HttpGet]
        //[AllowAnonymous]
        //[Route("viettelpostaddress")]
        //public async Task ViettelPostAddress()
        //{
        //    using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
        //    {
        //        var communeRepo = unitOfWork.GetRepository<cata_DanhMucPhuong>();
        //        var listCommune = communeRepo.GetAll().ToList();
        //        foreach (var item in listCommune)
        //        {
        //            item.VNPAsciiName = Utils.RemoveVietnameseSign(item.CommuneName.ToLower().Replace("phường", "").Replace("xã", "").Replace("tt", "").Replace("thị trấn", "").Trim());
        //            if (!string.IsNullOrEmpty(item.WardName_VP))
        //            {
        //                item.VTPAsciiName = Utils.RemoveVietnameseSign(item.WardName_VP.ToLower().Replace("phường ", "").Replace("xã", "").Replace("tt", "").Replace("thị trấn", "").Trim());
        //            }

        //        }
        //        var result = unitOfWork.Save();
        //    }


        //}
    }
}