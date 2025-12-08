using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.TemplateStage;
using NSPC.Business.Services.WorkItem;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using System.Linq.Expressions;
namespace NSPC.Business.Services.ProjectTemplate
{
    public class ProjectTemplateHandler : IProjectTemplateHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        public ProjectTemplateHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<Response<ProjectTemplateViewModel>> Create(ProjectTemplateCreateUpdateModel model)
        {
            try
            {
                if (await _dbContext.sm_ProjectTemplate.AnyAsync(x => x.Code == model.Code))
                    return Helper.CreateBadRequestResponse<ProjectTemplateViewModel>($"Mã code '{model.Code}' đã tồn tại!");

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var entity = new sm_ProjectTemplate();
                entity.Id = Guid.NewGuid();
                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                if (model.TemplateStages != null && model.TemplateStages.Any())
                {
                    entity.TemplateStages = model.TemplateStages.Select((stageModel, index) =>
                    {
                        var stage = _mapper.Map<sm_TemplateStage>(stageModel);
                        stage.Id = Guid.NewGuid();
                        stage.StepOrder = index + 1;
                        stage.ProjectTemplateId = entity.Id;
                        stage.CreatedByUserId = currentUser.UserId;
                        stage.CreatedOnDate = DateTime.Now;
                        if (stage?.Tasks.Any() == true)
                        {
                            stage.Tasks = stage.Tasks.Select((taskModel, index) =>
                            {
                                var task = _mapper.Map<sm_Task>(taskModel);
                                task.Id = Guid.NewGuid();
                                task.TemplateStageId = stage.Id;
                                task.StepOrder = index + 1;
                                return task;
                            }).ToList();
                        }
                        return stage;
                    }).ToList();
                }

                _dbContext.sm_ProjectTemplate.Add(entity);
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<ProjectTemplateViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<ProjectTemplateViewModel>(ex);
            }
        }

        public async Task<Response<ProjectTemplateViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_ProjectTemplate.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<ProjectTemplateViewModel>(string.Format("Mẫu dự án này không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<ProjectTemplateViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<ProjectTemplateViewModel>(ex);
            }
        }

        public async Task<Response<ProjectTemplateViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_ProjectTemplate
                    .Include(x => x.TemplateStages
                        .OrderBy(ts => ts.StepOrder))
                    .ThenInclude(x => x.Tasks.OrderBy(t => t.StepOrder))
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<ProjectTemplateViewModel>("Mã mẫu dự án không tồn tại trong hệ thống.");

                var result = _mapper.Map<ProjectTemplateViewModel>(entity);
                return new Response<ProjectTemplateViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<ProjectTemplateViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<ProjectTemplateViewModel>>> GetPage(ProjectTemplateQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_ProjectTemplate
                    .Include(x => x.TemplateStages
                    .OrderBy(ts => ts.StepOrder))
                    .ThenInclude(x => x.Tasks.OrderBy(t => t.StepOrder))
                    .AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<ProjectTemplateViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<ProjectTemplateViewModel>>(ex);
            }
        }
        private Expression<Func<sm_ProjectTemplate, bool>> BuildQuery(ProjectTemplateQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_ProjectTemplate>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()));

            //if (query.IsKhongTaiXe.HasValue && query.IsKhongTaiXe.Value == true)
            //{
            //    predicate.And(s => s.TaiXe == null || s.TaiXe.IdProjectTemplate == query.IdTaiXe);
            //}


            //if (!currentUser.ListRights.Contains("XE." + RightActionConstants.VIEWALL))
            //    predicate.And(s => s.CreatedByUserId == currentUser.UserId);
            return predicate;
        }

        public async Task<Response<ProjectTemplateViewModel>> Update(Guid id, ProjectTemplateCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.sm_ProjectTemplate
                    .Include(x => x.TemplateStages)
                    .ThenInclude(ts => ts.Tasks)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<ProjectTemplateViewModel>("Mẫu dự án không tồn tại trong hệ thống!");

                if (await _dbContext.sm_ProjectTemplate.AnyAsync(x => x.Code == model.Code && x.Id != id))
                    return Helper.CreateBadRequestResponse<ProjectTemplateViewModel>($"Mã code '{model.Code}' đã tồn tại!");

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                entity.Code = model.Code;
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                // Update TemplateStages
                var existingStages = entity.TemplateStages.ToList();
                var modelStages = model.TemplateStages ?? new List<TemplateStageCreateUpdateModel>();
                // Update or add stages
                for (int i = 0; i < modelStages.Count; i++)
                {
                    var stageModel = modelStages[i];
                    sm_TemplateStage stageEntity = null;

                    if (stageModel.Id.HasValue)
                    {
                        stageEntity = existingStages.FirstOrDefault(s => s.Id == stageModel.Id.Value);
                    }

                    if (stageEntity != null)
                    {
                        stageEntity.Name = stageModel.Name;
                        stageEntity.Description = stageModel.Description;
                        stageEntity.StepOrder = i + 1;
                        stageEntity.LastModifiedByUserId = currentUser.UserId;
                        stageEntity.LastModifiedOnDate = DateTime.Now;

                        var existingTasks = stageEntity.Tasks?.ToList() ?? new List<sm_Task>();
                        var modelTasks = stageModel.Tasks ?? new List<TaskCreateUpdateModel>();
                        // Update or add tasks
                        for (int j = 0; j < modelTasks.Count; j++)
                        {
                            var taskModel = modelTasks[j];
                            sm_Task taskEntity = null;
                            if (taskModel.Id.HasValue)
                                taskEntity = existingTasks.FirstOrDefault(t => t.Id == taskModel.Id.Value);
                            if (taskEntity != null)
                            {
                                _mapper.Map(taskModel, taskEntity);
                                taskEntity.StepOrder = j + 1;
                                taskEntity.LastModifiedByUserId = currentUser.UserId;
                                taskEntity.LastModifiedOnDate = DateTime.Now;
                            }
                            else
                            {
                                var newTask = _mapper.Map<sm_Task>(taskModel);
                                newTask.Id = Guid.NewGuid();
                                newTask.TemplateStageId = stageEntity.Id;
                                newTask.StepOrder = j + 1;
                                newTask.CreatedByUserId = currentUser.UserId;
                                newTask.CreatedOnDate = DateTime.Now;
                                stageEntity.Tasks.Add(newTask);
                                _dbContext.Add(newTask);
                            }
                        }
                        var modelTaskIds = modelTasks.Where(t => t.Id.HasValue).Select(t => t.Id.Value).ToList();
                        var tasksToRemove = existingTasks.Where(t => !modelTaskIds.Contains(t.Id)).ToList();
                        foreach (var task in tasksToRemove)
                        {
                            _dbContext.Remove(task);
                        }
                    }
                    else
                    {
                        var newStage = _mapper.Map<sm_TemplateStage>(stageModel);
                        newStage.Id = Guid.NewGuid();
                        newStage.ProjectTemplateId = entity.Id;
                        newStage.StepOrder = i + 1;
                        newStage.CreatedByUserId = currentUser.UserId;
                        newStage.CreatedOnDate = DateTime.Now;
                        newStage.Tasks = new List<sm_Task>();
                        if (stageModel.Tasks != null)
                        {
                            for (int j = 0; j < stageModel.Tasks.Count; j++)
                            {
                                var taskModel = stageModel.Tasks[j];
                                var newTask = _mapper.Map<sm_Task>(taskModel);
                                newTask.Id = Guid.NewGuid();
                                newTask.TemplateStageId = newStage.Id;
                                newTask.StepOrder = j + 1;
                                newTask.CreatedByUserId = currentUser.UserId;
                                newTask.CreatedOnDate = DateTime.Now;
                                newStage.Tasks.Add(newTask);
                                _dbContext.Add(newTask);
                            }
                        }
                        entity.TemplateStages.Add(newStage);
                        _dbContext.Add(newStage);
                    }
                }
                var modelStageIds = modelStages.Where(s => s.Id.HasValue).Select(s => s.Id.Value).ToList();
                var stagesToRemove = existingStages.Where(s => !modelStageIds.Contains(s.Id)).ToList();

                foreach (var stage in stagesToRemove)
                {
                    if (stage.Tasks != null)
                    {
                        foreach (var task in stage.Tasks.ToList())
                        {
                            _dbContext.Remove(task);
                        }
                    }
                    _dbContext.Remove(stage);
                }

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<ProjectTemplateViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<ProjectTemplateViewModel>(ex);
            }
        }
    }
}
