using NSPC.Common;

namespace NSPC.Business.Services
{
    public interface ISocialMediaHandler
    {
        // Post operations
        Task<Response<SocialPostListResponse>> GetPosts(SocialPostQueryModel query);
        Task<Response<SocialPostViewModel>> CreatePost(SocialPostCreateUpdateModel model);
        Task<Response<SocialPostViewModel>> UpdatePost(Guid id, SocialPostCreateUpdateModel model);
        Task<Response<SocialPostDetailViewModel>> GetPostById(Guid id);
        Task<Response> DeletePost(Guid id);

        // Comment operations
        Task<Response<SocialCommentViewModel>> CreateComment(SocialCommentCreateUpdateModel model);
        Task<Response<SocialCommentViewModel>> UpdateComment(Guid id, SocialCommentCreateUpdateModel model);
        Task<Response> DeleteComment(Guid id);

        // Reaction operations
        Task<Response<List<SocialReactionViewModel>>> UpdateReaction(UpdateReactionModel model);
    }
}