using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.TaskNotification;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Entity;
using SaleManagement.Data.Data.Entity.TaskHistory;
using Serilog;
using System.Linq.Expressions;
using static NSPC.Common.Helper;
namespace NSPC.Business.Services.WorkItem
{
    public class TaskHandler : ITaskHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        private readonly ITaskNotificationHandler _taskNotificationHandler;
        public TaskHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, ITaskNotificationHandler taskNotificationHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _taskNotificationHandler = taskNotificationHandler;
        }
        public async Task<Response<TaskViewModel>> Create(TaskCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                if (model.IdTemplateStage == null || model.IdTemplateStage == Guid.Empty)
                    return Helper.CreateBadRequestResponse<TaskViewModel>("Vui lòng chọn giai đoạn cho công việc!");

                var entity = _mapper.Map<sm_Task>(model);
                entity.Id = Guid.NewGuid();
                entity.Code = await AutoGenerateAdvanceTasksCode("TN-");
                entity.CreatedByUserId = currentUser.UserId;
                entity.Status = TaskStatus.NotStarted;

                // Chuẩn bị danh sách thông báo để gửi sau khi lưu dữ liệu
                var notificationEntities = new List<sm_TaskNotification>();

                if (model.ApproverIds?.Any() == true)
                {
                    entity.Approvers = model.ApproverIds.Select(userId => new sm_TaskApprover
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        UserId = userId
                    }).ToList();
                    foreach (var approver in entity.Approvers)
                    {
                        var notification = new sm_TaskNotification
                        {
                            Id = Guid.NewGuid(),
                            ApprovalType = "Approver",
                            UserId = approver.UserId,
                            TaskId = entity.Id,
                            CreatedByUserName = currentUser.FullName,
                            NotificationStatus = NotificationStatus.Joined,
                            CreatedByUserId = currentUser.UserId,
                        };
                        _dbContext.Add(notification);
                        // Đẩy thông báo sau khi SaveChangesAsync
                        notificationEntities.Add(notification);
                    }
                }
                if (model.ExecutorIds?.Any() == true)
                {
                    entity.Executors = model.ExecutorIds.Select(userId => new sm_TaskExecutor
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        UserId = userId
                    }).ToList();
                    foreach (var executor in entity.Executors)
                    {
                        var notification = new sm_TaskNotification
                        {
                            Id = Guid.NewGuid(),
                            ApprovalType = "Executor",
                            UserId = executor.UserId,
                            TaskId = entity.Id,
                            CreatedByUserName = currentUser.FullName,
                            NotificationStatus = NotificationStatus.Joined,
                            CreatedByUserId = currentUser.UserId,
                        };
                        _dbContext.Add(notification);
                        // Đẩy thông báo sau khi SaveChangesAsync
                        notificationEntities.Add(notification);
                    }
                }
                if (model.SubTasks != null && model.SubTasks.Any())
                {
                    var subTaskEntities = new List<sm_SubTask>();

                    foreach (var subTaskModel in model.SubTasks)
                    {
                        var subTaskEntity = _mapper.Map<sm_SubTask>(subTaskModel);
                        subTaskEntity.Id = Guid.NewGuid();
                        subTaskEntity.TaskId = entity.Id;
                        subTaskEntity.CreatedByUserId = currentUser.UserId;
                        subTaskEntity.CreatedByUserName = currentUser.UserName;
                        subTaskEntities.Add(subTaskEntity);
                        if (subTaskModel.ExecutorIds != null && subTaskModel.ExecutorIds.Any())
                        {
                            foreach (var executorId in subTaskModel.ExecutorIds)
                            {
                                var subTaskExecutor = new sm_SubTaskExecutor
                                {
                                    Id = Guid.NewGuid(),
                                    SubTaskId = subTaskEntity.Id,
                                    UserId = executorId,
                                    CreatedByUserId = currentUser.UserId,
                                    CreatedByUserName = currentUser.UserName,
                                    CreatedOnDate = DateTime.Now
                                };
                                _dbContext.Add(subTaskExecutor);
                            }
                        }
                    }
                    entity.SubTasks = subTaskEntities;
                }
                // Thêm lịch sử tạo công việc
                _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                {
                    Id = Guid.NewGuid(),
                    TaskId = entity.Id,
                    ActivityType = TaskActivityType.CreatedTask,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.FullName,
                });
                _dbContext.sm_Task.Add(entity);
                // Thêm thông báo hết hạn/cảnh báo hết hạn cho công việc nếu đến ngày (người thực hiện và người phê duyệt)
                AddTaskExpiryNotifications(entity);

                // Cập nhật ngày chỉnh sửa cuối cùng cho công trình
                var constructionEntity = await _dbContext.sm_Construction.FirstOrDefaultAsync(x => x.Id == entity.ConstructionId);
                if (constructionEntity != null)
                    constructionEntity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();

                // Gửi thông báo sau khi đã lưu dữ liệu thành công
                foreach (var notification in notificationEntities)
                    await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));

                return Helper.CreateSuccessResponse(_mapper.Map<TaskViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<TaskViewModel>(ex);
            }
        }

        public async Task<Response<TaskViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Task.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<TaskViewModel>(string.Format("Công việc này không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<TaskViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<TaskViewModel>(ex);
            }
        }

        public async Task<Response<TaskViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_Task
                    .Include(x => x.TaskUsageHistories.OrderByDescending(h => h.CreatedOnDate))
                    .Include(x => x.Construction)
                    .Include(x => x.Executors)
                    .ThenInclude(x => x.Idm_User)
                    .Include(x => x.Approvers)
                    .ThenInclude(x => x.Idm_User)
                    .Include(x => x.SubTasks.OrderBy(h => h.CreatedOnDate))
                    .ThenInclude(x => x.SubTaskExecutors)
                    .ThenInclude(x => x.Idm_User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<TaskViewModel>("Mã công việc không tồn tại trong hệ thống.");

                var result = _mapper.Map<TaskViewModel>(entity);
                return new Response<TaskViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<TaskViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<TaskViewModel>>> GetPage(TaskQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_Task
                     .Include(x => x.Construction)
                     .Include(x => x.Executors)
                         .ThenInclude(x => x.Idm_User)
                     .Include(x => x.Approvers)
                         .ThenInclude(x => x.Idm_User)
                     .Include(x => x.SubTasks)
                     .AsNoTracking()
                     .Where(predicate)
                    .OrderBy(x => x.PriorityOrder)
                    .ThenByDescending(x => x.CreatedOnDate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<TaskViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<TaskViewModel>>(ex);
            }
        }
        private Expression<Func<sm_Task, bool>> BuildQuery(TaskQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_Task>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.ConstructionId != Guid.Empty)
                predicate.And(s => s.ConstructionId == query.ConstructionId);

            if (query.IdTemplateStage != Guid.Empty)
                predicate.And(s => s.IdTemplateStage == query.IdTemplateStage);

            if (!string.IsNullOrEmpty(query.Status))
                if (Enum.TryParse<TaskStatus>(query.Status, true, out var statusEnum))
                    predicate.And(s => s.Status == statusEnum);

            if (!string.IsNullOrEmpty(query.PriorityLevel))
                if (Enum.TryParse<PriorityLevel>(query.PriorityLevel, true, out var priorityLevelEnum))
                    predicate.And(s => s.PriorityLevel == priorityLevelEnum);

            if (query.DueDateRange != null && query.DueDateRange.Length > 1)
                predicate.And(x =>
                    x.EndDateTime >= query.DueDateRange[0].Value.Date &&
                    x.EndDateTime <= query.DueDateRange[1].Value.Date
                );

            // Thêm điều kiện lọc theo userId (người thực hiện hoặc phê duyệt)
            if (query.UserId != Guid.Empty)
            {
                predicate.And(t =>
                    t.Executors.Any(e => e.UserId == query.UserId) ||
                    t.Approvers.Any(a => a.UserId == query.UserId)
                );
            }

            // Thêm điều kiện lọc theo userIdList (người thực hiện và phê duyệt đều phải thuộc danh sách này)
            if (query.UserIdList != null && query.UserIdList.Any())
                predicate.And(t =>
                    t.Executors.Any(e => e.UserId.HasValue && query.UserIdList.Contains(e.UserId.Value)) ||
                    t.Approvers.Any(a => a.UserId.HasValue && query.UserIdList.Contains(a.UserId.Value))
                );


            // Thêm filter theo năm truyền lên
            if (query.Year > 0)
                predicate.And(t => t.CreatedOnDate.Year == query.Year);
            return predicate;
        }

        public async Task<Response<TaskViewModel>> Update(Guid id, TaskCreateUpdateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Task
                    .Include(x => x.Approvers)
                    .Include(x => x.Executors)
                    .Include(x => x.SubTasks)
                    .ThenInclude(st => st.SubTaskExecutors)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<TaskViewModel>("Công việc không tồn tại trong hệ thống!");

                if (entity.Status == TaskStatus.PendingApproval || entity.Status == TaskStatus.Passed)
                    return Helper.CreateBadRequestResponse<TaskViewModel>("Không thể chỉnh sửa công việc khi đang ở trạng thái 'Chờ duyệt' hoặc 'Đạt'.");

                var endDateTimeOld = entity.EndDateTime;
                //Thêm lịch sử chỉnh sửa nếu có thay đổi nhugnwx thông tin
                bool isTaskInfoChanged =
                    entity.Name != model.Name ||
                    entity.StartDateTime != model.StartDateTime ||
                    entity.EndDateTime != model.EndDateTime ||
                    entity.PriorityLevel != (Enum.TryParse<PriorityLevel>(model.PriorityLevel, true, out var priorityLevel) ? priorityLevel : PriorityLevel.Low) ||
                    entity.Description != model.Description;

                if (isTaskInfoChanged)
                    _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        ActivityType = TaskActivityType.UpdatedTaskInfo,
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.FullName,
                    });

                // Thêm lịch sử tải file
                var oldAttachments = entity.Attachments ?? new List<jsonb_Attachment>();
                var newAttachments = model.Attachments ?? new List<jsonb_Attachment>();
                var addedAttachmentIds = newAttachments.Where(x => !oldAttachments.Any(a => a.Id == x.Id))
                                                        .Select(a => a.Id.ToString())
                                                        .Where(x => !string.IsNullOrEmpty(x))
                                                        .ToList();
                if (addedAttachmentIds.Any())
                {
                    _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        ActivityType = TaskActivityType.UploadedAttachment,
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.FullName,
                    });
                }

                entity.Name = model.Name;
                entity.StartDateTime = model.StartDateTime;
                entity.EndDateTime = model.EndDateTime;
                entity.PriorityLevel = Enum.TryParse<PriorityLevel>(model.PriorityLevel, true, out var priorityLevelEnum) ? priorityLevelEnum : PriorityLevel.Low;
                entity.PriorityOrder = model.PriorityOrder;
                entity.Attachments = model.Attachments;
                entity.Description = model.Description;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                // ===== Cập nhật người phê duyệt =====
                var modelApproverIds = model.ApproverIds ?? new List<Guid>();
                var existingApprovers = entity.Approvers.ToList();
                var existingApproverUserIds = existingApprovers
                    .Where(a => a.UserId.HasValue)
                    .Select(a => a.UserId.Value)
                    .ToList();

                // 1. Xóa người phê duyệt không còn trong danh sách mới
                var toRemoveApprovers = existingApprovers
                    .Where(a => a.UserId.HasValue && !modelApproverIds.Contains(a.UserId.Value))
                    .ToList();
                foreach (var approver in toRemoveApprovers)
                {
                    var notification = new sm_TaskNotification
                    {
                        Id = Guid.NewGuid(),
                        ApprovalType = "Approver",
                        UserId = approver.UserId,
                        TaskId = entity.Id,
                        CreatedByUserName = currentUser.FullName,
                        NotificationStatus = NotificationStatus.Left,
                        CreatedByUserId = currentUser.UserId,
                    };
                    _dbContext.Add(notification);
                    await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                    _dbContext.Remove(approver);
                }

                // 2. Thêm người phê duyệt mới chưa có trong danh sách hiện tại
                var toAddApproverIds = modelApproverIds
                    .Except(existingApproverUserIds)
                    .ToList();
                foreach (var userId in toAddApproverIds)
                {
                    var newApprover = new sm_TaskApprover
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        UserId = userId,
                        CreatedByUserId = currentUser.UserId,
                        CreatedOnDate = DateTime.Now
                    };
                    entity.Approvers.Add(newApprover);
                    var notification = new sm_TaskNotification
                    {
                        Id = Guid.NewGuid(),
                        ApprovalType = "Approver",
                        UserId = newApprover.UserId,
                        TaskId = entity.Id,
                        CreatedByUserName = currentUser.FullName,
                        NotificationStatus = NotificationStatus.Joined,
                        CreatedByUserId = currentUser.UserId,
                    };
                    _dbContext.Add(notification);
                    await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                    _dbContext.Add(newApprover);
                }

                // 3. Lưu lịch sử nếu thay đổi người phê duyệt
                bool isApproverChanged = !new HashSet<Guid>(modelApproverIds).SetEquals(existingApproverUserIds);
                if (isApproverChanged)
                {
                    _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        ActivityType = TaskActivityType.ChangedApprover,
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.FullName,
                    });
                }

                // ===== Cập nhật người thực hiện =====
                var modelExecutorIds = model.ExecutorIds ?? new List<Guid>();
                var existingExecutors = entity.Executors.ToList();
                var existingExecutorUserIds = existingExecutors
                    .Where(a => a.UserId.HasValue)
                    .Select(a => a.UserId.Value)
                    .ToList();

                // 1. Xóa người thực hiện không còn trong danh sách mới
                var toRemoveExecutors = existingExecutors
                    .Where(a => a.UserId.HasValue && !modelExecutorIds.Contains(a.UserId.Value))
                    .ToList();
                foreach (var executor in toRemoveExecutors)
                {
                    var notification = new sm_TaskNotification
                    {
                        Id = Guid.NewGuid(),
                        ApprovalType = "Executor",
                        UserId = executor.UserId,
                        TaskId = entity.Id,
                        CreatedByUserName = currentUser.FullName,
                        NotificationStatus = NotificationStatus.Left,
                        CreatedByUserId = currentUser.UserId,
                    };
                    _dbContext.Add(notification);
                    await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                    _dbContext.Remove(executor);
                }
                // 2. Thêm người thực hiện mới chưa có trong danh sách hiện tại
                var toAddExecutorIds = modelExecutorIds
                    .Except(existingExecutorUserIds)
                    .ToList();
                foreach (var userId in toAddExecutorIds)
                {
                    var newExecutor = new sm_TaskExecutor
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        UserId = userId,
                        CreatedByUserId = currentUser.UserId,
                    };
                    entity.Executors.Add(newExecutor);
                    var notification = new sm_TaskNotification
                    {
                        Id = Guid.NewGuid(),
                        ApprovalType = "Executor",
                        UserId = newExecutor.UserId,
                        TaskId = entity.Id,
                        CreatedByUserName = currentUser.FullName,
                        NotificationStatus = NotificationStatus.Joined,
                        CreatedByUserId = currentUser.UserId,
                    };
                    _dbContext.Add(notification);
                    await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                    _dbContext.Add(newExecutor);
                }
                // 3. Lưu lịch sử nếu thay đổi người thực hiện
                bool isExecutorChanged = !new HashSet<Guid>(modelExecutorIds).SetEquals(existingExecutorUserIds);
                if (isExecutorChanged)
                {
                    _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        ActivityType = TaskActivityType.ChangedAssignee,
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.FullName,
                    });
                }

                // ===== Cập nhật công việc con (subtask) =====
                var modelSubTasks = model.SubTasks ?? new List<SubTaskCreateUpdateModel>();
                var existingSubTasks = entity.SubTasks.ToList();
                var modelSubTaskIds = modelSubTasks.Where(s => s.Id != Guid.Empty).Select(s => s.Id).ToList();

                // 1. Xóa công việc con đã bị xóa
                var toRemoveSubTasks = existingSubTasks.Where(s => !modelSubTaskIds.Contains(s.Id)).ToList();
                foreach (var subTask in toRemoveSubTasks)
                {
                    _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        ActivityType = TaskActivityType.DeletedSubtask,
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.FullName,
                        NameSubtask = subTask.Name
                    });
                    _dbContext.Remove(subTask);
                }

                bool isUpdateSubTask = false;
                bool isChangedSubtaskAssignee = false;

                // 2. Cập nhật hoặc thêm công việc con
                foreach (var subTaskModel in modelSubTasks)
                {
                    var subTaskEntity = existingSubTasks.FirstOrDefault(s => s.Id == subTaskModel.Id);
                    if (subTaskEntity != null)
                    {
                        // 2.1. Kiểm tra thay đổi thông tin subtask
                        if (subTaskEntity.Name != subTaskModel.Name ||
                            subTaskEntity.DueDate != subTaskModel.DueDate
                            )
                            isUpdateSubTask = true;

                        // 2.2. Lưu lịch sử tải file cho công việc con
                        var oldSubTaskAttachments = subTaskEntity.Attachments ?? new List<jsonb_Attachment>();
                        var newSubTaskAttachments = subTaskModel.Attachments ?? new List<jsonb_Attachment>();
                        var addedSubTaskAttachmentIds = newSubTaskAttachments
                            .Where(x => !oldSubTaskAttachments.Any(a => a.Id == x.Id))
                            .Select(a => a.Id.ToString())
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList();
                        if (addedSubTaskAttachmentIds.Any())
                        {
                            _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                            {
                                Id = Guid.NewGuid(),
                                TaskId = entity.Id,
                                ActivityType = TaskActivityType.UploadedSubtaskAttachment,
                                CreatedByUserId = currentUser.UserId,
                                CreatedByUserName = currentUser.FullName,
                                NameSubtask = subTaskModel.Name
                            });
                        }

                        // 2.3. Lưu lịch sử hoàn thành công việc con
                        if (subTaskEntity.IsCompleted != subTaskModel.IsCompleted)
                        {
                            _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                            {
                                Id = Guid.NewGuid(),
                                TaskId = entity.Id,
                                ActivityType = subTaskModel.IsCompleted ? TaskActivityType.MarkedSubtaskCompleted : TaskActivityType.UnmarkedSubtaskCompleted,
                                CreatedByUserId = currentUser.UserId,
                                CreatedByUserName = currentUser.FullName,
                                NameSubtask = subTaskModel.Name
                            });
                        }
                        subTaskEntity.IsCompleted = subTaskModel.IsCompleted;
                        subTaskEntity.DueDate = subTaskModel.DueDate;
                        subTaskEntity.Name = subTaskModel.Name;
                        subTaskEntity.LastModifiedByUserId = currentUser.UserId;
                        subTaskEntity.LastModifiedByUserName = currentUser.UserName;
                        subTaskEntity.LastModifiedOnDate = DateTime.Now;
                        subTaskEntity.Attachments = subTaskModel.Attachments;

                        // 2.4. Xử lý executor cho subtask
                        var modelSubTaskExecutorIds = subTaskModel.ExecutorIds ?? new List<Guid>();
                        var existingSubTaskExecutors = subTaskEntity.SubTaskExecutors?.ToList() ?? new List<sm_SubTaskExecutor>();
                        var existingSubTaskExecutorUserIds = existingSubTaskExecutors
                            .Where(e => e.UserId.HasValue)
                            .Select(e => e.UserId.Value)
                            .ToList();

                        // Xóa executor không còn trong danh sách mới
                        var toRemoveSubTaskExecutors = existingSubTaskExecutors
                            .Where(e => e.UserId.HasValue && !modelSubTaskExecutorIds.Contains(e.UserId.Value))
                            .ToList();
                        // Thêm executor mới
                        var toAddSubTaskExecutorIds = modelSubTaskExecutorIds
                            .Except(existingSubTaskExecutorUserIds)
                            .ToList();

                        // Kiểm tra thay đổi executor subtask
                        if (toRemoveSubTaskExecutors.Any() || toAddSubTaskExecutorIds.Any())
                            isChangedSubtaskAssignee = true;

                        foreach (var executor in toRemoveSubTaskExecutors)
                            _dbContext.Remove(executor);

                        foreach (var userId in toAddSubTaskExecutorIds)
                        {
                            _dbContext.Add(new sm_SubTaskExecutor
                            {
                                Id = Guid.NewGuid(),
                                SubTaskId = subTaskEntity.Id,
                                UserId = userId,
                                CreatedByUserId = currentUser.UserId,
                                CreatedByUserName = currentUser.UserName,
                                CreatedOnDate = DateTime.Now
                            });
                        }

                        // Lưu lịch sử thay đổi executor subtask
                        if (isChangedSubtaskAssignee)
                        {
                            var historyChangedSubtaskAssignee = new sm_TaskUsageHistory
                            {
                                Id = Guid.NewGuid(),
                                TaskId = entity.Id,
                                ActivityType = TaskActivityType.ChangedSubtaskAssignee,
                                CreatedByUserId = currentUser.UserId,
                                CreatedByUserName = currentUser.FullName,
                                NameSubtask = subTaskModel.Name
                            };
                            _dbContext.sm_TaskUsageHistory.Add(historyChangedSubtaskAssignee);
                            isChangedSubtaskAssignee = false;
                        }
                    }
                    else
                    {
                        isUpdateSubTask = true;
                        // 2.5. Thêm mới công việc con
                        var newSubTask = _mapper.Map<sm_SubTask>(subTaskModel);
                        newSubTask.Id = Guid.NewGuid();
                        newSubTask.TaskId = entity.Id;
                        newSubTask.CreatedByUserId = currentUser.UserId;
                        newSubTask.CreatedByUserName = currentUser.UserName;
                        entity.SubTasks.Add(newSubTask);
                        // Thêm executor cho công việc con mới
                        if (subTaskModel.ExecutorIds != null && subTaskModel.ExecutorIds.Any())
                        {
                            foreach (var executorId in subTaskModel.ExecutorIds)
                            {
                                var subTaskExecutor = new sm_SubTaskExecutor
                                {
                                    Id = Guid.NewGuid(),
                                    SubTaskId = newSubTask.Id,
                                    UserId = executorId,
                                    CreatedByUserId = currentUser.UserId,
                                    CreatedByUserName = currentUser.UserName,
                                    CreatedOnDate = DateTime.Now
                                };
                                _dbContext.Add(subTaskExecutor);
                            }
                        }
                        _dbContext.Add(newSubTask);
                    }
                }

                // 3. Lưu lịch sử nếu có cập nhật(tên,hạn chót) các công việc con
                if (isUpdateSubTask)
                {
                    _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        ActivityType = TaskActivityType.UpdateSubTask,
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.FullName,
                    });
                }
                // Thêm thông báo hết hạn/cảnh báo hết hạn cho công việc nếu EndDateTime có thay đổi (Executors và Approvers)
                if (entity.EndDateTime != null && endDateTimeOld != entity.EndDateTime)
                    AddTaskExpiryNotifications(entity);

                // Cập nhật ngày chỉnh sửa cuối cùng cho công trình
                var constructionEntity = await _dbContext.sm_Construction.FirstOrDefaultAsync(x => x.Id == entity.ConstructionId);
                if (constructionEntity != null)
                    constructionEntity.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();

                string message = isUpdateSubTask
                    ? $"{currentUser.FullName} đã chỉnh sửa thông tin công việc con"
                    : "Chỉnh sửa thành công";

                return Helper.CreateSuccessResponse(_mapper.Map<TaskViewModel>(entity), message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<TaskViewModel>(ex);
            }
        }
        public async Task<string> AutoGenerateAdvanceTasksCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_Task
                    .AsNoTracking()
                    .Where(x => x.Code.Contains(code))
                    .OrderByDescending(x => x.CreatedOnDate)
                    .FirstOrDefaultAsync();

                if (result != null)
                {
                    var currentNum = result.Code.Substring(result.Code.Length - 3, 3);
                    var currentNumInt = int.Parse(currentNum) + 1;
                    var stringResult = "";
                    if (currentNumInt < 10)
                    {
                        stringResult = "00" + currentNumInt;
                    }
                    else if (currentNumInt >= 10 && currentNumInt < 100)
                    {
                        stringResult = "0" + currentNumInt;
                    }
                    else
                    {
                        stringResult = currentNumInt.ToString();
                    }

                    return code + stringResult;
                }
                else
                {
                    return code + "001";
                }
            }
            catch (Exception ex)
            {
                Log.Error("", ex);
                return string.Empty;
            }
        }
        public async Task<Response> DeleteMany(List<Guid> ids)
        {
            try
            {
                var entities = await _dbContext.sm_Task.Where(x => ids.Contains(x.Id)).ToListAsync();
                if (entities == null || entities.Count == 0)
                    return Helper.CreateNotFoundResponse<int>("Không tìm thấy công việc nào để xóa!");

                _dbContext.sm_Task.RemoveRange(entities);
                var affected = await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse($"Đã xóa {entities.Count} công việc thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Ids: {@ids}", ids);
                return Helper.CreateExceptionResponse<int>(ex);
            }
        }
        public async Task<Response<Pagination<TaskViewModel>>> GetPageByConstructionId(Guid constructionId, TaskQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                predicate = predicate.And(x => x.ConstructionId == constructionId);

                var queryResult = _dbContext.sm_Task
                    .Include(x => x.Construction)
                    .Include(x => x.Executors)
                        .ThenInclude(x => x.Idm_User)
                    .Include(x => x.Approvers)
                        .ThenInclude(x => x.Idm_User)
                    .Include(x => x.SubTasks)
                    .AsNoTracking()
                    .Where(predicate)
                   .OrderBy(x => x.PriorityOrder)
                   .ThenByDescending(x => x.CreatedOnDate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<TaskViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: ConstructionId: {@constructionId}, Query: {@query}", constructionId, query);
                return Helper.CreateExceptionResponse<Pagination<TaskViewModel>>(ex);
            }
        }

        public async Task<Response<TaskOverviewEachStage>> GetAnalyzeByEachStage(Guid idTemplateStage, Guid constructionId)
        {
            try
            {
                var newTaskOverview = new TaskOverviewEachStage();
                
                var listQuery = _dbContext.sm_Task
                    .AsNoTracking()
                    .Include(x => x.Construction)
                    .Include(x => x.SubTasks)
                    .Where(x => x.IdTemplateStage == idTemplateStage && x.ConstructionId == constructionId).ToList();
                
                
                newTaskOverview.TotalTask = listQuery.Count();
                newTaskOverview.PercentProcess =
                    listQuery.Count() > 0 ? Math.Round(((double)listQuery.Count(x => x.Status == TaskStatus.Passed) / listQuery.Count()) * 100) : 0;
                newTaskOverview.TotalDoneTask =
                    listQuery.Count(x => x.Status == TaskStatus.Passed);
                newTaskOverview.TotalLateTask = listQuery.Count(x => DateTime.Now.Date > x.EndDateTime);

                return Helper.CreateSuccessResponse(newTaskOverview);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<TaskOverviewEachStage>(ex);
            }
        }

        // Thêm hàm dùng chung cho cập nhật trạng thái
        private async Task UpdateTaskStatusInternal(sm_Task entity, TaskStatus parsedStatus, string status, string description)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            entity.Status = parsedStatus;
            entity.Description = description;
            entity.LastModifiedByUserId = currentUser.UserId;
            entity.LastModifiedByUserName = currentUser.UserName;
            entity.LastModifiedOnDate = DateTime.Now;

            // Cập nhật ngày chỉnh sửa cuối cùng cho công trình
            var constructionEntity = await _dbContext.sm_Construction.FirstOrDefaultAsync(x => x.Id == entity.ConstructionId);
            if (constructionEntity != null)
                constructionEntity.LastModifiedOnDate = DateTime.Now;

            // Nếu chuyển sang trạng thái "Đang thực hiện", lưu lịch sử bắt đầu công việc
            if (status == "InProgress")
            {
                _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                {
                    Id = Guid.NewGuid(),
                    TaskId = entity.Id,
                    ActivityType = TaskActivityType.StartTask,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.FullName,
                });
                // Thêm vào trong if (status == "InProgress") trong phương thức UpdateStatus
                foreach (var executor in entity?.Executors)
                {
                    var notification = new sm_TaskNotification
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        UserId = executor.UserId,
                        ApprovalType = "Executor",
                        NotificationStatus = NotificationStatus.StatusInProgress,
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.FullName
                    };
                    _dbContext.sm_TaskNotification.Add(notification);
                    await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                }
                foreach (var approver in entity?.Approvers)
                {
                    var notification = new sm_TaskNotification
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        UserId = approver.UserId,
                        ApprovalType = "Approver",
                        NotificationStatus = NotificationStatus.StatusInProgress,
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.FullName
                    };
                    _dbContext.sm_TaskNotification.Add(notification);
                    await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                }
            }
            // Nếu chuyển sang trạng thái "Chờ duyệt", lưu lịch sử và gửi thông báo cho người thực hiện
            if (status == "PendingApproval")
            {
                _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                {
                    Id = Guid.NewGuid(),
                    TaskId = entity.Id,
                    ActivityType = TaskActivityType.SubmittedForApproval,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.FullName,
                });

                // Gửi thông báo cho từng người phê duyệt
                foreach (var approver in entity?.Approvers)
                {
                    var notification = new sm_TaskNotification
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        UserId = approver.UserId,
                        NotificationStatus = NotificationStatus.PendingApproval,
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.FullName,
                        AvatarUrl = _dbContext.IdmUser.FirstOrDefault(x => x.Id == currentUser.UserId)?.AvatarUrl
                    };
                    _dbContext.sm_TaskNotification.Add(notification);
                    await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                }
            }
            // Nếu chuyển sang trạng thái "Đạt", lưu lịch sử hoàn thành
            if (status == "Passed")
            {
                _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                {
                    Id = Guid.NewGuid(),
                    TaskId = entity.Id,
                    ActivityType = TaskActivityType.MarkedAsPassed,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.FullName,
                });

                // Thêm thông báo cho từng Executor với StatusPassed
                foreach (var executor in entity?.Executors)
                {
                    var notification = new sm_TaskNotification
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        UserId = executor.UserId,
                        ApprovalType = "Executor",
                        NotificationStatus = NotificationStatus.StatusPassed,
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.FullName,
                        AvatarUrl = _dbContext.IdmUser.FirstOrDefault(x => x.Id == currentUser.UserId)?.AvatarUrl
                    };
                    _dbContext.sm_TaskNotification.Add(notification);
                    await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                }

                // --- Thông báo cho công việc tiếp theo khi công việc liền trước đã hoàn thành ---
                // Tìm công việc tiếp theo (PriorityOrder lớn hơn, gần nhất, cùng ConstructionId và IdTemplateStage, trạng thái khác Passed)
                var nextTask = await _dbContext.sm_Task
                    .Include(t => t.Executors)
                    .Include(t => t.Approvers)
                    .Where(t =>
                        t.ConstructionId == entity.ConstructionId &&
                        t.IdTemplateStage == entity.IdTemplateStage &&
                        t.PriorityOrder > entity.PriorityOrder &&
                        t.Status != TaskStatus.Passed)
                    .OrderBy(t => t.PriorityOrder)
                    .FirstOrDefaultAsync();

                if (nextTask != null)
                {
                    // Gửi thông báo cho Executors của nextTask
                    foreach (var executor in nextTask.Executors)
                    {
                        var notification = new sm_TaskNotification
                        {
                            Id = Guid.NewGuid(),
                            TaskId = nextTask.Id,
                            UserId = executor.UserId,
                            NotificationStatus = NotificationStatus.NextTaskCompletion,
                            CreatedByUserId = currentUser.UserId,
                            CreatedByUserName = currentUser.FullName,
                            AdditionalData = new List<string> { entity.Name }
                        };
                        _dbContext.sm_TaskNotification.Add(notification);
                        await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                    }

                    // Gửi thông báo cho Approvers của nextTask
                    foreach (var approver in nextTask.Approvers)
                    {
                        var notification = new sm_TaskNotification
                        {
                            Id = Guid.NewGuid(),
                            TaskId = nextTask.Id,
                            UserId = approver.UserId,
                            NotificationStatus = NotificationStatus.NextTaskCompletion,
                            CreatedByUserId = currentUser.UserId,
                            CreatedByUserName = currentUser.FullName,
                            AdditionalData = new List<string> { entity.Name }
                        };
                        _dbContext.sm_TaskNotification.Add(notification);
                        await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                    }
                }
            }
            // Nếu chuyển sang trạng thái "Không đạt", lưu lịch sử không hoàn thành
            if (status == "Failed")
            {
                _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                {
                    Id = Guid.NewGuid(),
                    TaskId = entity.Id,
                    ActivityType = TaskActivityType.MarkedAsFailed,
                    Description = entity.Description,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.FullName,
                });

                // Thêm thông báo cho từng Executor với StatusFailed
                foreach (var executor in entity?.Executors)
                {
                    var notification = new sm_TaskNotification
                    {
                        Id = Guid.NewGuid(),
                        TaskId = entity.Id,
                        UserId = executor.UserId,
                        ApprovalType = "Executor",
                        NotificationStatus = NotificationStatus.StatusFailed,
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.FullName,
                        AvatarUrl = _dbContext.IdmUser.FirstOrDefault(x => x.Id == currentUser.UserId)?.AvatarUrl
                    };
                    _dbContext.sm_TaskNotification.Add(notification);
                    await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                }
            }
        }

        public async Task<Response<TaskViewModel>> UpdateStatus(Guid id, string status, string description)
        {
            try
            {
                var entity = await _dbContext.sm_Task.Include(x => x.Approvers).Include(x => x.Executors).FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<TaskViewModel>("Công việc không tồn tại trong hệ thống!");

                if (!Enum.TryParse<TaskStatus>(status, true, out var parsedStatus))
                    return Helper.CreateNotFoundResponse<TaskViewModel>($"Giá trị trạng thái không hợp lệ: {status}");

                await UpdateTaskStatusInternal(entity, parsedStatus, status, description);
                await _dbContext.SaveChangesAsync();
                string message = "Cập nhật trạng thái thành công";
                if (status == "PendingApproval")
                    message = "Đã gửi duyệt công việc thành công.";
                return Helper.CreateSuccessResponse(_mapper.Map<TaskViewModel>(entity), message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Status: {@status}, Description: {@description}", id, status, description);
                return Helper.CreateExceptionResponse<TaskViewModel>(ex);
            }
        }

        public async Task<Response<List<TaskViewModel>>> UpdateStatusMany(List<Guid> ids, string status, string description)
        {
            try
            {
                if (!Enum.TryParse<TaskStatus>(status, true, out var parsedStatus))
                    return Helper.CreateNotFoundResponse<List<TaskViewModel>>($"Gía trị trạng thái không hợp lệ: {status}");

                var entities = await _dbContext.sm_Task
                    .Include(x => x.Approvers)
                    .Include(x => x.Executors)
                    .Where(x => ids.Contains(x.Id))
                    .ToListAsync();

                if (entities == null || entities.Count == 0)
                    return Helper.CreateNotFoundResponse<List<TaskViewModel>>("Không tìm thấy công việc nào để cập nhật!");

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                foreach (var entity in entities)
                {
                    await UpdateTaskStatusInternal(entity, parsedStatus, status, description);
                }

                await _dbContext.SaveChangesAsync();
                string message = "Cập nhật trạng thái thành công";
                if (status == "PendingApproval")
                    message = "Đã gửi duyệt công việc thành công.";

                var result = _mapper.Map<List<TaskViewModel>>(entities);
                return Helper.CreateSuccessResponse(result, message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Ids: {@ids}, Status: {@status}, Description: {@description}", ids, status, description);
                return Helper.CreateExceptionResponse<List<TaskViewModel>>(ex);
            }
        }
        public async Task CheckAndNotifyTasksExpiringSoon()
        {
            var tasks = await _dbContext.sm_Task
                .Include(t => t.Executors)
                .Include(t => t.Approvers)
                .Where(t => t.ConstructionId != null && t.ConstructionId != Guid.Empty && t.EndDateTime.HasValue)
                .ToListAsync();

            foreach (var task in tasks)
                AddTaskExpiryNotifications(task);
            await _dbContext.SaveChangesAsync();
        }
        private void AddTaskExpiryNotifications(sm_Task task)
        {
            var now = DateTime.Now.Date;
            if (task.EndDateTime.HasValue)
            {
                var daysDifference = (now - task.EndDateTime.Value.Date).TotalDays;

                // Xác định trạng thái thông báo dựa trên số ngày
                NotificationStatus? status = daysDifference switch
                {
                    -3 => NotificationStatus.WarningSoonExpire,
                    0 => NotificationStatus.Due,
                    1 => NotificationStatus.Overdue,
                    _ => null
                };

                if (status.HasValue)
                {
                    // Hàm chung để thêm thông báo
                    async void AddNotification(IEnumerable<dynamic> users, string approvalType)
                    {
                        foreach (var user in users)
                        {
                            if (user?.UserId != Guid.Empty)
                            {
                                var notification = new sm_TaskNotification
                                {
                                    Id = Guid.NewGuid(),
                                    TaskId = task.Id,
                                    UserId = user.UserId,
                                    ApprovalType = approvalType,
                                    NotificationStatus = status.Value
                                };
                                _dbContext.sm_TaskNotification.Add(notification);
                                await _taskNotificationHandler.CreatePushNotification(_mapper.Map<TaskNotificationViewModel>(notification));
                            }
                        }
                    }

                    // Thêm thông báo cho Executors và Approvers
                    AddNotification(task?.Executors ?? Enumerable.Empty<dynamic>(), "Executor");
                    AddNotification(task?.Approvers ?? Enumerable.Empty<dynamic>(), "Approver");
                }
            }
        }
        public async Task<Response<int>> GetMaxPriorityOrderByConstructionIdAndTemplateStage(Guid constructionId, Guid idTemplateStage)
        {
            try
            {
                // Lấy giá trị PriorityOrder lớn nhất của các task thuộc dự án (constructionId) và idTemplateStage
                var maxPriorityOrder = 0; // Giá trị mặc định
                if (await _dbContext.sm_Task
                    .AnyAsync(x => x.ConstructionId == constructionId
                                && x.IdTemplateStage == idTemplateStage
                                && x.PriorityOrder.HasValue))
                {
                    maxPriorityOrder = await _dbContext.sm_Task
                        .Where(x => x.ConstructionId == constructionId
                                 && x.IdTemplateStage == idTemplateStage
                                 && x.PriorityOrder.HasValue)
                        .Select(x => x.PriorityOrder.Value)
                        .MaxAsync();
                }
                // Nếu lớn nhất là 100 thì trả về 99
                var result = maxPriorityOrder == 100 ? 99 : maxPriorityOrder;

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: ConstructionId: {@constructionId}, IdTemplateStage: {@idTemplateStage}", constructionId, idTemplateStage);
                return Helper.CreateExceptionResponse<int>(ex);
            }
        }

        public async Task<Response<TaskStatusSummaryViewModel>> GetTaskStatusSummary(TaskQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_Task
                    .AsNoTracking()
                    .Where(predicate);

                var summary = new TaskStatusSummaryViewModel
                {
                    Total = await queryResult.CountAsync(),
                    NotStarted = await queryResult.CountAsync(x => x.Status == TaskStatus.NotStarted),
                    InProgress = await queryResult.CountAsync(x => x.Status == TaskStatus.InProgress),
                    PendingApproval = await queryResult.CountAsync(x => x.Status == TaskStatus.PendingApproval),
                    Failed = await queryResult.CountAsync(x => x.Status == TaskStatus.Failed),
                    Passed = await queryResult.CountAsync(x => x.Status == TaskStatus.Passed)
                };

                return Helper.CreateSuccessResponse(summary);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@query}", query);
                return Helper.CreateExceptionResponse<TaskStatusSummaryViewModel>(ex);
            }
        }

        public async Task<Response<TaskOverviewSummaryViewModel>> GetTaskOverviewSummary(TaskQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_Task
                    .Include(x => x.Construction)
                    .AsNoTracking()
                    .Where(predicate);

                var totalTasks = await queryResult.CountAsync();
                var completedTasks = await queryResult.CountAsync(x => x.Status == TaskStatus.Passed);
                var failedTasks = await queryResult.CountAsync(x => x.Status == TaskStatus.Failed);
                var inProgressTasks = await queryResult.CountAsync(x => x.Status == TaskStatus.InProgress);
                var notStartedTasks = await queryResult.CountAsync(x => x.Status == TaskStatus.NotStarted);
                var overdueTasks = await queryResult.CountAsync(x => x.EndDateTime.HasValue && x.EndDateTime.Value < DateTime.Now && x.Status != TaskStatus.Passed);

                // Dự án
                var projectQuery = queryResult.Where(x => x.ConstructionId != null && x.ConstructionId != Guid.Empty);
                var projectIds = await projectQuery.Select(x => x.ConstructionId.Value).Distinct().ToListAsync();
                var totalProjects = projectIds.Count;

                var projects = await _dbContext.sm_Construction
                    .Where(x => projectIds.Contains(x.Id))
                    .ToListAsync();

                var projectsDesigning = projects.Count(x => x.StatusCode == "IS_DESIGNING");
                var projectsAuthorSupervisor = projects.Count(x => x.StatusCode == "AUTHOR_SUPERVISOR");

                double percent(int part, int total) => total == 0 ? 0 : Math.Round((double)part * 100 / total, 2);

                var summary = new TaskOverviewSummaryViewModel
                {
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    CompletedTasksPercent = percent(completedTasks, totalTasks),
                    OverdueTasks = overdueTasks,
                    OverdueTasksPercent = percent(overdueTasks, totalTasks),
                    FailedTasks = failedTasks,
                    FailedTasksPercent = percent(failedTasks, totalTasks),
                    InProgressTasks = inProgressTasks,
                    InProgressTasksPercent = percent(inProgressTasks, totalTasks),
                    NotStartedTasks = notStartedTasks,
                    NotStartedTasksPercent = percent(notStartedTasks, totalTasks),
                    TotalProjects = totalProjects,
                    ProjectsDesigning = projectsDesigning,
                    ProjectsDesigningPercent = percent(projectsDesigning, totalProjects),
                    ProjectsAuthorSupervisor = projectsAuthorSupervisor,
                    ProjectsAuthorSupervisorPercent = percent(projectsAuthorSupervisor, totalProjects)
                };

                return Helper.CreateSuccessResponse(summary);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@query}", query);
                return Helper.CreateExceptionResponse<TaskOverviewSummaryViewModel>(ex);
            }
        }

    }
}
