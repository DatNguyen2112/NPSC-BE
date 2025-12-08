using NSPC.Business;
using NSPC.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSPC.Api.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Module điều hướng
    /// </summary>

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/bsd/navigations")]
    [ApiExplorerSettings(GroupName = "Admin Navigation"), AuthorizeByToken]
    public class BsdNavigationController : ControllerBase
    {
        private readonly INavigationHandler _navigationHanlder;

        public BsdNavigationController(INavigationHandler navigationHanlder)
        {
            _navigationHanlder = navigationHanlder;
        }

        /// <summary>
        /// Lấy danh sách theo người dùng
        /// </summary>
        /// <param name="userId">Id người dùng</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("user/{userId}")]
        [ProducesResponseType(typeof(Response<List<NavigationModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUserIdAsync(Guid? userId = null)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            userId = userId.HasValue ? userId : requestInfo.UserId;
            // Call service
            var result = await _navigationHanlder.GetByUserIdAsync(userId.Value);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách theo người dùng web app
        /// </summary>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("user/webapp")]
        [ProducesResponseType(typeof(Response<List<NavigationModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUserAsync(Guid? userId = null)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            userId = requestInfo.UserId;
            // Call service
            var result = await _navigationHanlder.GetByUserIdAsync(userId.Value, NSPCConstants.AppChanel.WebAdmin);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách theo người dùng mobile
        /// </summary>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("user/mobile")]
        [ProducesResponseType(typeof(Response<List<NavigationModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUserMobileAsync()
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var userId = requestInfo.UserId;
            // Call service
            var result = await _navigationHanlder.GetByUserIdAsync(userId, NSPCConstants.AppChanel.Mobile);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách của bản thân
        /// </summary>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("owner")]
        [ProducesResponseType(typeof(Response<List<NavigationModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByOwnerAsync()
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var actorId = requestInfo.UserId;
            // Call service
            var result = await _navigationHanlder.GetByUserIdAsync(actorId);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách dạng tree
        /// </summary>
        /// <param name="isGetRoles">Lấy kèm nhóm quyền</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("tree")]
        [ProducesResponseType(typeof(Response<List<NavigationModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTreeAsync(bool isGetRoles)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            // Call service
            var result = await _navigationHanlder.GetTreeAsync(isGetRoles);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="request">Dữ liệu thêm mới</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("")]
        [ProducesResponseType(typeof(Response<NavigationModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] NavigationCreateModel request)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var actorId = requestInfo.UserId;
            // Call service
            var result = await _navigationHanlder.CreateAsync(request, actorId);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="request">Dữ liệu cập nhật</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] NavigationUpdateModel request)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var actorId = requestInfo.UserId;
            // Call service
            var result = await _navigationHanlder.UpdateAsync(id, request, actorId);
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
            var result = await _navigationHanlder.DeleteAsync(id);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Gán nhóm quyền
        /// </summary>
        /// <param name="id">Id bản ghi</param>
        /// <param name="listRoleId">Danh sách id nhóm quyền</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut, Route("{id}/map")]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateRoleInNavigations(Guid id, [FromBody] List<Guid> listRoleId)
        {
            // Call service
            var result = await _navigationHanlder.UpdateRoleInNavigations(id, listRoleId);
            // Hander response
            return Helper.TransformData(result);
        }
    }
}