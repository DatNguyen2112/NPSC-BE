using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSPC.Business;
using NSPC.Common;

namespace NSPC.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Module Quản lý người dùng
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/idm/users")]
    [ApiExplorerSettings(GroupName = "Admin User")]
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _userHandler;
        public UserController(IUserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        #region CRUD

        /// <summary>
        /// Admin thêm mới
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        /// <returns>Kết quả trả về</returns>
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpPost, Route("")]
        [ProducesResponseType(typeof(Response<UserModel>), StatusCodes.Status200OK)]
        [RightValidate("USER", RightActionConstants.ADD)]
        // [Allow(RoleConstants.AdminRoleCode)]
        public async Task<IActionResult> CreateAsync([FromBody] UserCreateModel model)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _userHandler.CreateAsync(model, requestInfo.TenantId);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Set user password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Authorize, HttpPut, Route("{id}/password")]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        [RightValidate("USER", RightActionConstants.UPDATE)]
        //[Allow(RoleConstants.AdminRoleCode)]
        public async Task<IActionResult> ChangePasswordAsyncV1(Guid id, [FromBody] UserPasswordModel model)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            // Call service
            var result = await _userHandler.ForceChangePassword(id, model.Password);

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
        [RightValidate("USER", RightActionConstants.UPDATE)]
        //[Allow(RoleConstants.AdminRoleCode, RoleConstants.CSKHRoleCode, RoleConstants.KTRoleCode)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UserUpdateModel model)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _userHandler.UpdateAsync(id, model);

            // Hander response
            return Helper.TransformData(result);
        }


        /// <summary>
        ///  Xem thông tin người dùng hiện tại
        /// </summary>
        /// <returns></returns>
        [Authorize, HttpGet, Route("info")]
        [ProducesResponseType(typeof(Response<UserModel>), StatusCodes.Status200OK)]
        [RightValidate("USER", RightActionConstants.VIEW)]
        public async Task<IActionResult> GetInfoAsync()
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _userHandler.GetDetail(requestInfo.UserId, requestInfo.Language);

            // If no user found => unAuthorized
            if (result.Code == System.Net.HttpStatusCode.NotFound)
                result.Code = System.Net.HttpStatusCode.Unauthorized;

            // If user is found but is locked or not active
            if (result.Code == System.Net.HttpStatusCode.OK)
            {
                if (result.Data.IsLockedOut)
                    return Helper.TransformData(new Response(System.Net.HttpStatusCode.Unauthorized, "User is locked."));

                if (result.Data.IsLockedOut)
                    return Helper.TransformData(new Response(System.Net.HttpStatusCode.Unauthorized, "User is not active yet."));
            }

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        ///  Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize, HttpPut, Route("info")]
        [RightValidate("USER", RightActionConstants.UPDATE)]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateInfoAsync([FromBody] UserInfoUpdateModel model)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _userHandler.UpdateInfoAsync(requestInfo.UserId, model);

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
        [Allow(RoleConstants.AdminRoleCode)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            // Call service
            var result = await _userHandler.DeleteAsync(id);
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
        [ProducesResponseType(typeof(Response<UserModel>), StatusCodes.Status200OK)]
        [RightValidate("USER", RightActionConstants.VIEW)]
        //[Allow(RoleConstants.AdminRoleCode, RoleConstants.FarmerSideRoleCode)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            // Call service
            var result = await _userHandler.GetDetail(id, LanguageConstants.Vietnamese);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách khách hàng
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
        [RightValidateGroup("USER.VIEW", 
            "CONSTRUCTION.VIEW", "CONSTRUCTION.VIEWALL", "CONSTRUCTION.VIEWBYINDIVIDUAL", "CONSTRUCTION.VIEWBYTEAM", "CONSTRUCTION.VIEWBYDEPARTMENT", "CONSTRUCTIONNEWS.VIEW")]
        [ProducesResponseType(typeof(ResponsePagination<UserModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilterAsync([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string filter = "{}", [FromQuery] string sort = "")
        {
            var requestInfo = Helper.GetRequestInfo(Request);

            // Only allow admin or partner
            //if (requestInfo.Level < 5)
            //    return Helper.TransformData(Helper.CreateForbiddenResponse());

            // Call service
            sort = string.IsNullOrWhiteSpace(sort) ? "-CreatedOnDate" : sort + ",-CreatedOnDate";
            var filterObject = JsonConvert.DeserializeObject<UserQueryModel>(filter);
            filterObject.Sort = sort != null ? sort : filterObject.Sort;
            if (requestInfo.ApplicationId != AppConstants.HO_APP)
            {
                filterObject.ApplicationId = filterObject.ApplicationId ?? requestInfo.ApplicationId;
            }
            filterObject.Size = size;
            filterObject.Page = page;

            // Filfer for partner
            //if (requestInfo.Level == RoleConstants.PartnerLevel)
            //{
            //    filterObject.PartnerId = requestInfo.UserId;
            //}    

            var result = await _userHandler.AdminGetPageAsync(filterObject);

            // Hander response
            return Helper.TransformData(result);
        }


        #endregion CRUD

        /// <summary>
        /// Kiểm tra tên
        /// </summary>
        /// <param name="name"></param>

        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("checkname/{name}")]
        [Allow(RoleConstants.AdminRoleCode, RoleConstants.AdminRoleCode)]
        //[ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        public async Task<Response<bool>> CheckNameAvailability(string name)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);


            // Call service
            var result = await _userHandler.CheckNameAvailability(name);

            // Hander response
            return result;
        }

        /// <summary>
        /// Lấy về chi tiết
        /// </summary>
        /// <param name="id">Id bản ghi</param>

        /// <returns></returns>
        /// <response code="200">Thành công</response>
        [HttpGet, Route("{id}/detail")]
        [Authorize]
        [RightValidate("USER", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(Response<UserModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            // Call service
            var result = await _userHandler.GetDetail(id, requestInfo.Language);
            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Khóa tài khoản người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize, HttpPut, Route("{id}/lock")]
        [RightValidate("USER", RightActionConstants.UPDATE)]
        // [Allow(RoleConstants.AdminRoleCode)]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> LockUserAsync(Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var result = await _userHandler.ChangeLockStatusAsync(id, true, requestInfo.UserId);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Mở khóa tài khoản người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize, HttpPut, Route("{id}/unlock")]
        [RightValidate("USER", RightActionConstants.UPDATE)]
        // [Allow(RoleConstants.AdminRoleCode)]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> UnlockUserAsync(Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _userHandler.ChangeLockStatusAsync(id, false, requestInfo.UserId);

            // Hander response
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Kích hoạt tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize, HttpPut, Route("{id}/active")]
        [RightValidate("USER", RightActionConstants.UPDATE)]
        // [Allow(RoleConstants.AdminRoleCode)]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> ActiveUserAsync(Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _userHandler.ChangeActiveStatusAsync(id, true, requestInfo.UserId);

            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Kích hoạt tài khoản
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize, HttpPut, Route("{id}/deactive")]
        [RightValidate("USER", RightActionConstants.UPDATE)]
        // [Allow(RoleConstants.AdminRoleCode)]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeactiveUserAsync(Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _userHandler.ChangeActiveStatusAsync(id, false, requestInfo.UserId);

            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Thu hồi token
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize, HttpPut, Route("{id}/revoke-token")]
        // [Allow(RoleConstants.AdminRoleCode)]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeToken(Guid id)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);

            var result = await _userHandler.RevokeToken(id);

            // Hander response
            return Helper.TransformData(result);
        }
        /// <summary>
        /// Upload avatar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize, HttpPut, Route("{id}/avatar")]
        //[Allow(RoleConstants.AdminRoleCode)]
        [ProducesResponseType(typeof(ResponseUpdate), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAvatar(Guid id, string avatarUrl)
        {
            // Get Token Info
            var requestInfo = Helper.GetRequestInfo(Request);
            var uploadAvatar = new UserSetAvatarUrlModel();
            uploadAvatar.UserId = id;
            uploadAvatar.AvatarUrl = avatarUrl;
            var result = await _userHandler.UpdateAvatarUrl(uploadAvatar);

            // Hander response
            return Helper.TransformData(result);
        }
    }
}