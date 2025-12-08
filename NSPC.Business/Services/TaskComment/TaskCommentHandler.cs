using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Entity;
using Serilog;
using System.Linq.Expressions;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public class TaskCommentHandler : ITaskCommentHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;

        public TaskCommentHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<TaskCommentViewModel>> Create(TaskCommentCreateModel model, RequestUser currentUser)
        {
            try
            {
                var userId = currentUser.UserId;
                var userName = currentUser.FullName;

                var entity = _mapper.Map<sm_TaskComment>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.FullName;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.sm_TaskComment.Add(entity);

                var notification = model.TagIds.Select(tagUserId => new sm_TaskNotification
                {
                    Id = Guid.NewGuid(),
                    TaskId = entity.TaskId,
                    UserId = Guid.Parse(tagUserId),
                    NotificationStatus = NotificationStatus.Mentioned,
                    CreatedByUserName = currentUser.FullName,
                    CreatedByUserId = currentUser.UserId,
                    AvatarUrl = _dbContext.IdmUser.FirstOrDefault(x => x.Id == currentUser.UserId)?.AvatarUrl
                }).ToList();

                _dbContext.sm_TaskNotification.AddRange(notification);

                var createResult = await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<TaskCommentViewModel>(entity), "Thêm mới bình luận thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<TaskCommentViewModel>(ex);
            }
        }

        public async Task<Response<TaskCommentViewModel>> Update(Guid Id,
            TaskCommentCreateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_TaskComment.AsNoTracking()
                    .Include(x => x.CreatedByUser)
                    .FirstOrDefaultAsync(x => x.Id == Id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<TaskCommentViewModel>("Không tìm thấy comment");
                }

                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;

                entity.Content = model.Content;

                _dbContext.sm_TaskComment.Update(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<TaskCommentViewModel>(entity), "Chỉnh sửa bình luận thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<TaskCommentViewModel>(ex);
            }
        }

        public async Task<Response<TaskCommentViewModel>> Delete(Guid Id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_TaskComment.AsNoTracking()
                    .Include(x => x.CreatedByUser)
                    .FirstOrDefaultAsync(x => x.Id == Id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<TaskCommentViewModel>(string.Format("Không tìm thấy comment!"));

                _dbContext.Remove(entity);
                var createResult = await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<TaskCommentViewModel>(entity), "Xóa bình luận thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", Id);
                return Helper.CreateExceptionResponse<TaskCommentViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<TaskCommentViewModel>>> GetPageAsync(
            TaskQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_TaskComment.AsNoTracking()
                    .Include(x => x.CreatedByUser)
                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                return Helper.CreateSuccessResponse(_mapper.Map<Pagination<TaskCommentViewModel>>(data));

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@query}", query);
                return Helper.CreateExceptionResponse<Pagination<TaskCommentViewModel>>(ex);
            }
        }

        private Expression<Func<sm_TaskComment, bool>> BuildQuery(TaskQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_TaskComment>(true);

            if (query.TaskId.HasValue)
            {
                predicate.And(x => x.TaskId == query.TaskId);
            }

            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Content.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.ParticipantId.HasValue)
                predicate.And(s => s.CreatedByUser.Id == query.ParticipantId);

            return predicate;
        }
    }
}

