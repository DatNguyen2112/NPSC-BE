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
using DocumentFormat.OpenXml.Drawing.Diagrams;
using MongoDB.Driver.Linq;
using NSPC.Business.Services.WorkItem;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public class TaskPersonalHandler : ITaskPersonalHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public TaskPersonalHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<TaskPersonalViewModel>> Create(TaskPersonalCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var entity = _mapper.Map<sm_TaskPersonal>(model);
                entity.Id = Guid.NewGuid();
                entity.Code = await AutoGenerateAdvanceTasksCode("PTN-");
                entity.CreatedByUserId = currentUser.UserId;
                entity.Status = TaskStatus.NotStarted;
                
                #region Fill Item's Line No

                if (model.SubTaskPersonals != null && model.SubTaskPersonals.Count > 0)
                {
                    model.SubTaskPersonals = model.SubTaskPersonals.OrderBy(x => x.LineNo).ToList();
                    for (int i = 0; i < model.SubTaskPersonals.Count; i++)
                        model.SubTaskPersonals[i].LineNo = i + 1;
                }
                #endregion

                if (model.SubTaskPersonals != null && model.SubTaskPersonals.Any())
                {
                    var subTaskPersonalsEntities = new List<sm_SubTaskPersonal>();

                    foreach (var subTaskPersonalModel in model.SubTaskPersonals)
                    {
                        var subTaskPersonalEntity = _mapper.Map<sm_SubTaskPersonal>(subTaskPersonalModel);
                        subTaskPersonalEntity.Id = Guid.NewGuid();
                        subTaskPersonalEntity.TaskPersonalId = entity.Id;
                        subTaskPersonalEntity.CreatedByUserId = currentUser.UserId;
                        subTaskPersonalEntity.CreatedByUserName = currentUser.UserName;
                        subTaskPersonalsEntities.Add(subTaskPersonalEntity);
                    }

                    entity.SubTasksPersonal = subTaskPersonalsEntities;
                }

                _dbContext.sm_TaskPersonal.Add(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<TaskPersonalViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<TaskPersonalViewModel>(ex);
            }
        }
        
        public async Task<string> AutoGenerateAdvanceTasksCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_TaskPersonal
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

        public async Task<Response<TaskPersonalViewModel>> Update(Guid id, TaskPersonalCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var entity =
                    await _dbContext.sm_TaskPersonal
                        .Include(x => x.SubTasksPersonal)
                        .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<TaskPersonalViewModel>("Công việc không tồn tại trong hệ thống!");

                if (entity.Status == TaskStatus.Passed)
                    return Helper.CreateBadRequestResponse<TaskPersonalViewModel>("Không thể chỉnh sửa công việc khi đang ở trạng thái 'Đạt'.");
                
                #region Fill Item's Line No

                if (model.SubTaskPersonals != null && model.SubTaskPersonals.Count > 0)
                {
                    model.SubTaskPersonals = model.SubTaskPersonals.OrderBy(x => x.LineNo).ToList();
                    for (int i = 0; i < model.SubTaskPersonals.Count; i++)
                        model.SubTaskPersonals[i].LineNo = i + 1;
                }
                #endregion
                
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;
                
                entity.Name = model.Name;
                entity.StartDateTime = model.StartDateTime;
                entity.EndDateTime = model.EndDateTime;
                entity.PriorityLevel = Enum.TryParse<PriorityPersonalLevel>(model.PriorityLevel, true, out var priorityLevelEnum) ? priorityLevelEnum : PriorityPersonalLevel.Low;
                entity.Note = model.Note;
                entity.TaskType = model.TaskType;
                
                // Remove Old Sales Order Item -> Re-add
                _dbContext.RemoveRange(entity.SubTasksPersonal);
                entity.SubTasksPersonal = new List<sm_SubTaskPersonal>();

                foreach (var modelItem in model.SubTaskPersonals)
                {
                    var item = _mapper.Map<sm_SubTaskPersonal>(modelItem);
                    entity.SubTasksPersonal.Add(item);

                    item.TaskPersonalId = entity.Id;
                    item.Name = modelItem.Name;
                    item.IsCompleted = modelItem.IsCompleted;
                    item.DueDate = modelItem.DueDate;
                    item.CreatedOnDate = DateTime.Now;
                    item.CreatedByUserName = currentUser.UserName;
                    item.CreatedByUserId = currentUser.UserId;
                }
                
                // B3. Thêm lại item sau khi tính toán
                foreach (var item in entity.SubTasksPersonal)
                {
                    _dbContext.sm_SubTaskPersonal.Add(item);
                }

                _dbContext.sm_TaskPersonal.Update(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<TaskPersonalViewModel>(entity), "Cập nhật thành công");

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<TaskPersonalViewModel>(ex);
            }
        }

        public async Task<Response<TaskPersonalViewModel>> GetById(Guid id)
        {
            try
            {
                var entity =
                    await _dbContext.sm_TaskPersonal
                        .Include(x => x.SubTasksPersonal)
                        .FirstOrDefaultAsync(x => x.Id == id);

                var result = _mapper.Map<TaskPersonalViewModel>(entity);
                return new Response<TaskPersonalViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<TaskPersonalViewModel>(ex);
            }
        }
        
        public async Task<Response<Pagination<TaskPersonalViewModel>>> GetPage(TaskPersonalQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_TaskPersonal
                    .AsNoTracking()
                    .Include(x => x.SubTasksPersonal)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .Where(predicate);
                
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<TaskPersonalViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<TaskPersonalViewModel>>(ex);
            }
        }

        private Expression<Func<sm_TaskPersonal, bool>> BuildQuery(TaskPersonalQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_TaskPersonal>(true);
            
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower()) 
                                   || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()));
            
            if (!string.IsNullOrEmpty(query.PriorityLevel))
                if (Enum.TryParse<PriorityPersonalLevel>(query.PriorityLevel, true, out var priorityLevelEnum))
                    predicate.And(s => s.PriorityLevel == priorityLevelEnum);

            if (query.DueDateRange != null && query.DueDateRange.Length > 1)
                predicate.And(x =>
                    x.EndDateTime >= query.DueDateRange[0].Value.Date &&
                    x.EndDateTime <= query.DueDateRange[1].Value.Date
                );

            if (!string.IsNullOrEmpty(query.TaskType))
            {
                predicate.And(x => x.TaskType == query.TaskType);
            }

            if (!string.IsNullOrEmpty(query.Status))
                if (Enum.TryParse<TaskStatus>(query.Status, true, out var statusEnum))
                    predicate.And(s => s.Status == statusEnum);

            return predicate;
        }


        public async Task<Response<TaskPersonalViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_TaskPersonal
                    .Include(x => x.SubTasksPersonal)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<TaskPersonalViewModel>(string.Format("Công việc này không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<TaskPersonalViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<TaskPersonalViewModel>(ex);
            }
        }

        public async Task<Response> DeleteMany(List<Guid> ids)
        {
            try
            {
                var entities = await _dbContext.sm_TaskPersonal.Where(x => ids.Contains(x.Id)).ToListAsync();
                if (entities == null || entities.Count == 0)
                    return Helper.CreateNotFoundResponse<int>("Không tìm thấy công việc nào để xóa!");

                _dbContext.sm_TaskPersonal.RemoveRange(entities);
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

        private async Task UpdateTaskStatusInternal(sm_TaskPersonal entity, TaskStatus parsedStatus, string status)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            entity.Status = parsedStatus;
            entity.LastModifiedByUserId = currentUser.UserId;
            entity.LastModifiedByUserName = currentUser.UserName;
            entity.LastModifiedOnDate = DateTime.Now;
        }

        public async Task<Response<TaskPersonalViewModel>> UpdateStatus(Guid id, string status)
        {
            try
            {
                var entity = await _dbContext.sm_TaskPersonal
                    .Include(x => x.SubTasksPersonal)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<TaskPersonalViewModel>("Công việc không tồn tại trong hệ thống!");

                if (!Enum.TryParse<TaskStatus>(status, true, out var parsedStatus))
                    return Helper.CreateNotFoundResponse<TaskPersonalViewModel>($"Giá trị trạng thái không hợp lệ: {status}");

                await UpdateTaskStatusInternal(entity, parsedStatus, status);
                await _dbContext.SaveChangesAsync();
                string message = "Cập nhật trạng thái thành công";
                if (status == "PendingApproval")
                    message = "Đã gửi duyệt công việc thành công.";
                return Helper.CreateSuccessResponse(_mapper.Map<TaskPersonalViewModel>(entity), message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<TaskPersonalViewModel>(ex);
            }
        }

        public async Task<Response<List<TaskPersonalViewModel>>> UpdateStatusMany(List<Guid> ids, string status)
        {
            try
            {
                if (!Enum.TryParse<TaskStatus>(status, true, out var parsedStatus))
                    return Helper.CreateNotFoundResponse<List<TaskPersonalViewModel>>($"Gía trị trạng thái không hợp lệ: {status}");

                var entities = await _dbContext.sm_TaskPersonal
                    .Include(x => x.SubTasksPersonal)
                    .Where(x => ids.Contains(x.Id))
                    .ToListAsync();

                if (entities == null || entities.Count == 0)
                    return Helper.CreateNotFoundResponse<List<TaskPersonalViewModel>>("Không tìm thấy công việc nào để cập nhật!");

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                foreach (var entity in entities)
                {
                    await UpdateTaskStatusInternal(entity, parsedStatus, status);
                }

                await _dbContext.SaveChangesAsync();
                string message = "Cập nhật trạng thái thành công";
                if (status == "PendingApproval")
                    message = "Đã gửi duyệt công việc thành công.";

                var result = _mapper.Map<List<TaskPersonalViewModel>>(entities);
                return Helper.CreateSuccessResponse(result, message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Ids: {@ids}, Status: {@status}, Description: {@description}", ids, status);
                return Helper.CreateExceptionResponse<List<TaskPersonalViewModel>>(ex);
            }
        }
        
        public async Task<Response<TaskStatusSummaryViewModel>> GetTaskStatusSummary(TaskPersonalQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_TaskPersonal
                    .AsNoTracking()
                    .Where(predicate);

                var summary = new TaskStatusSummaryViewModel
                {
                    Total = await queryResult.CountAsync(),
                    NotStarted = await queryResult.CountAsync(x => x.Status == TaskStatus.NotStarted),
                    InProgress = await queryResult.CountAsync(x => x.Status == TaskStatus.InProgress),
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
    }
}

