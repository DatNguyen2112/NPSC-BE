using NSPC.Business;
using NSPC.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSPC.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace NSPC.Api.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Module tham số hệ thống
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/parameters")]
    [Allow(RoleConstants.AdminRoleCode)]
    [ApiExplorerSettings(GroupName = "Admin Parameter")]
    public class BsdParameterController : ControllerBase
    {
        private readonly IParameterHandler _parameterHandler;
        private readonly SMDbContext _dbContext;

        public BsdParameterController(IParameterHandler parameterHandler, SMDbContext dbContext)
        {
            _parameterHandler = parameterHandler;
            _dbContext = dbContext;
        }

        #region CRUD

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("")]
        [ProducesResponseType(typeof(Response<ParameterModel>), StatusCodes.Status200OK)]
        //        [SwaggerRequestExample(typeof(ParameterCreateModel), typeof(MockupObject<ParameterCreateModel>))]
        public async Task<IActionResult> CreateAsync([FromBody] ParameterCreateModel model)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var actorId = requestInfo.UserId;
            var appId = requestInfo.ApplicationId;
            // Call service
            var result = await _parameterHandler.CreateAsync(model, appId, actorId);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        //        [SwaggerRequestExample(typeof(ParameterUpdateModel), typeof(MockupObject<ParameterUpdateModel>))]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] ParameterUpdateModel model)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var actorId = requestInfo.UserId;
            var appId = requestInfo.ApplicationId;
            // Call service
            var result = await _parameterHandler.UpdateAsync(id, model, appId, actorId);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(ResponseDelete), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            // Call service
            var result = await _parameterHandler.DeleteAsync(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa danh sách
        /// </summary>
        /// <param name="listId">Danh sách id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpDelete, Route("")]
        [ProducesResponseType(typeof(ResponseDeleteMulti), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteRangeAsync([FromQuery]List<Guid> listId)
        {
            // Call service
            var result = await _parameterHandler.DeleteRangeAsync(listId);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về theo Id
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("{id:guid}")]
        [ProducesResponseType(typeof(Response<ParameterModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            // Call service
            var result = await _parameterHandler.FindAsync(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về theo tên
        /// </summary>
        /// <param name="name">Tên bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("{name}")]
        [ProducesResponseType(typeof(Response<ParameterModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(string name)
        {
            // Call service
            var result = await _parameterHandler.FindByNameAsync(name);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// thông tin iframe es
        /// </summary>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, Allow(RoleConstants.AdminRoleCode), HttpGet, Route("elastic-search")]
        [ProducesResponseType(typeof(Response<ParameterModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetElasticSearchParameter()
        {
            // Call service
            var result = await _parameterHandler.FindByNameAsync("DASH_BOARD_ELASTIC");
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
        [Authorize, HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<ParameterModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilterAsync([FromQuery]int page = 1, [FromQuery]int size = 20, [FromQuery]string filter = "{}", [FromQuery]string sort = "")
        {
            var requestInfo = Helper.GetRequestInfo(Request);
            // Call service
            var filterObject = JsonConvert.DeserializeObject<ParameterQueryModel>(filter);
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            if (requestInfo.ApplicationId != AppConstants.HO_APP)
            {
                filterObject.ApplicationId = filterObject.ApplicationId ?? requestInfo.ApplicationId;
            }
            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _parameterHandler.GetPageAsync(filterObject);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về tất cả
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("all")]
        [ProducesResponseType(typeof(Response<List<ParameterModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync([FromQuery]string filter = "{}")
        {
            var filterObject = JsonConvert.DeserializeObject<ParameterQueryModel>(filter);
            var result = await _parameterHandler.GetAllAsync(filterObject);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Sửa tất cả
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize, Route("many")]
        public async Task<Response> UpdateManyParameter(ParameterUpdateManyModel model)
        {
            var list = await _dbContext.BsdParameter.ToListAsync();
            foreach (var item in model.List)
            {
                var param = list.Where(x => x.Id == item.Id).FirstOrDefault();
                if (param != null)
                {
                    param.Value = item.Value;
                }
            }
            await _dbContext.SaveChangesAsync();
            ParameterCollection.Instance.LoadToHashSet();

            return new Response(System.Net.HttpStatusCode.OK, "Chỉnh sửa thành công");
        }

        #endregion CRUD

        //[NonAction]
        //[CapSubscribe(CapQueueConstants.FROM_ADMIN_UPDATE_PARAMETER)]
        //public void UpdateParams(ParameterModel model)
        //{
        //    Log.Information("Received parameter update for " + model.Name + ", new value is " + model.Value);
        //    _parameterHandler.SaveEntity(model);
        //}
    }
}