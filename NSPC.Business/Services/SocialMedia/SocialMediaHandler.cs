using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Entity;
using Serilog;

namespace NSPC.Business.Services
{
    public class SocialMediaHandler : ISocialMediaHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAttachmentHandler _attachmentHandler;
        private readonly IMapper _mapper;

        public SocialMediaHandler(
            SMDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IAttachmentHandler attachmentHandler,
            IMapper mapper
        )
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _attachmentHandler = attachmentHandler;
            _mapper = mapper;
        }

        #region Post Operations

        public async Task<Response<SocialPostListResponse>> GetPosts(SocialPostQueryModel query)
        {
            try
            {
                var totalPosts = await _dbContext.sm_SocialPost.CountAsync();
                var posts = await _dbContext.sm_SocialPost
                    .OrderByDescending(x => x.CreatedOnDate)
                    .Skip(Math.Max(0, query.Skip))
                    .Take(Math.Max(1, query.Size))
                    .ToListAsync();
                var postIdList = posts.Select(x => x.Id).ToList();
                var comments = await _dbContext.sm_SocialComment
                    .Where(x => postIdList.Contains(x.PostId))
                    .ToListAsync();
                var userIdList = new HashSet<Guid>();
                userIdList.UnionWith(posts.Select(x => x.CreatedByUserId));
                userIdList.UnionWith(comments.Select(x => x.CreatedByUserId));

                var users = await _dbContext.IdmUser
                    .Where(x => userIdList.ToList().Contains(x.Id))
                    .ToListAsync();
                var postViewModels = _mapper.Map<List<SocialPostViewModel>>(posts);

                var response = new SocialPostListResponse
                {
                    Posts = postViewModels,
                    TotalPosts = totalPosts,
                    HasMore = query.Skip + query.Size < totalPosts
                };

                Log.Information(
                    "GetPosts succeeded - Skip: {Skip}, Size: {Size}, TotalPosts: {TotalPosts}, HasMore: {HasMore}",
                    query.Skip, query.Size, totalPosts, response.HasMore);
                return Helper.CreateSuccessResponse(response, "Lấy danh sách bài viết thành công");
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                Log.Information("GetPosts failed with exception - Skip: {Skip}, Size: {Size}", query.Skip, query.Size);
                return Helper.CreateExceptionResponse<SocialPostListResponse>(e);
            }
        }

        public async Task<Response<SocialPostViewModel>> CreatePost(SocialPostCreateUpdateModel model)
        {
            try
            {
                var validationResult = await ValidatePostCreateUpdateModel<SocialPostViewModel>(model);
                if (validationResult != null) return validationResult;

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var entity = _mapper.Map<sm_SocialPost>(model);
                entity.Id = Guid.NewGuid();
                entity.Attachments = await ProcessAttachments(entity.Id, entity.CreatedOnDate, model.Attachments);
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = entity.CreatedOnDate;
                entity.TenantId = currentUser.TenantId;

                _dbContext.sm_SocialPost.Add(entity);
                await _dbContext.SaveChangesAsync();
                await _dbContext.Entry(entity).Reference(p => p.CreatedByUser).LoadAsync();

                var result = _mapper.Map<SocialPostViewModel>(entity);
                Log.Information("CreatePost succeeded - PostId: {PostId}, Content: {Content}", entity.Id,
                    entity.Content);
                return Helper.CreateSuccessResponse(result, "Tạo bài viết thành công");
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                Log.Information("CreatePost failed with exception - Model: {@Model}", model);
                return Helper.CreateExceptionResponse<SocialPostViewModel>(e);
            }
        }

        public async Task<Response<SocialPostViewModel>> UpdatePost(Guid id, SocialPostCreateUpdateModel model)
        {
            try
            {
                var validationResult = await ValidatePostCreateUpdateModel<SocialPostViewModel>(model);
                if (validationResult != null) return validationResult;

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var entity = await _dbContext.sm_SocialPost
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Log.Information("UpdatePost failed - entity not found - PostId: {PostId}", id);
                    return Helper.CreateNotFoundResponse<SocialPostViewModel>("Bài viết không tồn tại");
                }

                if (entity.CreatedByUserId != currentUser.UserId)
                {
                    Log.Information("UpdatePost failed - user not allowed - PostId: {PostId}", id);
                    return Helper.CreateForbiddenResponse<SocialPostViewModel>(
                        "Bạn không có quyền chỉnh sửa bài viết này");
                }

                entity.Content = model.Content;
                entity.Attachments = await ProcessAttachments(entity.Id, entity.CreatedOnDate, model.Attachments);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                _dbContext.sm_SocialPost.Update(entity);
                await _dbContext.SaveChangesAsync();

                var result = _mapper.Map<SocialPostViewModel>(entity);
                Log.Information("UpdatePost succeeded - PostId: {PostId}, Content: {Content}", entity.Id,
                    entity.Content);
                return Helper.CreateSuccessResponse(result, "Cập nhật bài viết thành công");
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                Log.Information("UpdatePost failed with exception - PostId: {PostId}, Model: {@Model}", id, model);
                return Helper.CreateExceptionResponse<SocialPostViewModel>(e);
            }
        }

        public async Task<Response<SocialPostDetailViewModel>> GetPostById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_SocialPost
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Log.Information("GetPostById failed - entity not found - PostId: {PostId}", id);
                    return Helper.CreateNotFoundResponse<SocialPostDetailViewModel>("Bài viết không tồn tại");
                }

                var comments = await _dbContext.sm_Comments
                    .Where(x => x.PostId == id)
                    .ToListAsync();

                var result = _mapper.Map<SocialPostDetailViewModel>(entity);

                Log.Information("GetPostById succeeded - PostId: {PostId}", id);
                return Helper.CreateSuccessResponse(result, "Lấy thông tin bài viết thành công");
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                Log.Information("GetPostById failed with exception - PostId: {PostId}", id);
                return Helper.CreateExceptionResponse<SocialPostDetailViewModel>(e);
            }
        }

        public async Task<Response> DeletePost(Guid id)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var entity = await _dbContext.sm_SocialPost.FindAsync(id);

                if (entity == null)
                {
                    Log.Information("DeletePost failed - entity not found - PostId: {PostId}", id);
                    return Helper.CreateNotFoundResponse("Bài viết không tồn tại");
                }

                if (entity.CreatedByUserId != currentUser.UserId)
                {
                    Log.Information("DeletePost failed - user not allowed - PostId: {PostId}", id);
                    return Helper.CreateForbiddenResponse("Bạn không có quyền xóa bài viết này");
                }

                _dbContext.sm_SocialPost.Remove(entity);
                await _dbContext.SaveChangesAsync();

                Log.Information("DeletePost succeeded - PostId: {PostId}", id);
                return Helper.CreateSuccessResponse("Xóa bài viết thành công");
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                Log.Information("DeletePost failed with exception - PostId: {PostId}", id);
                return Helper.CreateExceptionResponse(e);
            }
        }

        #endregion

        #region Comment Operations

        public async Task<Response<SocialCommentViewModel>> CreateComment(SocialCommentCreateUpdateModel model)
        {
            try
            {
                var validationResult = await ValidateCommentCreateUpdateModel<SocialCommentViewModel>(model);
                if (validationResult != null) return validationResult;

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var entity = _mapper.Map<sm_SocialComment>(model);
                entity.Id = Guid.NewGuid();
                entity.Attachments = await ProcessAttachments(entity.Id, entity.CreatedOnDate, model.Attachments);
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = entity.CreatedOnDate;
                entity.TenantId = currentUser.TenantId;

                _dbContext.sm_SocialComment.Add(entity);
                await _dbContext.SaveChangesAsync();
                await _dbContext.Entry(entity).Reference(p => p.CreatedByUser).LoadAsync();

                var result = _mapper.Map<SocialCommentViewModel>(entity);
                Log.Information(
                    "CreateComment succeeded - CommentId: {CommentId}, PostId: {PostId}, ParentCommentId: {ParentCommentId}",
                    entity.Id, entity.PostId, entity.ParentCommentId);
                return Helper.CreateSuccessResponse(result, "Tạo comment thành công");
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                Log.Information("CreateComment failed with exception - Model: {@Model}", model);
                return Helper.CreateExceptionResponse<SocialCommentViewModel>(e);
            }
        }

        public async Task<Response<SocialCommentViewModel>> UpdateComment(Guid id, SocialCommentCreateUpdateModel model)
        {
            try
            {
                var validationResult = await ValidateCommentCreateUpdateModel<SocialCommentViewModel>(model, true);
                if (validationResult != null) return validationResult;

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var entity = await _dbContext.sm_SocialComment.FindAsync(id);

                if (entity == null)
                {
                    Log.Information("UpdateComment failed - entity not found - CommentId: {CommentId}", id);
                    return Helper.CreateNotFoundResponse<SocialCommentViewModel>("Comment không tồn tại");
                }

                if (entity.CreatedByUserId != currentUser.UserId)
                {
                    Log.Information("UpdateComment failed - user not allowed - CommentId: {CommentId}", id);
                    return Helper.CreateForbiddenResponse<SocialCommentViewModel>(
                        "Bạn không có quyền chỉnh sửa comment này");
                }

                entity.Content = model.Content;
                entity.Attachments = await ProcessAttachments(entity.Id, entity.CreatedOnDate, model.Attachments);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                _dbContext.sm_SocialComment.Update(entity);
                await _dbContext.SaveChangesAsync();

                var result = _mapper.Map<SocialCommentViewModel>(entity);
                Log.Information("UpdateComment succeeded - CommentId: {CommentId}", entity.Id);
                return Helper.CreateSuccessResponse(result, "Cập nhật comment thành công");
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                Log.Information("UpdateComment failed with exception - CommentId: {CommentId}, Model: {@Model}", id,
                    model);
                return Helper.CreateExceptionResponse<SocialCommentViewModel>(e);
            }
        }

        public async Task<Response> DeleteComment(Guid id)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var entity = await _dbContext.sm_SocialComment.FindAsync(id);

                if (entity == null)
                {
                    Log.Information("DeleteComment failed - entity not found - CommentId: {CommentId}", id);
                    return Helper.CreateNotFoundResponse("Comment không tồn tại");
                }

                if (entity.CreatedByUserId != currentUser.UserId)
                {
                    Log.Information("DeleteComment failed - user not allowed - CommentId: {CommentId}", id);
                    return Helper.CreateForbiddenResponse("Bạn không có quyền xóa comment này");
                }

                _dbContext.sm_SocialComment.Remove(entity);
                await _dbContext.SaveChangesAsync();

                Log.Information("DeleteComment succeeded - CommentId: {CommentId}", id);
                return Helper.CreateSuccessResponse("Xóa comment thành công");
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                Log.Information("DeleteComment failed with exception - CommentId: {CommentId}", id);
                return Helper.CreateExceptionResponse(e);
            }
        }

        #endregion

        #region Reaction Operations

        public async Task<Response<List<SocialReactionViewModel>>> UpdateReaction(UpdateReactionModel model)
        {
            try
            {
                var validationResult = ValidateUpdateReactionModel<List<SocialReactionViewModel>>(model);
                if (validationResult != null) return validationResult;

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                if (model.IsPost)
                {
                    var post = await _dbContext.sm_SocialPost.FindAsync(model.TargetId);
                    if (post == null)
                    {
                        Log.Information("UpdateReaction failed - post not found - PostId: {PostId}", model.TargetId);
                        return Helper.CreateNotFoundResponse<List<SocialReactionViewModel>>("Bài viết không tồn tại");
                    }

                    post.Reactions = UpdateReactionList(post.Reactions, currentUser, model);
                    _dbContext.sm_SocialPost.Update(post);
                }
                else
                {
                    var comment = await _dbContext.sm_SocialComment.FindAsync(model.TargetId);
                    if (comment == null)
                    {
                        Log.Information("UpdateReaction failed - comment not found - CommentId: {CommentId}",
                            model.TargetId);
                        return Helper.CreateNotFoundResponse<List<SocialReactionViewModel>>("Comment không tồn tại");
                    }

                    comment.Reactions = UpdateReactionList(comment.Reactions, currentUser, model);
                    _dbContext.sm_SocialComment.Update(comment);
                }

                await _dbContext.SaveChangesAsync();

                // Get updated reactions to return
                var reactions = new List<jsonb_SocialReaction>();

                if (model.IsPost)
                {
                    var post = await _dbContext.sm_SocialPost.FindAsync(model.TargetId);
                    reactions = post?.Reactions ?? reactions;
                }
                else
                {
                    var comment = await _dbContext.sm_SocialComment.FindAsync(model.TargetId);
                    reactions = comment?.Reactions ?? reactions;
                }

                var result = _mapper.Map<List<SocialReactionViewModel>>(reactions);
                Log.Information(
                    "UpdateReaction succeeded - IsPost: {IsPost}, TargetId: {TargetId}, IsAdd: {IsAdd}, Reaction: {Reaction}",
                    model.IsPost, model.TargetId, model.IsAdd, model.Reaction);
                return Helper.CreateSuccessResponse(result, "Cập nhật reaction thành công");
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                Log.Information("UpdateReaction failed with exception - Model: {@Model}", model);
                return Helper.CreateExceptionResponse<List<SocialReactionViewModel>>(e);
            }
        }

        #endregion

        #region Helper Methods

        private List<jsonb_SocialReaction> UpdateReactionList(
            List<jsonb_SocialReaction> currentReactions,
            Helper.RequestUser currentUser,
            UpdateReactionModel model
        )
        {
            currentReactions ??= new List<jsonb_SocialReaction>();

            var existingReaction = currentReactions.FirstOrDefault(r => r.UserId == currentUser.UserId);

            if (model.IsAdd)
            {
                if (existingReaction != null)
                {
                    // Update existing reaction
                    existingReaction.Reaction = Enum.Parse<SocialReactionType>(model.Reaction);
                    existingReaction.ReactedDate = DateTime.Now;
                }
                else
                {
                    // Add new reaction
                    currentReactions.Add(new jsonb_SocialReaction
                    {
                        UserId = currentUser.UserId,
                        UserName = currentUser.UserName,
                        Reaction = Enum.Parse<SocialReactionType>(model.Reaction),
                        ReactedDate = DateTime.Now
                    });
                }
            }
            else if (!model.IsAdd && existingReaction != null)
            {
                currentReactions.Remove(existingReaction);
            }

            return currentReactions;
        }

        private async Task<List<jsonb_Attachment>> ProcessAttachments(
            Guid targetId,
            DateTime createdOnDate,
            List<jsonb_Attachment> attachments
        )
        {
            try
            {
                var attachmentIdList = attachments.Select(x => x.Id).ToList();

                if (attachmentIdList.Count == 0)
                {
                    return new List<jsonb_Attachment>();
                }

                var allAttachments = await _dbContext.erp_Attachment
                    .Where(x => attachmentIdList.Contains(x.Id))
                    .ToListAsync();

                foreach (var att in allAttachments)
                {
                    att.EntityId = targetId;
                    att.EntityType = attachments.FirstOrDefault(x => x.Id == att.Id)?.DocType;
                    att.Description = attachments.FirstOrDefault(x => x.Id == att.Id)?.Description;

                    var moveFileResult = _attachmentHandler.MoveEntityAttachment(
                        att.DocType,
                        att.EntityType,
                        targetId,
                        att.FilePath,
                        createdOnDate
                    );

                    if (moveFileResult.IsSuccess)
                    {
                        att.FilePath = moveFileResult.Data;
                    }
                }

                return allAttachments.Select(x => new jsonb_Attachment
                {
                    Id = x.Id,
                    Description = x.Description,
                    DocType = x.DocType,
                    FileName = x.OriginalFileName,
                    FilePath = x.FilePath,
                    FileType = x.FileType,
                    FileSize = x.FileSize
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        #endregion

        #region Validation Methods

        private Task<Response<T>> ValidatePostCreateUpdateModel<T>(SocialPostCreateUpdateModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                Log.Information("ValidatePostCreateUpdateModel failed - Content is required");
                return Task.FromResult(Helper.CreateBadRequestResponse<T>("Nội dung bài viết không được để trống"));
            }

            return Task.FromResult<Response<T>>(null);
        }

        private async Task<Response<T>> ValidateCommentCreateUpdateModel<T>(
            SocialCommentCreateUpdateModel model,
            bool isUpdate = false
        )
        {
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                Log.Information("ValidateCommentCreateUpdateModel failed - Content is required");
                return Helper.CreateBadRequestResponse<T>("Nội dung comment không được để trống");
            }

            if (isUpdate) return null;

            if (model.PostId == null && model.ParentCommentId == null)
            {
                Log.Information(
                    "ValidateCommentCreateUpdateModel failed - Either PostId or ParentCommentId is required");
                return Helper.CreateBadRequestResponse<T>("Phải chỉ định bài viết hoặc comment cha");
            }

            // Validate ParentCommentId exists
            if (model.ParentCommentId != null)
            {
                var parentComment = await _dbContext.sm_SocialComment.FindAsync(model.ParentCommentId);
                if (parentComment == null)
                {
                    Log.Information(
                        "ValidateCommentCreateUpdateModel failed - Parent comment not found - ParentCommentId: {ParentCommentId}",
                        model.ParentCommentId);
                    return Helper.CreateBadRequestResponse<T>("Comment cha không tồn tại");
                }

                // Set PostId from parent comment
                model.PostId = parentComment.PostId;
            }
            else if (model.PostId != null)
            {
                // Validate PostId exists
                var postExists = await _dbContext.sm_SocialPost.AnyAsync(x => x.Id == model.PostId);
                if (!postExists)
                {
                    Log.Information("ValidateCommentCreateUpdateModel failed - Post not found - PostId: {PostId}",
                        model.PostId);
                    return Helper.CreateBadRequestResponse<T>("Bài viết không tồn tại");
                }
            }

            return null;
        }

        private Response<T> ValidateUpdateReactionModel<T>(UpdateReactionModel model)
        {
            if (model.TargetId == Guid.Empty)
            {
                Log.Information("ValidateUpdateReactionModel failed - TargetId is required");
                return Helper.CreateBadRequestResponse<T>("ID đối tượng không hợp lệ");
            }

            if (model.IsAdd)
            {
                if (string.IsNullOrWhiteSpace(model.Reaction) ||
                    !Enum.TryParse<SocialReactionType>(model.Reaction, out _))
                {
                    Log.Information("ValidateUpdateReactionModel failed - Invalid Reaction: {Reaction}",
                        model.Reaction);
                    return Helper.CreateBadRequestResponse<T>(
                        "Loại reaction không hợp lệ (Like, Haha, Love, Wow, Sad, Angry)");
                }
            }

            return null;
        }

        #endregion
    }
}