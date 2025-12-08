using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSPC.Business;
using NSPC.Common;

namespace NSPC.API
{
    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/idm/right-map-role")]
    [ApiExplorerSettings(GroupName = "Admin Right Map Role")]
    [Authorize]
    [AuthorizeByToken]
    public class RightMapRoleController : ControllerBase
    {
        private readonly IRightMapRoleHandler _handler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public RightMapRoleController(IRightMapRoleHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Gán quyền cho nhóm
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        [HttpPost, Route("")]
        [RightValidate("RIGHTMAPROLE", RightActionConstants.ADD)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] RightMapRoleAssignModel model)
        {
            var result = await _handler.AddRightToRole(model.RoleId, model.RightIds);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Gỡ quyền cho nhóm
        /// </summary>
        /// <param name="model">Dữ liệu</param>
        [HttpDelete, Route("")]
        [RightValidate("RIGHTMAPROLE", RightActionConstants.DELETE)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove([FromBody] RightMapRoleAssignModel model)
        {
            var result = await _handler.RemoveRightFromRole(model.RoleId, model.RightIds);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Gỡ quyền cho nhóm
        /// </summary>
        /// <param name="roleId">Dữ liệu</param>
        [HttpGet, Route("{roleId}")]
        [RightValidate("RIGHTMAPROLE", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(Response<List<RightMapRoleViewModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid roleId)
        {
            var result = await _handler.GetRightsInRole(roleId);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách cấu hình quyền cho nhóm
        /// </summary>
        /// <param name="groupCode"></param>
        [HttpGet, Route("config/{groupCode}")]
        [RightValidate("RIGHTMAPROLE", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(Response<Dictionary<string, List<string>>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRightMapConfig(string groupCode)
        {
            var result = await _handler.GetRightMapConfig(groupCode);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách quyền cho nhóm
        /// </summary>
        /// <param name="groupCode"></param>
        [HttpGet, Route("code/{groupCode}")]
        [ProducesResponseType(typeof(Response<RightViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetRightMapConfigByCode(string groupCode)
        {
            var result = _handler.GetRightMapConfigByCode(groupCode);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy danh sách quyền cho nhiều nhóm
        /// </summary>
        /// <param name="groupCode"></param>
        [HttpGet, Route("codes/{groupCode}")]
        [ProducesResponseType(typeof(Response<RightViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetRightMapConfigByListCode(string groupCode)
        {
            var listCode = groupCode.Split(',').ToList();
            var result = _handler.GetRightMapConfigByListCode(listCode);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cấu hình quyền cho nhóm quyền
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="model"></param>
        [HttpPut, Route("config/{groupCode}")]
        [RightValidate("RIGHTMAPROLE", RightActionConstants.UPDATE)]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfigRightMap(string groupCode,
            [FromBody] List<RightMapRoleAssignModel> model)
        {
            var result = await _handler.ConfigRightMap(groupCode, model);
            return Helper.TransformData(result);
        }
    }
}