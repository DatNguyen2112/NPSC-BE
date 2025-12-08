using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using SharpCompress.Common;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static NSPC.Common.Helper;

namespace NSPC.Business
{
    public class TaskManagementHandler : ITaskManagementHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IAttachmentHandler _attachmentHandler;


        public TaskManagementHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IAttachmentHandler attachmentHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _attachmentHandler = attachmentHandler;
        }

        #region TaskManagement
        public async Task<Response<TaskManagementViewModel>> CreateTask(TaskManagementCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = _mapper.Map<sm_TaskManagement>(model);
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.TenantId = currentUser.TenantId;
                if (model.StartDate > DateTime.Now)
                {
                    entity.Status = TaskManagementConstant.TaskManagementConstantStatus.DRAFT;
                } 
                else
                {
                    entity.Status = TaskManagementConstant.TaskManagementConstantStatus.INPROGRESS;
                }    

                _dbContext.sm_TaskManagement.Add(entity);

                await _dbContext.SaveChangesAsync();

                if (model.Attachments != null)
                {
                    /* Attachment Process */
                    await processAttachment(entity.Id, model.Attachments);
                }

                var history = new TaskManagementHistoryCreateUpdateModel();
                history.Action = $"{currentUser.FullName} đã tạo công việc {entity.Title}";
                history.TaskManagementId = entity.Id;
                         
                var taskHistory = await CreateTaskHistory(history, currentUser);
                if (!taskHistory.IsSuccess)
                {
                    return CreateBadRequestResponse<TaskManagementViewModel>(taskHistory.Message);
                }

                if (model.UserIds != null)
                {
                    foreach (var item in model.UserIds)
                    {
                        var assigneeModel = new TaskManagementAssigneeCreateUpdateModel()
                        {
                            TaskManagementId = entity.Id,
                            UserId = item
                        };
                        var assignee = await CreateTaskAssignee(assigneeModel, currentUser);
                        if (!assignee.IsSuccess)
                        {
                            return CreateBadRequestResponse<TaskManagementViewModel>(assignee.Message);
                        }
                    }
                }

                var result = await GetByIdTask(entity.Id, currentUser);

                if (result.IsSuccess)
                    result.Message = "Thêm mới thành công";
                return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return CreateExceptionResponse<TaskManagementViewModel>(ex);
            }
        }

        private async Task processAttachment(Guid taskId, List<jsonb_Attachment> attachments)
        {
            try
            {
                var attachmentListId = attachments.Select(x => x.Id).ToList();
                // Process attachments
                if (attachmentListId.Count > 0)
                {
                    var news = await _dbContext.sm_TaskManagement.Where(x => x.Id == taskId).FirstOrDefaultAsync();

                    var allAttachments = await _dbContext.erp_Attachment.Where(x => attachmentListId.Contains(x.Id))
                        .ToListAsync();

                    foreach (var att in allAttachments)
                    {
                        // UpdatePaid entity
                        att.EntityId = news.Id;
                        att.EntityType = AttachmentEntityTypeConstants.Task;
                        att.Description = attachments.Where(x => x.Id == att.Id).FirstOrDefault()?.Description;

                        // Move files to new folder
                        var moveFileResult = _attachmentHandler.MoveEntityAttachment(att.DocType, att.EntityType,
                            news.Id, att.FilePath, news.CreatedOnDate);
                        if (moveFileResult.IsSuccess)
                            att.FilePath = moveFileResult.Data;
                    }

                    if (allAttachments != null && allAttachments.Count() > 0)
                        news.Attachments = allAttachments.Select(x => new jsonb_Attachment
                        {
                            Description = x.Description,
                            DocType = x.DocType,
                            FilePath = x.FilePath,
                            Id = x.Id
                        }).ToList();
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }


        public async Task<Response<List<TaskManagementViewModel>>> CreateMultiTask(List<TaskManagementCreateUpdateModel> model, RequestUser currentUser)
        {
            try
            {
                if (model.Count <= 0 || model == null)
                    return CreateBadRequestResponse<List<TaskManagementViewModel>>("");

                var result = new List<TaskManagementViewModel>();
                foreach (var item in model)
                {
                    var entity = _mapper.Map<sm_TaskManagement>(item);
                    entity.CreatedByUserId = currentUser.UserId;
                    entity.CreatedByUserName = currentUser.UserName;
                    entity.CreatedOnDate = DateTime.Now;
                    entity.LastModifiedByUserId = currentUser.UserId;
                    entity.LastModifiedByUserName = currentUser.UserName;
                    entity.LastModifiedOnDate = DateTime.Now;
                    entity.TenantId = currentUser.TenantId;

                    _dbContext.sm_TaskManagement.Add(entity);

                    await _dbContext.SaveChangesAsync();

                    if (item.Attachments != null)
                    {
                        /* Attachment Process */
                        await processAttachment(entity.Id, item.Attachments);
                    }

                    var history = new TaskManagementHistoryCreateUpdateModel();
                    history.Action = $"Người dùng {currentUser.UserName} đã tạo công việc {entity.Title}";
                    history.TaskManagementId = entity.Id;

                    var taskHistory = await CreateTaskHistory(history, currentUser);
                    if (!taskHistory.IsSuccess)
                    {
                        return CreateBadRequestResponse<List<TaskManagementViewModel>>(taskHistory.Message);
                    }

                    if (item.UserIds != null)
                    {
                        foreach (var userId in item.UserIds)
                        {
                            var assigneeModel = new TaskManagementAssigneeCreateUpdateModel()
                            {
                                TaskManagementId = entity.Id,
                                UserId = userId
                            };
                            var assignee = await CreateTaskAssignee(assigneeModel, currentUser);
                            if (!assignee.IsSuccess)
                            {
                                return CreateBadRequestResponse<List<TaskManagementViewModel>>(assignee.Message);
                            }
                        }
                    }

                    var task = await GetByIdTask(entity.Id, currentUser);

                    if (task.IsSuccess)
                    {
                        result.Add(task.Data);
                    }
                    else
                    {
                        return CreateBadRequestResponse<List<TaskManagementViewModel>>(task.Message);
                    }
                }

                return CreateSuccessResponse(result);

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return CreateExceptionResponse<List<TaskManagementViewModel>>(ex);
            }
        }

        public async Task<Response<TaskManagementViewModel>> UpdateTask(Guid id, TaskManagementCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var task = await _dbContext.sm_TaskManagement.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (task == null)
                {
                    return CreateNotFoundResponse<TaskManagementViewModel>("Không tìm thấy công việc");
                }

                task.Title = model.Title;
                task.Type = model.Type;
                task.Description = model.Description;
                task.ConstructionId = model.ConstructionId.Value;
                task.Status = model.Status;
                task.Attachments = model.Attachments;
                task.DueDate = model.DueDate;
                task.StartDate = model.StartDate;
                task.LastModifiedByUserId = currentUser.UserId;
                task.LastModifiedByUserName = currentUser.UserName;
                task.LastModifiedOnDate = DateTime.Now;                
                _dbContext.sm_TaskManagement.Update(task);               

                await _dbContext.SaveChangesAsync();

                var history = new TaskManagementHistoryCreateUpdateModel();
                history.Action = $"{currentUser.FullName} đã cập nhật công việc {task.Title}";
                history.TaskManagementId = task.Id;

                var taskHistory = await CreateTaskHistory(history, currentUser);
                if (!taskHistory.IsSuccess)
                {
                    return CreateBadRequestResponse<TaskManagementViewModel>(taskHistory.Message);
                }

                var result = await GetByIdTask(task.Id, currentUser);

                if (result.IsSuccess)
                    result.Message = "Cập nhật thành công";
                return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return CreateExceptionResponse<TaskManagementViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<TaskManagementViewModel>>> GetPageTask(TaskManagementQueryModel query, RequestUser currentUser)
        {
            try
            {
                var predicate = BuildQueryTask(query);
                predicate = predicate.And(x => x.TenantId == currentUser.TenantId);
                if (!currentUser.ListRoles.Contains(RoleConstants.AdminRoleCode) && !currentUser.ListRoles.Contains(RoleConstants.SuperAdminRoleCode))
                {
                    predicate = predicate.And(x => x.sm_TaskManagementAssignees.Any(y => y.UserId == currentUser.UserId));
                }    
                var queryResult = _dbContext.sm_TaskManagement.Include(x => x.sm_TaskManagementAssignees).ThenInclude(x => x.idm_User)
                                                                .Include(x => x.sm_TaskManagementComments)
                                                                .Include(x => x.sm_TaskManagementHistories)
                                                                .Include(x => x.sm_TaskManagementMileStones)
                                                                .Include(x => x.sm_Construction)
                                                                .AsNoTracking().Where(predicate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<TaskManagementViewModel>>(data);

                return CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return CreateExceptionResponse<Pagination<TaskManagementViewModel>>(ex);
            }
        }

        public async Task<Response<TaskSummanryModel>> GetSummaryTask(TaskManagementQueryModel query, RequestUser currentUser)
        {
            try
            {
                var predicate = BuildQueryTask(query);
                predicate = predicate.And(x => x.TenantId == currentUser.TenantId);
                if (!currentUser.ListRoles.Contains(RoleConstants.AdminRoleCode) && !currentUser.ListRoles.Contains(RoleConstants.SuperAdminRoleCode))
                {
                    predicate = predicate.And(x => x.sm_TaskManagementAssignees.Any(y => y.UserId == currentUser.UserId));
                }
                var all = _dbContext.sm_TaskManagement.AsNoTracking().Where(predicate).Count();
                var inprogress = _dbContext.sm_TaskManagement.AsNoTracking().Where(predicate.And(x => x.Status == TaskManagementConstant.TaskManagementConstantStatus.INPROGRESS)).Count();
                var paused = _dbContext.sm_TaskManagement.AsNoTracking().Where(predicate.And(x => x.Status == TaskManagementConstant.TaskManagementConstantStatus.PAUSED)).Count();
                var draft = _dbContext.sm_TaskManagement.AsNoTracking().Where(predicate.And(x => x.Status == TaskManagementConstant.TaskManagementConstantStatus.DRAFT)).Count();
                var finished = _dbContext.sm_TaskManagement.AsNoTracking().Where(predicate.And(x => x.Status == TaskManagementConstant.TaskManagementConstantStatus.FINISHED)).Count();
                var result = new TaskSummanryModel()
                {
                    TotalTask = all,
                    TotalDraftTask = draft,
                    TotalFinishedTask = finished,
                    TotalInprogressTask = inprogress,
                    TotalPausedTask = paused
                };

                return CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return CreateExceptionResponse<TaskSummanryModel>(ex);
            }
        }

        public async Task<Response<TaskManagementViewModel>> GetByIdTask(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_TaskManagement.Include(x => x.sm_TaskManagementAssignees).ThenInclude(x => x.idm_User)
                                                                .Include(x => x.sm_TaskManagementComments)
                                                                .Include(x => x.sm_TaskManagementHistories)
                                                                .Include(x => x.sm_TaskManagementMileStones)
                                                                .Include(x => x.sm_Construction)
                                                                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return CreateNotFoundResponse<TaskManagementViewModel>("Công việc không tồn tại trong hệ thống.");

                var result = _mapper.Map<TaskManagementViewModel>(entity);

                return new Response<TaskManagementViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return CreateExceptionResponse<TaskManagementViewModel>(ex);
            }
        }

        public async Task<Response> DeleteTask(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_TaskManagement.FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return CreateNotFoundResponse("Công việc không tồn tại trong hệ thống.");
                Log.Information("UserId: {0} deleted taskId {1} has title: {2}", id, entity.Id, entity.Title);
                _dbContext.sm_TaskManagement.Remove(entity);

                await _dbContext.SaveChangesAsync();
               
                return new Response("Xóa công việc thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return CreateExceptionResponse(ex);
            }
        }

        private Expression<Func<sm_TaskManagement, bool>> BuildQueryTask(TaskManagementQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_TaskManagement>(true);           
          
            if (!string.IsNullOrEmpty(query.Title))
            {
                predicate = predicate.And(x => x.Title.ToLower().Contains(query.Title.ToLower()));
            }

            if (query.ConstructionId != null)
            {
                predicate = predicate.And(x => x.ConstructionId.Value == query.ConstructionId.Value);
            }

            if (!string.IsNullOrEmpty(query.FullTextSearch))
            {
                predicate = predicate.And(x => x.Title.ToLower().Contains(query.FullTextSearch.ToLower()));
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                predicate = predicate.And(x => x.Status.ToLower() == query.Status.ToLower());
            }

            return predicate;
        }

        public async Task<Response<TaskManagementViewModel>> MarkAsDoneTask(Guid id, RequestUser currentUser)
        {
            try
            {
                var task = await _dbContext.sm_TaskManagement.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (task == null)
                {
                    return CreateNotFoundResponse<TaskManagementViewModel>("Không tìm thấy công việc");
                }
                task.Status = TaskManagementConstant.TaskManagementConstantStatus.FINISHED;
                var result = await GetByIdTask(id, currentUser);

                if (result.IsSuccess)
                    result.Message = "Cập nhật thành công";
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return CreateExceptionResponse<TaskManagementViewModel>(ex);
            }
        }

        #endregion

        #region TaskManagementAssignee
        public async Task<Response<TaskManagementAssigneeViewModel>> CreateTaskAssignee(TaskManagementAssigneeCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var userEntity = await _dbContext.IdmUser.Where(x => x.Id == model.UserId && x.TenantId == currentUser.TenantId).FirstOrDefaultAsync();
                if (userEntity == null)
                {
                    return CreateNotFoundResponse<TaskManagementAssigneeViewModel>("Không tìm thấy người dùng được giao việc");
                }

                var userAssigned = await _dbContext.sm_TaskManagementAssignee.Where(x => x.UserId == model.UserId && x.TaskManagementId == model.TaskManagementId && x.TenantId == currentUser.TenantId).FirstOrDefaultAsync();
                if (userAssigned != null)
                {
                    return CreateBadRequestResponse<TaskManagementAssigneeViewModel>("Người dùng đã được giao công việc này");
                }

                var task = await _dbContext.sm_TaskManagement.Where(x => x.Id == model.TaskManagementId && x.TenantId == currentUser.TenantId).FirstOrDefaultAsync();
                if (task == null)
                {
                    return CreateNotFoundResponse<TaskManagementAssigneeViewModel>("Không tìm thấy công việc");
                }

                var entity = _mapper.Map<sm_TaskManagementAssignee>(model);
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.TenantId = currentUser.TenantId;

                _dbContext.sm_TaskManagementAssignee.Add(entity);
                await _dbContext.SaveChangesAsync();

                var history = new TaskManagementHistoryCreateUpdateModel();
                history.Action = $"Người dùng {userEntity.UserName} được giao công việc {task.Title}";
                history.TaskManagementId = entity.TaskManagementId;
                var taskHistory = await CreateTaskHistory(history, currentUser);
                if (!taskHistory.IsSuccess)
                {
                    return CreateBadRequestResponse<TaskManagementAssigneeViewModel>(taskHistory.Message);
                }

                var result = await GetByIdTaskAssignee(entity.Id, currentUser);

                if (result.IsSuccess)
                    result.Message = "Thêm mới thành công";
                return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return CreateExceptionResponse<TaskManagementAssigneeViewModel>(ex);
            }
        }

        //public async Task<Response<TaskManagementAssigneeViewModel>> UpdateTaskAssignee(Guid id, TaskManagementAssigneeCreateUpdateModel model, RequestUser currentUser)
        //{
        //    try
        //    {
        //        var userAssigned = await _dbContext.IdmUser.Where(x => x.Id == model.UserId).FirstOrDefaultAsync();
        //        if (userAssigned != null)
        //        {
        //            return CreateNotFoundResponse<TaskManagementAssigneeViewModel>("Không tìm thấy người dùng được giao việc");
        //        }

        //        var task = await _dbContext.sm_TaskManagement.Where(x => x.Id == model.TaskManagementId).FirstOrDefaultAsync();
        //        if (task != null)
        //        {
        //            return CreateNotFoundResponse<TaskManagementAssigneeViewModel>("Không tìm thấy công việc");
        //        }

        //        var entity = _mapper.Map<sm_TaskManagementAssignee>(model);
        //        entity.CreatedByUserId = currentUser.UserId;
        //        entity.CreatedByUserName = currentUser.UserName;
        //        entity.LastModifiedByUserId = currentUser.UserId;
        //        entity.LastModifiedByUserName = currentUser.UserName;

        //        _dbContext.sm_TaskManagementAssignee.Update(entity);

        //        await _dbContext.SaveChangesAsync();

        //        var result = await GetByIdTaskAssignee(entity.Id, currentUser);

        //        if (result.IsSuccess)
        //            result.Message = "Thêm mới thành công";
        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        Log.Information("Params: Model: {@model}", model);
        //        return CreateExceptionResponse<TaskManagementAssigneeViewModel>(ex);
        //    }
        //}

        public async Task<Response<Pagination<TaskManagementAssigneeViewModel>>> GetPageTaskAssignee(TaskManagementAssigneeQueryModel query, RequestUser currentUser)
        {
            try
            {
                var predicate = BuildQueryTaskAssignee(query);
                predicate.And(x => x.TenantId == currentUser.TenantId);
                var queryResult = _dbContext.sm_TaskManagementAssignee.Include(x => x.idm_User).AsNoTracking().Where(predicate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<TaskManagementAssigneeViewModel>>(data);

                return CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return CreateExceptionResponse<Pagination<TaskManagementAssigneeViewModel>>(ex);
            }
        }

        public async Task<Response<TaskManagementAssigneeViewModel>> GetByIdTaskAssignee(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_TaskManagementAssignee.Include(x => x.idm_User).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == currentUser.TenantId);

                if (entity == null)
                    return CreateNotFoundResponse<TaskManagementAssigneeViewModel>("Giao việc không tồn tại trong hệ thống.");

                var result = _mapper.Map<TaskManagementAssigneeViewModel>(entity);

                return new Response<TaskManagementAssigneeViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return CreateExceptionResponse<TaskManagementAssigneeViewModel>(ex);
            }
        }

        public async Task<Response> DeleteTaskAssignee(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_TaskManagementAssignee.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == currentUser.TenantId);

                if (entity == null)
                    return CreateNotFoundResponse("Giao việc không tồn tại trong hệ thống.");

                var userAssigned = await _dbContext.IdmUser.Where(x => x.Id == entity.UserId && x.TenantId == currentUser.TenantId).FirstOrDefaultAsync();
                if (userAssigned == null)
                {
                    return CreateNotFoundResponse<TaskManagementAssigneeViewModel>("Không tìm thấy người dùng được giao việc");
                }

                var task = await _dbContext.sm_TaskManagement.Where(x => x.Id == entity.TaskManagementId && x.TenantId == currentUser.TenantId).FirstOrDefaultAsync();
                if (task == null)
                {
                    return CreateNotFoundResponse<TaskManagementAssigneeViewModel>("Không tìm thấy công việc");
                }

                _dbContext.sm_TaskManagementAssignee.Remove(entity);

                await _dbContext.SaveChangesAsync();

                var history = new TaskManagementHistoryCreateUpdateModel();
                history.Action = $"Người dùng {userAssigned.Name} được gỡ khỏi công việc {task.Title}";
                history.TaskManagementId = entity.TaskManagementId;

                var taskHistory = await CreateTaskHistory(history, currentUser);
                if (!taskHistory.IsSuccess)
                {
                    return CreateBadRequestResponse<TaskManagementAssigneeViewModel>(taskHistory.Message);
                }

                return new Response("Xóa giao việc thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return CreateExceptionResponse(ex);
            }
        }

        private Expression<Func<sm_TaskManagementAssignee, bool>> BuildQueryTaskAssignee(TaskManagementAssigneeQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_TaskManagementAssignee>(true);

            if (query.TaskManagementId.HasValue)
            {
                predicate.And(x => x.TaskManagementId == query.TaskManagementId.Value);
            }

            return predicate;
        }
        #endregion

        #region TaskManagementComment
        public async Task<Response<TaskManagementCommentViewModel>> CreateTaskComment(TaskManagementCommentCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var task = await _dbContext.sm_TaskManagement.Where(x => x.Id == model.TaskManagementId && x.TenantId == currentUser.TenantId).FirstOrDefaultAsync();
                if (task == null)
                {
                    return CreateNotFoundResponse<TaskManagementCommentViewModel>("Không tìm thấy công việc");
                }

                var entity = _mapper.Map<sm_TaskManagementComment>(model);
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.TenantId = currentUser.TenantId;

                _dbContext.sm_TaskManagementComment.Add(entity);

                await _dbContext.SaveChangesAsync();

                var history = new TaskManagementHistoryCreateUpdateModel();
                history.Action = $"Người dùng {currentUser.UserName} đã thêm bình luận trong công việc {task.Title}";
                history.TaskManagementId = entity.TaskManagementId;
                var historyEntity = _mapper.Map<sm_TaskManagementHistory>(history);

                _dbContext.sm_TaskManagementHistory.Add(historyEntity);
                await _dbContext.SaveChangesAsync();

                var result = await GetByIdTaskComment(entity.Id, currentUser);

                if (result.IsSuccess)
                    result.Message = "Thêm mới thành công";
                return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return CreateExceptionResponse<TaskManagementCommentViewModel>(ex);
            }
        }

        public async Task<Response<TaskManagementCommentViewModel>> UpdateTaskComment(Guid id, TaskManagementCommentCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var task = await _dbContext.sm_TaskManagement.Where(x => x.Id == model.TaskManagementId && x.TenantId == currentUser.TenantId).FirstOrDefaultAsync();
                if (task == null)
                {
                    return CreateNotFoundResponse<TaskManagementCommentViewModel>("Không tìm thấy công việc");
                }
                var comment = await _dbContext.sm_TaskManagementComment.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (comment == null)
                {
                    return CreateNotFoundResponse<TaskManagementCommentViewModel>("Không tìm thấy bình luận");
                }

                comment.Content = model.Content;              
                comment.LastModifiedByUserId = currentUser.UserId;
                comment.LastModifiedByUserName = currentUser.UserName;
                comment.LastModifiedOnDate = DateTime.Now;

                _dbContext.sm_TaskManagementComment.Update(comment);

                await _dbContext.SaveChangesAsync();

                var history = new TaskManagementHistoryCreateUpdateModel();
                history.Action = $"{currentUser.FullName} đã cập nhật bình luận trong công việc {task.Title}";
                history.TaskManagementId = comment.TaskManagementId;
                await CreateTaskHistory(history, currentUser);

                var result = await GetByIdTaskComment(comment.Id, currentUser);

                if (result.IsSuccess)
                    result.Message = "Cập nhật thành công";
                return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return CreateExceptionResponse<TaskManagementCommentViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<TaskManagementCommentViewModel>>> GetPageTaskComment(TaskManagementCommentQueryModel query, RequestUser currentUser)
        {
            try
            {
                var predicate = BuildQueryTaskComment(query);
                predicate = predicate.And(x => x.TenantId == currentUser.TenantId);
                var queryResult = _dbContext.sm_TaskManagementComment.Include(x => x.idm_User).AsNoTracking().Where(predicate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<TaskManagementCommentViewModel>>(data);

                return CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return CreateExceptionResponse<Pagination<TaskManagementCommentViewModel>>(ex);
            }
        }

        public async Task<Response<TaskManagementCommentViewModel>> GetByIdTaskComment(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_TaskManagementComment.Include(x => x.idm_User).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == currentUser.TenantId);

                if (entity == null)
                    return CreateNotFoundResponse<TaskManagementCommentViewModel>("Bình luận không tồn tại trong hệ thống.");

                var result = _mapper.Map<TaskManagementCommentViewModel>(entity);

                return new Response<TaskManagementCommentViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return CreateExceptionResponse<TaskManagementCommentViewModel>(ex);
            }
        }

        public async Task<Response> DeleteTaskComment(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_TaskManagementComment.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == currentUser.TenantId);

                if (entity == null)
                    return CreateNotFoundResponse("Bình luận không tồn tại trong hệ thống.");

                var task = await _dbContext.sm_TaskManagement.Where(x => x.Id == entity.TaskManagementId && x.TenantId == currentUser.TenantId).FirstOrDefaultAsync();
                if (task == null)
                {
                    return CreateNotFoundResponse("Công việc không tồn tại trong hệ thống.");
                }

                _dbContext.sm_TaskManagementComment.Remove(entity);

                await _dbContext.SaveChangesAsync();

                var history = new TaskManagementHistoryCreateUpdateModel();
                history.Action = $"Người dùng {currentUser.UserName} đã xóa bình luận trong công việc {task.Title}";
                history.TaskManagementId = entity.TaskManagementId;
                await CreateTaskHistory(history, currentUser);

                return new Response("Xóa bình luận thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return CreateExceptionResponse(ex);
            }
        }

        private Expression<Func<sm_TaskManagementComment, bool>> BuildQueryTaskComment(TaskManagementCommentQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_TaskManagementComment>(true);
            if(query.TaskManagementId.HasValue)
            {
                predicate.And(x => x.TaskManagementId == query.TaskManagementId.Value);
            }    
            return predicate;
        }
        #endregion

        #region TaskManagementHistory
        public async Task<Response<TaskManagementHistoryViewModel>> CreateTaskHistory(TaskManagementHistoryCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = _mapper.Map<sm_TaskManagementHistory>(model);
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.TenantId = currentUser.TenantId;

                _dbContext.sm_TaskManagementHistory.Add(entity);

                await _dbContext.SaveChangesAsync();

                var result = await GetByIdTaskHistory(entity.Id, currentUser);

                if (result.IsSuccess)
                    result.Message = "Thêm mới thành công";
                return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return CreateExceptionResponse<TaskManagementHistoryViewModel>(ex);
            }
        }


        //public async Task<Response<TaskManagementHistoryViewModel>> UpdateTaskHistory(Guid id, TaskManagementHistoryCreateUpdateModel model, RequestUser currentUser)
        //{
        //    try
        //    {
        //        var entity = _mapper.Map<sm_TaskManagementHistory>(model);
        //        entity.CreatedByUserId = currentUser.UserId;
        //        entity.CreatedByUserName = currentUser.UserName;
        //        entity.LastModifiedByUserId = currentUser.UserId;
        //        entity.LastModifiedByUserName = currentUser.UserName;

        //        _dbContext.sm_TaskManagementHistory.Update(entity);

        //        await _dbContext.SaveChangesAsync();

        //        var result = await GetByIdTaskHistory(entity.Id, currentUser);

        //        if (result.IsSuccess)
        //            result.Message = "Cập nhật mới thành công";
        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        Log.Information("Params: Model: {@model}", model);
        //        return CreateExceptionResponse<TaskManagementHistoryViewModel>(ex);
        //    }
        //}

        public async Task<Response<Pagination<TaskManagementHistoryViewModel>>> GetPageTaskHistory(TaskManagementHistoryQueryModel query, RequestUser currentUser)
        {
            try
            {
                var predicate = BuildQueryTaskHistory(query);
                predicate = predicate.And(x => x.TenantId == currentUser.TenantId);
                var queryResult = _dbContext.sm_TaskManagementHistory.AsNoTracking().OrderBy(x => x.CreatedOnDate).Where(predicate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<TaskManagementHistoryViewModel>>(data);

                return CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return CreateExceptionResponse<Pagination<TaskManagementHistoryViewModel>>(ex);
            }
        }
        public async Task<Response<TaskManagementHistoryViewModel>> GetByIdTaskHistory(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_TaskManagementHistory.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == currentUser.TenantId);

                if (entity == null)
                    return CreateNotFoundResponse<TaskManagementHistoryViewModel>("Lịch sử công việc không tồn tại trong hệ thống.");

                var result = _mapper.Map<TaskManagementHistoryViewModel>(entity);

                return new Response<TaskManagementHistoryViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return CreateExceptionResponse<TaskManagementHistoryViewModel>(ex);
            }
        }

        private Expression<Func<sm_TaskManagementHistory, bool>> BuildQueryTaskHistory(TaskManagementHistoryQueryModel query)
        {

            var predicate = PredicateBuilder.New<sm_TaskManagementHistory>(true);
            if (query.TaskManagementId != null)
            {
                predicate = predicate.And(x => x.TaskManagementId == query.TaskManagementId);
            }
            
            return predicate;
        }
        #endregion

        #region TaskManagementMileStone
        public async Task<Response<TaskManagementMileStoneViewModel>> CreateTaskMileStone(TaskManagementMileStoneCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {

                var task = await _dbContext.sm_TaskManagement.Where(x => x.Id == model.TaskManagementId && x.TenantId == currentUser.TenantId).FirstOrDefaultAsync();
                if (task == null)
                {
                    return CreateNotFoundResponse<TaskManagementMileStoneViewModel>("Không tìm thấy công việc");
                }

                if (model.StartDate < task.StartDate)
                    return CreateBadRequestResponse<TaskManagementMileStoneViewModel>("Ngày bắt đầu của milestone phải lớn hơn hoặc bằng ngày bắt đầu của công việc");

                if (model.DueDate > task.DueDate)
                    return CreateBadRequestResponse<TaskManagementMileStoneViewModel>("Ngày hoàn thành của milestone phải lớn nhỏ hoặc bằng ngày hoàn thành của công việc");

                var entity = _mapper.Map<sm_TaskManagementMileStone>(model);
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.TenantId = currentUser.TenantId;

                _dbContext.sm_TaskManagementMileStone.Add(entity);
                await _dbContext.SaveChangesAsync();

                var result = await GetByIdTaskMileStone(entity.Id, currentUser);

                if (result.IsSuccess)
                    result.Message = "Thêm mới thành công";
                return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return CreateExceptionResponse<TaskManagementMileStoneViewModel>(ex);
            }
        }

        public async Task<Response<TaskManagementMileStoneViewModel>> UpdateTaskMileStone(Guid id, TaskManagementMileStoneCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var userAssigned = await _dbContext.sm_TaskManagementMileStone.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (userAssigned != null)
                {
                    return CreateNotFoundResponse<TaskManagementMileStoneViewModel>("Không tìm thấy milestone");
                }

                var task = await _dbContext.sm_TaskManagement.Where(x => x.Id == model.TaskManagementId).FirstOrDefaultAsync();
                if (task != null)
                {
                    return CreateNotFoundResponse<TaskManagementMileStoneViewModel>("Không tìm thấy công việc");
                }

                if (model.StartDate < task.StartDate)
                    return CreateBadRequestResponse<TaskManagementMileStoneViewModel>("Ngày bắt đầu của milestone phải lớn hơn hoặc bằng ngày bắt đầu của công việc");

                if (model.DueDate > task.DueDate)
                    return CreateBadRequestResponse<TaskManagementMileStoneViewModel>("Ngày hoàn thành của milestone phải lớn nhỏ hoặc bằng ngày hoàn thành của công việc");

                var entity = _mapper.Map<sm_TaskManagementMileStone>(model);
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;

                _dbContext.sm_TaskManagementMileStone.Update(entity);

                await _dbContext.SaveChangesAsync();

                var result = await GetByIdTaskMileStone(entity.Id, currentUser);

                if (result.IsSuccess)
                    result.Message = "Thêm mới thành công";
                return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return CreateExceptionResponse<TaskManagementMileStoneViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<TaskManagementMileStoneViewModel>>> GetPageTaskMileStone(TaskManagementMileStoneQueryModel query, RequestUser currentUser)
        {
            try
            {
                var predicate = BuildQueryTaskMileStone(query);
                predicate = predicate.And(x => x.TenantId == currentUser.TenantId);
                var queryResult = _dbContext.sm_TaskManagementMileStone.AsNoTracking().Where(predicate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<TaskManagementMileStoneViewModel>>(data);

                return CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return CreateExceptionResponse<Pagination<TaskManagementMileStoneViewModel>>(ex);
            }
        }

        public async Task<Response<TaskManagementMileStoneViewModel>> GetByIdTaskMileStone(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_TaskManagementMileStone.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.TenantId == currentUser.TenantId);

                if (entity == null)
                    return CreateNotFoundResponse<TaskManagementMileStoneViewModel>("MileStone không tồn tại trong hệ thống.");

                var result = _mapper.Map<TaskManagementMileStoneViewModel>(entity);

                return new Response<TaskManagementMileStoneViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return CreateExceptionResponse<TaskManagementMileStoneViewModel>(ex);
            }
        }

        public async Task<Response> DeleteTaskMileStone(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_TaskManagementMileStone.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == currentUser.TenantId);

                if (entity == null)
                    return CreateNotFoundResponse("MileStone không tồn tại trong hệ thống.");

                //var userAssigned = await _dbContext.IdmUser.Where(x => x.Id == entity.UserId && x.TenantId == currentUser.TenantId).FirstOrDefaultAsync();
                //if (userAssigned == null)
                //{
                //    return CreateNotFoundResponse<TaskManagementMileStoneViewModel>("Không tìm thấy người dùng được giao việc");
                //}

                var task = await _dbContext.sm_TaskManagement.Where(x => x.Id == entity.TaskManagementId && x.TenantId == currentUser.TenantId).FirstOrDefaultAsync();
                if (task == null)
                {
                    return CreateNotFoundResponse<TaskManagementMileStoneViewModel>("Không tìm thấy công việc");
                }

                _dbContext.sm_TaskManagementMileStone.Remove(entity);

                await _dbContext.SaveChangesAsync();

                //var history = new TaskManagementHistoryCreateUpdateModel();
                ////history.Action = $"Người dùng {userAssigned.UserName} được gỡ khỏi công việc {task.Title}";
                //history.TaskManagementId = entity.TaskManagementId;

                //var taskHistory = await CreateTaskHistory(history, currentUser);
                //if (!taskHistory.IsSuccess)
                //{
                //    return CreateBadRequestResponse<TaskManagementMileStoneViewModel>(taskHistory.Message);
                //}

                return new Response("Xóa mileStone thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return CreateExceptionResponse(ex);
            }
        }

        private Expression<Func<sm_TaskManagementMileStone, bool>> BuildQueryTaskMileStone(TaskManagementMileStoneQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_TaskManagementMileStone>(true);

            return predicate;
        }
        #endregion
    }
}
