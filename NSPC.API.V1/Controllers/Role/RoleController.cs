
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSPC.Business;
using NSPC.Common;

namespace OpenData.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Module nhóm người dùng
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/idm/roles")]
    [ApiExplorerSettings(GroupName = "Admin Role")]
    [AuthorizeByToken, Allow(RoleConstants.AdminRoleCode)]
    public class RoleController : ControllerBase
    {
        private readonly IRoleHandler _roleHandler;

        public RoleController(IRoleHandler roleHandler)
        {
            _roleHandler = roleHandler;
        }

        #region CRUD

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <param name="applicationId"></param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpPost, Route("")]
        [ProducesResponseType(typeof(Response<RoleModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] RoleCreateModel model, [FromQuery] Guid? applicationId = null)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var actorId = requestInfo.UserId;
            var appId = applicationId ?? requestInfo.ApplicationId;
            if (!model.ApplicatonId.HasValue) model.ApplicatonId = appId;
            // Call service
            var result = await _roleHandler.CreateAsync(model, appId, actorId);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="model">Dữ liệu</param>
        /// <param name="applicationId"></param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] RoleUpdateModel model, [FromQuery] Guid? applicationId = null)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var actorId = requestInfo.UserId;
            var appId = applicationId ?? requestInfo.ApplicationId;
            if (!model.ApplicatonId.HasValue) model.ApplicatonId = appId;
            // Call service
            var result = await _roleHandler.UpdateAsync(id, model, appId, actorId);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(ResponseDelete), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            // Call service
            var result = await _roleHandler.DeleteAsync(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa danh sách
        /// </summary>
        /// <param name="listId">Danh sách id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpDelete, Route("")]
        [ProducesResponseType(typeof(ResponseDeleteMulti), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteRangeAsync([FromBody] List<Guid> listId)
        {
            // Call service
            var result = await _roleHandler.DeleteRangeAsync(listId);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về theo Id
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(Response<RoleModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            // Call service
            var result = await _roleHandler.FindAsync(id);
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
        [HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponsePagination<RoleModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilterAsync([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var requestInfo = Helper.GetRequestInfo(Request);
            // Call service
            var filterObject = JsonConvert.DeserializeObject<RoleQueryModel>(filter);
            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            if (requestInfo.ApplicationId != AppConstants.HO_APP)
            {
                filterObject.ApplicationId = filterObject.ApplicationId ?? requestInfo.ApplicationId;
            }
            filterObject.Size = size;
            filterObject.Page = page;
            var result = await _roleHandler.GetPageAsync2(filterObject);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy về tất cả
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [HttpGet, Route("all")]
        [ProducesResponseType(typeof(Response<List<RoleModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync([FromQuery] int size = 20, [FromQuery] int page = 1, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var requestInfo = Helper.GetRequestInfo(Request);
            // Call service
            var filterObject = JsonConvert.DeserializeObject<RoleQueryModel>(filter);

            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + "," + "-CreatedOnDate";
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            filterObject.Size = size;
            filterObject.Page = page;

            var result = await _roleHandler.GetAllAsync(filterObject);
            // Hander response
            return Helper.TransformData(result);
        }

        #endregion CRUD

        /// <summary>
        /// Lấy về chi tiết
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [HttpGet, Route("{id}/detail")]
        [RightValidate("ROLE", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(Response<List<RoleDetailModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDetail(Guid id, [FromQuery] Guid? applicationId = null)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            var appId = applicationId ?? requestInfo.ApplicationId;
            // Call service
            var result = await _roleHandler.GetDetail(id, appId);
            // Hander response
            return Helper.TransformData(result);
        }


        // /// <summary>
        // /// Lấy về danh sách quyền thuộc nhóm
        // /// </summary>
        // /// <param name="id">Id bản ghi</param>
        // /// <param name="applicationId"></param>
        // /// <returns></returns>
        // /// <response code="200">Thành công</response>
        // [HttpGet, Route("{id}/right")]
        // [ProducesResponseType(typeof(Response<List<BaseRightModel>>), StatusCodes.Status200OK)]
        // public async Task<IActionResult> GetRightMapRoleAsync(Guid id, [FromQuery] Guid? applicationId = null)
        // {
        //     // Get Token Info
        //     var requestInfo = Helper.GetRequestInfo(Request);
        //
        //     var appId = applicationId ?? requestInfo.ApplicationId;
        //     // Call service
        //     var result = await _rightMapRoleHandler.GetRightMapRoleAsync(id, appId);
        //     // Hander response
        //     return Helper.TransformData(result);
        // }

        // /// <summary>
        // /// Gán quyền vào nhóm
        // /// </summary>
        // /// <param name="id">Id bản ghi</param>
        // /// <param name="listRightId"></param>
        // /// <param name="applicationId"></param>
        // /// <returns></returns>
        // /// <response code="200">Thành công</response>
        // [HttpPost, Route("{id}/right")]
        // [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        // public async Task<IActionResult> AddRightMapRoleAsync([FromBody] List<Guid> listRightId, Guid id, [FromQuery] Guid? applicationId = null)
        // {
        //     // Get Token Info
        //     var requestInfo = Helper.GetRequestInfo(Request);
        //     var actorId = requestInfo.UserId;
        //     var appId = applicationId ?? requestInfo.ApplicationId;
        //     // Call service
        //     var result = await _rightMapRoleHandler.AddRightMapRoleAsync(id, listRightId, appId, appId, actorId);
        //     // Hander response
        //     return Helper.TransformData(result);
        // }


        // /// <summary>
        // /// Gỡ quyền khỏi nhóm
        // /// </summary>
        // /// <param name="id">Id bản ghi</param>
        // /// <param name="model"></param>
        // /// <param name="applicationId"></param>
        // /// <returns></returns>
        // /// <response code="200">Thành công</response>
        // [HttpDelete, Route("{id}/right")]
        // [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        // public async Task<IActionResult> DeleteRightMapRoleAsync([FromBody] RoleUserModel model, Guid id, [FromQuery] Guid? applicationId = null)
        // {
        //     // Get Token Info
        //     var requestInfo = Helper.GetRequestInfo(Request);
        //
        //     var appId = applicationId ?? requestInfo.ApplicationId;
        //     // Call service
        //     var result = await _rightMapRoleHandler.DeleteRightMapRoleAsync(id, model.Id, appId);
        //     // Hander response
        //     return Helper.TransformData(result);
        // }


    }
}