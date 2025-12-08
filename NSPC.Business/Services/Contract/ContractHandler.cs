using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.ConstructionActitvityLog;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.ActivityHistory;
using NSPC.Data.Data.Entity.Contract;
using Serilog;
using System.Linq.Expressions;
using OfficeOpenXml;

namespace NSPC.Business.Services.Contract
{
    public class ContractHandler : IContractHandler
    {
        private readonly List<ImplementationStatus> _statusList = new()
        {
            ImplementationStatus.PendingApproval,
            ImplementationStatus.NotImplemented,
            ImplementationStatus.Approved,
            ImplementationStatus.InProgress,
            ImplementationStatus.OnHoldOrSuspended
        };

        private readonly Dictionary<ImplementationStatus, string> _implementationStatusDict =
            new()
            {
                { ImplementationStatus.PendingApproval, "Đang trình duyệt" },
                { ImplementationStatus.NotImplemented, "Chưa triển khai" },
                { ImplementationStatus.Approved, "Đã phê duyệt" },
                { ImplementationStatus.InProgress, "Đang thực hiện" },
                { ImplementationStatus.OnHoldOrSuspended, "Vướng mắc/Tạm dừng" },
            };

        private readonly Dictionary<InvoiceStatus, string> _invoiceStatusDict =
            new()
            {
                { InvoiceStatus.Issued, "Đã xuất hoá đơn" },
                { InvoiceStatus.NotIssued, "Chưa xuất hoá đơn" }
            };

        private readonly Dictionary<AcceptanceDocumentStatus, string> _acceptanceDocumentStatusDict =
            new()
            {
                { AcceptanceDocumentStatus.TransferredToOwner, "Đã chuyển HS cho CĐT" },
                { AcceptanceDocumentStatus.NotPrepared, "Chưa lập hồ sơ" }
            };

        private readonly Dictionary<SupplementaryContractRequired, string> _supplementaryContractRequiredDict =
            new()
            {
                { SupplementaryContractRequired.Required, "Cần ký PLHĐ" },
                { SupplementaryContractRequired.NotRequired, "Không cần ký PLHĐ" }
            };

        private readonly SMDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAttachmentHandler _attachmentHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConstructionActivityLogHandler _constructionActivityLogHandler;

        public ContractHandler(
            SMDbContext dbContext,
            IMapper mapper,
            IAttachmentHandler attachmentHandler,
            IHttpContextAccessor httpContextAccessor,
            IConstructionActivityLogHandler constructionActivityLogHandler
        )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _attachmentHandler = attachmentHandler;
            _httpContextAccessor = httpContextAccessor;
            _constructionActivityLogHandler = constructionActivityLogHandler;
        }

        public async Task<Response<ContractViewModel>> Create(ContractCreateUpdateModel model)
        {
            try
            {
                var validationResult = await ValidateCreateUpdateModel<ContractViewModel>(model);
                if (validationResult.Response != null)
                {
                    Log.Information(
                        "ContractHandler.{MethodName} validation failed. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                        nameof(Create), model, validationResult.Response.IsSuccess, validationResult.Response.Message);

                    return validationResult.Response;
                }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var contract = new sm_Contract
                {
                    Id = Guid.NewGuid(),
                    Code = model.Code,
                    ContractNumber = model.ContractNumber,
                    ConstructionId = model.ConstructionId,
                    TemplateStageId = validationResult.TemplateStage.Id,
                    TemplateStage = validationResult.TemplateStage,
                    AssignmentAYear = model.AssignmentAYear,
                    ConsultingServiceId = model.ConsultingServiceId,
                    ValueBeforeVatAmount = model.ValueBeforeVatAmount,
                    ExpectedVolume = model.ExpectedVolume,
                    AcceptanceValueBeforeVatAmount = model.AcceptanceValueBeforeVatAmount,
                    PaidAmount = model.PaidAmount,
                    TaxRatePercentage = model.TaxRatePercentage,
                    ContractSigningDate = model.ContractSigningDate,
                    ContractDurationDays = model.ContractDurationDays,
                    Issues = model.Issues,
                    Notes = model.Notes,
                    ImplementationStatus = Enum.Parse<ImplementationStatus>(model.ImplementationStatus),
                    AcceptanceDocumentStatus = Enum.Parse<AcceptanceDocumentStatus>(model.AcceptanceDocumentStatus),
                    ExpectedApprovalMonth = model.ExpectedApprovalMonth,
                    ApprovalDate = model.ApprovalDate,
                    DesignApprovalDate = model.DesignApprovalDate,
                    ExpectedAcceptanceMonth = model.ExpectedAcceptanceMonth,
                    InvoiceStatus = Enum.Parse<InvoiceStatus>(model.InvoiceStatus),
                    InvoiceIssuanceDates = model.InvoiceIssuanceDates,
                    AcceptanceYear = model.AcceptanceYear,
                    HandoverRecordDate = model.HandoverRecordDate,
                    SiteSurveyRecordDate = model.SiteSurveyRecordDate,
                    SurveyAcceptanceRecordDate = model.SurveyAcceptanceRecordDate,
                    SupplementaryContractRequired =
                        Enum.Parse<SupplementaryContractRequired>(model.SupplementaryContractRequired),
                    AcceptancePlan = model.AcceptancePlan,
                    CreatedOnDate = DateTime.Now,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    TenantId = currentUser.TenantId
                };
                contract.Appendices = await ProcessAppendixAttachment(contract, model.Appendices);

                var activityHistory = new sm_ActiviyHisroty
                {
                    Id = Guid.NewGuid(),
                    EntityId = contract.Id,
                    EntityType = "Contract",
                    Action = ContractHistoryAction.CREATE,
                    Description = null,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    TenantId = currentUser.TenantId
                };

                _dbContext.sm_Contract.Add(contract);
                _dbContext.sm_ActiviyHisroty.Add(activityHistory);
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động thêm mới hợp đồng vào bảng sm_ConstructionActivityLog

                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã thêm mới hợp đồng",
                        CodeLinkDescription = $"{contract.Code}",
                        OrderId = contract.Id,
                        ConstructionId = contract.ConstructionId,
                        ActionType = ConstructionConstants.ActionType.CONTRACT
                    }, currentUser);

                    var constructionEntity =  await _dbContext.sm_Construction.FirstOrDefaultAsync(x => x.Id == contract.ConstructionId);

                    if (constructionEntity != null)
                    {
                        constructionEntity.LastModifiedOnDate = DateTime.Now;
                        _dbContext.sm_Construction.Update(constructionEntity);
                        await _dbContext.SaveChangesAsync();
                    }
                    
                    #endregion
                }

                var result = await _dbContext.sm_Contract
                    .Include(x => x.Construction)
                    .Include(x => x.ConsultingService)
                    .FirstOrDefaultAsync(x => x.Id == contract.Id);

                var response = Helper.CreateSuccessResponse(
                    _mapper.Map<ContractViewModel>(result),
                    "Tạo hợp đồng thành công"
                );

                Log.Information(
                    "ContractHandler.{MethodName} succeeded. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Create), model, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<ContractViewModel>(e);

                Log.Information(
                    "ContractHandler.{MethodName} failed with exception. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Create), model, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        public async Task<Response<ContractViewModel>> Update(Guid id, ContractCreateUpdateModel model)
        {
            try
            {
                var contract = await _dbContext.sm_Contract.FindAsync(id);
                if (contract == null)
                {
                    var notFoundResponse =
                        Helper.CreateBadRequestResponse<ContractViewModel>("Không tìm thấy hợp đồng");

                    Log.Information(
                        "ContractHandler.{MethodName} failed - entity not found. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                        nameof(Update), id, model, notFoundResponse.IsSuccess, notFoundResponse.Message);

                    return notFoundResponse;
                }

                var validationResult = await ValidateCreateUpdateModel<ContractViewModel>(model, id);
                if (validationResult.Response != null)
                {
                    Log.Information(
                        "ContractHandler.{MethodName} validation failed. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                        nameof(Update), id, model, validationResult.Response.IsSuccess,
                        validationResult.Response.Message);

                    return validationResult.Response;
                }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                contract.Code = model.Code;
                contract.ContractNumber = model.ContractNumber;
                contract.ConstructionId = model.ConstructionId;
                contract.TemplateStageId = validationResult.TemplateStage.Id;
                contract.TemplateStage = validationResult.TemplateStage;
                contract.AssignmentAYear = model.AssignmentAYear;
                contract.ConsultingServiceId = model.ConsultingServiceId;
                contract.ValueBeforeVatAmount = model.ValueBeforeVatAmount;
                contract.ExpectedVolume = model.ExpectedVolume;
                contract.AcceptanceValueBeforeVatAmount = model.AcceptanceValueBeforeVatAmount;
                contract.PaidAmount = model.PaidAmount;
                contract.TaxRatePercentage = model.TaxRatePercentage;
                contract.ContractSigningDate = model.ContractSigningDate;
                contract.ContractDurationDays = model.ContractDurationDays;
                contract.Issues = model.Issues;
                contract.Notes = model.Notes;
                contract.ImplementationStatus = Enum.Parse<ImplementationStatus>(model.ImplementationStatus);
                contract.AcceptanceDocumentStatus =
                    Enum.Parse<AcceptanceDocumentStatus>(model.AcceptanceDocumentStatus);
                contract.ExpectedApprovalMonth = model.ExpectedApprovalMonth;
                contract.ApprovalDate = model.ApprovalDate;
                contract.DesignApprovalDate = model.DesignApprovalDate;
                contract.ExpectedAcceptanceMonth = model.ExpectedAcceptanceMonth;
                contract.InvoiceStatus = Enum.Parse<InvoiceStatus>(model.InvoiceStatus);
                contract.InvoiceIssuanceDates = model.InvoiceIssuanceDates;
                contract.AcceptanceYear = model.AcceptanceYear;
                contract.HandoverRecordDate = model.HandoverRecordDate;
                contract.SiteSurveyRecordDate = model.SiteSurveyRecordDate;
                contract.SurveyAcceptanceRecordDate = model.SurveyAcceptanceRecordDate;
                contract.SupplementaryContractRequired =
                    Enum.Parse<SupplementaryContractRequired>(model.SupplementaryContractRequired);
                contract.AcceptancePlan = model.AcceptancePlan;
                contract.LastModifiedOnDate = DateTime.Now;
                contract.LastModifiedByUserId = currentUser.UserId;
                contract.LastModifiedByUserName = currentUser.UserName;
                contract.Appendices = await ProcessAppendixAttachment(contract, model.Appendices, contract.Appendices);

                var activityHistory = new sm_ActiviyHisroty
                {
                    Id = Guid.NewGuid(),
                    EntityId = contract.Id,
                    EntityType = "Contract",
                    Action = ContractHistoryAction.UPDATE,
                    Description = null,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    TenantId = currentUser.TenantId
                };

                _dbContext.sm_Contract.Update(contract);
                _dbContext.sm_ActiviyHisroty.Add(activityHistory);
                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động cập nhật thông tin hợp đồng vào bảng sm_ConstructionActivityLog

                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã cập nhật thông tin hợp đồng",
                        CodeLinkDescription = $"{contract.Code}",
                        OrderId = contract.Id,
                        ConstructionId = contract.ConstructionId,
                        ActionType = ConstructionConstants.ActionType.CONTRACT
                    }, currentUser);

                    #endregion
                }

                var result = await _dbContext.sm_Contract
                    .Include(x => x.Construction)
                    .Include(x => x.ConsultingService)
                    .FirstOrDefaultAsync(x => x.Id == contract.Id);

                var response = Helper.CreateSuccessResponse(
                    _mapper.Map<ContractViewModel>(result),
                    "Cập nhật hợp đồng thành công"
                );

                Log.Information(
                    "ContractHandler.{MethodName} succeeded. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Update), id, model, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<ContractViewModel>(e);

                Log.Information(
                    "ContractHandler.{MethodName} failed with exception. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Update), id, model, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        public async Task<Response<Pagination<ContractViewModel>>> GetPage(ContractQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_Contract
                    .Include(x => x.Construction)
                    .ThenInclude(x => x.sm_Investor)
                    .Include(x => x.ConsultingService)
                    .Where(predicate);

                var data = await queryResult.GetPageAsync(query);
                var result = _mapper.Map<Pagination<ContractViewModel>>(data);

                var response = Helper.CreateSuccessResponse(result);

                Log.Information(
                    "ContractHandler.{MethodName} succeeded. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(GetPage), query, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<Pagination<ContractViewModel>>(e);

                Log.Information(
                    "ContractHandler.{MethodName} failed with exception. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(GetPage), query, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        public async Task<Response<Dictionary<string, int>>> CountByStatus(ContractQueryModel query)
        {
            var predicate = BuildQuery(query, true);
            predicate.And(x => _statusList.Contains(x.ImplementationStatus));

            var result = await _dbContext.sm_Contract
                .Where(predicate)
                .GroupBy(x => x.ImplementationStatus)
                .Select(x => new { x.Key, Count = x.Count() })
                .ToDictionaryAsync(x => x.Key.ToString(), x => x.Count);

            result["All"] = result.Values.Sum();

            var response = Helper.CreateSuccessResponse(result);

            Log.Information(
                "ContractHandler.{MethodName} succeeded. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                nameof(CountByStatus), query, response.IsSuccess, response.Message);

            return response;
        }

        public async Task<Response<ContractDetailViewModel>> GetById(Guid id)
        {
            try
            {
                var contract = await _dbContext.sm_Contract
                    .AsNoTracking()
                    .Include(x => x.Construction)
                    .ThenInclude(x => x.sm_Investor)
                    .Include(x => x.ConsultingService)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.LastModifiedByUser)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (contract == null)
                {
                    var notFoundResponse =
                        Helper.CreateBadRequestResponse<ContractDetailViewModel>("Không tìm thấy hợp đồng");

                    Log.Information(
                        "ContractHandler.{MethodName} failed - entity not found. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(GetById), id, notFoundResponse.IsSuccess, notFoundResponse.Message);

                    return notFoundResponse;
                }

                var activityHistories = await _dbContext.sm_ActiviyHisroty
                    .Where(x => x.EntityId == contract.Id && x.EntityType == "Contract")
                    .OrderByDescending(x => x.CreatedOnDate)
                    .ToListAsync();
                var userIds = activityHistories
                    .Select(x => x.CreatedByUserId)
                    .Concat(activityHistories.Select(x => x.LastModifiedByUserId!.Value))
                    .Distinct()
                    .ToList();
                var users = await _dbContext.IdmUser
                    .Where(x => userIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.Name);
                var result = _mapper.Map<ContractDetailViewModel>(contract);

                result.ActivityHistories = _mapper.Map<List<ActivityHistoryViewModel>>(activityHistories);
                result.ActivityHistories.ForEach(x =>
                {
                    x.CreatedByUserFullName = users[x.CreatedByUserId];
                    x.LastModifiedByFullName = users[x.LastModifiedByUserId!.Value];
                });

                var response = Helper.CreateSuccessResponse(result);

                Log.Information(
                    "ContractHandler.{MethodName} succeeded. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(GetById), id, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<ContractDetailViewModel>(e);

                Log.Information(
                    "ContractHandler.{MethodName} failed with exception. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(GetById), id, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var contract = await _dbContext.sm_Contract.FindAsync(id);
                if (contract == null)
                {
                    var notFoundResponse = Helper.CreateBadRequestResponse("Không tìm thấy hợp đồng");

                    Log.Information(
                        "ContractHandler.{MethodName} failed - entity not found. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(Delete), id, notFoundResponse.IsSuccess, notFoundResponse.Message);

                    return notFoundResponse;
                }

                var activityHistory = new sm_ActiviyHisroty
                {
                    Id = Guid.NewGuid(),
                    EntityId = contract.Id,
                    EntityType = "Contract",
                    Action = ContractHistoryAction.DELETE,
                    Description = null,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    TenantId = currentUser.TenantId
                };

                _dbContext.sm_Contract.Remove(contract);
                _dbContext.sm_ActiviyHisroty.Add(activityHistory);
                await _dbContext.SaveChangesAsync();

                var response = Helper.CreateSuccessResponse("Xóa hợp đồng thành công");

                Log.Information(
                    "ContractHandler.{MethodName} succeeded. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Delete), id, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse(e);

                Log.Information(
                    "ContractHandler.{MethodName} failed with exception. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Delete), id, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        public async Task<(Response, Stream, string)> ExportExcelFile(ContractQueryModel model)
        {
            try
            {
                var predicate = BuildQuery(model);
                var query = _dbContext.sm_Contract
                    .Include(x => x.Construction)
                    .ThenInclude(x => x.sm_Investor)
                    .Include(x => x.ConsultingService)
                    .Where(predicate);

                if (model.Size >= 0)
                {
                    query = query.Skip(((model.Page ?? 1) - 1) * (model.Size ?? 10));
                    query = query.Take(model.Size ?? 10);
                }

                var contracts = await query
                    .OrderByDescending(x => x.CreatedOnDate)
                    .ToListAsync();

                var templateFilePath = Utils.CombineUnixPath(
                    ConfigCollection.Instance.StaticFiles_Folder,
                    "excel-template",
                    "ContractsTemplate.xlsx");

                if (!File.Exists(templateFilePath))
                {
                    Log.Error("Contracts Template Not Found: {Path}", templateFilePath);
                    var templateNotFoundResponse = Helper.CreateExceptionResponse("Không tìm thấy file template");

                    Log.Information(
                        "Contracts.{MethodName} failed - template not found. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                        nameof(ExportExcelFile), query, templateNotFoundResponse.IsSuccess,
                        templateNotFoundResponse.Message);

                    return (templateNotFoundResponse, null, null);
                }

                Log.Information("Vehicle Request Template Path: {Path}", templateFilePath);

                ExcelPackage.License.SetNonCommercialPersonal("Geneat Hardware");
                var viewModels = _mapper.Map<List<ContractViewModel>>(contracts);
                using var package = new ExcelPackage(new FileInfo(templateFilePath));
                var worksheet = package.Workbook.Worksheets[0];
                var table = worksheet.Tables[0];

                if (contracts.Count > 1)
                {
                    table.AddRow(contracts.Count - 1);
                }

                for (var i = 0; i < contracts.Count; i++)
                {
                    var contract = contracts[i];
                    var viewModel = viewModels[i];
                    var row = table.DataRows[i];

                    row.SetValue(1, contract.Code);
                    row.SetValue(2, contract.Construction.Code);
                    row.SetValue(3, contract.Construction.Name);
                    row.SetValue(4, contract.TemplateStage.Name);
                    row.SetValue(5, contract.Construction.sm_Investor.Code);
                    row.SetValue(5, contract.AssignmentAYear);
                    row.SetValue(7, contract.ExpectedApprovalMonth);
                    row.SetValue(8, contract.ApprovalDate);
                    row.SetValue(9, viewModel.Construction.Voltage.Title);
                    row.SetValue(10, contract.ExpectedAcceptanceMonth);

                    if (contract.InvoiceIssuanceDates.Count > 0)
                    {
                        row.SetValue(11, contract.InvoiceIssuanceDates[0]);
                    }

                    row.SetValue(12, contract.ConsultingService.Title);

                    if (contract.AcceptanceYear != null)
                    {
                        row.SetValue(13, contract.AcceptanceYear);
                    }

                    if (contract.ExpectedVolume != null)
                    {
                        row.SetValue(14, contract.ExpectedVolume);
                    }

                    if (contract.ValueBeforeVatAmount != null)
                    {
                        row.SetValue(15, contract.ValueBeforeVatAmount);
                    }

                    if (contract.AcceptanceValueBeforeVatAmount != null)
                    {
                        row.SetValue(16, contract.AcceptanceValueBeforeVatAmount);
                    }

                    if (contract.PaidAmount != null)
                    {
                        row.SetValue(25, contract.PaidAmount);
                    }

                    if (contract.AcceptanceValueBeforeVatAmount != null && contract.PaidAmount != null)
                    {
                        row.SetValue(26, contract.AcceptanceValueBeforeVatAmount - contract.PaidAmount);
                    }

                    if (contract.InvoiceIssuanceDates.Count > 0)
                    {
                        row.SetValue(27, contract.InvoiceIssuanceDates[0]);
                    }

                    row.SetValue(28, contract.AcceptancePlan);
                    row.SetValue(29, _implementationStatusDict[contract.ImplementationStatus]);
                    row.SetValue(30, _invoiceStatusDict[contract.InvoiceStatus]);

                    if (contract.DesignApprovalDate != null)
                    {
                        row.SetValue(31, contract.DesignApprovalDate);
                    }

                    row.SetValue(32, _acceptanceDocumentStatusDict[contract.AcceptanceDocumentStatus]);

                    row.SetValue(34,
                        contract.SiteSurveyRecordDate != null
                            ? contract.SiteSurveyRecordDate.Value
                            : "Chưa thực hiện");

                    row.SetValue(35,
                        contract.SurveyAcceptanceRecordDate != null
                            ? contract.SurveyAcceptanceRecordDate.Value
                            : "Chưa thực hiện");

                    row.SetValue(36, _supplementaryContractRequiredDict[contract.SupplementaryContractRequired]);

                    if (contract.InvoiceIssuanceDates.Count > 0)
                    {
                        row.SetValue(38, contract.InvoiceIssuanceDates[0].Year);
                    }

                    if (contract.InvoiceIssuanceDates.Count > 0)
                    {
                        row.SetValue(39, contract.InvoiceIssuanceDates[0]);
                    }

                    row.SetValue(40, contract.Issues);
                    row.SetValue(41, contract.Notes);
                    row.SetValue(42, contract.ContractNumber);

                    if (contract.ContractSigningDate != null)
                    {
                        row.SetValue(43, contract.ContractSigningDate.Value);
                    }

                    if (contract.ContractDurationDays != null)
                    {
                        row.SetValue(44, contract.ContractDurationDays);
                    }
                }

                var slicer1 = table.Columns[4].AddSlicer();
                slicer1.ColumnCount = 12;
                slicer1.SetPosition(0, 0, 0, 0);
                slicer1.SetSize(946, 120);

                var slicer2 = table.Columns[12].AddSlicer();
                slicer2.ColumnCount = 6;
                slicer2.SetPosition(0, 947);
                slicer2.SetSize(461, 116);

                var slicer3 = table.Columns[9].AddSlicer();
                slicer3.ColumnCount = 4;
                slicer3.SetPosition(0, 1411);
                slicer3.SetSize(300, 123);

                const string fileName = "Danh sach hop dong.xlsx";
                var outputStream = new MemoryStream();
                await package.SaveAsAsync(outputStream);
                outputStream.Position = 0;

                Log.Information(
                    "ContractHandler.{MethodName} succeeded. Input: {Input}, Result: Success, Message: Excel file generated successfully",
                    nameof(ExportExcelFile), query);

                return (null, outputStream, fileName);
            }
            catch (Exception e)
            {
                Log.Error(e, "Lỗi khi tạo file Excel cho hợp đồng");
                var errorResponse = Helper.CreateExceptionResponse(e);

                Log.Information(
                    "ContractHandler.{MethodName} failed with exception. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(ExportExcelFile), model, errorResponse.IsSuccess, errorResponse.Message);

                return (errorResponse, null, null);
            }
        }

        public async Task<Response> ImportExcelFile(IFormFile formFile, bool overwrite)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("Geneat Hardware");
                await using var fileStream = formFile.OpenReadStream();
                using var package = new ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets["Tong hop doanh thu"];
                var table = worksheet.Tables[0];
                var dataRows = table.DataRows;
                var contracts = dataRows
                    .Select(x => (Line: x.RowRange.Start.Row, Row: x, Contract: new sm_Contract
                    {
                        Id = Guid.NewGuid(),
                        Code = x.GetValue<string>(1),
                        AssignmentAYear = x.GetValue<int>(5)
                    }))
                    .ToList();
                var codes = contracts.Select(x => x.Contract.Code).ToList();
                var duplicateCodes = codes.GroupBy(x => x)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.Key)
                    .ToList();

                if (duplicateCodes.Count > 0)
                {
                    var duplicateCodesResponse = Helper.CreateBadRequestResponse(
                        $"Mã hợp đồng bị trùng: {string.Join(", ", duplicateCodes)}");

                    Log.Information(
                        "ContractHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                        nameof(ImportExcelFile), formFile?.FileName, duplicateCodesResponse.IsSuccess,
                        duplicateCodesResponse.Message);

                    return duplicateCodesResponse;
                }

                var existedContracts = await _dbContext.sm_Contract
                    .Where(x => codes.Contains(x.Code))
                    .ToListAsync();

                if (overwrite)
                {
                    _dbContext.sm_Contract.RemoveRange(existedContracts);
                }
                else
                {
                    if (existedContracts.Count > 0)
                    {
                        var existedContractsResponse = Helper.CreateBadRequestResponse(
                            $"Mã hợp đồng đã tồn tại: {string.Join(", ", existedContracts.Select(x => x.Code))}");

                        Log.Information(
                            "ContractHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ImportExcelFile), formFile?.FileName, existedContractsResponse.IsSuccess,
                            existedContractsResponse.Message);

                        return existedContractsResponse;
                    }
                }

                foreach (var tuple in contracts)
                {
                    if (!string.IsNullOrWhiteSpace(tuple.Contract.Code)) continue;
                    var emptyCodeResponse = Helper.CreateBadRequestResponse($"Mã hợp đồng tại dòng {tuple.Line} trống");

                    Log.Information(
                        "ContractHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                        nameof(ImportExcelFile), formFile?.FileName, emptyCodeResponse.IsSuccess,
                        emptyCodeResponse.Message);

                    return emptyCodeResponse;
                }

                foreach (var tuple in contracts)
                {
                    if (tuple.Contract.AssignmentAYear != 0) continue;
                    var invalidYearResponse =
                        Helper.CreateBadRequestResponse($"Năm giao A tại dòng {tuple.Line} không hợp lệ");

                    Log.Information(
                        "ContractHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                        nameof(ImportExcelFile), formFile?.FileName, invalidYearResponse.IsSuccess,
                        invalidYearResponse.Message);

                    return invalidYearResponse;
                }

                var constructionCodes = dataRows
                    .Select(x => x.GetValue<string>(2))
                    .Distinct()
                    .ToList();
                var constructions = await _dbContext.sm_Construction
                    .Where(x => constructionCodes.Contains(x.Code))
                    .ToListAsync();
                foreach (var tuple in contracts)
                {
                    var construction = constructions
                        .FirstOrDefault(x => x.Code == tuple.Row.GetValue<string>(2));
                    if (construction == null)
                    {
                        var constructionNotFoundResponse = Helper.CreateBadRequestResponse(
                            $"Không tìm thấy công trình/dự án {tuple.Row.GetValue<string>(2)} ở dòng {tuple.Line}");

                        Log.Information(
                            "ContractHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ImportExcelFile), formFile?.FileName, constructionNotFoundResponse.IsSuccess,
                            constructionNotFoundResponse.Message);

                        return constructionNotFoundResponse;
                    }

                    tuple.Contract.ConstructionId = construction.Id;

                    var templateStage = construction.TemplateStages
                        .FirstOrDefault(x => x.Name == tuple.Row.GetValue<string>(4));
                    if (templateStage == null)
                    {
                        var templateStageNotFoundResponse = Helper.CreateBadRequestResponse(
                            $"Không tìm thấy giai đoạn {tuple.Row.GetValue<string>(4)} ở dòng {tuple.Line}");

                        Log.Information(
                            "ContractHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ImportExcelFile), formFile?.FileName, templateStageNotFoundResponse.IsSuccess,
                            templateStageNotFoundResponse.Message);

                        return templateStageNotFoundResponse;
                    }

                    tuple.Contract.TemplateStageId = templateStage.Id;
                    tuple.Contract.TemplateStage = templateStage;
                }

                var consultingServiceNames = dataRows
                    .Select(x => x.GetValue<string>(12))
                    .Distinct()
                    .ToList();
                var consultingServices = await _dbContext.sm_CodeType
                    .Where(x => consultingServiceNames.Contains(x.Title))
                    .ToListAsync();
                foreach (var tuple in contracts)
                {
                    var consultingService = consultingServices
                        .FirstOrDefault(x => x.Title == tuple.Row.GetValue<string>(12));
                    if (consultingService == null)
                    {
                        var consultingServiceNotFoundResponse = Helper.CreateBadRequestResponse(
                            $"Không tìm thấy dịch vụ tư vấn {tuple.Row.GetValue<string>(12)} ở dòng {tuple.Line}");

                        Log.Information(
                            "ContractHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ImportExcelFile), formFile?.FileName, consultingServiceNotFoundResponse.IsSuccess,
                            consultingServiceNotFoundResponse.Message);

                        return consultingServiceNotFoundResponse;
                    }

                    tuple.Contract.ConsultingServiceId = consultingService.Id;
                }

                foreach (var tuple in contracts)
                {
                    tuple.Contract.ValueBeforeVatAmount = tuple.Row.GetValue<decimal>(15);
                    tuple.Contract.ExpectedVolume = tuple.Row.GetValue<decimal>(14);
                    tuple.Contract.AcceptanceValueBeforeVatAmount = tuple.Row.GetValue<decimal>(16);
                    tuple.Contract.PaidAmount = tuple.Row.GetValue<decimal>(25);
                    tuple.Contract.TaxRatePercentage = tuple.Row.GetValue<float>(24);

                    tuple.Contract.ContractNumber = tuple.Row.GetValue<string>(42);
                    tuple.Contract.ContractSigningDate = tuple.Row.GetValue<DateTime?>(43);
                    tuple.Contract.ContractDurationDays = tuple.Row.GetValue<int?>(44);
                    tuple.Contract.Issues = tuple.Row.GetValue<string>(40);
                    tuple.Contract.Notes = tuple.Row.GetValue<string>(41);

                    if (_implementationStatusDict.ContainsValue(tuple.Row.GetValue<string>(29)))
                    {
                        tuple.Contract.ImplementationStatus = Enum.Parse<ImplementationStatus>(
                            _implementationStatusDict.First(x => x.Value == tuple.Row.GetValue<string>(29)).Key
                                .ToString());
                    }
                    else
                    {
                        var implementationStatusResponse = Helper.CreateBadRequestResponse(
                            $"Tình hình thực hiện không hợp lệ ở dòng {tuple.Line}");

                        Log.Information(
                            "ContractHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ImportExcelFile), formFile?.FileName, implementationStatusResponse.IsSuccess,
                            implementationStatusResponse.Message);

                        return implementationStatusResponse;
                    }

                    tuple.Contract.ExpectedApprovalMonth = tuple.Row.GetValue<string>(7);
                    tuple.Contract.ApprovalDate = tuple.Row.GetValue<DateTime?>(8);
                    tuple.Contract.ExpectedAcceptanceMonth = tuple.Row.GetValue<string>(10);
                    tuple.Contract.InvoiceIssuanceDates = new List<DateTime?> { tuple.Row.GetValue<DateTime?>(11) }
                        .Where(x => x.HasValue)
                        .Select(x => x.Value)
                        .ToList();
                    tuple.Contract.AcceptanceYear = tuple.Row.GetValue<int?>(13);

                    if (_acceptanceDocumentStatusDict.ContainsValue(tuple.Row.GetValue<string>(32)))
                    {
                        tuple.Contract.AcceptanceDocumentStatus = Enum.Parse<AcceptanceDocumentStatus>(
                            _acceptanceDocumentStatusDict.First(x => x.Value == tuple.Row.GetValue<string>(32)).Key
                                .ToString());
                    }
                    else
                    {
                        var acceptanceDocumentStatusResponse = Helper.CreateBadRequestResponse(
                            $"Tình hình lập hồ sơ nghiệm thu không hợp lệ ở dòng {tuple.Line}");

                        Log.Information(
                            "ContractHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ImportExcelFile), formFile?.FileName, acceptanceDocumentStatusResponse.IsSuccess,
                            acceptanceDocumentStatusResponse.Message);

                        return acceptanceDocumentStatusResponse;
                    }

                    tuple.Contract.DesignApprovalDate = tuple.Row.GetValue<DateTime?>(31);

                    if (_invoiceStatusDict.ContainsValue(tuple.Row.GetValue<string>(30)))
                    {
                        tuple.Contract.InvoiceStatus = Enum.Parse<InvoiceStatus>(
                            _invoiceStatusDict.First(x => x.Value == tuple.Row.GetValue<string>(30)).Key
                                .ToString());
                    }
                    else
                    {
                        var invoiceStatusResponse = Helper.CreateBadRequestResponse(
                            $"Tình hình xuất hoá đơn không hợp lệ ở dòng {tuple.Line}");

                        Log.Information(
                            "ContractHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ImportExcelFile), formFile?.FileName, invoiceStatusResponse.IsSuccess,
                            invoiceStatusResponse.Message);

                        return invoiceStatusResponse;
                    }

                    if (tuple.Row.GetValue<string>(34) != "Chưa thực hiện")
                    {
                        tuple.Contract.SiteSurveyRecordDate = tuple.Row.GetValue<DateTime?>(34);
                    }

                    if (tuple.Row.GetValue<string>(35) != "Chưa thực hiện")
                    {
                        tuple.Contract.SurveyAcceptanceRecordDate = tuple.Row.GetValue<DateTime?>(35);
                    }

                    if (_supplementaryContractRequiredDict.ContainsValue(tuple.Row.GetValue<string>(36)))
                    {
                        tuple.Contract.SupplementaryContractRequired = Enum.Parse<SupplementaryContractRequired>(
                            _supplementaryContractRequiredDict.First(x => x.Value == tuple.Row.GetValue<string>(36)).Key
                                .ToString());
                    }
                    else
                    {
                        var supplementaryContractRequiredResponse = Helper.CreateBadRequestResponse(
                            $"Cần ký PLHĐ không hợp lệ ở dòng {tuple.Line}");

                        Log.Information(
                            "ContractHandler.{MethodName} validation failed. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ImportExcelFile), formFile?.FileName,
                            supplementaryContractRequiredResponse.IsSuccess,
                            supplementaryContractRequiredResponse.Message);

                        return supplementaryContractRequiredResponse;
                    }

                    tuple.Contract.AcceptancePlan = tuple.Row.GetValue<string>(28);
                }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                foreach (var tuple in contracts)
                {
                    tuple.Contract.CreatedByUserId = currentUser.UserId;
                    tuple.Contract.CreatedByUserName = currentUser.UserName;
                    tuple.Contract.CreatedOnDate = DateTime.Now;
                    tuple.Contract.LastModifiedByUserId = currentUser.UserId;
                    tuple.Contract.LastModifiedByUserName = currentUser.UserName;
                    tuple.Contract.LastModifiedOnDate = DateTime.Now;
                    tuple.Contract.TenantId = currentUser.TenantId;
                }

                _dbContext.sm_Contract.AddRange(contracts.Select(x => x.Contract));
                await _dbContext.SaveChangesAsync();

                var response = Helper.CreateSuccessResponse("Import thành công");

                Log.Information(
                    "ContractHandler.{MethodName} succeeded. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(ImportExcelFile), formFile?.FileName, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, "Lỗi khi nhập file hợp đồng");
                var errorResponse = Helper.CreateExceptionResponse(e);

                Log.Information(
                    "ContractHandler.{MethodName} failed with exception. Input: {Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(ImportExcelFile), formFile, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        private async Task<List<ContractAppendixItem>> ProcessAppendixAttachment(
            sm_Contract contract,
            List<ContractAppendixInputItem> inputAppendices,
            List<ContractAppendixItem> oldAppendices = null
        )
        {
            try
            {
                oldAppendices ??= new List<ContractAppendixItem>();

                var oldAppendixAttachmentIdList = oldAppendices
                    .Where(x => x.Attachment != null)
                    .Select(x => x.Attachment.Id)
                    .ToList();
                var attachmentIdList = inputAppendices
                    .Where(x => x.AttachmentId.HasValue && !oldAppendixAttachmentIdList.Contains(x.AttachmentId.Value))
                    .Select(x => x.AttachmentId.Value)
                    .ToList();
                var allAttachments = attachmentIdList.Count == 0
                    ? new List<erp_Attachment>()
                    : await _dbContext.erp_Attachment
                        .Where(x => attachmentIdList.Contains(x.Id) && !oldAppendixAttachmentIdList.Contains(x.Id))
                        .ToListAsync();

                var result = new List<ContractAppendixItem>();

                foreach (var appendixInputItem in inputAppendices)
                {
                    var appendixItem = new ContractAppendixItem
                    {
                        Content = appendixInputItem.Content
                    };

                    result.Add(appendixItem);

                    if (!appendixInputItem.AttachmentId.HasValue) continue;

                    var matchOldAppendix = oldAppendices
                        .Where(x => x.Attachment != null)
                        .Select(x => x.Attachment)
                        .FirstOrDefault(x => x.Id == appendixInputItem.AttachmentId);

                    if (matchOldAppendix != null)
                    {
                        appendixItem.Attachment = matchOldAppendix;
                        continue;
                    }

                    var matchAttachment = allAttachments.FirstOrDefault(x => x.Id == appendixInputItem.AttachmentId);
                    if (matchAttachment == null) continue;

                    var moveFileResult = _attachmentHandler.MoveEntityAttachment(matchAttachment.DocType,
                        matchAttachment.EntityType,
                        contract.Id, matchAttachment.FilePath, contract.CreatedOnDate);
                    if (!moveFileResult.IsSuccess) continue;

                    matchAttachment.EntityId = contract.Id;
                    matchAttachment.FilePath = moveFileResult.Data;
                    appendixItem.Attachment = new AppendixAttachment
                    {
                        Id = matchAttachment.Id,
                        FileName = matchAttachment.OriginalFileName,
                        FileType = matchAttachment.FileType,
                        FilePath = moveFileResult.Data
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                throw;
            }
        }

        private Expression<Func<sm_Contract, bool>> BuildQuery(ContractQueryModel query, bool queryForCount = false)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var predicate = PredicateBuilder.New<sm_Contract>(true);

            if (currentUser.TenantId != null)
            {
                predicate.And(x => x.TenantId == currentUser.TenantId);
            }

            // FullTextSearch tìm kiếm theo Code và tên của Construction
            if (!string.IsNullOrWhiteSpace(query.FullTextSearch))
            {
                predicate.And(x =>
                    x.Code.ToLower().Contains(query.FullTextSearch.Trim().ToLower()) ||
                    x.Construction.Name.ToLower().Contains(query.FullTextSearch.Trim().ToLower()) ||
                    x.AssignmentAYear.ToString() == query.FullTextSearch.Trim());
            }

            // Tìm kiếm theo Code
            if (!string.IsNullOrWhiteSpace(query.Code))
            {
                predicate.And(x => x.Code.ToLower().Contains(query.Code.Trim().ToLower()));
            }

            // Tìm kiếm theo ContractNumber
            if (!string.IsNullOrWhiteSpace(query.ContractNumber))
            {
                predicate.And(x => x.ContractNumber.ToLower().Contains(query.ContractNumber.Trim().ToLower()));
            }

            // Tìm kiếm theo ConstructionId
            if (query.ConstructionId.HasValue && query.ConstructionId.Value != Guid.Empty)
            {
                predicate.And(x => x.ConstructionId == query.ConstructionId.Value);
            }

            // Tìm kiếm theo TemplateStageId
            if (query.TemplateStageId.HasValue && query.TemplateStageId.Value != Guid.Empty)
            {
                predicate.And(x => x.TemplateStage.Id == query.TemplateStageId.Value);
            }

            // Tìm kiếm theo ConsultingServiceId
            if (query.ConsultingServiceId.HasValue && query.ConsultingServiceId.Value != Guid.Empty)
            {
                predicate.And(x => x.ConsultingServiceId == query.ConsultingServiceId.Value);
            }

            // Tìm kiếm theo AssignmentAYear
            if (query.AssignmentAYear.HasValue)
            {
                predicate.And(x => x.AssignmentAYear == query.AssignmentAYear.Value);
            }

            // Tìm kiếm theo khoảng giá trị hợp đồng (trước VAT)
            if (query.ValueBeforeVatAmountRange != null && query.ValueBeforeVatAmountRange.Length > 0)
            {
                if (query.ValueBeforeVatAmountRange[0].HasValue)
                {
                    predicate.And(x => x.ValueBeforeVatAmount >= query.ValueBeforeVatAmountRange[0].Value);
                }

                if (query.ValueBeforeVatAmountRange.Length > 1 && query.ValueBeforeVatAmountRange[1].HasValue)
                {
                    predicate.And(x => x.ValueBeforeVatAmount <= query.ValueBeforeVatAmountRange[1].Value);
                }
            }

            // Tìm kiếm theo khoảng thời gian phê duyệt
            if (query.ApprovalDateRange != null && query.ApprovalDateRange.Length > 0)
            {
                if (query.ApprovalDateRange[0].HasValue)
                {
                    predicate.And(x => x.ApprovalDate >= query.ApprovalDateRange[0].Value.Date);
                }

                if (query.ApprovalDateRange.Length > 1 && query.ApprovalDateRange[1].HasValue)
                {
                    predicate.And(x => x.ApprovalDate <= query.ApprovalDateRange[1].Value.Date.AddDays(1).AddTicks(-1));
                }
            }

            // Tìm kiếm theo khoảng thời gian xuất hóa đơn
            if (query.InvoiceIssuanceDateRange != null && query.InvoiceIssuanceDateRange.Length > 0)
            {
                if (query.InvoiceIssuanceDateRange[0].HasValue)
                {
                    predicate.And(x =>
                        x.InvoiceIssuanceDates.Any(date => date >= query.InvoiceIssuanceDateRange[0].Value.Date));
                }

                if (query.InvoiceIssuanceDateRange.Length > 1 && query.InvoiceIssuanceDateRange[1].HasValue)
                {
                    predicate.And(x => x.InvoiceIssuanceDates.Any(date =>
                        date <= query.InvoiceIssuanceDateRange[1].Value.Date.AddDays(1).AddTicks(-1)));
                }
            }

            if (!queryForCount)
            {
                // Tìm kiếm theo tình hình thực hiện
                if (!string.IsNullOrWhiteSpace(query.ImplementationStatus))
                {
                    if (Enum.TryParse<ImplementationStatus>(query.ImplementationStatus, out var implementationStatus))
                    {
                        predicate.And(x => x.ImplementationStatus == implementationStatus);
                    }
                    else
                    {
                        predicate.And(x => false);
                    }
                }

                // Tìm kiếm theo tình hình lập hồ sơ nghiệm thu
                if (!string.IsNullOrWhiteSpace(query.AcceptanceDocumentStatus))
                {
                    if (Enum.TryParse<AcceptanceDocumentStatus>(query.AcceptanceDocumentStatus,
                            out var acceptanceDocumentStatus))
                    {
                        predicate.And(x => x.AcceptanceDocumentStatus == acceptanceDocumentStatus);
                    }
                    else
                    {
                        predicate.And(x => false);
                    }
                }

                // Tìm kiếm theo tình hình xuất hoá đơn
                if (!string.IsNullOrWhiteSpace(query.InvoiceStatus))
                {
                    if (Enum.TryParse<InvoiceStatus>(query.InvoiceStatus, out var invoiceStatus))
                    {
                        predicate.And(x => x.InvoiceStatus == invoiceStatus);
                    }
                    else
                    {
                        predicate.And(x => false);
                    }
                }

                // Tìm kiếm theo cần ký PLHĐ
                if (!string.IsNullOrWhiteSpace(query.SupplementaryContractRequired))
                {
                    if (Enum.TryParse<SupplementaryContractRequired>(query.SupplementaryContractRequired,
                            out var supplementaryContractRequired))
                    {
                        predicate.And(x => x.SupplementaryContractRequired == supplementaryContractRequired);
                    }
                    else
                    {
                        predicate.And(x => false);
                    }
                }
            }

            // Tìm kiếm theo cấp điện áp
            if (!string.IsNullOrWhiteSpace(query.VoltageTypeCode))
            {
                predicate.And(x =>
                    x.Construction.VoltageTypeCode.ToLower().Contains(query.VoltageTypeCode.Trim().ToLower()));
            }

            return predicate;
        }

        private async Task<ValidationResult<T>> ValidateCreateUpdateModel<T>(
            ContractCreateUpdateModel model, Guid? id = null)
        {
            var result = new ValidationResult<T>();

            // Validate required fields
            if (string.IsNullOrWhiteSpace(model.Code))
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Mã hợp đồng không được để trống");
                return result;
            }

            if (model.ConstructionId == Guid.Empty)
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Công trình/dự án không được để trống");
                return result;
            }

            if (model.TemplateStageId == Guid.Empty)
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Giai đoạn không được để trống");
                return result;
            }

            if (model.ConsultingServiceId == Guid.Empty)
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Dịch vụ tư vấn không được để trống");
                return result;
            }

            if (model.AssignmentAYear <= 0)
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Năm giao A phải lớn hơn 0");
                return result;
            }

            if (!Enum.TryParse<ImplementationStatus>(model.ImplementationStatus, out _))
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Tình hình thực hiện không hợp lệ");
                return result;
            }

            if (!Enum.TryParse<AcceptanceDocumentStatus>(model.AcceptanceDocumentStatus, out _))
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Tình hình lập hồ sơ nghiệm thu không hợp lệ");
                return result;
            }

            if (!Enum.TryParse<InvoiceStatus>(model.InvoiceStatus, out _))
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Tình hình xuất hoá đơn không hợp lệ");
                return result;
            }

            if (!Enum.TryParse<SupplementaryContractRequired>(model.SupplementaryContractRequired, out _))
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Cần ký PLHĐ không hợp lệ");
                return result;
            }

            // Check if Code is unique
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var exists = await _dbContext.sm_Contract
                .AnyAsync(x =>
                    x.Code.ToLower() == model.Code.ToLower() && (id == null || x.Id != id) &&
                    x.TenantId == currentUser.TenantId);

            if (exists)
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Mã hợp đồng đã tồn tại");
                return result;
            }

            // Validate Construction exists
            var construction =
                await _dbContext.sm_Construction.FirstOrDefaultAsync(x => x.Id == model.ConstructionId);
            if (construction == null)
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Công trình/dự án không tồn tại");
                return result;
            }

            // Validate TemplateStage exists
            var templateStage = construction.TemplateStages.FirstOrDefault(x => x.Id == model.TemplateStageId);
            result.TemplateStage = templateStage;
            if (templateStage == null)
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Giai đoạn không tồn tại");
                return result;
            }

            // Validate ConsultingService exists and has the correct type (CodeTypeConstants.ConsultService)
            var consultingService = await _dbContext.sm_CodeType
                .FirstOrDefaultAsync(x =>
                    x.Id == model.ConsultingServiceId && x.Type == CodeTypeConstants.ConsultService);

            if (consultingService == null)
            {
                result.Response = Helper.CreateBadRequestResponse<T>("Dịch vụ tư vấn không tồn tại");
                return result;
            }

            return result;
        }

        private Expression<Func<sm_Contract, bool>> BuildQueryDebtReport(DebtReportQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var predicate = PredicateBuilder.New<sm_Contract>(true);

            if (currentUser.TenantId != null)
            {
                predicate.And(x => x.TenantId == currentUser.TenantId);
            }

            if (!string.IsNullOrWhiteSpace(query.FullTextSearch))
            {
                predicate.And(x =>
                    x.Code.ToLower().Contains(query.FullTextSearch.Trim().ToLower()) ||
                    x.Construction.Name.ToLower().Contains(query.FullTextSearch.Trim().ToLower()) ||
                    x.AssignmentAYear.ToString() == query.FullTextSearch.Trim());
            }

            if (query.MinClosingDebt.HasValue)
                predicate = predicate.And(x =>
                    x.AcceptanceValueBeforeVatAmount - x.PaidAmount >= query.MinClosingDebt.Value);

            if (query.MaxClosingDebt.HasValue)
                predicate = predicate.And(x =>
                    x.AcceptanceValueBeforeVatAmount - x.PaidAmount <= query.MaxClosingDebt.Value);


            //Query Nợ cuối kỳ khác 0(Positive)
            if (query.Positive.HasValue && query.Positive.Value)
                predicate = predicate.And(x => x.AcceptanceValueBeforeVatAmount - x.PaidAmount != 0);
            return predicate;
        }

        public async Task<Response<Pagination<DebtReportViewModel>>> GetPageDebtReport(DebtReportQueryModel query)
        {
            try
            {
                var predicate = BuildQueryDebtReport(query);
                if (query.GroupBy == "project") return await GetPageDebtReportProject(query);
                if (query.GroupBy == "investor") return await GetPageDebtReportInvestor(query);
                var queryResult = _dbContext.sm_Contract
                    .Include(x => x.Construction)
                    .ThenInclude(x => x.sm_Investor)
                    .Include(x => x.ConsultingService)
                    .Where(predicate);

                var data = await queryResult.GetPageAsync(query);

                var result = new Pagination<DebtReportViewModel>
                {
                    Page = data.Page,
                    TotalPages = data.TotalPages,
                    Size = data.Size,
                    NumberOfElements = data.NumberOfElements,
                    TotalElements = data.TotalElements,
                    Content = data.Content.Select(contract => new DebtReportViewModel
                    {
                        Id = contract.Id,
                        Code = contract.Code,
                        Name = contract.Construction?.Name,
                        InvestorTypeName = contract.Construction?.sm_Investor?.Name,
                        AcceptanceValueBeforeVatAmount = contract.AcceptanceValueBeforeVatAmount,
                        PaidAmount = contract.PaidAmount,
                        Construction = _mapper.Map<ConstructionViewModel>(contract.Construction),
                    }).ToList()
                };

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                Log.Information("Params: Query: {@query}", query);
                return Helper.CreateExceptionResponse<Pagination<DebtReportViewModel>>(e);
            }
        }

        public async Task<Response<Pagination<DebtReportViewModel>>> GetPageDebtReportProject(
            DebtReportQueryModel query)
        {
            try
            {
                var predicate = BuildQueryDebtReport(query);
                var queryResult = _dbContext.sm_Contract.Include(x => x.Construction);

                var groupedQuery = queryResult
                    .GroupBy(x => new { x.ConstructionId, x.Construction.Code, x.Construction.Name })
                    .Select(g => new DebtReportViewModel
                    {
                        Id = g.Key.ConstructionId,
                        Code = g.Key.Code,
                        Name = g.Key.Name,
                        AcceptanceValueBeforeVatAmount = g.Sum(x => x.AcceptanceValueBeforeVatAmount ?? 0),
                        PaidAmount = g.Sum(x => x.PaidAmount ?? 0)
                    });
                if (query.MinClosingDebt.HasValue)
                    groupedQuery = groupedQuery.Where(x =>
                        x.AcceptanceValueBeforeVatAmount - x.PaidAmount >= query.MinClosingDebt.Value);
                if (query.MaxClosingDebt.HasValue)
                    groupedQuery = groupedQuery.Where(x =>
                        x.AcceptanceValueBeforeVatAmount - x.PaidAmount <= query.MaxClosingDebt.Value);
                if (query.Positive.HasValue && query.Positive.Value)
                    groupedQuery = groupedQuery.Where(x => x.AcceptanceValueBeforeVatAmount - x.PaidAmount != 0);
                if (!string.IsNullOrWhiteSpace(query.FullTextSearch))
                    groupedQuery = groupedQuery.Where(x =>
                        x.Code.ToLower().Contains(query.FullTextSearch.Trim().ToLower()) ||
                        x.Name.ToLower().Contains(query.FullTextSearch.Trim().ToLower()));

                var data = await groupedQuery.GetPageAsync(query);

                var result = new Pagination<DebtReportViewModel>
                {
                    Page = data.Page,
                    TotalPages = data.TotalPages,
                    Size = data.Size,
                    NumberOfElements = data.NumberOfElements,
                    TotalElements = data.TotalElements,
                    Content = data.Content
                };

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return Helper.CreateExceptionResponse<Pagination<DebtReportViewModel>>(e);
            }
        }

        public async Task<Response<Pagination<DebtReportViewModel>>> GetPageDebtReportInvestor(
            DebtReportQueryModel query)
        {
            try
            {
                var predicate = BuildQueryDebtReport(query);
                var queryResult = _dbContext.sm_Contract.Include(x => x.Construction)
                    .ThenInclude(x => x.sm_Investor)
                    .ThenInclude(x => x.InvestorType);

                var contractsQuery = queryResult
                    .GroupBy(x => x.Construction.sm_Investor.Id)
                    .Select(g => new DebtReportViewModel
                    {
                        Code = g.First().Construction.sm_Investor.Code,
                        Name = g.First().Construction.sm_Investor.Name,
                        InvestorTypeName = g.First().Construction.sm_Investor.InvestorType.Name,
                        AcceptanceValueBeforeVatAmount = g.Sum(x => x.AcceptanceValueBeforeVatAmount ?? 0),
                        PaidAmount = g.Sum(x => x.PaidAmount ?? 0),
                    });
                if (query.MinClosingDebt.HasValue)
                    contractsQuery = contractsQuery.Where(x =>
                        x.AcceptanceValueBeforeVatAmount - x.PaidAmount >= query.MinClosingDebt.Value);
                if (query.MaxClosingDebt.HasValue)
                    contractsQuery = contractsQuery.Where(x =>
                        x.AcceptanceValueBeforeVatAmount - x.PaidAmount <= query.MaxClosingDebt.Value);
                if (query.Positive.HasValue && query.Positive.Value)
                    contractsQuery = contractsQuery.Where(x => x.AcceptanceValueBeforeVatAmount - x.PaidAmount != 0);
                if (!string.IsNullOrWhiteSpace(query.FullTextSearch))
                    contractsQuery = contractsQuery.Where(x =>
                        x.Code.ToLower().Contains(query.FullTextSearch.Trim().ToLower()) ||
                        x.Name.ToLower().Contains(query.FullTextSearch.Trim().ToLower()) ||
                        x.InvestorTypeName.ToLower().Contains(query.FullTextSearch.Trim().ToLower()));

                var data = await contractsQuery.GetPageAsync(query);

                var result = new Pagination<DebtReportViewModel>
                {
                    Page = data.Page,
                    TotalPages = data.TotalPages,
                    Size = data.Size,
                    NumberOfElements = data.NumberOfElements,
                    TotalElements = data.TotalElements,
                    Content = data.Content
                };

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                return Helper.CreateExceptionResponse<Pagination<DebtReportViewModel>>(e);
            }
        }
    }

    internal class ValidationResult<T>
    {
        public Response<T> Response { get; set; }
        public jsonb_TemplateStage TemplateStage { get; set; }
    }
}