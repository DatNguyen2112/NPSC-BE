using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSPC.Business.Services;
using NSPC.Common;

namespace NSPC.API.V1.Controllers
{
    /// <summary>
    /// Social Media Controller
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/social-media")]
    [ApiExplorerSettings(GroupName = "Social Media")]
    public class SocialMediaController : ControllerBase
    {
        private readonly ISocialMediaHandler _handler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="handler"></param>
        public SocialMediaController(ISocialMediaHandler handler)
        {
            _handler = handler;
        }

        #region Post Operations

        /// <summary>
        /// Lấy danh sách bài viết
        /// </summary>
        /// <param name="skip">Số bài viết bỏ qua</param>
        /// <param name="size">Số bài viết lấy về</param>
        /// <param name="sort">Sắp xếp</param>
        /// <returns></returns>
        [HttpGet, Route("posts")]
        [Authorize, RightValidate("CONSTRUCTIONNEWS", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(Response<SocialPostListResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPosts([FromQuery] int skip = 0, [FromQuery] int size = 20,
            [FromQuery] string sort = "-CreatedOnDate")
        {
            var query = new SocialPostQueryModel
            {
                Skip = skip,
                Size = size,
                Sort = sort
            };
            var result = await _handler.GetPosts(query);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Tạo bài viết mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("posts")]
        [Authorize, RightValidate("CONSTRUCTIONNEWS", "POST")]
        [ProducesResponseType(typeof(Response<SocialPostViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePost([FromBody] SocialPostCreateUpdateModel model)
        {
            var result = await _handler.CreatePost(model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Lấy bài viết theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("posts/{id}")]
        [Authorize, RightValidate("CONSTRUCTIONNEWS", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(Response<SocialPostDetailViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var result = await _handler.GetPostById(id);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật bài viết
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("posts/{id}")]
        [Authorize, RightValidate("CONSTRUCTIONNEWS", "POST")]
        [ProducesResponseType(typeof(Response<SocialPostViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePost(Guid id, [FromBody] SocialPostCreateUpdateModel model)
        {
            var result = await _handler.UpdatePost(id, model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa bài viết
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("posts/{id}")]
        [Authorize, RightValidate("CONSTRUCTIONNEWS", "POST")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var result = await _handler.DeletePost(id);
            return Helper.TransformData(result);
        }

        #endregion

        #region Comment Operations

        /// <summary>
        /// Tạo comment mới cho bài viết hoặc trả lời comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("comments")]
        [Authorize, RightValidate("CONSTRUCTIONNEWS", "COMMENT")]
        [ProducesResponseType(typeof(Response<SocialCommentViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateComment([FromBody] SocialCommentCreateUpdateModel model)
        {
            var result = await _handler.CreateComment(model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Cập nhật comment
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut, Route("comments/{id}")]
        [Authorize, RightValidate("CONSTRUCTIONNEWS", "COMMENT")]
        [ProducesResponseType(typeof(Response<SocialCommentViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] SocialCommentCreateUpdateModel model)
        {
            var result = await _handler.UpdateComment(id, model);
            return Helper.TransformData(result);
        }

        /// <summary>
        /// Xóa comment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("comments/{id}")]
        [Authorize, RightValidate("CONSTRUCTIONNEWS", "COMMENT")]
        [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var result = await _handler.DeleteComment(id);
            return Helper.TransformData(result);
        }

        #endregion

        #region Reaction Operations

        /// <summary>
        /// Cập nhật reaction cho bài viết hoặc comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("reactions")]
        [Authorize, RightValidate("CONSTRUCTIONNEWS", RightActionConstants.VIEW)]
        [ProducesResponseType(typeof(Response<List<SocialReactionViewModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateReaction([FromBody] UpdateReactionModel model)
        {
            var result = await _handler.UpdateReaction(model);
            return Helper.TransformData(result);
        }

        #endregion
    }
}