using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using NSPC.Business.Services.ConstructionActitvityLog;
using NSPC.Business.Services.Contract;
using NSPC.Business.Services.ExecutionTeams;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.Contract;
using NSPC.Data.Entity;
using OfficeOpenXml;
using SaleManagement.Data.Data.Entity.TaskHistory;
using Serilog;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.VisualBasic.CompilerServices;
using static NSPC.Common.Helper;
using Utils = NSPC.Common.Utils;

namespace NSPC.Business.Services
{
    public class ConstructionHandler : IConstructionHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        private readonly IConstructionActivityLogHandler _constructionActivityLogHandler;
        private readonly IContractHandler _contractHandler;

        public ConstructionHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper,
            IConstructionActivityLogHandler constructionActivityLogHandler, IContractHandler contractHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
            _constructionActivityLogHandler = constructionActivityLogHandler;
            _contractHandler = contractHandler;
        }

        /// <summary>
        /// Tạo mới công trình/dự án
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<ConstructionViewModel>> Create(ConstructionCreateUpdateModel model,
            RequestUser currentUser)
        {
            try
            {
                var userId = currentUser.UserId;
                var userName = currentUser.FullName;

                if (model.Code != null)
                {
                    if (_dbContext.sm_Construction.Any(x => x.Code == model.Code))
                        return Helper.CreateBadRequestResponse<ConstructionViewModel>(
                            $"Mã dự án ${model.Code} đã tồn tại!");
                }

                List<idm_User> allUser = new List<idm_User>();

                if (model.ExecutionTeams != null && model.ExecutionTeams.Count > 0)
                {
                    // Fill tất cả user
                    var allUserIds = model.ExecutionTeams.Select(x => x.EmployeeId).ToList();
                    allUser = await _dbContext.IdmUser.AsNoTracking()
                        .Where(x => allUserIds.Contains(x.Id))
                        .ToListAsync();
                }

                var entity = _mapper.Map<sm_Construction>(model);

                if (model.Code != null)
                {
                    entity.Code = model.Code;
                }
                else
                {
                    entity.Code = await GetNewCode(ConstructionConstants.PrefixCode.ConstructionCode);
                }

                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.FullName;
                entity.CreatedOnDate = DateTime.Now;
                entity.PriorityName = ConstructionConstants.FetchStatus(model.PriorityCode)?.Name;
                entity.DocumentStatusName = ConstructionConstants.FetchStatus(model.DocumentStatusCode)?.Name;
                entity.ExecutionStatusName = ConstructionConstants.FetchStatus(model.ExecutionStatusCode)?.Name;
                entity.StatusName = ConstructionConstants.FetchStatus(model.StatusCode)?.Name;

                var projectTemplate = await _dbContext.sm_ProjectTemplate
                    .Include(x => x.TemplateStages
                        .OrderBy(ts => ts.StepOrder))
                    .ThenInclude(x => x.Tasks.OrderBy(t => t.StepOrder))
                    .FirstOrDefaultAsync(x => x.Id == model.ConstructionTemplateId);

                if (model.TemplateStages?.Any() == true)
                {
                    model.TemplateStages = model.TemplateStages.OrderBy(x => x.StepOrder).ToList();

                    for (var i = 0; i < model.TemplateStages.Count; i++)
                    {
                        model.TemplateStages[i].StepOrder = i + 1;
                    }

                    entity.TemplateStages = model.TemplateStages
                        .OrderBy(x => x.StepOrder)
                        .Select(stage => new jsonb_TemplateStage
                        {
                            Id = stage.Id, // nếu stage mới
                            StepOrder = stage.StepOrder,
                            Name = stage.Name,
                            Description = stage.Description,
                            ExpiredDate = stage.ExpiredDate
                        })
                        .ToList();
                }
                else if (projectTemplate?.TemplateStages?.Any() == true)
                {
                    entity.TemplateStages = projectTemplate.TemplateStages.OrderBy(x => x.StepOrder)
                        .Select(stage => new jsonb_TemplateStage
                        {
                            Id = stage.Id,
                            StepOrder = stage.StepOrder,
                            Name = stage.Name,
                            Description = stage.Description,
                            ExpiredDate = stage.ExpiredDate,
                        }).ToList();
                    // Add all tasks of the template stages to the database for this construction
                    var tasksToAdd = new List<sm_Task>();
                    foreach (var stage in projectTemplate.TemplateStages)
                    {
                        if (stage.Tasks != null && stage.Tasks.Any())
                        {
                            foreach (var templateTask in stage.Tasks.OrderBy(t => t.StepOrder))
                            {
                                var newTask = new sm_Task
                                {
                                    Id = Guid.NewGuid(),
                                    Code = await AutoGenerateAdvanceTasksCode("TN-"),
                                    Name = templateTask.Name,
                                    Description = templateTask.Description,
                                    StepOrder = templateTask.StepOrder,
                                    PriorityLevel = PriorityLevel.Medium,
                                    Status = TaskStatus.NotStarted,
                                    ConstructionId = entity.Id,
                                    IdTemplateStage = stage.Id,
                                    CreatedByUserId = currentUser.UserId,
                                    CreatedByUserName = currentUser.FullName,
                                };
                                // Thêm lịch sử tạo công việc
                                _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                                {
                                    Id = Guid.NewGuid(),
                                    TaskId = newTask.Id,
                                    ActivityType = TaskActivityType.CreatedTask,
                                    CreatedByUserId = currentUser.UserId,
                                    CreatedByUserName = currentUser.FullName,
                                });
                                tasksToAdd.Add(newTask);
                            }
                        }
                    }

                    if (tasksToAdd.Count > 0)
                        _dbContext.sm_Task.AddRange(tasksToAdd);
                }

                foreach (var item in entity.sm_ExecutionTeams)
                {
                    var user = allUser.FirstOrDefault(x => x.Id == item.EmployeeId);

                    if (user != null)
                    {
                        item.EmployeeName = user.Name;
                        item.MaPhongBan = user.MaPhongBan;
                        item.MaTo = user.MaTo;
                        item.EmployeeAvatarUrl = user.AvatarUrl;
                        item.ConstructionId = entity.Id;
                    }
                }

                _dbContext.sm_Construction.Add(entity);
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động thêm mới công trình dự án vào bảng sm_ConstructionActivityLog

                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = $"đã tạo công trình",
                        CodeLinkDescription = null,
                        OrderId = entity.Id,
                        ConstructionId = entity.Id,
                        ActionType = ConstructionConstants.ActionType.CONSTRUCTION
                    }, currentUser);

                    #endregion
                }

                Log.Information("Thêm mới thành công, Model: {@model}, UserId: {@userId}, UserName: {@userName}",
                    userId, model, userName);
                return Helper.CreateSuccessResponse(_mapper.Map<ConstructionViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<ConstructionViewModel>(ex);
            }
        }

        private async Task<string> GetNewCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_Construction.AsNoTracking().Where(x => x.Code.Contains(code))
                    .OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

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
                Log.Error("123", ex);
                return string.Empty;
            }
        }

        public async Task<Response> CheckOverloadedEmployeesAsync(CheckOverloadModel model)
        {
            try
            {
                bool isOverload = false;
                var list = new List<EmployeeHasOverload>();
                var targetEmployeeIds = new List<Guid>();
                
                if (model.ProjectId.HasValue)
                {
                    var entity = await _dbContext.sm_Construction
                        .Include(x => x.sm_ExecutionTeams)
                        .FirstOrDefaultAsync(x => x.Id == model.ProjectId);

                    if (entity == null)
                        return Helper.CreateBadRequestResponse<OverloadEmployeeModel>("Không tồn tại công trình");

                    var oldEmployeeIds = entity.sm_ExecutionTeams
                        .Select(x => x.EmployeeId)
                        .ToList();

                    var newEmployeeIds = model.PayloadEmployee.ExecutionTeams?
                        .Select(x => x.EmployeeId)
                        .ToList() ?? new List<Guid>();

                    targetEmployeeIds = newEmployeeIds.Except(oldEmployeeIds).ToList();
                }
                else
                {
                    targetEmployeeIds = model.PayloadEmployee.ExecutionTeams?
                        .Select(x => x.EmployeeId)
                        .ToList() ?? new List<Guid>();
                }
                
                if (targetEmployeeIds.Any())
                {
                    var rawData = await _dbContext.sm_ExecutionTeams
                        .Where(pm => targetEmployeeIds.Contains(pm.EmployeeId)
                                     && pm.ConstructionId != model.ProjectId
                                     && pm.sm_Construction.ExecutionStatusCode == "IN_PROGRESS")
                        .Select(pm => new
                        {
                            pm.EmployeeId,
                            EmployeeName = _dbContext.IdmUser
                                .Where(u => u.Id == pm.EmployeeId)
                                .Select(u => u.Name)
                                .FirstOrDefault(),
                            ConstructionId = pm.sm_Construction.Id,
                            ConstructionName = pm.sm_Construction.Name,
                            ConstructionCode = pm.sm_Construction.Code,
                            StatusCode = pm.sm_Construction.StatusCode,
                            StatusName = pm.sm_Construction.StatusName,
                            TaskCount = pm.sm_Construction.Tasks.Count()
                        })
                        .ToListAsync();
                    
                    list = rawData
                        .GroupBy(x => new { x.EmployeeId, x.EmployeeName })
                        .Where(g => g.Select(x => x.ConstructionId).Distinct().Count() >= 2)
                        .Select(g => new EmployeeHasOverload
                        {
                            Name = g.Key.EmployeeName,
                            ListEmployeeHasOverloads = g.Select(x => new ConstructionHasEmployeeOverload
                            {
                                NameConstruction = x.ConstructionName,
                                CodeConstruction = x.ConstructionCode,
                                StatusCodeConstruction = x.StatusCode,
                                StatusNameConstruction = x.StatusName,
                                TotalTaskInConstruction = x.TaskCount
                            }).Distinct().ToList()
                        })
                        .ToList();

                    isOverload = list.Any();
                }

                // 🧩 Trả về kết quả
                var responseModel = new OverloadEmployeeModel
                {
                    IsOverload = isOverload,
                    ListEmployeeHasOverloads = list
                };

                return Helper.CreateSuccessResponse(responseModel);
            }
            catch (Exception ex)
            {
                return Helper.CreateExceptionResponse<OverloadEmployeeModel>(ex);
            }
        }


        /// <summary>
        /// Chi tiết công trình dự án
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<ConstructionViewModel>> GetById(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Construction
                    .Include(x => x.sm_ProjectTemplate)
                    .Include(x => x.sm_Investor)
                    .ThenInclude(x => x.InvestorType)
                    .Include(x => x.sm_Contract)
                    .ThenInclude(x => x.ConsultingService)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Log.Information(
                        $"Không tìm thấy công trình với id {entity.Id}, UserId:  {currentUser.UserId}, userName: {currentUser.FullName}");
                    return Helper.CreateBadRequestResponse<ConstructionViewModel>("Không tìm thấy bản ghi");
                }
                else
                {
                    await _dbContext.Entry(entity)
                        .Collection(x => x.sm_ExecutionTeams)
                        .Query()
                        .LoadAsync();

                    await _dbContext.Entry(entity)
                        .Collection(x => x.sm_ConstructionActivityLog)
                        .Query()
                        .LoadAsync();

                    await _dbContext.Entry(entity)
                        .Collection(x => x.sm_IssueManagements)
                        .Query()
                        .LoadAsync();

                    await _dbContext.Entry(entity)
                        .Collection(x => x.Tasks)
                        .Query()
                        .LoadAsync();
                }

                Log.Information(
                    $"Lấy chi tiết công trình với id {entity.Id}, UserId: {currentUser.UserId}, userName: {currentUser.FullName}");
                return Helper.CreateSuccessResponse<ConstructionViewModel>(_mapper.Map<ConstructionViewModel>(entity));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<ConstructionViewModel>(ex);
            }
        }

        /// <summary>
        /// Cập nhật công trình/dự án
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Response<ConstructionViewModel>> Update(Guid id, ConstructionCreateUpdateModel model,
            RequestUser currentUser)
        {
            try
            {
                var userId = currentUser.UserId;
                var userName = currentUser.FullName;

                var entity = await _dbContext.sm_Construction
                    .Include(x => x.sm_ExecutionTeams)
                    .Include(x => x.sm_ProjectTemplate)
                    .Include(x => x.sm_ConstructionActivityLog)
                    .Include(x => x.sm_Investor)
                    .ThenInclude(x => x.InvestorType)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Log.Information(
                        $"Không tìm thấy công trình với id {entity.Id},  UserId: {currentUser.UserId}, UserName: {currentUser.FullName}");
                    return Helper.CreateBadRequestResponse<ConstructionViewModel>("Không tìm thấy bản ghi");
                }

                List<idm_User> allUser = new List<idm_User>();

                if (model.ExecutionTeams != null && model.ExecutionTeams.Count > 0)
                {
                    // Fill tất cả user
                    var allUserIds = model.ExecutionTeams.Select(x => x.EmployeeId).ToList();
                    allUser = await _dbContext.IdmUser.AsNoTracking()
                        .Where(x => allUserIds.Contains(x.Id))
                        .ToListAsync();
                }

                if (model.Code != null)
                {
                    if (_dbContext.sm_Construction.Any(x => x.Code == model.Code && x.Id != id))
                        return Helper.CreateBadRequestResponse<ConstructionViewModel>(
                            $"Mã dự án {model.Code} đã tồn tại!");
                }

                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;

                if (model.Code != null)
                {
                    entity.Code = model.Code;
                }
                else
                {
                    entity.Code = await GetNewCode(ConstructionConstants.PrefixCode.ConstructionCode);
                }

                entity.Name = model.Name;
                entity.Note = model.Note;
                entity.StatusCode = model.StatusCode;
                entity.StatusName = ConstructionConstants.FetchStatus(model.StatusCode).Name;

                entity.PriorityCode = model.PriorityCode;
                entity.PriorityName = ConstructionConstants.FetchStatus(model.PriorityCode)?.Name;

                entity.DocumentStatusCode = model.DocumentStatusCode;
                entity.DocumentStatusName = ConstructionConstants.FetchStatus(model.DocumentStatusCode)?.Name;

                entity.ExecutionStatusCode = model.ExecutionStatusCode;
                entity.ExecutionStatusName = ConstructionConstants.FetchStatus(model.ExecutionStatusCode)?.Name;

                entity.CompletionByCompany = model.CompletionByCompany;
                entity.CompletionByInvestor = model.CompletionByInvestor;

                entity.OwnerTypeCode = model.OwnerTypeCode;
                entity.InvestorId = model.InvestorId;

                entity.DeliveryDate = model.DeliveryDate;

                entity.VoltageTypeCode = model.VoltageTypeCode;

                // Remove Old Execution Teams -> Re-add
                _dbContext.RemoveRange(entity.sm_ExecutionTeams);
                entity.sm_ExecutionTeams = new List<sm_ExecutionTeams>();

                if (model.ExecutionTeams != null && model.ExecutionTeams.Count > 0)
                {
                    foreach (var item in model.ExecutionTeams)
                    {
                        var executionTeamsEntity = _mapper.Map<sm_ExecutionTeams>(item);
                        entity.sm_ExecutionTeams.Add(executionTeamsEntity);

                        // Fill constructionId
                        executionTeamsEntity.ConstructionId = entity.Id;

                        // Fill info user
                        var user = allUser.FirstOrDefault(x => x.Id == item.EmployeeId);

                        if (user != null)
                        {
                            executionTeamsEntity.EmployeeName = user.Name;
                            executionTeamsEntity.MaPhongBan = user.MaPhongBan;
                            executionTeamsEntity.MaTo = user.MaTo;
                            executionTeamsEntity.EmployeeAvatarUrl = user.AvatarUrl;
                        }
                    }
                }

                // B3. Thêm lại item sau khi process xong
                foreach (var item in entity.sm_ExecutionTeams)
                {
                    _dbContext.sm_ExecutionTeams.Add(item);
                }

                var isTemplateChanged = entity.ConstructionTemplateId != model.ConstructionTemplateId;

                if (isTemplateChanged && model.ConstructionTemplateId.HasValue)
                {
                    // Xóa tất cả task cũ của công trình này
                    var oldTasks = await _dbContext.sm_Task.Where(t => t.ConstructionId == entity.Id).ToListAsync();
                    if (oldTasks.Any())
                        _dbContext.sm_Task.RemoveRange(oldTasks);

                    // Lấy template mới
                    var projectTemplate = await _dbContext.sm_ProjectTemplate
                        .Include(x => x.TemplateStages.OrderBy(ts => ts.StepOrder))
                        .ThenInclude(x => x.Tasks.OrderBy(t => t.StepOrder))
                        .FirstOrDefaultAsync(x => x.Id == model.ConstructionTemplateId);

                    if (model.TemplateStages?.Any() == true)
                    {
                        model.TemplateStages = model.TemplateStages.OrderBy(x => x.StepOrder).ToList();

                        for (var i = 0; i < model.TemplateStages.Count; i++)
                        {
                            model.TemplateStages[i].StepOrder = i + 1;
                        }

                        entity.TemplateStages = model.TemplateStages
                            .OrderBy(x => x.StepOrder)
                            .Select(stage => new jsonb_TemplateStage
                            {
                                Id = stage.Id, // nếu stage mới
                                Name = stage.Name,
                                Description = stage.Description,
                                ExpiredDate = stage.ExpiredDate
                            })
                            .ToList();
                    }
                    else if (projectTemplate?.TemplateStages?.Any() == true)
                    {
                        entity.TemplateStages = projectTemplate.TemplateStages.OrderBy(x => x.StepOrder)
                            .Select(stage => new jsonb_TemplateStage
                            {
                                Id = stage.Id,
                                StepOrder = stage.StepOrder,
                                Name = stage.Name,
                                Description = stage.Description,
                                ExpiredDate = stage.ExpiredDate,
                            }).ToList();
                        // Add all tasks of the template stages to the database for this construction
                        var tasksToAdd = new List<sm_Task>();
                        foreach (var stage in projectTemplate.TemplateStages)
                        {
                            if (stage.Tasks != null && stage.Tasks.Any())
                            {
                                foreach (var templateTask in stage.Tasks.OrderBy(t => t.StepOrder))
                                {
                                    var newTask = new sm_Task
                                    {
                                        Id = Guid.NewGuid(),
                                        Code = await AutoGenerateAdvanceTasksCode("TN-"),
                                        Name = templateTask.Name,
                                        Description = templateTask.Description,
                                        StepOrder = templateTask.StepOrder,
                                        PriorityLevel = PriorityLevel.Medium,
                                        Status = TaskStatus.NotStarted,
                                        ConstructionId = entity.Id,
                                        IdTemplateStage = stage.Id,
                                        CreatedByUserId = currentUser.UserId,
                                        CreatedByUserName = currentUser.FullName,
                                    };
                                    // Thêm lịch sử tạo công việc
                                    _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                                    {
                                        Id = Guid.NewGuid(),
                                        TaskId = newTask.Id,
                                        ActivityType = TaskActivityType.CreatedTask,
                                        CreatedByUserId = currentUser.UserId,
                                        CreatedByUserName = currentUser.FullName,
                                    });
                                    tasksToAdd.Add(newTask);
                                }
                            }
                        }

                        if (tasksToAdd.Count > 0)
                            _dbContext.sm_Task.AddRange(tasksToAdd);
                    }

                    entity.ConstructionTemplateId = model.ConstructionTemplateId;
                }
                else
                {
                    // Lấy template mới
                    var projectTemplate = await _dbContext.sm_ProjectTemplate
                        .Include(x => x.TemplateStages.OrderBy(ts => ts.StepOrder))
                        .ThenInclude(x => x.Tasks.OrderBy(t => t.StepOrder))
                        .FirstOrDefaultAsync(x => x.Id == model.ConstructionTemplateId);

                    if (model.TemplateStages?.Any() == true)
                    {
                        model.TemplateStages = model.TemplateStages.OrderBy(x => x.StepOrder).ToList();

                        for (var i = 0; i < model.TemplateStages.Count; i++)
                        {
                            model.TemplateStages[i].StepOrder = i + 1;
                        }

                        entity.TemplateStages = model.TemplateStages
                            .OrderBy(x => x.StepOrder)
                            .Select(stage => new jsonb_TemplateStage
                            {
                                Id = stage.Id, // nếu stage mới
                                Name = stage.Name,
                                Description = stage.Description,
                                ExpiredDate = stage.ExpiredDate
                            })
                            .ToList();
                    }
                    else if (projectTemplate?.TemplateStages?.Any() == true)
                    {
                        entity.TemplateStages = projectTemplate.TemplateStages.OrderBy(x => x.StepOrder)
                            .Select(stage => new jsonb_TemplateStage
                            {
                                Id = stage.Id,
                                StepOrder = stage.StepOrder,
                                Name = stage.Name,
                                Description = stage.Description,
                                ExpiredDate = stage.ExpiredDate,
                            }).ToList();
                        // Add all tasks of the template stages to the database for this construction
                        var tasksToAdd = new List<sm_Task>();
                        foreach (var stage in projectTemplate.TemplateStages)
                        {
                            if (stage.Tasks != null && stage.Tasks.Any())
                            {
                                foreach (var templateTask in stage.Tasks.OrderBy(t => t.StepOrder))
                                {
                                    var newTask = new sm_Task
                                    {
                                        Id = Guid.NewGuid(),
                                        Code = await AutoGenerateAdvanceTasksCode("TN-"),
                                        Name = templateTask.Name,
                                        Description = templateTask.Description,
                                        StepOrder = templateTask.StepOrder,
                                        PriorityLevel = PriorityLevel.Medium,
                                        Status = TaskStatus.NotStarted,
                                        ConstructionId = entity.Id,
                                        IdTemplateStage = stage.Id,
                                        CreatedByUserId = currentUser.UserId,
                                        CreatedByUserName = currentUser.FullName,
                                    };
                                    // Thêm lịch sử tạo công việc
                                    _dbContext.sm_TaskUsageHistory.Add(new sm_TaskUsageHistory
                                    {
                                        Id = Guid.NewGuid(),
                                        TaskId = newTask.Id,
                                        ActivityType = TaskActivityType.CreatedTask,
                                        CreatedByUserId = currentUser.UserId,
                                        CreatedByUserName = currentUser.FullName,
                                    });
                                    tasksToAdd.Add(newTask);
                                }
                            }
                        }

                        if (tasksToAdd.Count > 0)
                            _dbContext.sm_Task.AddRange(tasksToAdd);
                    }

                    entity.ConstructionTemplateId = model.ConstructionTemplateId;
                }

                _dbContext.sm_Construction.Update(entity);
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động cập nhật thông tin công trình vào bảng sm_ConstructionActivityLog

                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = $"đã cập nhật thông tin công trình",
                        CodeLinkDescription = null,
                        OrderId = entity.Id,
                        ConstructionId = entity.Id,
                        ActionType = ConstructionConstants.ActionType.CONSTRUCTION
                    }, currentUser);

                    #endregion
                }

                Log.Information("Cập nhật thành công, UserId: {@userId}, UserName: {@userName}, Model: {@model}",
                    userId, userName, model);
                return Helper.CreateSuccessResponse<ConstructionViewModel>(_mapper.Map<ConstructionViewModel>(entity),
                    "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<ConstructionViewModel>(ex);
            }
        }

        public async Task<Response<ConstructionViewModel>> Delete(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Construction
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Log.Information(
                        $"Không tìm thấy bản ghi với Id: {entity.Id}, UserId: {currentUser.UserId},  UserName: {currentUser.FullName}");
                    return Helper.CreateBadRequestResponse<ConstructionViewModel>("Không tìm thấy bản ghi");
                }

                _dbContext.sm_Construction.Remove(entity);
                await _dbContext.SaveChangesAsync();

                Log.Information(
                    $"Xoá thành công với Id: {{entity.Id}}, UserId: {currentUser.UserId}, UserName: {currentUser.FullName}");

                return Helper.CreateSuccessResponse<ConstructionViewModel>
                    (_mapper.Map<ConstructionViewModel>(entity), "Xoá thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<ConstructionViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<ConstructionViewModel>>> GetPage(ConstructionQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_Construction.AsNoTracking()
                    .Include(x => x.sm_ExecutionTeams)
                    .Include(x => x.sm_Investor)
                    .ThenInclude(x => x.InvestorType)
                    .Include(x => x.sm_IssueManagements)
                    .Include(x => x.Tasks)
                    .Where(predicate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<ConstructionViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<Pagination<ConstructionViewModel>>(ex);
            }
        }

        /// <summary>
        /// Lấy ra danh sách tổ thực hiện
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Response<Pagination<ExecutionTeamsViewModel>>> GetExecutionTeamsInConstruction(
            ExecutionTeamsQueryModel query)
        {
            try
            {
                var predicate = BuildQueryExecutionTeams(query);
                var queryResult = _dbContext.sm_ExecutionTeams.AsNoTracking().Where(predicate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<ExecutionTeamsViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<Pagination<ExecutionTeamsViewModel>>(ex);
            }
        }

        /// <summary>
        /// Build Query Execution Teams
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private Expression<Func<sm_ExecutionTeams, bool>> BuildQueryExecutionTeams(ExecutionTeamsQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_ExecutionTeams>(true);

            predicate.And(x => x.ConstructionId == query.ConstructionId);

            if (!string.IsNullOrEmpty(query.MaPhongBan))
            {
                predicate.And(x => x.MaPhongBan == query.MaPhongBan);
            }

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date);
            }

            return predicate;
        }

        private Expression<Func<sm_Construction, bool>> BuildQuery(ConstructionQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_Construction>(true);

            if (!string.IsNullOrEmpty(query.PermissionCode) && query.PermissionCode == "VIEWBYDEPARTMENT")
            {
                predicate.And(s => s.sm_ExecutionTeams.Any(x => x.MaPhongBan == currentUser.MaPhongBan));
            }

            if (!string.IsNullOrEmpty(query.PermissionCode) && query.PermissionCode == "VIEWBYTEAM")
            {
                predicate.And(s => s.sm_ExecutionTeams.Any(x => x.MaTo == currentUser.MaTo));
            }

            if (!string.IsNullOrEmpty(query.PermissionCode) && query.PermissionCode == "VIEWBYINDIVIDUAL")
            {
                predicate.And(s => s.sm_ExecutionTeams.Any(x => x.EmployeeId == currentUser.UserId));
            }

            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate = predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower())
                                               || s.Name.ToLower().Contains(query.FullTextSearch.ToLower())
                );

            if (!string.IsNullOrEmpty(query.StatusCode))
            {
                predicate.And(x => x.StatusCode == query.StatusCode);
            }

            if (!string.IsNullOrEmpty(query.ExecutionStatusCode))
            {
                predicate.And(x => x.ExecutionStatusCode == query.ExecutionStatusCode);
            }

            if (!string.IsNullOrEmpty(query.DocumentStatusCode))
            {
                predicate.And(x => x.DocumentStatusCode == query.DocumentStatusCode);
            }

            if (query.DeliveryDate.HasValue)
            {
                predicate.And(x => x.DeliveryDate.Value.Date == query.DeliveryDate.Value);
            }

            if (query.InvestorId.HasValue)
            {
                predicate.And(x => x.InvestorId == query.InvestorId);
            }

            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date);
            }

            // if (!currentUser.ListRights.Contains("CONSTRUCTION.VIEWALL")
            //     || !currentUser.ListRights.Contains("CONSTRUCTION.VIEWBYDEPARTMENT")
            //     || !currentUser.ListRights.Contains("CONSTRUCTION.VIEWBYTEAM")
            //     || !currentUser.ListRights.Contains("CONSTRUCTION.VIEWBYINDIVIDUAL") 
            //     || !currentUser.ListRights.Contains("DASHBOARD.VIEW"))
            // {
            //     predicate.And(x => x.CreatedByUserId ==  currentUser.UserId);
            // }

            // Lọc theo người thực hiện hoặc người phê duyệt công việc (Tasks)
            if (query.TaskUserId.HasValue)
                predicate.And(c => c.Tasks.Any(t =>
                    (t.Executors.Any(e => e.UserId == query.TaskUserId.Value)) ||
                    (t.Approvers.Any(a => a.UserId == query.TaskUserId.Value))
                ));

            return predicate;
        }

        #region Dashboard

        // Thống kê tổng dashboard
        public async Task<Response<ConstructionSummary>> ConstructionSummaryAll(ConstructionQueryModel query)
        {
            try
            {
                var newConstructionSummaryAll = new ConstructionSummary();
                var constructionAnalyzeViewModelData = await ConstructionAnalyzeAllDashboard(query);
                var constructionAnalyzeByPriorityData = await ChartPercentPriorityInConstruction(query);
                var constructionAnalyzeByInvestorData = await ChartPercentInvestorInConstruction(query);
                var chartConstructionQuantityByInvestorData = await ChartConstructionQuantityByInvestor(query);
                var chartTopFiveConstructionHasBigQualityData =
                    await ChartTopFiveConstructionHasBigQuality(new ContractQueryModel());
                var topFiveInvestorHasLowQualityData = await TopFiveInvestorHasLowQuality(query);
                var topCashbookTransactionAnalyzeData = await TopCashbookTransactionAnalyze(new DebtReportQueryModel()
                {
                    GroupBy = "investor",
                });
                var chartTopConstructionHasIssueData = await ChartTopConstructionHasIssue(query);


                newConstructionSummaryAll.ConstructionAnalyzeViewModelData = constructionAnalyzeViewModelData?.Data;
                newConstructionSummaryAll.ListConstructionAnalyzeByInvestorData =
                    constructionAnalyzeByInvestorData?.Data;
                newConstructionSummaryAll.ListConstructionAnalyzeByPriorityData =
                    constructionAnalyzeByPriorityData?.Data;
                newConstructionSummaryAll.ListTopFiveInvestorHasLowQualityData = topFiveInvestorHasLowQualityData?.Data;
                newConstructionSummaryAll.ListTopFiveConstructionHasBigQualityData =
                    chartTopFiveConstructionHasBigQualityData?.Data;
                newConstructionSummaryAll.ListConstructionQuantityByInvestorData =
                    chartConstructionQuantityByInvestorData?.Data;
                newConstructionSummaryAll.ListTopConstructionHasIssueData = chartTopConstructionHasIssueData?.Data;
                newConstructionSummaryAll.ListTopFiveConstructionHasBigDebtData =
                    topCashbookTransactionAnalyzeData?.Data;


                return Helper.CreateSuccessResponse(newConstructionSummaryAll);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Thống kê tổng hợp đồng
        public async Task<Response<ContractSummary>> ContractSummaryAll(AnalyzeContractQueryModel query)
        {
            try
            {
                var newContractSummaryAll = new ContractSummary();
                var chartRevenueContractData = await ChartRevenueContract(query);
                var chartAnalyzeContractAmountData = await ChartAnalyzeContractAmount(query);
                var chartAnalyzeContractApprovePercentData = await ChartAnalyzeContractApprovePercent(query);
                var chartAnalyzeRevenueContractApprovePercentData =
                    await ChartAnalyzeRevenueContractApprovePercent(query);
                var chartAnalyzeByInvestor = await ChartAnalyzeByInvestor(query);

                newContractSummaryAll.AnalyzeByInvestorData = chartAnalyzeByInvestor?.Data;
                newContractSummaryAll.AnalyzeContractAmountData = chartAnalyzeContractAmountData?.Data;
                newContractSummaryAll.AnalyzePercentData = chartAnalyzeContractApprovePercentData?.Data;
                newContractSummaryAll.AnalyzeApprovePercentData = chartAnalyzeRevenueContractApprovePercentData?.Data;
                newContractSummaryAll.AnalyzeRevenueContractData = chartRevenueContractData?.Data;

                return Helper.CreateSuccessResponse(newContractSummaryAll);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Thống kê theo tiêu chí
        public async Task<Response<ConstructionAnalyzeViewModel>> ConstructionAnalyzeAllDashboard(
            ConstructionQueryModel query)
        {
            try
            {
                // List construction
                var listConstruction = await GetPage(query);

                var constructionAnalyzeViewModel = new ConstructionAnalyzeViewModel();

                var constructionAnalyzeByIssue = new ConstructionAnalyzeByIssue();
                var constructionAnalyzeByVoltage = new ConstructionAnalyzeByVoltage();
                var constructionAnalyzeByDocumentStatus = new ConstructionAnalyzeByDocumentStatus();
                var constructionAnalyzeByStatus = new ConstructionAnalyzeByStatus();
                var constructionAnalyzeByConsultService = new ContractAnalyzeByConsultService();

                // Total
                decimal listContractEntity = _dbContext.sm_Contract.ToList().Count;
                decimal listConstructionEntity = listConstruction.Data.TotalElements;

                if (listConstruction.Data.Content.Count() > 0)
                {
                    #region Thống kê dự án có vướng mắc

                    decimal totalConstructionHasIssueQuantity =
                        listConstruction.Data.Content.Where(x => x.IsHasIssue == true).ToList().Count;
                    decimal totalConstructionNotIssueQuantity = listConstruction.Data.Content
                        .Where(x => x.IsHasIssue == false).ToList().Count;
                    constructionAnalyzeByIssue.TotalConstructionQuantity = listConstruction.Data.Content.Count;
                    constructionAnalyzeByIssue.TotalConstructionHasIssueQuantity = totalConstructionHasIssueQuantity;
                    constructionAnalyzeByIssue.TotalConstructionNotIssueQuantity = totalConstructionNotIssueQuantity;
                    constructionAnalyzeByIssue.ConstructionHasIssuePercent = listConstructionEntity > 0
                        ? Math.Round((totalConstructionHasIssueQuantity / listConstructionEntity) * 100, 2)
                        : 0;
                    constructionAnalyzeByIssue.ConstructionNotIssuePercent = listConstructionEntity > 0
                        ? Math.Round((totalConstructionNotIssueQuantity / listConstruction.Data.TotalElements) * 100, 2)
                        : 0;

                    #endregion

                    #region Thống kê dự án theo loại cấp điện áp

                    decimal totalConstructionHas110kV = listConstruction.Data.Content
                        .Where(x => x.VoltageTypeCode
                                    == "VT-0002")
                        .ToList()
                        .Count;
                    decimal totalConstructionHas220kV = listConstruction.Data.Content
                        .Where(x => x.VoltageTypeCode
                                    == "VT-0003")
                        .ToList()
                        .Count;
                    decimal totalConstructionHasMediumVoltage = listConstruction.Data.Content
                        .Where(x => x.VoltageTypeCode
                                    == "VT-0001")
                        .ToList()
                        .Count;

                    constructionAnalyzeByVoltage.TotalConstructionHas110kV = totalConstructionHas110kV;
                    constructionAnalyzeByVoltage.TotalConstructionHas220kV = totalConstructionHas220kV;
                    constructionAnalyzeByVoltage.TotalConstructionHasMediumVoltage = totalConstructionHasMediumVoltage;
                    constructionAnalyzeByVoltage.ConstructionHas110kVPercent = listConstructionEntity > 0
                        ? Math.Round((totalConstructionHas110kV / listConstructionEntity) * 100, 2)
                        : 0;
                    constructionAnalyzeByVoltage.ConstructionHas220kVPercent = listConstructionEntity > 0
                        ? Math.Round((totalConstructionHas220kV / listConstructionEntity) * 100, 2)
                        : 0;
                    constructionAnalyzeByVoltage.ConstructionHasMediumVoltagePercent = listConstructionEntity > 0
                        ? Math.Round((totalConstructionHasMediumVoltage / listConstructionEntity) * 100, 2)
                        : 0;

                    #endregion

                    #region Thống kê dự án theo tình trạng hồ sơ

                    decimal totalConstructionApprove = listConstruction.Data.Content
                        .Where(x => x.DocumentStatusCode == ConstructionConstants.DocumentStatusCode.APPROVED).ToList()
                        .Count;
                    decimal totalConstructionNotApprove = listConstruction.Data.Content
                        .Where(x => x.DocumentStatusCode == ConstructionConstants.DocumentStatusCode.NOT_APPROVE)
                        .ToList().Count;

                    constructionAnalyzeByDocumentStatus.TotalConstructionApproved = totalConstructionApprove;
                    constructionAnalyzeByDocumentStatus.TotalConstructionNotApproved = totalConstructionNotApprove;
                    constructionAnalyzeByDocumentStatus.ConstructionApprovedPercent = listConstructionEntity > 0
                        ? Math.Round((totalConstructionApprove / listConstructionEntity) * 100, 2)
                        : 0;
                    constructionAnalyzeByDocumentStatus.ConstructionNotApprovedPercent = listConstructionEntity > 0
                        ? Math.Round((totalConstructionNotApprove / listConstructionEntity) * 100, 2)
                        : 0;

                    #endregion

                    #region Thống kê dự án theo tình trạng

                    decimal totalConstructionIsDesigning = listConstruction.Data.Content
                        .Where(x => x.StatusCode == ConstructionConstants.StatusCode.IS_DESIGNING)
                        .ToList().Count;
                    decimal totalConstructionSuperviosAuthor = listConstruction.Data.Content
                        .Where(x => x.StatusCode == ConstructionConstants.StatusCode.AUTHOR_SUPERVISOR)
                        .ToList().Count;

                    constructionAnalyzeByStatus.TotalConstructionIsDesigning = totalConstructionIsDesigning;
                    constructionAnalyzeByStatus.TotalConstructionSupervisorAuthor = totalConstructionSuperviosAuthor;
                    constructionAnalyzeByStatus.TotalConstructionIsDesigningPercent = listConstructionEntity > 0
                        ? Math.Round((totalConstructionIsDesigning / listConstructionEntity) * 100, 2)
                        : 0;
                    constructionAnalyzeByStatus.TotalConstructionSupervisorAuthorPercent = listConstructionEntity > 0
                        ? Math.Round((totalConstructionSuperviosAuthor / listConstructionEntity) * 100, 2)
                        : 0;

                    #endregion

                    #region Thống kê hợp đồng theo loại dịch vụ tư vấn

                    decimal totalContractByKSTK = _dbContext.sm_Contract
                        .Where(x => x.ConsultingService.Code == "TV-001").ToList().Count;
                    decimal totalContractByTest = _dbContext.sm_Contract
                        .Where(x => x.ConsultingService.Code == "TV-002").ToList().Count;

                    constructionAnalyzeByConsultService.TotalContractByKSTK = totalContractByKSTK;
                    constructionAnalyzeByConsultService.TotalContractByTest = totalContractByTest;
                    constructionAnalyzeByConsultService.ContractByKSTKPercent = listContractEntity > 0
                        ? Math.Round((totalContractByKSTK / listContractEntity) * 100, 2)
                        : 0;
                    constructionAnalyzeByConsultService.ContractByTestPercent = listContractEntity > 0
                        ? Math.Round((totalContractByTest / listContractEntity) * 100, 2)
                        : 0;

                    #endregion
                }

                constructionAnalyzeViewModel.ConstructionAnalyzeByIssueData = constructionAnalyzeByIssue;
                constructionAnalyzeViewModel.ConstructionAnalyzeByVoltageData = constructionAnalyzeByVoltage;
                constructionAnalyzeViewModel.ConstructionAnalyzeByDocumentStatusData =
                    constructionAnalyzeByDocumentStatus;
                constructionAnalyzeViewModel.ConstructionAnalyzeByStatusData = constructionAnalyzeByStatus;
                constructionAnalyzeViewModel.ConstructionAnalyzeByConsultServiceData =
                    constructionAnalyzeByConsultService;

                return Helper.CreateSuccessResponse(constructionAnalyzeViewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Thống kê tỷ lệ dự án theo mức độ ưu tiên
        public async Task<Response<List<ConstructionAnalyzeByPriorityOrInvestor>>> ChartPercentPriorityInConstruction(
            ConstructionQueryModel query)
        {
            try
            {
                List<ConstructionAnalyzeByPriorityOrInvestor> newConstructionChartByPriority =
                    new List<ConstructionAnalyzeByPriorityOrInvestor>();

                // List construction
                var listConstruction = await GetPage(query);

                if (listConstruction.Data.Content.Count() > 0)
                {
                    var groupByArr = _mapper.Map<List<ConstructionViewModel>>(listConstruction.Data.Content)
                        .GroupBy(x => new { x.PriorityCode, x.PriorityName }).Select(x => new ConstructionViewModel()
                        {
                            PriorityCode = x.FirstOrDefault().PriorityCode,
                            PriorityName = x.FirstOrDefault().PriorityName,
                        });

                    foreach (var item in groupByArr)
                    {
                        newConstructionChartByPriority.Add(new ConstructionAnalyzeByPriorityOrInvestor
                        {
                            Name = item.PriorityName,
                            Value = _dbContext.sm_Construction.Where(x => x.PriorityCode == item.PriorityCode).ToList()
                                .Count,
                        });
                    }
                }

                return Helper.CreateSuccessResponse(newConstructionChartByPriority);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Thống kê tỷ lệ dự án theo chủ đầu tư
        public async Task<Response<List<ConstructionAnalyzeByPriorityOrInvestor>>> ChartPercentInvestorInConstruction(
            ConstructionQueryModel query)
        {
            try
            {
                List<ConstructionAnalyzeByPriorityOrInvestor> newConstructionChartByInvestor =
                    new List<ConstructionAnalyzeByPriorityOrInvestor>();

                // List construction
                var listConstruction = await GetPage(query);

                if (listConstruction.Data.Content.Count() > 0)
                {
                    var groupByArr = _mapper.Map<List<ConstructionViewModel>>(listConstruction.Data.Content)
                        .GroupBy(x => new { x.OwnerTypeCode }).Select(x => new ConstructionViewModel()
                        {
                            OwnerTypeCode = x.FirstOrDefault().OwnerTypeCode,
                            Investor = x.FirstOrDefault().Investor,
                        });

                    foreach (var item in groupByArr)
                    {
                        newConstructionChartByInvestor.Add(new ConstructionAnalyzeByPriorityOrInvestor
                        {
                            Name = item.Investor.InvestorType.Code,
                            Value = _dbContext.sm_Construction.Where(x => x.OwnerTypeCode == item.OwnerTypeCode)
                                .ToList().Count,
                        });
                    }
                }

                return Helper.CreateSuccessResponse(newConstructionChartByInvestor);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Thống kê số lượng dự án theo chủ đầu tư
        public async Task<Response<List<ConstructionAnalyzeByPriorityOrInvestor>>> ChartConstructionQuantityByInvestor(
            ConstructionQueryModel query)
        {
            try
            {
                List<ConstructionAnalyzeByPriorityOrInvestor> newConstructionChartByInvestor =
                    new List<ConstructionAnalyzeByPriorityOrInvestor>();

                var listInvestor = _dbContext.sm_Investor.ToList().OrderBy(x => x.CreatedOnDate);

                foreach (var item in listInvestor)
                {
                    newConstructionChartByInvestor.Add(new ConstructionAnalyzeByPriorityOrInvestor
                    {
                        Name = item.Name,
                        Value = _dbContext.sm_Construction.Where(x => x.InvestorId == item.Id).ToList().Count,
                    });
                }

                return Helper.CreateSuccessResponse(newConstructionChartByInvestor);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Top 5 giá trị nghiệm thu trước VAT lớn nhất
        public async Task<Response<List<TopFiveConstructionHasBigQuality>>> ChartTopFiveConstructionHasBigQuality(
            ContractQueryModel query)
        {
            try
            {
                List<TopFiveConstructionHasBigQuality> topFiveConstructionHasBigQuality =
                    new List<TopFiveConstructionHasBigQuality>();

                // List construction
                var listContract = await _contractHandler.GetPage(query);

                if (listContract.Data.Content.Count() > 0)
                {
                    foreach (var item in listContract.Data.Content)
                    {
                        topFiveConstructionHasBigQuality.Add(new TopFiveConstructionHasBigQuality()
                        {
                            TotalExpectedAmountBeforeVAT = item.AcceptanceValueBeforeVatAmount ?? 0,
                            ConstructionName = item.Construction.Name,
                            TotalHasExportBill = item.PaidAmount ?? 0,
                            TotalRemaining = item.AcceptanceValueBeforeVatAmount - item.PaidAmount ?? 0,
                        });
                    }
                }

                var top5AmountAnalyze = topFiveConstructionHasBigQuality
                    .Take(5).Where(x => x.TotalExpectedAmountBeforeVAT > 0)
                    .OrderByDescending(x => x.TotalExpectedAmountBeforeVAT).ToList();

                return Helper.CreateSuccessResponse(top5AmountAnalyze);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Top 5 dự án có tiến độ chậm nhất
        public async Task<Response<List<TopFiveInvestorHasLowQuality>>> TopFiveInvestorHasLowQuality(
            ConstructionQueryModel query)
        {
            try
            {
                List<TopFiveInvestorHasLowQuality> topFiveInvestorHasLowQuality =
                    new List<TopFiveInvestorHasLowQuality>();

                var listConstruction = await GetPage(query);
                var topFiveConstruction = listConstruction.Data.Content
                    .Where(x => x.Tasks.Any(x => x.EndDateTime != null && DateTime.Now > x.EndDateTime.Date))
                    .Take(5).ToList();

                foreach (var item in topFiveConstruction)
                {
                    var taskEntityList = _dbContext.sm_Task.Where(x => x.ConstructionId == item.Id).ToList();

                    decimal totalTaskQuantity = taskEntityList.Count;
                    decimal totalTaskPassedQuantity =
                        taskEntityList.Where(x => x.Status == TaskStatus.Passed).ToList().Count;
                    decimal totalTaskExpiredQuantity =
                        taskEntityList.Where(x => x.EndDateTime != null && DateTime.Now > x.EndDateTime).ToList().Count;

                    if (taskEntityList.Count > 0)
                    {
                        var newObject = new TopFiveInvestorHasLowQuality()
                        {
                            ConstructionId = item.Id,
                            ConstructionName = item.Name,
                            ConstructionCode = item.Code,
                            ExecutionTeams = item.ExecutionTeams,
                            TotalTemplateStageQuantity = item.TemplateStages.Count,
                            TotalTaskQuantity = totalTaskQuantity,
                            TotalTaskCompletedQuantity = totalTaskPassedQuantity,
                            TotalTaskExpiredDateQuantity = totalTaskExpiredQuantity,
                            StatusName = item.StatusName,
                            StatusCode = item.StatusCode,
                            ConstructionProcess = Math.Ceiling((totalTaskPassedQuantity / totalTaskQuantity) * 100)
                        };

                        topFiveInvestorHasLowQuality.Add(newObject);
                    }
                }

                // Check phần tử trước và phần tử sau bằng nhau thì swap (dùng thuật toán bubble sort)
                for (int i = 0; i < topFiveInvestorHasLowQuality.Count - 1; i++)
                {
                    for (int j = 0; j < topFiveInvestorHasLowQuality.Count - i - 1; j++)
                    {
                        var current = topFiveInvestorHasLowQuality[j];
                        var next = topFiveInvestorHasLowQuality[j + 1];

                        if (current.TotalTaskExpiredDateQuantity < next.TotalTaskExpiredDateQuantity)
                        {
                            topFiveInvestorHasLowQuality[j] = next;
                            topFiveInvestorHasLowQuality[j + 1] = current;
                        }
                        else if (current.TotalTaskExpiredDateQuantity == next.TotalTaskExpiredDateQuantity &&
                                 current.ConstructionProcess > next.ConstructionProcess)
                        {
                            topFiveInvestorHasLowQuality[j] = next;
                            topFiveInvestorHasLowQuality[j + 1] = current;
                        }
                    }
                }


                return Helper.CreateSuccessResponse(
                    topFiveInvestorHasLowQuality.ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Top khách hàng chủ đầu tư theo công nợ
        public async Task<Response<List<TopAmountAnalyze>>> TopCashbookTransactionAnalyze(DebtReportQueryModel query)
        {
            try
            {
                List<TopAmountAnalyze> topAmountAnalyze = new List<TopAmountAnalyze>();

                var listInvestor = await _contractHandler.GetPageDebtReport(query);

                if (listInvestor.Data.Content.Count() > 0)
                {
                    foreach (var item in listInvestor.Data.Content)
                    {
                        topAmountAnalyze.Add(new TopAmountAnalyze()
                        {
                            Title = item.Name,
                            Amount = item.AcceptanceValueBeforeVatAmount - item.PaidAmount ?? 0
                        });
                    }
                }

                return Helper.CreateSuccessResponse(topAmountAnalyze
                    .Where(x => x.Amount > 0)
                    .OrderByDescending(x => x.Amount).ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Top dự án có vướng mắc chưa xử lý
        public async Task<Response<List<TopConstructionHasIssue>>> ChartTopConstructionHasIssue(
            ConstructionQueryModel query)
        {
            try
            {
                List<TopConstructionHasIssue> topFiveConstructionHasIssue = new List<TopConstructionHasIssue>();

                // List construction
                var listConstruction = await GetPage(query);

                if (listConstruction.Data.Content.Count() > 0)
                {
                    foreach (var item in listConstruction.Data.Content
                                 .Where(x =>
                                     x.IssueManagements.Any(x => x.Status == StatusIssue.WAIT_PROCESSING)))
                    {
                        topFiveConstructionHasIssue.Add(new TopConstructionHasIssue()
                        {
                            ConstructionName = item.Name,
                            IssueManagements = item.IssueManagements,
                            TotalIssuePending = item.IssueManagements
                                .Where(x => x.Status == StatusIssue.WAIT_PROCESSING).ToList().Count,
                            TotalIssueExpired = item.IssueManagements
                                .Where(x => x.ExpiryDate != null && DateTime.Now > x.ExpiryDate.Value).ToList().Count,
                            ConstructionId = item.Id,
                        });
                    }
                }

                var top5AmountAnalyze =
                    topFiveConstructionHasIssue.OrderByDescending(x => x.TotalIssuePending).ToList();

                return Helper.CreateSuccessResponse(top5AmountAnalyze);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Thống kê hợp đồng theo nhiều tiêu chí
        public async Task<Response<AnalyzeRevenueContract>> ChartRevenueContract(AnalyzeContractQueryModel query)
        {
            try
            {
                var predicate = AnalyzeContractBuildQuery(query);
                var newAnalyzeContractAmount = new AnalyzeRevenueContract();

                var listContractEntity = _dbContext.sm_Contract.AsNoTracking().Where(predicate);

                if (listContractEntity.ToList().Count > 0)
                {
                    newAnalyzeContractAmount.TotalContractQuantity = listContractEntity.ToList().Count;
                    newAnalyzeContractAmount.TotalExpectedAmountBeforeVAT =
                        listContractEntity.ToList().Sum(x => x.ValueBeforeVatAmount ?? 0);
                    newAnalyzeContractAmount.TotalAmountHasExportBillOrder =
                        listContractEntity.ToList().Sum(x => x.PaidAmount ?? 0);
                    newAnalyzeContractAmount.TotalExpectedAmount =
                        listContractEntity.ToList().Sum(x => x.ExpectedVolume ?? 0);
                    newAnalyzeContractAmount.TotalReceiptAmount =
                        listContractEntity.ToList().Sum(x => x.AcceptanceValueBeforeVatAmount ?? 0);
                    newAnalyzeContractAmount.TotalRemainingAmount =
                        listContractEntity.ToList().Sum(x => x.AcceptanceValueBeforeVatAmount ?? 0)
                        - listContractEntity.ToList().Sum(x => x.PaidAmount ?? 0);
                }

                return Helper.CreateSuccessResponse(newAnalyzeContractAmount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        private Expression<Func<sm_Contract, bool>> AnalyzeContractBuildQuery(AnalyzeContractQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_Contract>(true);

            if (query.InvestorCodes != null && query.InvestorCodes.Count() > 0)
                predicate.And(s => query.InvestorCodes.Contains(s.Construction.sm_Investor.Code));

            if (query.DeliveryDateArr != null && query.DeliveryDateArr.Count() > 0)
            {
                predicate.And(s => query.DeliveryDateArr.Contains(s.AssignmentAYear));
            }

            return predicate;
        }

        // Số lượng và giá trị hợp đồng (trước VAT) theo chủ đầu tư
        public async Task<Response<List<AnalyzeContractAmount>>> ChartAnalyzeContractAmount(
            AnalyzeContractQueryModel query)
        {
            try
            {
                var predicate = AnalyzeContractBuildQuery(query);
                var newAnalyzeContractAmount = new List<AnalyzeContractAmount>();

                var listContractEntity = _dbContext.sm_Contract.AsNoTracking()
                    .Include(x => x.Construction)
                    .ThenInclude(x => x.sm_Investor)
                    .Where(predicate);

                var listInvestorEntity = _dbContext.sm_Investor.ToList().OrderBy(x => x.CreatedOnDate);

                foreach (var item in listInvestorEntity)
                {
                    newAnalyzeContractAmount.Add(new AnalyzeContractAmount()
                    {
                        InvestorName = item.Name,
                        ContractQuantity = listContractEntity.Where(x => x.Construction.sm_Investor.Code == item.Code)
                            .ToList().Count,
                        ContractAmount =
                            listContractEntity.Where(x => x.Construction.sm_Investor.Code == item.Code)
                                .Sum(x => x.ValueBeforeVatAmount) ?? 0
                    });
                }

                return Helper.CreateSuccessResponse(newAnalyzeContractAmount);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Tỉ lệ số lượng hợp đồng đã phê duyệt
        public async Task<Response<List<AnalyzePercent>>> ChartAnalyzeContractApprovePercent(
            AnalyzeContractQueryModel query)
        {
            try
            {
                var predicate = AnalyzeContractBuildQuery(query);
                var newAnalyzePercent = new List<AnalyzePercent>();

                var listContractEntity = _dbContext.sm_Contract.AsNoTracking()
                    .Include(x => x.Construction)
                    .ThenInclude(x => x.sm_Investor)
                    .Where(predicate);

                decimal totalContractEntity = listContractEntity
                    .Where(x => x.ImplementationStatus == ImplementationStatus.Approved).ToList().Count;

                if (totalContractEntity > 0)
                {
                    newAnalyzePercent.Add(new AnalyzePercent()
                    {
                        Name = "Đã nghiệm thu",
                        Value = Math.Round(((listContractEntity
                                                .Where(x => x.ImplementationStatus == ImplementationStatus.Approved &&
                                                            x.AcceptanceValueBeforeVatAmount > 0).ToList().Count) /
                                            totalContractEntity) *
                                           100, 2),
                    });

                    newAnalyzePercent.Add(new AnalyzePercent()
                    {
                        Name = "Chưa nghiệm thu",
                        Value = 100 - Math.Round(((listContractEntity
                                                      .Where(x =>
                                                          x.ImplementationStatus == ImplementationStatus.Approved &&
                                                          x.AcceptanceValueBeforeVatAmount > 0).ToList().Count) /
                                                  totalContractEntity) *
                                                 100, 2),
                    });


                    return Helper.CreateSuccessResponse(newAnalyzePercent);
                }
                else
                {
                    return Helper.CreateSuccessResponse(new List<AnalyzePercent>());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Tỷ lệ giá trị hợp đồng (trước VAT) đã phê duyệt
        public async Task<Response<List<AnalyzePercent>>> ChartAnalyzeRevenueContractApprovePercent(
            AnalyzeContractQueryModel query)
        {
            try
            {
                var predicate = AnalyzeContractBuildQuery(query);
                var newAnalyzePercent = new List<AnalyzePercent>();

                var listContractEntity = _dbContext.sm_Contract.AsNoTracking()
                    .Where(predicate);

                decimal totalContractEntity = listContractEntity
                    .Where(x => x.ImplementationStatus == ImplementationStatus.Approved).ToList()
                    .Sum(x => x.ValueBeforeVatAmount) ?? 0;
                decimal totalContractHasPayment = listContractEntity.ToList()
                    .Where(x => x.ImplementationStatus == ImplementationStatus.Approved)
                    .Sum(x => x.AcceptanceValueBeforeVatAmount) ?? 0;

                if (totalContractEntity > 0)
                {
                    newAnalyzePercent.Add(new AnalyzePercent()
                    {
                        Name = "Đã nghiệm thu",
                        Value = Math.Round((totalContractHasPayment / totalContractEntity) * 100, 2)
                    });

                    newAnalyzePercent.Add(new AnalyzePercent()
                    {
                        Name = "Chưa nghiệm thu",
                        Value = 100 - (Math.Round((totalContractHasPayment / totalContractEntity) * 100, 2))
                    });

                    return Helper.CreateSuccessResponse(newAnalyzePercent);
                }
                else
                {
                    return Helper.CreateSuccessResponse(new List<AnalyzePercent>());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        // Tổng sản lượng dự kiến và giá trị nghiệm thu theo chủ đầu tư
        public async Task<Response<List<AnalyzeByInvestor>>> ChartAnalyzeByInvestor(AnalyzeContractQueryModel query)
        {
            try
            {
                var predicate = AnalyzeContractBuildQuery(query);
                var newAnalyzeByInvestor = new List<AnalyzeByInvestor>();

                var listContractEntity = _dbContext.sm_Contract.AsNoTracking()
                    .Include(x => x.Construction)
                    .ThenInclude(x => x.sm_Investor)
                    .Where(predicate);

                var listInvestorEntity = _dbContext.sm_Investor.ToList().OrderBy(x => x.CreatedOnDate);

                if (listContractEntity.ToList().Count > 0)
                {
                    foreach (var item in listInvestorEntity)
                    {
                        newAnalyzeByInvestor.Add(new AnalyzeByInvestor()
                        {
                            InvestorName = item.Name,
                            ExpectedQuantity = listContractEntity
                                                   .Where(x => x.Construction.sm_Investor.Code == item.Code)
                                                   .Sum(x => x.ExpectedVolume) ??
                                               0,
                            ReceivedAmount = listContractEntity.Where(x => x.Construction.sm_Investor.Code == item.Code)
                                .Sum(x => x.AcceptanceValueBeforeVatAmount) ?? 0
                        });
                    }

                    return Helper.CreateSuccessResponse(newAnalyzeByInvestor);
                }
                else
                {
                    return Helper.CreateSuccessResponse(new List<AnalyzeByInvestor>());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        #endregion

        public async Task<(Response, Stream, string)> ExportExcelFile(ConstructionQueryModel model)
        {
            try
            {
                // Tạo predicate để lọc dựa trên các tham số trong query
                var predicate = BuildQuery(model);

                var page = model.Page ?? 1;
                var size = model.Size ?? 20;


                // Lấy danh sách công trình từ cơ sở dữ liệu dựa trên lọc và phân trang
                var query = _dbContext.sm_Construction.AsNoTracking()
                    .Include(x => x.sm_ExecutionTeams)
                    .Include(x => x.sm_ProjectTemplate)
                    .Include(x => x.sm_ConstructionActivityLog)
                    .Include(x => x.sm_Investor)
                    .ThenInclude(x => x.InvestorType)
                    .Where(predicate);

                if (size <= 0)
                {
                    page = 1;
                    size = 20;
                }

                query = query
                    .OrderByDescending(x => x.CreatedOnDate)
                    .Skip((page - 1) * size)
                    .Take(size);

                var constructions = await query.ToListAsync();

                var templateFilePath = Utils.CombineUnixPath(
                    ConfigCollection.Instance.StaticFiles_Folder,
                    "excel-template",
                    "ConstructionTemplate.xlsx");

                if (!File.Exists(templateFilePath))
                {
                    Log.Error("Construction Template Not Found: {Path}", templateFilePath);
                    var templateNotFoundResponse = Helper.CreateExceptionResponse("Không tìm thấy file template");

                    Log.Information(
                        "Construction.{MethodName} failed - template not found. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                        nameof(ExportExcelFile), query, templateNotFoundResponse.IsSuccess,
                        templateNotFoundResponse.Message);

                    return (templateNotFoundResponse, null, null);
                }

                Log.Information("Construction Template Path: {Path}", templateFilePath);

                ExcelPackage.License.SetNonCommercialPersonal("Geneat Hardware");
                using var package = new ExcelPackage(new FileInfo(templateFilePath));
                var worksheet = package.Workbook.Worksheets[0];
                var table = worksheet.Tables[0];

                if (constructions.Count > 1)
                {
                    table.AddRow(constructions.Count - 1);
                }

                for (var i = 0; i < constructions.Count; i++)
                {
                    var construction = constructions[i];
                    var row = table.DataRows[i];

                    row.SetValue(0, i + 1);
                    row.SetValue(1, construction.StatusName ?? null);
                    row.SetValue(2, construction.Code != null ? construction.Code : null);
                    row.SetValue(3, construction.Name);
                    row.SetValue(4,
                        CodeTypeCollection.Instance.FetchCode(construction.VoltageTypeCode, LanguageConstants.Default,
                            construction.TenantId)?.Title ?? null);
                    row.SetValue(5, construction.PriorityName ?? null);
                    row.SetValue(6, construction.sm_Investor.Name ?? null);
                    row.SetValue(7, construction.sm_Investor.InvestorType?.Name ?? null);

                    List<string> executionParticipantTeams = new List<string>();
                    List<string> executionFollowerTeams = new List<string>();
                    List<string> executionProjectTeams = new List<string>();

                    if (construction.sm_ExecutionTeams != null && construction.sm_ExecutionTeams.Count > 0)
                    {
                        executionParticipantTeams = construction.sm_ExecutionTeams
                            .Where(x => x.UserType == "participants")
                            .Select(x => x.EmployeeName).Distinct().ToList();
                        executionFollowerTeams = construction.sm_ExecutionTeams.Where(x => x.UserType == "follower")
                            .Select(x => x.EmployeeName).Distinct().ToList();

                        var executionTeams = await GetExecutionTeamsInConstruction(new ExecutionTeamsQueryModel()
                        {
                            ConstructionId = construction.Id
                        });

                        if (executionTeams.Data.Content != null && executionTeams.Data.Content.Count > 0)
                        {
                            executionProjectTeams = executionTeams.Data.Content
                                .Where(x => x.ToThucHien != null)
                                .Select(x => x.ToThucHien?.Title).Distinct().ToList();
                        }
                    }

                    row.SetValue(8,
                        construction.DeliveryDate != null ? construction.DeliveryDate?.ToString("dd/MM/yyyy") : null);
                    row.SetValue(9, executionProjectTeams.Count > 0 ? string.Join(", ", executionProjectTeams) : null);
                    row.SetValue(10,
                        executionFollowerTeams.Count > 0 ? string.Join(", ", executionFollowerTeams) : null);
                    row.SetValue(11,
                        executionParticipantTeams.Count > 0 ? string.Join(", ", executionParticipantTeams) : null);
                    row.SetValue(12, construction.CompletionByInvestor ?? null);
                    row.SetValue(13, construction.CompletionByCompany ?? null);
                    row.SetValue(14, construction.DocumentStatusName ?? null);
                }

                // Sắp xếp slicer theo bố cục trong hình
                var slicer1 = table.Columns[1].AddSlicer(); // StatusName
                slicer1.ColumnCount = 4;
                slicer1.SetPosition(0, 0, 0, 0);
                slicer1.SetSize(300, 123);

                var slicer2 = table.Columns[5].AddSlicer(); // PriorityName
                slicer2.ColumnCount = 4;
                slicer2.SetPosition(0, 300);
                slicer2.SetSize(300, 123);

                var slicer3 = table.Columns[14].AddSlicer(); // DocumentStatusName
                slicer3.ColumnCount = 4;
                slicer3.SetPosition(0, 600);
                slicer3.SetSize(300, 123);

                var slicer4 = table.Columns[7].AddSlicer(); // InvestorType Name
                slicer4.ColumnCount = 4;
                slicer4.SetPosition(0, 900);
                slicer4.SetSize(300, 123);

                var slicer5 = table.Columns[6].AddSlicer(); // Investor Name
                slicer5.ColumnCount = 6;
                slicer5.SetPosition(0, 1200);
                slicer5.SetSize(461, 116);

                const string fileName = "Danh sách công trình/dự án.xlsx";
                var outputStream = new MemoryStream();
                await package.SaveAsAsync(outputStream);
                outputStream.Position = 0;

                Log.Information(
                    "ConstructionHandler.{MethodName} succeeded. Input: {Input}, Result: Success, Message: Excel file generated successfully",
                    nameof(ExportExcelFile), query);

                return (null, outputStream, fileName);
            }
            catch (Exception e)
            {
                Log.Error(e, "Lỗi khi tạo file Excel cho công trình");
                var errorResponse = Helper.CreateExceptionResponse(e);

                Log.Information(
                    "ConstructionHandler.{MethodName} failed with exception. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(ExportExcelFile), model, errorResponse.IsSuccess, errorResponse.Message);

                return (errorResponse, null, null);
            }
        }

        // /// <summary>
        // /// Hàm xuất file danh sách đơn nhập hàng excel theo query
        // /// </summary>
        // /// <param name="query"></param>
        // /// <returns></returns>
        // /// <exception cref="NotImplementedException"></exception>
        // public async Task<Response<string>> ExportListToExcel(ConstructionQueryModel query, RequestUser currentUser)
        // {
        //     try
        //     {
        //         // Tạo predicate để lọc dựa trên các tham số trong query
        //         var predicate = BuildQuery(query);
        //
        //         // Lấy danh sách công trình từ cơ sở dữ liệu dựa trên lọc và phân trang
        //         var constructionEntity = await _dbContext.sm_Construction.AsNoTracking()
        //             .Include(x => x.sm_ExecutionTeams)
        //             .Include(x => x.sm_ProjectTemplate)
        //             .Include(x => x.sm_ConstructionActivityLog)
        //             .Include(x => x.sm_Investor)
        //             .ThenInclude(x => x.InvestorType)
        //             .Where(predicate)
        //             .OrderByDescending(x => x.CreatedOnDate)
        //             .GetPageAsync(query);
        //
        //         // Kiểm tra nếu không có dữ liệu trả về trong trang hiện tại
        //         if (constructionEntity == null || constructionEntity.Content == null ||
        //             constructionEntity.Content.Count == 0)
        //             return Helper.CreateNotFoundResponse<string>("Không có công trình nào tồn tại trong hệ thống.");
        //
        //         // Đặt tên file và đường dẫn template
        //         var fileName = $"Danh sách công trình_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}.xlsx";
        //         var filePath = Path.Combine(_staticsFolder, fileName);
        //         var templatePath = Path.Combine(_staticsFolder, "excel-template/ConstructionTemplate.xlsx");
        //
        //         if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
        //             return Helper.CreateBadRequestResponse<string>("Không tìm thấy file template");
        //
        //         ExcelPackage.License.SetNonCommercialPersonal("Geneat Hardware");
        //
        //         // Mở template Excel và điền dữ liệu vào file
        //         using (var package = new ExcelPackage(new FileInfo(templatePath)))
        //         {
        //             var worksheet = package.Workbook.Worksheets[0]; // Sử dụng worksheet đầu tiên
        //             
        //             // Đặt độ rộng cố định cho cột A (cột STT) là 8
        //             worksheet.Column(1).Width = 8;
        //
        //             // Điền dữ liệu vào bảng Excel (giả sử bắt đầu từ hàng thứ 4)
        //             int startRow = 5;
        //             int index = 1;
        //
        //             foreach (var order in constructionEntity.Content)
        //             {
        //                 worksheet.Cells[startRow, 1].Value = index; // STT
        //                 worksheet.Cells[startRow, 2].Value = order.StatusName ?? null; // Tình trạng dự án
        //                 worksheet.Cells[startRow, 3].Value = order.Code != null ? order.Code : null; // Mã công trình
        //                 worksheet.Cells[startRow, 4].Value = $"{order.Name}"; // Tên công trình
        //                 worksheet.Cells[startRow, 5].Value =  CodeTypeCollection.Instance.FetchCode(order.VoltageTypeCode, LanguageConstants.Default, order.TenantId)?.Title ?? null; // Cấp điện áp
        //                 worksheet.Cells[startRow, 6].Value = order.PriorityName; // Mức độ ưu tiên
        //
        //                 List<string> executionParticipantTeams = new List<string>();
        //                 List<string> executionFollowerTeams = new List<string>();
        //                 List<string> executionProjectTeams = new List<string>();
        //
        //                 if (order.sm_ExecutionTeams != null && order.sm_ExecutionTeams.Count > 0)
        //                 {
        //                     executionParticipantTeams = order.sm_ExecutionTeams.Where(x => x.UserType == "participants")
        //                         .Select(x => x.EmployeeName).Distinct().ToList();
        //                     executionFollowerTeams = order.sm_ExecutionTeams.Where(x => x.UserType == "follower")
        //                         .Select(x => x.EmployeeName).Distinct().ToList();
        //
        //                     var executionTeams = await GetExecutionTeamsInConstruction(new ExecutionTeamsQueryModel()
        //                     {
        //                         ConstructionId = order.Id
        //                     });
        //
        //                     if (executionTeams.Data.Content != null && executionTeams.Data.Content.Count > 0)
        //                     {
        //                         executionProjectTeams = executionTeams.Data.Content
        //                             .Where(x => x.ToThucHien != null)
        //                             .Select(x => x.ToThucHien?.Title).Distinct().ToList();
        //                     }
        //                 }
        //                 
        //                 worksheet.Cells[startRow, 7].Value = order.sm_Investor.Name ?? null; // Chủ đầu tư
        //                 worksheet.Cells[startRow, 8].Value = order.sm_Investor.InvestorType.Name ?? null; // Loại chủ đầu 
        //                 worksheet.Cells[startRow, 9].Value = order.DeliveryDate != null ? order.DeliveryDate?.ToString("dd/MM/yyyy") : null; // Năm giao A
        //                 worksheet.Cells[startRow, 9].Value = executionProjectTeams.Count > 0 ? string.Join(", ", executionProjectTeams) : null; 
        //                 worksheet.Cells[startRow, 10].Value = executionFollowerTeams.Count > 0 ? string.Join(", ", executionFollowerTeams) : null;
        //                 worksheet.Cells[startRow, 11].Value = executionParticipantTeams.Count > 0 ? string.Join(", ", executionParticipantTeams) : null;
        //                 worksheet.Cells[startRow, 13].Value = order.CompletionByInvestor ?? null; // Tiến độ hoàn thành theo kế hoạch chủ đầu tư
        //                 worksheet.Cells[startRow, 14].Value = order.CompletionByCompany ?? null; // Tiến độ hoàn thành dự kiến theo XNTV
        //                 worksheet.Cells[startRow, 15].Value = order.DocumentStatusName ?? null; // Tình trạng dự án
        //                 
        //
        //                 startRow++;
        //                 index++;
        //             }
        //
        //             int lastDataRow = startRow - 1;
        //
        //             // Thêm đường viền cho các ô đã điền dữ liệu
        //             using (var range =
        //                    worksheet.Cells[4, 1, lastDataRow,
        //                        15]) // điều chỉnh cột cuối (18) tùy theo số lượng cột bạn có
        //             {
        //                 range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                 range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                 range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                 range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //             }
        //
        //             // Xóa các dòng thừa sau khi điền dữ liệu
        //             int totalRows = worksheet.Dimension.End.Row;
        //             if (totalRows > lastDataRow)
        //             {
        //                 worksheet.DeleteRow(lastDataRow + 1, totalRows - lastDataRow);
        //             }
        //             
        //             // // Thêm AutoFilter cho dòng tiêu đề
        //             // worksheet.Cells["D4:N4"].AutoFilter = true;
        //
        //             // Tự động điều chỉnh kích thước các cột từ cột thứ hai đến cột cuối cùng
        //             worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column].AutoFitColumns();
        //
        //             // Lưu file Excel đã điền dữ liệu
        //             await package.SaveAsAsync(new FileInfo(filePath));
        //         }
        //
        //         Log.Information(
        //             $"Xuất file thành công, UserId: {currentUser.UserId}, UserName: {currentUser.FullName}");
        //         return Helper.CreateSuccessResponse<string>(filePath, "Xuất file thành công");
        //     }
        //     catch (Exception ex)
        //     {
        //         Log.Error(ex.Message, "Xuất file thất bại");
        //         Log.Information("Params: Query: {@Param}", query);
        //         return Helper.CreateExceptionResponse<string>(ex);
        //     }
        // }

        public async Task<Response<ConstructionViewModel>> ToggleTemplateStageIsDone(Guid constructionId,
            Guid templateStageId, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Construction.FirstOrDefaultAsync(x => x.Id == constructionId);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<ConstructionViewModel>(
                        "Công trình không tồn tại trong hệ thống!");

                var stage = entity.TemplateStages?.FirstOrDefault(s => s.Id == templateStageId);
                if (stage == null)
                    return Helper.CreateNotFoundResponse<ConstructionViewModel>(
                        "Giai đoạn không tồn tại trong công việc!");

                entity.TemplateStages = entity.TemplateStages
                    .Select(s =>
                    {
                        if (s.Id == templateStageId)
                            s.IsDone = !s.IsDone; // Đảo ngược trạng thái
                        return s;
                    })
                    .ToList();

                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động cập nhật thông tin công trình vào bảng sm_ConstructionActivityLog

                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = $"đã cập nhật trạng thái giai đoạn",
                        CodeLinkDescription = $"{stage.Name}",
                        OrderId = entity.Id,
                        ConstructionId = entity.Id,
                        ActionType = ConstructionConstants.ActionType.CONSTRUCTION,
                        StepOrder = stage.StepOrder,
                    }, currentUser);

                    #endregion
                }

                var result = _mapper.Map<ConstructionViewModel>(entity);
                return Helper.CreateSuccessResponse(result, "Cập nhật trạng thái giai đoạn thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: constructionId: {@constructionId}, templateStageId: {@templateStageId}",
                    constructionId, templateStageId);
                return Helper.CreateExceptionResponse<ConstructionViewModel>(ex);
            }
        }

        public async Task<Response> ImportExcelFile(IFormFile formFile, bool overwrite)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("Geneat Hardware");
                await using var fileStream = formFile.OpenReadStream();
                using var package = new ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets["Danh sách công trình_dự án"];
                var table = worksheet.Tables[0];
                var dataRows = table.DataRows;
                var constructions = dataRows
                    .Select(x => (Line: x.RowRange.Start.Row, Row: x, Constructions: new sm_Construction()
                    {
                        Id = Guid.NewGuid(),
                        Code = x.GetValue<string>(2),
                    }))
                    .ToList();

                // Sinh mã tự động nếu Code bị null hoặc rỗng
                var today = DateTime.Now.ToString("ddMMyy");
                var prefix = $"PN-{today}";

                var existingCodesInDb = await _dbContext.sm_Construction
                    .Where(x => x.Code.StartsWith(prefix))
                    .Select(x => x.Code)
                    .ToListAsync();

                var existingNumbers = existingCodesInDb
                    .Select(code =>
                    {
                        var numberPart = code.Replace(prefix, "");
                        return int.TryParse(numberPart, out var n) ? n : 0;
                    })
                    .ToList();

                int counter = existingNumbers.Count > 0 ? existingNumbers.Max() + 1 : 1;

                var usedCodes = new HashSet<string>(existingCodesInDb
                    .Concat(constructions.Select(c => c.Constructions.Code)
                        .Where(c => !string.IsNullOrWhiteSpace(c))));

                foreach (var tuple in constructions)
                {
                    if (string.IsNullOrWhiteSpace(tuple.Constructions.Code))
                    {
                        string newCode;
                        do
                        {
                            newCode = $"{prefix}{counter:D3}";
                            counter++;
                        } while (usedCodes.Contains(newCode));

                        tuple.Constructions.Code = newCode;
                        usedCodes.Add(newCode);
                    }
                    else
                    {
                        tuple.Constructions.Code = tuple.Row.GetValue<string>(2);
                    }
                }

                var codes = constructions.Select(x => x.Constructions.Code).ToList();
                var duplicateCodes = codes.GroupBy(x => x)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key)
                    .ToList();

                if (duplicateCodes.Count > 0)
                {
                    var duplicateCodesResponse = Helper.CreateBadRequestResponse(
                        $"Mã công trình bị trùng: {string.Join(", ", duplicateCodes)}");

                    Log.Information(
                        "ConstructionHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                        nameof(ImportExcelFile), formFile?.FileName, duplicateCodesResponse.IsSuccess,
                        duplicateCodesResponse.Message);

                    return duplicateCodesResponse;
                }

                var existedConstruction = await _dbContext.sm_Construction
                    .Where(x => codes.Contains(x.Code))
                    .ToListAsync();

                if (overwrite)
                {
                    _dbContext.sm_Construction.RemoveRange(existedConstruction);
                }
                else
                {
                    if (existedConstruction.Count > 0)
                    {
                        var existedConstructionResponse = Helper.CreateBadRequestResponse(
                            $"Mã công trình đã tồn tại");

                        Log.Information(
                            "ConstructionHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ImportExcelFile), formFile?.FileName, existedConstructionResponse.IsSuccess,
                            existedConstructionResponse.Message);

                        return existedConstructionResponse;
                    }
                }

                foreach (var tuple in constructions)
                {
                    if (!string.IsNullOrWhiteSpace(tuple.Constructions.Code)) continue;
                    var emptyCodeResponse =
                        Helper.CreateBadRequestResponse($"Mã công trình tại dòng {tuple.Line} trống");

                    Log.Information(
                        "ConstructionHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                        nameof(ImportExcelFile), formFile?.FileName, emptyCodeResponse.IsSuccess,
                        emptyCodeResponse.Message);

                    return emptyCodeResponse;
                }


                foreach (var tuple in constructions)
                {
                    List<sm_ExecutionTeams> executionTeams = new List<sm_ExecutionTeams>();

                    /// Lưu danh sách nhân sự tham gia vào mảng executionItems
                    foreach (var items in tuple.Row.GetValue<string>(10).Split(", "))
                    {
                        var userEntity = _dbContext.IdmUser.FirstOrDefault(x => x.Name == items);

                        if (userEntity != null)
                        {
                            executionTeams.Add(new sm_ExecutionTeams()
                            {
                                ConstructionId = tuple.Constructions.Id,
                                EmployeeId = userEntity.Id,
                                EmployeeAvatarUrl = userEntity.AvatarUrl,
                                EmployeeName = userEntity.Name,
                                MaPhongBan = userEntity.MaPhongBan,
                                MaTo = userEntity.MaTo,
                                UserType = "follower"
                            });
                        }
                    }

                    tuple.Constructions.sm_ExecutionTeams = executionTeams;

                    /// Add nốt danh sách người theo dõi vào mảng ExecutionItems mới đã lưu bên trên
                    foreach (var items in tuple.Row.GetValue<string>(11).Split(", "))
                    {
                        var userEntity = _dbContext.IdmUser.FirstOrDefault(x => x.Name == items);

                        if (userEntity != null)
                        {
                            tuple.Constructions.sm_ExecutionTeams.Add(new sm_ExecutionTeams()
                            {
                                ConstructionId = tuple.Constructions.Id,
                                EmployeeId = userEntity.Id,
                                EmployeeAvatarUrl = userEntity.AvatarUrl,
                                EmployeeName = userEntity.Name,
                                MaPhongBan = userEntity.MaPhongBan,
                                MaTo = userEntity.MaTo,
                                UserType = "participants"
                            });
                        }
                    }

                    tuple.Constructions.StatusName = tuple.Row.GetValue<string>(1);
                    tuple.Constructions.StatusCode =
                        ConstructionConstants.FetchCode(tuple.Row.GetValue<string>(1))?.Code ?? null;
                    tuple.Constructions.Name = tuple.Row.GetValue<string>(3);
                    tuple.Constructions.VoltageTypeCode =
                        _dbContext.sm_CodeType.FirstOrDefault(x => x.Title == tuple.Row.GetValue<string>(4))?.Code ??
                        null;
                    tuple.Constructions.PriorityCode =
                        ConstructionConstants.FetchCode(tuple.Row.GetValue<string>(5))?.Code ?? null;
                    tuple.Constructions.PriorityName = tuple.Row.GetValue<string>(5) ?? null;
                    tuple.Constructions.InvestorId = _dbContext.sm_Investor.AsNoTracking()
                        .Include(x => x.InvestorType)
                        .FirstOrDefault(x => x.Name == tuple.Row.GetValue<string>(6)).Id;
                    tuple.Constructions.OwnerTypeCode = _dbContext.sm_Investor.AsNoTracking()
                                                            .Include(x => x.InvestorType)
                                                            .FirstOrDefault(
                                                                x => x.Name == tuple.Row.GetValue<string>(6))
                                                            ?.InvestorType?.Code
                                                        ?? _dbContext.sm_CodeType.FirstOrDefault(x =>
                                                            x.Title == tuple.Row.GetValue<string>(7))?.Code;
                    tuple.Constructions.DeliveryDate = DateTime.TryParse(tuple.Row.GetValue<string>(8), out var dd)
                        ? dd
                        : DateTime.Now;
                    tuple.Constructions.CompletionByInvestor = tuple.Row.GetValue<string>(12) ?? null;
                    tuple.Constructions.CompletionByCompany = tuple.Row.GetValue<string>(13) ?? null;
                    tuple.Constructions.DocumentStatusCode =
                        ConstructionConstants.FetchCode(tuple.Row.GetValue<string>(14))?.Code ?? null;
                    tuple.Constructions.DocumentStatusName = tuple.Row.GetValue<string>(14) ?? null;
                }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                foreach (var tuple in constructions)
                {
                    tuple.Constructions.CreatedByUserId = currentUser.UserId;
                    tuple.Constructions.CreatedByUserName = currentUser.UserName;
                    tuple.Constructions.CreatedOnDate = DateTime.Now;
                    tuple.Constructions.LastModifiedByUserId = currentUser.UserId;
                    tuple.Constructions.LastModifiedByUserName = currentUser.UserName;
                    tuple.Constructions.LastModifiedOnDate = DateTime.Now;
                    tuple.Constructions.TenantId = currentUser.TenantId;
                }

                _dbContext.sm_Construction.AddRange(constructions.Select(x => x.Constructions));
                await _dbContext.SaveChangesAsync();

                var response = Helper.CreateSuccessResponse("Import thành công");

                Log.Information(
                    "ConstructionHandler.{MethodName} succeeded. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(ImportExcelFile), formFile?.FileName, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "Lỗi khi nhập file công trình");
                var errorResponse = Helper.CreateExceptionResponse(e);

                Log.Information(
                    "ConstructionHandler.{MethodName} failed with exception. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(ImportExcelFile), formFile?.FileName, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        // public async Task<Response<List<ConstructionViewModel>>> Import(string path, RequestUser currentUser)
        // {
        //     try
        //     {
        //         var filePath = Path.Combine(_staticsFolder, path);
        //         if (string.IsNullOrEmpty(path) || !File.Exists(filePath))
        //             return Helper.CreateBadRequestResponse<List<ConstructionViewModel>>("Đường dẫn không tồn tại");
        //
        //         // // Xoá toàn bộ dữ liệu hiện tại trong bảng Construction và ExecutionTeams
        //         // var allConstructions = _dbContext.sm_Construction.ToList();
        //         // if (allConstructions.Any())
        //         // {
        //         //     _dbContext.sm_Construction.RemoveRange(allConstructions);
        //         //     await _dbContext.SaveChangesAsync();
        //         // }
        //
        //         var result = new List<ConstructionViewModel>();
        //         ExcelPackage.License.SetNonCommercialPersonal("Geneat Hardware");
        //
        //         using (var package = new ExcelPackage(filePath))
        //         {
        //             var worksheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Dự án");
        //             if (worksheet == null)
        //                 return Helper.CreateBadRequestResponse<List<ConstructionViewModel>>(
        //                     "Không tìm thấy sheet 'Dự án'");
        //
        //             int startRow = 5;
        //             int lastRow = worksheet.Dimension.End.Row;
        //
        //             for (int rowIndex = startRow; rowIndex <= lastRow; rowIndex++)
        //             {
        //                 var sttCell = worksheet.Cells[rowIndex, 1];
        //                 if (sttCell == null || sttCell.Value == null) break;
        //
        //                 // var createdDateStr = worksheet.Cells[rowIndex, 2].Text;
        //                 // var modifiedDateStr = worksheet.Cells[rowIndex, 3].Text;
        //                 // var code = worksheet.Cells[rowIndex, 4].Text?.Trim();
        //                 // var name = worksheet.Cells[rowIndex, 5].Text?.Trim();
        //                 // var investorValue = worksheet.Cells[rowIndex, 6].Text?.Trim();
        //                 // var projects = worksheet.Cells[rowIndex, 7].Text?.Trim();
        //                 // var participants = worksheet.Cells[rowIndex, 8].Text?.Trim();
        //                 // var followers = worksheet.Cells[rowIndex, 9].Text?.Trim();
        //                 // var deliveryDateStr = worksheet.Cells[rowIndex, 10].Text?.Trim();
        //                 // var statusName = worksheet.Cells[rowIndex, 2].Text?.Trim();
        //                 // var execStatusName = worksheet.Cells[rowIndex, 12].Text?.Trim();
        //                 // var docStatusName = worksheet.Cells[rowIndex, 13].Text?.Trim();
        //                 // var templateName = worksheet.Cells[rowIndex, 14].Text?.Trim();
        //
        //                 var statusName = worksheet.Cells[rowIndex, 2].Text?.Trim(); // Tình trạng dự án
        //                 var name = worksheet.Cells[rowIndex, 3].Text?.Trim();
        //                 var voltageTypeName = worksheet.Cells[rowIndex, 4].Text?.Trim();
        //                 var priorityName = worksheet.Cells[rowIndex, 5].Text?.Trim();
        //                 var stepProject = worksheet.Cells[rowIndex, 6].Text?.Trim();
        //                 var investorValue = worksheet.Cells[rowIndex, 7].Text?.Trim(); // CĐT
        //                 var investorTypeValue = worksheet.Cells[rowIndex, 8].Text?.Trim(); // Loại CĐT
        //                 var projects = worksheet.Cells[rowIndex, 9].Text?.Trim();
        //                 var participants = worksheet.Cells[rowIndex, 10].Text?.Trim();
        //                 var followers = worksheet.Cells[rowIndex, 11].Text?.Trim();
        //                 var deliveryDateStr = worksheet.Cells[rowIndex, 12].Text?.Trim();
        //                 var docStatusName = worksheet.Cells[rowIndex, 24].Text?.Trim(); // Tình trạng hồ sơ
        //
        //                 // if (result.Any(x => x.Code == code))
        //                 //     return Helper.CreateBadRequestResponse<List<ConstructionViewModel>>(
        //                 //         $"Mã công trình bị trùng: {code}");
        //
        //                 // string investorCode = null,
        //                 //     ownerTypeCode = null,
        //                 //     constructionName = null;
        //                 // // List<ExecutionTeamsCreateModel> executionTeams = new List<ExecutionTeamsCreateModel>();
        //                 // if (!string.IsNullOrWhiteSpace(investorValue))
        //                 // {
        //                 //     var parts = investorValue.Split('-');
        //                 //     investorCode = parts[0].Trim();
        //                 //     ownerTypeCode = parts.Length > 1 ? parts[1].Trim() : null;
        //                 // }
        //
        //                 // if (!string.IsNullOrWhiteSpace(name))
        //                 // {
        //                 //     var parts = name.Split('-');
        //                 //     constructionName = parts[0].Trim();
        //                 // }
        //
        //                 var investorEntity = _dbContext.sm_Investor.AsNoTracking()
        //                     .Include(x => x.InvestorType)
        //                     .FirstOrDefault(x => x.Name == investorValue);
        //
        //                 // var templateEntity = _dbContext.sm_ProjectTemplate
        //                 //     .FirstOrDefault(x => x.Name == templateName);
        //
        //                 var item = new ConstructionCreateUpdateModel
        //                 {
        //                     // Code = code,
        //                     Name = name ?? null,
        //                     InvestorId = investorEntity?.Id ?? Guid.NewGuid(),
        //                     OwnerTypeCode = investorEntity?.InvestorType?.Code ?? null,
        //                     VoltageTypeCode = _dbContext.sm_CodeType.FirstOrDefault(x => x.Title == voltageTypeName)?.Code ?? null,
        //                     StatusName = statusName ?? null,
        //                     StatusCode = ConstructionConstants.FetchCode(statusName).Code ?? null,
        //                     // ExecutionStatusName = execStatusName,
        //                     // ExecutionStatusCode = ConstructionConstants.FetchCode(execStatusName).Code,
        //                     DocumentStatusName = docStatusName ?? null,
        //                     DocumentStatusCode = ConstructionConstants.FetchCode(docStatusName).Code ?? null,
        //                     ConstructionTemplateId = null,
        //                     // ConstructionTemplateId = templateEntity?.Id ?? Guid.NewGuid(),
        //                     DeliveryDate = DateTime.TryParse(deliveryDateStr, out var dd) ? dd : DateTime.Now,
        //                     ExecutionTeams = new List<ExecutionTeamsCreateModel>()
        //                 };
        //
        //                 // var voltageTypeEntity = _dbContext.sm_CodeType.FirstOrDefault(x => x.Title == voltageTypeName);
        //
        //                 item.PriorityCode = ConstructionConstants.FetchCode(priorityName).Code ?? null;
        //                 
        //
        //                 // /// Lưu danh sách nhân sự tham gia vào mảng executionItems
        //                 // foreach (var items in participants.Split(", "))
        //                 // {
        //                 //     var userEntity = _dbContext.IdmUser.FirstOrDefault(x => x.Name == items);
        //                 //
        //                 //     if (userEntity != null)
        //                 //     {
        //                 //         executionTeams.Add(new ExecutionTeamsCreateModel()
        //                 //         {
        //                 //             EmployeeId = userEntity.Id,
        //                 //             UserType = "participants"
        //                 //         });
        //                 //     }
        //                 // }
        //                 //
        //                 // item.ExecutionTeams = executionTeams;
        //                 //
        //                 // /// Add nốt danh sách người theo dõi vào mảng ExecutionItems mới đã lưu bên trên
        //                 // foreach (var items in followers.Split(", "))
        //                 // {
        //                 //     var userEntity = _dbContext.IdmUser.FirstOrDefault(x => x.Name == items);
        //                 //
        //                 //     if (userEntity != null)
        //                 //     {
        //                 //         item.ExecutionTeams.Add(new ExecutionTeamsCreateModel()
        //                 //         {
        //                 //             EmployeeId = userEntity.Id,
        //                 //             UserType = "follower"
        //                 //         });
        //                 //     }
        //                 // }
        //
        //                 var data = await Create(item, currentUser);
        //
        //                 if (data.IsSuccess)
        //                 {
        //                     result.Add(data.Data);
        //                 }
        //                 else
        //                 {
        //                     break;
        //                 }
        //             }
        //         }
        //
        //         Log.Information(
        //             $"Import thành công, UserName: {currentUser.FullName}, UserId: {currentUser.UserId}, path: {path}");
        //         return new Response<List<ConstructionViewModel>>(System.Net.HttpStatusCode.OK, result,
        //             $"Nhập file excel thành công");
        //     }
        //     catch (Exception ex)
        //     {
        //         Log.Error(ex, "Lỗi khi import công trình từ Excel");
        //         Log.Information("Params: Path: {@path}", path);
        //         return Helper.CreateExceptionResponse<List<ConstructionViewModel>>(ex);
        //     }
        // }
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

        public async Task<Response<ConstructionViewModel>> UpdateTemplateStages(Guid constructionId,
            List<jsonb_TemplateStage> templateStages, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Construction
                    .Include(x => x.Tasks)
                    .FirstOrDefaultAsync(x => x.Id == constructionId);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<ConstructionViewModel>(
                        "Công trình không tồn tại trong hệ thống!");

                // Lấy danh sách Id các stage cũ
                var oldStageIds = entity.TemplateStages?.Select(s => s.Id).ToList() ?? new List<Guid>();
                // Lấy danh sách Id các stage mới
                var newStageIds = templateStages?.Select(s => s.Id).ToList() ?? new List<Guid>();

                // Tìm các stage đã bị xóa
                var deletedStageIds = oldStageIds.Except(newStageIds).ToList();

                if (deletedStageIds.Any())
                {
                    // Xóa các task thuộc các stage đã bị xóa
                    var tasksToDelete = _dbContext.sm_Task.Where(t =>
                        t.ConstructionId == constructionId &&
                        deletedStageIds.Contains(t.IdTemplateStage ?? Guid.Empty));
                    _dbContext.sm_Task.RemoveRange(tasksToDelete);
                }

                entity.TemplateStages = templateStages;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;

                _dbContext.sm_Construction.Update(entity);
                await _dbContext.SaveChangesAsync();

                var result = _mapper.Map<ConstructionViewModel>(entity);
                return Helper.CreateSuccessResponse(result, "Cập nhật danh sách giai đoạn thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: constructionId: {@constructionId}, templateStages: {@templateStages}",
                    constructionId, templateStages);
                return Helper.CreateExceptionResponse<ConstructionViewModel>(ex);
            }
        }

        public async Task<Response<List<jsonb_TemplateStage>>> GetTemplateStagesWithIsDoneStatus(Guid constructionId)
        {
            try
            {
                var construction = await _dbContext.sm_Construction
                    .Include(x => x.Tasks)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == constructionId);

                if (construction == null)
                    return Helper.CreateNotFoundResponse<List<jsonb_TemplateStage>>(
                        "Công trình không tồn tại trong hệ thống!");

                var templateStages = construction.TemplateStages ?? new List<jsonb_TemplateStage>();

                // Lấy danh sách các task của công trình
                var tasks = construction.Tasks ?? new List<sm_Task>();

                // Các trạng thái không cho phép đánh dấu isDone = true
                var notAllowStatuses = new[] { TaskStatus.InProgress, TaskStatus.PendingApproval, TaskStatus.Failed };

                foreach (var stage in templateStages)
                {
                    // Lấy các task thuộc stage này
                    var stageTasks = tasks.Where(t => t.IdTemplateStage == stage.Id).ToList();
                    // Nếu có task ở trạng thái không cho phép thì không cho isDone = true
                    if (stageTasks.Any(t => notAllowStatuses.Contains(t.Status)))
                    {
                        stage.IsDone = true;
                    }
                }

                return Helper.CreateSuccessResponse(templateStages);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<List<jsonb_TemplateStage>>(ex);
            }
        }
    }
}