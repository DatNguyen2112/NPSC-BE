using System.Diagnostics;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data.Data.Entity.VehicleRequest;
using Serilog;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using LinqKit;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.ActivityHistory;
using Spire.Doc;
using Document = Spire.Doc.Document;
using Newtonsoft.Json;
using NSPC.Business.Services.TaskNotification;
using NSPC.Data;
using NSPC.Data.Data.Entity.Configuration;

namespace NSPC.Business.Services.VehicleRequest
{
    /// <summary>
    /// Handler for vehicle request operations
    /// </summary>
    public class VehicleRequestHandler : IVehicleRequestHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ITaskNotificationHandler _taskNotificationHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public VehicleRequestHandler(
            SMDbContext dbContext,
            IHttpContextAccessor
                httpContextAccessor,
            IMapper mapper,
            ITaskNotificationHandler taskNotificationHandler
        )
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _taskNotificationHandler = taskNotificationHandler;
        }

        /// <summary>
        /// Create a new vehicle request
        /// </summary>
        public async Task<Response<VehicleRequestViewModel>> Create(VehicleRequestCreateUpdateModel model)
        {
            try
            {
                // Validate model
                var validationResponse = await ValidateCreateUpdateModel(model);
                if (validationResponse != null)
                {
                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                        nameof(Create), model, validationResponse.IsSuccess, validationResponse.Message);

                    return validationResponse;
                }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                // Create entity
                var entity = _mapper.Map<sm_VehicleRequest>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;
                entity.LastModifiedByUserId = entity.CreatedByUserId;
                entity.LastModifiedByUserName = entity.CreatedByUserName;
                entity.LastModifiedOnDate = entity.CreatedOnDate;
                entity.TenantId = currentUser.TenantId;
                entity.Status = VehicleRequestStatus.Draft;

                // Generate request code
                entity.RequestCode = await GenerateNewRequestCode();

                // Set names from related entities
                await SetEntityNames(entity);

                // Add activity history
                var activityHistory = new sm_ActiviyHisroty
                {
                    Id = Guid.NewGuid(),
                    EntityId = entity.Id,
                    EntityType = "VehicleRequest",
                    Action = VehicleRequestHistoryAction.CREATE,
                    Description = null,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    TenantId = currentUser.TenantId
                };

                // Save to database
                _dbContext.sm_VehicleRequest.Add(entity);
                _dbContext.sm_ActiviyHisroty.Add(activityHistory);
                await _dbContext.SaveChangesAsync();

                var response = Helper.CreateSuccessResponse(_mapper.Map<VehicleRequestViewModel>(entity),
                    "Tạo yêu cầu xe thành công");

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Create), model, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<VehicleRequestViewModel>(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Create), model, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        /// <summary>
        /// Update an existing vehicle request
        /// </summary>
        public async Task<Response<VehicleRequestViewModel>> Update(Guid id, VehicleRequestCreateUpdateModel model)
        {
            try
            {
                // Find entity
                var entity = await _dbContext.sm_VehicleRequest.FindAsync(id);
                if (entity == null)
                {
                    var notFoundResponse =
                        Helper.CreateNotFoundResponse<VehicleRequestViewModel>("Không tìm thấy yêu cầu xe");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - entity not found. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                        nameof(Update), id, model, notFoundResponse.IsSuccess, notFoundResponse.Message);

                    return notFoundResponse;
                }

                // Check if request can be updated
                if (entity.Status == VehicleRequestStatus.Approved)
                {
                    var badRequestResponse = Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                        "Không thể cập nhật yêu cầu xe đã được duyệt");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                        nameof(Update), id, model, badRequestResponse.IsSuccess, badRequestResponse.Message);

                    return badRequestResponse;
                }

                // Validate model
                var validationResponse = await ValidateCreateUpdateModel(model);
                if (validationResponse != null)
                {
                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                        nameof(Update), id, model, validationResponse.IsSuccess, validationResponse.Message);

                    return validationResponse;
                }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                // Update entity
                _mapper.Map(model, entity);
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                // Set names from related entities
                await SetEntityNames(entity);

                // Add activity history
                var activityHistory = new sm_ActiviyHisroty
                {
                    Id = Guid.NewGuid(),
                    EntityId = entity.Id,
                    EntityType = "VehicleRequest",
                    Action = VehicleRequestHistoryAction.UPDATE,
                    Description = null,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    TenantId = currentUser.TenantId
                };
                _dbContext.sm_ActiviyHisroty.Add(activityHistory);

                // Save to database
                await _dbContext.SaveChangesAsync();

                var response = Helper.CreateSuccessResponse(_mapper.Map<VehicleRequestViewModel>(entity),
                    "Cập nhật yêu cầu xe thành công");

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Update), id, model, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<VehicleRequestViewModel>(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Update), id, model, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        /// <summary>
        /// Get a vehicle request by ID with activity history
        /// </summary>
        public async Task<Response<VehicleRequestDetailViewModel>> GetById(Guid id)
        {
            try
            {
                // Find entity with related data
                var entity = await _dbContext.sm_VehicleRequest
                    .Include(x => x.User)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.RequestedVehicle)
                    .ThenInclude(x => x.LoaiXe)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    var notFoundResponse =
                        Helper.CreateNotFoundResponse<VehicleRequestDetailViewModel>("Không tìm thấy yêu cầu xe");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - entity not found. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(GetById), id, notFoundResponse.IsSuccess, notFoundResponse.Message);

                    return notFoundResponse;
                }

                // Get activity history for this vehicle request
                var activityHistories = await _dbContext.sm_ActiviyHisroty
                    .Where(x => x.EntityId == id && x.EntityType == "VehicleRequest")
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

                // Get other vehicle requests in the same sharing group (if any)
                List<sm_VehicleRequest> sharingGroupRequests = new();
                if (entity.SharingGroupId.HasValue)
                {
                    sharingGroupRequests = await _dbContext.sm_VehicleRequest
                        .Include(x => x.User)
                        .Where(x => x.SharingGroupId == entity.SharingGroupId && x.Id != id)
                        .OrderBy(x => x.CreatedOnDate)
                        .ToListAsync();
                }

                // Map to detail view model
                var result = _mapper.Map<VehicleRequestDetailViewModel>(entity);

                result.ActivityHistories = _mapper.Map<List<ActivityHistoryViewModel>>(activityHistories);
                result.ActivityHistories.ForEach(x =>
                {
                    x.CreatedByUserFullName = users[x.CreatedByUserId];
                    x.LastModifiedByFullName = users[x.LastModifiedByUserId!.Value];
                });

                result.SharingGroupRequests = _mapper.Map<List<VehicleRequestViewModel>>(sharingGroupRequests);

                var response = Helper.CreateSuccessResponse(result);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(GetById), id, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<VehicleRequestDetailViewModel>(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(GetById), id, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        /// <summary>
        /// Get a paginated list of vehicle requests
        /// </summary>
        public async Task<Response<Pagination<VehicleRequestViewModel>>> GetPage(VehicleRequestQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryable = _dbContext.sm_VehicleRequest
                    .Include(x => x.User)
                    .AsNoTracking()
                    .AsQueryable();

                if (query.IncludeVehicle)
                {
                    queryable = queryable
                        .Include(x => x.RequestedVehicle)
                        .ThenInclude(x => x.LoaiXe);
                }

                var queryResult = queryable
                    .Where(predicate)
                    .OrderByDescending(x => x.CreatedOnDate);

                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<VehicleRequestViewModel>>(data);

                var response = Helper.CreateSuccessResponse(result);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(GetPage), query, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<Pagination<VehicleRequestViewModel>>(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(GetPage), query, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        /// <summary>
        /// Delete a vehicle request
        /// </summary>
        public async Task<Response> Delete(Guid id)
        {
            try
            {
                // Find entity
                var entity = await _dbContext.sm_VehicleRequest.FindAsync(id);
                if (entity == null)
                {
                    var notFoundResponse = Helper.CreateNotFoundResponse("Không tìm thấy yêu cầu xe");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - entity not found. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(Delete), id, notFoundResponse.IsSuccess, notFoundResponse.Message);

                    return notFoundResponse;
                }

                // Check if request can be deleted
                if (entity.Status is VehicleRequestStatus.WaitingForSharing or VehicleRequestStatus.PendingApproval)
                {
                    var badRequestResponse =
                        Helper.CreateBadRequestResponse("Không thể xóa yêu cầu xe đang chờ duyệt hoặc chờ ghép xe");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(Delete), id, badRequestResponse.IsSuccess, badRequestResponse.Message);

                    return badRequestResponse;
                }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                // Handle sharing group logic if the request being deleted is part of a sharing group
                if (entity.SharingGroupId.HasValue)
                {
                    // Get other requests in the same sharing group
                    var otherRequestsInGroup = await _dbContext.sm_VehicleRequest
                        .Where(x => x.SharingGroupId == entity.SharingGroupId && x.Id != id)
                        .ToListAsync();

                    // If after deletion there will be only 1 request left, remove SharingGroupId from that request
                    if (otherRequestsInGroup.Count == 1)
                    {
                        var lastRequest = otherRequestsInGroup.First();
                        lastRequest.SharingGroupId = null;
                        lastRequest.LastModifiedByUserId = currentUser.UserId;
                        lastRequest.LastModifiedByUserName = currentUser.UserName;
                        lastRequest.LastModifiedOnDate = DateTime.Now;
                    }
                }

                // Delete from database
                _dbContext.sm_VehicleRequest.Remove(entity);

                // Add activity history
                var activityHistory = new sm_ActiviyHisroty
                {
                    Id = Guid.NewGuid(),
                    EntityId = entity.Id,
                    EntityType = "VehicleRequest",
                    Action = VehicleRequestHistoryAction.DELETE,
                    Description = null,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    TenantId = currentUser.TenantId
                };

                _dbContext.sm_ActiviyHisroty.Add(activityHistory);
                await _dbContext.SaveChangesAsync();

                var response = Helper.CreateSuccessResponse("Xóa yêu cầu xe thành công");

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Delete), id, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(Delete), id, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        /// <summary>
        /// Approve or reject a vehicle request
        /// </summary>
        public async Task<Response<List<VehicleRequestViewModel>>> ProcessApproval(Guid id,
            VehicleRequestApprovalModel model)
        {
            try
            {
                // Find entity
                var entity = await _dbContext.sm_VehicleRequest.FindAsync(id);
                if (entity == null)
                {
                    var notFoundResponse =
                        Helper.CreateNotFoundResponse<List<VehicleRequestViewModel>>("Không tìm thấy yêu cầu xe");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - entity not found. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                        nameof(ProcessApproval), id, model, notFoundResponse.IsSuccess, notFoundResponse.Message);

                    return notFoundResponse;
                }

                // Check if request can be processed for approval
                if (entity.Status != VehicleRequestStatus.PendingApproval)
                {
                    var badRequestResponse = Helper.CreateBadRequestResponse<List<VehicleRequestViewModel>>(
                        "Chỉ có thể phê duyệt hoặc từ chối yêu cầu xe ở trạng thái Chờ duyệt");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                        nameof(ProcessApproval), id, model, badRequestResponse.IsSuccess, badRequestResponse.Message);

                    return badRequestResponse;
                }

                // Check for vehicle conflicts if RequestedVehicleId is specified
                if (model.IsApproved && entity.RequestedVehicleId.HasValue)
                {
                    var startDate = entity.StartDateTime.Date;
                    var endDate = entity.EndDateTime.Date;
                    var conflictingRequests = await _dbContext.sm_VehicleRequest
                        .Include(x => x.User)
                        .Include(x => x.RequestedVehicle)
                        .ThenInclude(x => x.LoaiXe)
                        .Where(x =>
                            x.Id != id &&
                            x.RequestedVehicleId == entity.RequestedVehicleId &&
                            ((startDate >= x.StartDateTime && startDate <= x.EndDateTime) ||
                             (endDate >= x.StartDateTime && endDate <= x.EndDateTime)) ||
                            ((startDate <= x.StartDateTime && endDate >= x.StartDateTime) ||
                             (startDate <= x.EndDateTime && endDate >= x.EndDateTime)) &&
                            (x.Status == VehicleRequestStatus.Approved ||
                             x.Status == VehicleRequestStatus.Shared)
                        )
                        .ToListAsync();
                    conflictingRequests = conflictingRequests
                        .Where(x => x.RequestedVehicleId == entity.RequestedVehicleId)
                        .ToList();

                    if (conflictingRequests.Any())
                    {
                        var shareable = conflictingRequests
                            .Where(x => x.StartDateTime == startDate)
                            .ToList();
                        var conflictResponse = new Response<List<VehicleRequestViewModel>>(
                            HttpStatusCode.BadRequest,
                            _mapper.Map<List<VehicleRequestViewModel>>(shareable),
                            $"Yêu cầu bị trùng xe và ngày với {conflictingRequests.Count} yêu cầu khác");

                        Log.Information(
                            "VehicleRequestHandler.{MethodName} validation failed - vehicle conflicts found. Input: Id={Id}, ConflictCount: {ConflictCount}, ShareableCount: {ShareableCount}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ProcessApproval), id, conflictingRequests.Count, shareable.Count,
                            conflictResponse.IsSuccess,
                            conflictResponse.Message);

                        return conflictResponse;
                    }
                }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                if (model.IsApproved)
                {
                    entity.Status = VehicleRequestStatus.Approved;
                    entity.RejectNotes = null;
                }
                else
                {
                    // Check if reject notes is provided when rejecting
                    if (string.IsNullOrWhiteSpace(model.RejectNotes))
                    {
                        var rejectNotesResponse = Helper.CreateBadRequestResponse<List<VehicleRequestViewModel>>(
                            "Vui lòng nhập lý do từ chối");

                        Log.Information(
                            "VehicleRequestHandler.{MethodName} validation failed. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ProcessApproval), id, model, rejectNotesResponse.IsSuccess,
                            rejectNotesResponse.Message);

                        return rejectNotesResponse;
                    }

                    entity.Status = VehicleRequestStatus.Rejected;
                    entity.RejectNotes = model.RejectNotes;
                }

                // Update status
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                // Add activity history
                var activityHistory = new sm_ActiviyHisroty
                {
                    Id = Guid.NewGuid(),
                    EntityId = entity.Id,
                    EntityType = "VehicleRequest",
                    Action =
                        model.IsApproved ? VehicleRequestHistoryAction.APPROVE : VehicleRequestHistoryAction.REJECT,
                    Description = model.IsApproved ? null : model.RejectNotes,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    TenantId = currentUser.TenantId
                };
                _dbContext.sm_ActiviyHisroty.Add(activityHistory);

                var me = await _dbContext.IdmUser.FirstOrDefaultAsync(x => x.Id == currentUser.UserId);
                var notification = new sm_TaskNotification
                {
                    Id = Guid.NewGuid(),
                    UserId = entity.UserId,
                    NotificationStatus = model.IsApproved
                        ? NotificationStatus.VehicleRequestApprove
                        : NotificationStatus.VehicleRequestReject,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.FullName,
                    CreatedOnDate = DateTime.Now,
                    AvatarUrl = Utils.FetchHost(me.AvatarUrl),
                    AdditionalData = new List<string> { entity.DestinationLocation, entity.Id.ToString() }
                };
                _dbContext.sm_TaskNotification.Add(notification);
                await _taskNotificationHandler.CreatePushNotification(
                    _mapper.Map<TaskNotificationViewModel>(notification));

                // Save to database
                await _dbContext.SaveChangesAsync();

                string message = model.IsApproved
                    ? "Phê duyệt yêu cầu xe thành công"
                    : "Từ chối yêu cầu xe thành công";

                var response = Helper.CreateSuccessResponse(new List<VehicleRequestViewModel>(), message);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                    nameof(ProcessApproval), id, model, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<List<VehicleRequestViewModel>>(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Input: Id={Id}, Model={@Model}, Result: {IsSuccess}, Message: {Message}",
                    nameof(ProcessApproval), id, model, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        /// <summary>
        /// Submit a vehicle request for approval
        /// </summary>
        public async Task<Response<List<VehicleRequestViewModel>>> SubmitForApproval(Guid id)
        {
            try
            {
                // Find entity
                var entity = await _dbContext.sm_VehicleRequest.FindAsync(id);
                if (entity == null)
                {
                    var notFoundResponse =
                        Helper.CreateNotFoundResponse<List<VehicleRequestViewModel>>("Không tìm thấy yêu cầu xe");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - entity not found. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(SubmitForApproval), id, notFoundResponse.IsSuccess, notFoundResponse.Message);

                    return notFoundResponse;
                }

                // Check if request can be submitted for approval
                if (entity.Status != VehicleRequestStatus.Draft && entity.Status != VehicleRequestStatus.Rejected)
                {
                    var badRequestResponse = Helper.CreateBadRequestResponse<List<VehicleRequestViewModel>>(
                        "Chỉ có thể gửi duyệt yêu cầu xe ở trạng thái Nháp hoặc Từ chối");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(SubmitForApproval), id, badRequestResponse.IsSuccess, badRequestResponse.Message);

                    return badRequestResponse;
                }

                // Check for vehicle conflicts if RequestedVehicleId is specified
                if (entity.RequestedVehicleId.HasValue)
                {
                    var startDate = entity.StartDateTime.Date;
                    var endDate = entity.EndDateTime.Date;
                    var conflictingRequests = await _dbContext.sm_VehicleRequest
                        .Include(x => x.User)
                        .Include(x => x.RequestedVehicle)
                        .ThenInclude(x => x.LoaiXe)
                        .Where(x =>
                            x.Id != id &&
                            x.RequestedVehicleId == entity.RequestedVehicleId &&
                            ((startDate >= x.StartDateTime && startDate <= x.EndDateTime) ||
                             (endDate >= x.StartDateTime && endDate <= x.EndDateTime)) ||
                            ((startDate <= x.StartDateTime && endDate >= x.StartDateTime) ||
                             (startDate <= x.EndDateTime && endDate >= x.EndDateTime)) &&
                            (x.Status == VehicleRequestStatus.Approved ||
                             x.Status == VehicleRequestStatus.Shared)
                        )
                        .ToListAsync();
                    conflictingRequests = conflictingRequests
                        .Where(x => x.RequestedVehicleId == entity.RequestedVehicleId)
                        .ToList();

                    if (conflictingRequests.Any())
                    {
                        var shareable = conflictingRequests
                            .Where(x => x.StartDateTime == startDate)
                            .ToList();
                        var conflictResponse = new Response<List<VehicleRequestViewModel>>(
                            HttpStatusCode.BadRequest,
                            _mapper.Map<List<VehicleRequestViewModel>>(shareable),
                            $"Yêu cầu bị trùng với {conflictingRequests.Count} yêu cầu khác cùng xe và ngày");

                        Log.Information(
                            "VehicleRequestHandler.{MethodName} validation failed - vehicle conflicts found. Input: Id={Id}, ConflictCount: {ConflictCount}, ShareableCount: {ShareableCount}, Result: {IsSuccess}, Message: {Message}",
                            nameof(SubmitForApproval), id, conflictingRequests.Count, shareable.Count,
                            conflictResponse.IsSuccess,
                            conflictResponse.Message);

                        return conflictResponse;
                    }
                }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                // Update status
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.Status = VehicleRequestStatus.PendingApproval;
                entity.RejectNotes = null;

                // Add activity history
                var activityHistory = new sm_ActiviyHisroty
                {
                    Id = Guid.NewGuid(),
                    EntityId = entity.Id,
                    EntityType = "VehicleRequest",
                    Action = VehicleRequestHistoryAction.SUBMIT,
                    Description = null,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    TenantId = currentUser.TenantId
                };
                _dbContext.sm_ActiviyHisroty.Add(activityHistory);

                var receiverIds = new List<Guid>();
                var configEntity = await _dbContext.sm_Configuration
                    .FirstOrDefaultAsync(x => x.Key == "VR_EXPORT" && x.TenantId == currentUser.TenantId);

                if (configEntity != null && !string.IsNullOrWhiteSpace(configEntity.Value))
                {
                    try
                    {
                        var config = JsonConvert.DeserializeObject<VehicleRequestExportConfig>(configEntity.Value);
                        if (config.VehicleCoordinatorId.HasValue)
                        {
                            receiverIds.Add(config.VehicleCoordinatorId.Value);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                if (receiverIds.Count > 0)
                {
                    var me = await _dbContext.IdmUser.FirstOrDefaultAsync(x => x.Id == currentUser.UserId);
                    foreach (var receiverId in receiverIds)
                    {
                        var notification = new sm_TaskNotification
                        {
                            Id = Guid.NewGuid(),
                            UserId = receiverId,
                            NotificationStatus = NotificationStatus.VehicleRequestSendApproval,
                            CreatedByUserId = currentUser.UserId,
                            CreatedByUserName = currentUser.FullName,
                            CreatedOnDate = DateTime.Now,
                            AvatarUrl = Utils.FetchHost(me.AvatarUrl),
                            AdditionalData = new List<string> { entity.DestinationLocation, entity.Id.ToString() }
                        };
                        _dbContext.sm_TaskNotification.Add(notification);
                        await _taskNotificationHandler.CreatePushNotification(
                            _mapper.Map<TaskNotificationViewModel>(notification));
                    }
                }

                // Save to database
                await _dbContext.SaveChangesAsync();

                var response = Helper.CreateSuccessResponse(new List<VehicleRequestViewModel>(),
                    "Gửi duyệt yêu cầu xe thành công");

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(SubmitForApproval), id, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<List<VehicleRequestViewModel>>(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(SubmitForApproval), id, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        /// <summary>
        /// Xuất yêu cầu xin xe ra file PDF
        /// </summary>
        /// <param name="id">ID của yêu cầu xin xe</param>
        /// <returns>
        /// Tuple gồm 3 phần tử:
        /// - Item1: Response lỗi (null nếu thành công)
        /// - Item2: Stream chứa dữ liệu PDF (null nếu có lỗi)
        /// - Item3: Tên file PDF (null nếu có lỗi)
        /// </returns>
        public async Task<(Response, Stream, string)> GetRequestPdf(Guid id)
        {
            try
            {
                // Lấy đường dẫn đến file template Word
                var templateFilePath = Utils.CombineUnixPath(
                    ConfigCollection.Instance.StaticFiles_Folder,
                    "excel-template",
                    "VehicleRequestTemplate.docx");

                // Kiểm tra file template tồn tại
                if (!File.Exists(templateFilePath))
                {
                    Log.Error("Vehicle Request Template Not Found: {Path}", templateFilePath);
                    var templateNotFoundResponse = Helper.CreateExceptionResponse("Không tìm thấy file template");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - template not found. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(GetRequestPdf), id, templateNotFoundResponse.IsSuccess,
                        templateNotFoundResponse.Message);

                    return (templateNotFoundResponse, null, null);
                }

                Log.Information("Vehicle Request Template Path: {Path}", templateFilePath);

                // Lấy thông tin yêu cầu xin xe từ database
                var vehicleRequest = await _dbContext.sm_VehicleRequest
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (vehicleRequest == null)
                {
                    var notFoundResponse = Helper.CreateNotFoundResponse("Không tìm thấy yêu cầu xin xe");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - entity not found. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(GetRequestPdf), id, notFoundResponse.IsSuccess, notFoundResponse.Message);

                    return (notFoundResponse, null, null);
                }

                // Lấy config xuất báo cáo
                var exportConfig = await GetExportConfig();
                if (!exportConfig.IsSuccess)
                {
                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - export config error. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(GetRequestPdf), id, exportConfig.IsSuccess, exportConfig.Message);

                    return (exportConfig, null, null);
                }

                // Lấy chức vụ của phụ trách điều xe
                string vehicleCoordinatorPosition = null;
                if (exportConfig.Data.VehicleCoordinatorId.HasValue)
                {
                    vehicleCoordinatorPosition = await _dbContext.IdmUser
                        .Where(x => x.Id == exportConfig.Data.VehicleCoordinatorId)
                        .Select(x => x.mk_ChucVu.TenChucVu)
                        .FirstOrDefaultAsync();
                }

                // Lấy tên của tổ trưởng
                var teamLeaderName = "";
                if (!string.IsNullOrWhiteSpace(vehicleRequest.User.MaTo))
                {
                    teamLeaderName = await _dbContext.IdmUser
                        .Where(x => x.IdChucVu != null && x.mk_ChucVu.MaChucVu == "TT" &&
                                    vehicleRequest.User.MaTo == x.MaTo)
                        .Select(x => x.Name)
                        .FirstOrDefaultAsync();
                    teamLeaderName ??= "";
                }

                // Đọc file template vào memory stream
                var templateFileBytes = await File.ReadAllBytesAsync(templateFilePath);
                using var templateDataStream = new MemoryStream();
                templateDataStream.Write(templateFileBytes, 0, templateFileBytes.Length);
                templateDataStream.Position = 0;

                // Thay thế các placeholder trong template bằng dữ liệu thực tế
                if (!FillDocumentTemplate(templateDataStream, vehicleRequest, exportConfig.Data,
                        vehicleCoordinatorPosition, teamLeaderName))
                {
                    var fillTemplateErrorResponse = Helper.CreateExceptionResponse("Lỗi khi điền dữ liệu vào template");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - template fill error. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(GetRequestPdf), id, fillTemplateErrorResponse.IsSuccess,
                        fillTemplateErrorResponse.Message);

                    return (fillTemplateErrorResponse, null, null);
                }

                // Chuyển đổi file Word thành PDF
                var pdfStream = ConvertDocxToPdf(templateDataStream);
                if (pdfStream == null)
                {
                    var convertToPdfErrorResponse = Helper.CreateExceptionResponse("Lỗi khi chuyển đổi file sang PDF");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - PDF conversion error. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(GetRequestPdf), id, convertToPdfErrorResponse.IsSuccess,
                        convertToPdfErrorResponse.Message);

                    return (convertToPdfErrorResponse, null, null);
                }

                // Trả về stream PDF và tên file
                var fileName = $"Phieu xin xe {vehicleRequest.RequestCode}.pdf";

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Input: Id={Id}, Result: Success, Message: PDF generated successfully",
                    nameof(GetRequestPdf), id);

                return (null, pdfStream, fileName);
            }
            catch (Exception e)
            {
                Log.Error(e, "Lỗi khi tạo file PDF cho yêu cầu xin xe");
                var errorResponse = Helper.CreateExceptionResponse(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(GetRequestPdf), id, errorResponse.IsSuccess, errorResponse.Message);

                return (errorResponse, null, null);
            }
        }

        /// <summary>
        /// Điền dữ liệu từ đối tượng VehicleRequest vào template Word
        /// </summary>
        /// <param name="templateStream">Stream chứa template Word</param>
        /// <param name="vehicleRequest">Đối tượng yêu cầu xin xe</param>
        /// <param name="exportConfig">Cấu hình xuất báo cáo</param>
        /// <param name="vehicleCoordinatorPosition">Chức vụ của phụ trách điều xe</param>
        /// <param name="teamLeaderName">Tên của tổ trưởng</param>
        /// <returns>True nếu thành công, False nếu có lỗi</returns>
        private static bool FillDocumentTemplate(
            MemoryStream templateStream,
            sm_VehicleRequest vehicleRequest,
            VehicleRequestExportConfig exportConfig,
            string vehicleCoordinatorPosition,
            string teamLeaderName
        )
        {
            try
            {
                // Đảm bảo stream ở vị trí đầu để đọc và ghi
                templateStream.Position = 0;

                using (var templateDoc = WordprocessingDocument.Open(templateStream, true))
                {
                    var body = templateDoc.MainDocumentPart?.Document.Body;
                    if (body == null)
                    {
                        Log.Error("Template document has no body content");
                        return false;
                    }

                    // Lấy tất cả các đoạn văn bản trong document
                    var texts = body.Descendants<Text>()
                        .Where(text => text.Text.StartsWith('[') && text.Text.EndsWith(']'))
                        .ToList();

                    foreach (var text in texts)
                    {
                        // Kiểm tra xem text có phải là placeholder hay không (placeholder có dạng [PropertyName])
                        if (!text.Text.StartsWith('[') || !text.Text.EndsWith(']')) continue;

                        // Lấy tên thuộc tính từ placeholder (bỏ dấu [ và ])
                        var propertyName = text.Text[1..^1];

                        // Lấy giá trị thuộc tính từ đối tượng VehicleRequest
                        var propertyValue = vehicleRequest.GetPropValue(propertyName);

                        switch (propertyValue)
                        {
                            // Định dạng đặc biệt cho kiểu DateTime
                            case DateTime dateTime:
                                propertyValue = dateTime.ToString("dd/MM/yyyy");
                                break;
                            // Nếu là string, không rỗng và không kết thúc bằng dấu chấm thì thêm dấu chấm
                            case string strValue when
                                !string.IsNullOrEmpty(strValue) &&
                                !strValue.EndsWith("."):
                                propertyValue = strValue + ".";
                                break;
                        }

                        if (propertyValue == null)
                        {
                            switch (propertyName)
                            {
                                case "CompanyNameConfig":
                                    propertyValue = exportConfig.CompanyName ?? "";
                                    break;
                                case "DepartmentNameConfig":
                                    propertyValue = exportConfig.DepartmentName ?? "";
                                    break;
                                case "CurrentDate":
                                    var today = DateTime.Now;
                                    propertyValue = $"ngày {today.Day} tháng {today.Month} năm {today.Year}";
                                    break;
                                case "VehicleCoordinatorPositionConfig":
                                    propertyValue = vehicleCoordinatorPosition ?? "";
                                    break;
                                case "VehicleCoordinatorConfig":
                                    propertyValue = exportConfig.VehicleCoordinator ?? "";
                                    break;
                                case "TeamLeaderConfig":
                                    propertyValue = teamLeaderName ?? "";
                                    break;
                                case "Requester":
                                    propertyValue = vehicleRequest.UserName;
                                    break;
                            }
                        }

                        if (propertyValue == null)
                        {
                            continue;
                        }

                        // Thay thế placeholder bằng giá trị thực tế
                        text.Text = text.Text.Replace($"[{propertyName}]", propertyValue.ToString());
                    }

                    // Lưu các thay đổi vào document
                    templateDoc.MainDocumentPart.Document.Save();
                }

                // Đặt lại vị trí stream về đầu để sử dụng tiếp
                templateStream.Position = 0;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error filling document template");
                return false;
            }
        }

        /// <summary>
        /// Chuyển đổi file Word thành PDF
        /// </summary>
        /// <param name="docxStream">Stream chứa file Word</param>
        /// <returns>Stream chứa file PDF hoặc null nếu có lỗi</returns>
        private static MemoryStream ConvertDocxToPdf(MemoryStream docxStream)
        {
            try
            {
                // Đảm bảo stream ở vị trí đầu
                docxStream.Position = 0;

                // Sử dụng FreeSpire.Doc để chuyển đổi Word sang PDF
                using var wordDocument = new Document();
                wordDocument.LoadFromStream(docxStream, FileFormat.Docx);

                // Lưu PDF vào memory stream mới
                var outputStream = new MemoryStream();
                wordDocument.SaveToStream(outputStream, FileFormat.PDF);
                outputStream.Position = 0;

                return outputStream;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error converting DOCX to PDF");
                return null;
            }
        }

        /// <summary>
        /// Submit vehicle sharing request
        /// </summary>
        public async Task<Response<VehicleRequestViewModel>> SubmitVehicleSharing(Guid draftRequestId,
            List<Guid> approvedRequestIds)
        {
            try
            {
                // Validate input
                if (approvedRequestIds == null || !approvedRequestIds.Any())
                {
                    var invalidInputResponse = Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                        "Danh sách yêu cầu xe để ghép không được để trống");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: DraftRequestId={DraftRequestId}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                        nameof(SubmitVehicleSharing), draftRequestId, approvedRequestIds,
                        invalidInputResponse.IsSuccess, invalidInputResponse.Message);

                    return invalidInputResponse;
                }

                // Find draft request
                var draftRequest = await _dbContext.sm_VehicleRequest.FindAsync(draftRequestId);
                if (draftRequest == null)
                {
                    var notFoundResponse =
                        Helper.CreateNotFoundResponse<VehicleRequestViewModel>("Không tìm thấy yêu cầu xe nháp");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - draft request not found. Input: DraftRequestId={DraftRequestId}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                        nameof(SubmitVehicleSharing), draftRequestId, approvedRequestIds, notFoundResponse.IsSuccess,
                        notFoundResponse.Message);

                    return notFoundResponse;
                }

                // Check if draft request is in Draft status
                if (draftRequest.Status != VehicleRequestStatus.Draft &&
                    draftRequest.Status != VehicleRequestStatus.Rejected)
                {
                    var invalidStatusResponse = Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                        "Chỉ có thể gửi duyệt ghép xe cho yêu cầu ở trạng thái Nháp hoặc Từ chối");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: DraftRequestId={DraftRequestId}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                        nameof(SubmitVehicleSharing), draftRequestId, approvedRequestIds,
                        invalidStatusResponse.IsSuccess, invalidStatusResponse.Message);

                    return invalidStatusResponse;
                }

                // Find approved/shared requests
                var approvedRequests = await _dbContext.sm_VehicleRequest
                    .Where(x => approvedRequestIds.Contains(x.Id))
                    .ToListAsync();

                // Check if all approved requests exist
                if (approvedRequests.Count != approvedRequestIds.Count)
                {
                    var missingRequestsResponse = Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                        "Một hoặc nhiều yêu cầu xe để ghép không tồn tại");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: DraftRequestId={DraftRequestId}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                        nameof(SubmitVehicleSharing), draftRequestId, approvedRequestIds,
                        missingRequestsResponse.IsSuccess, missingRequestsResponse.Message);

                    return missingRequestsResponse;
                }

                // Check if all approved requests are in Approved or Shared status
                var invalidStatusRequests = approvedRequests
                    .Where(x => x.Status != VehicleRequestStatus.Approved && x.Status != VehicleRequestStatus.Shared)
                    .ToList();

                if (invalidStatusRequests.Any())
                {
                    var invalidApprovedStatusResponse = Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                        "Các yêu cầu xe để ghép phải ở trạng thái Đã duyệt hoặc Đã ghép xe");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: DraftRequestId={DraftRequestId}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                        nameof(SubmitVehicleSharing), draftRequestId, approvedRequestIds,
                        invalidApprovedStatusResponse.IsSuccess, invalidApprovedStatusResponse.Message);

                    return invalidApprovedStatusResponse;
                }

                // Check if draft request has vehicle specified
                if (!draftRequest.RequestedVehicleId.HasValue)
                {
                    var noVehicleResponse = Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                        "Yêu cầu xe nháp phải chỉ định xe để có thể ghép");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: DraftRequestId={DraftRequestId}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                        nameof(SubmitVehicleSharing), draftRequestId, approvedRequestIds, noVehicleResponse.IsSuccess,
                        noVehicleResponse.Message);

                    return noVehicleResponse;
                }

                // Check if all approved requests have the same vehicle and start date as draft request
                var draftVehicleId = draftRequest.RequestedVehicleId.Value;
                var draftStartDate = draftRequest.StartDateTime.Date;

                var conflictingRequests = approvedRequests
                    .Where(x => x.RequestedVehicleId != draftVehicleId ||
                                x.StartDateTime.Date != draftStartDate)
                    .ToList();

                if (conflictingRequests.Any())
                {
                    var inconsistentRequestsResponse = Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                        "Tất cả các yêu cầu xe phải cùng xe và cùng ngày bắt đầu mới có thể ghép");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: DraftRequestId={DraftRequestId}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                        nameof(SubmitVehicleSharing), draftRequestId, approvedRequestIds,
                        inconsistentRequestsResponse.IsSuccess, inconsistentRequestsResponse.Message);

                    return inconsistentRequestsResponse;
                }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                // Determine SharingGroupId
                Guid sharingGroupId;
                var existingSharingGroupId =
                    approvedRequests.FirstOrDefault(x => x.SharingGroupId.HasValue)?.SharingGroupId;

                if (existingSharingGroupId.HasValue)
                {
                    // Use existing SharingGroupId
                    sharingGroupId = existingSharingGroupId.Value;
                }
                else
                {
                    // Create new SharingGroupId and assign to all approved requests
                    sharingGroupId = Guid.NewGuid();

                    foreach (var approvedRequest in approvedRequests)
                    {
                        approvedRequest.SharingGroupId = sharingGroupId;
                        approvedRequest.LastModifiedByUserId = currentUser.UserId;
                        approvedRequest.LastModifiedByUserName = currentUser.UserName;
                        approvedRequest.LastModifiedOnDate = DateTime.Now;
                    }
                }

                // Update draft request
                draftRequest.SharingGroupId = sharingGroupId;
                draftRequest.Status = VehicleRequestStatus.WaitingForSharing;
                draftRequest.LastModifiedByUserId = currentUser.UserId;
                draftRequest.LastModifiedByUserName = currentUser.UserName;
                draftRequest.LastModifiedOnDate = DateTime.Now;

                // Add activity history for draft request
                var activityHistory = new sm_ActiviyHisroty
                {
                    Id = Guid.NewGuid(),
                    EntityId = draftRequest.Id,
                    EntityType = "VehicleRequest",
                    Action = VehicleRequestHistoryAction.SUBMIT_SHARING,
                    Description = null,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    TenantId = currentUser.TenantId
                };
                _dbContext.sm_ActiviyHisroty.Add(activityHistory);

                var receiverIds = new List<Guid>();
                var configEntity = await _dbContext.sm_Configuration
                    .FirstOrDefaultAsync(x => x.Key == "VR_EXPORT" && x.TenantId == currentUser.TenantId);

                if (configEntity != null && !string.IsNullOrWhiteSpace(configEntity.Value))
                {
                    try
                    {
                        var config = JsonConvert.DeserializeObject<VehicleRequestExportConfig>(configEntity.Value);
                        if (config.VehicleCoordinatorId.HasValue)
                        {
                            receiverIds.Add(config.VehicleCoordinatorId.Value);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                if (receiverIds.Count > 0)
                {
                    var me = await _dbContext.IdmUser.FirstOrDefaultAsync(x => x.Id == currentUser.UserId);
                    foreach (var receiverId in receiverIds)
                    {
                        var notification = new sm_TaskNotification
                        {
                            Id = Guid.NewGuid(),
                            UserId = receiverId,
                            NotificationStatus = NotificationStatus.VehicleRequestWaitingForSharing,
                            CreatedByUserId = currentUser.UserId,
                            CreatedByUserName = currentUser.FullName,
                            CreatedOnDate = DateTime.Now,
                            AvatarUrl = Utils.FetchHost(me.AvatarUrl),
                            AdditionalData = new List<string>
                                { draftRequest.DestinationLocation, draftRequest.Id.ToString() }
                        };

                        foreach (var approvedRequest in approvedRequests)
                        {
                            notification.AdditionalData.Add(approvedRequest.RequestCode);
                        }

                        _dbContext.sm_TaskNotification.Add(notification);
                        await _taskNotificationHandler.CreatePushNotification(
                            _mapper.Map<TaskNotificationViewModel>(notification));
                    }
                }

                // Save to database
                await _dbContext.SaveChangesAsync();

                var response = Helper.CreateSuccessResponse(_mapper.Map<VehicleRequestViewModel>(draftRequest),
                    "Gửi yêu cầu ghép xe thành công");

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Input: DraftRequestId={DraftRequestId}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                    nameof(SubmitVehicleSharing), draftRequestId, approvedRequestIds, response.IsSuccess,
                    response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<VehicleRequestViewModel>(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Input: DraftRequestId={DraftRequestId}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                    nameof(SubmitVehicleSharing), draftRequestId, approvedRequestIds, errorResponse.IsSuccess,
                    errorResponse.Message);

                return errorResponse;
            }
        }

        /// <summary>
        /// Approve vehicle sharing request
        /// </summary>
        public async Task<Response<VehicleRequestViewModel>> ApproveVehicleSharing(Guid id,
            List<Guid> approvedRequestIds)
        {
            try
            {
                // Find entity
                var entity = await _dbContext.sm_VehicleRequest.FindAsync(id);
                if (entity == null)
                {
                    var notFoundResponse =
                        Helper.CreateNotFoundResponse<VehicleRequestViewModel>("Không tìm thấy yêu cầu xe");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} failed - entity not found. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(ApproveVehicleSharing), id, notFoundResponse.IsSuccess, notFoundResponse.Message);

                    return notFoundResponse;
                }

                // Check if request can be approved for sharing
                if (entity.Status != VehicleRequestStatus.WaitingForSharing &&
                    entity.Status != VehicleRequestStatus.PendingApproval)
                {
                    var badRequestResponse = Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                        "Chỉ có thể duyệt ghép xe cho yêu cầu ở trạng thái Chờ ghép xe hoặc Chờ duyệt");

                    Log.Information(
                        "VehicleRequestHandler.{MethodName} validation failed. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                        nameof(ApproveVehicleSharing), id, badRequestResponse.IsSuccess, badRequestResponse.Message);

                    return badRequestResponse;
                }

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                var approvedRequests = new List<sm_VehicleRequest>();

                if (entity.Status == VehicleRequestStatus.PendingApproval)
                {
                    // Validate input
                    if (approvedRequestIds == null || !approvedRequestIds.Any())
                    {
                        var invalidInputResponse = Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                            "Danh sách yêu cầu xe để ghép không được để trống");

                        Log.Information(
                            "VehicleRequestHandler.{MethodName} validation failed. Input: Id={Id}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ApproveVehicleSharing), id, approvedRequestIds,
                            invalidInputResponse.IsSuccess, invalidInputResponse.Message);

                        return invalidInputResponse;
                    }

                    // Find approved/shared requests
                    approvedRequests = await _dbContext.sm_VehicleRequest
                        .Where(x => approvedRequestIds.Contains(x.Id))
                        .ToListAsync();

                    // Check if all approved requests exist
                    if (approvedRequests.Count != approvedRequestIds.Count)
                    {
                        var missingRequestsResponse = Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                            "Một hoặc nhiều yêu cầu xe để ghép không tồn tại");

                        Log.Information(
                            "VehicleRequestHandler.{MethodName} validation failed. Input: Id={Id}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ApproveVehicleSharing), id, approvedRequestIds,
                            missingRequestsResponse.IsSuccess, missingRequestsResponse.Message);

                        return missingRequestsResponse;
                    }

                    // Determine SharingGroupId
                    Guid sharingGroupId;
                    var existingSharingGroupId =
                        approvedRequests.FirstOrDefault(x => x.SharingGroupId.HasValue)?.SharingGroupId;

                    if (existingSharingGroupId.HasValue)
                    {
                        // Use existing SharingGroupId
                        sharingGroupId = existingSharingGroupId.Value;
                    }
                    else
                    {
                        // Create new SharingGroupId and assign to all approved requests
                        sharingGroupId = Guid.NewGuid();

                        foreach (var approvedRequest in approvedRequests)
                        {
                            approvedRequest.SharingGroupId = sharingGroupId;
                            approvedRequest.LastModifiedByUserId = currentUser.UserId;
                            approvedRequest.LastModifiedByUserName = currentUser.UserName;
                            approvedRequest.LastModifiedOnDate = DateTime.Now;
                        }
                    }

                    // Update vehicle request
                    entity.SharingGroupId = sharingGroupId;
                }
                else
                {
                    if (!entity.SharingGroupId.HasValue)
                    {
                        var missingSharingGroupIdResponse = Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                            "Yêu cầu xin xe đang ở trạng thái Chờ ghép xe nhưng chưa được gán vào nhóm ghép xe");

                        Log.Information(
                            "VehicleRequestHandler.{MethodName} validation failed. Input: Id={Id}, ApprovedRequestIds={@ApprovedRequestIds}, Result: {IsSuccess}, Message: {Message}",
                            nameof(ApproveVehicleSharing), id, approvedRequestIds,
                            missingSharingGroupIdResponse.IsSuccess, missingSharingGroupIdResponse.Message);

                        return missingSharingGroupIdResponse;
                    }

                    approvedRequests = await _dbContext.sm_VehicleRequest
                        .Where(x => x.SharingGroupId == entity.SharingGroupId && x.Id != id)
                        .ToListAsync();
                }

                // Update status to Shared
                entity.Status = VehicleRequestStatus.Shared;
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                // Add activity history
                var activityHistory = new sm_ActiviyHisroty
                {
                    Id = Guid.NewGuid(),
                    EntityId = entity.Id,
                    EntityType = "VehicleRequest",
                    Action = VehicleRequestHistoryAction.APPROVE_SHARING,
                    Description = null,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.UserName,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = currentUser.UserId,
                    LastModifiedByUserName = currentUser.UserName,
                    LastModifiedOnDate = DateTime.Now,
                    TenantId = currentUser.TenantId
                };
                _dbContext.sm_ActiviyHisroty.Add(activityHistory);

                var me = await _dbContext.IdmUser.FirstOrDefaultAsync(x => x.Id == currentUser.UserId);
                var notification = new sm_TaskNotification
                {
                    Id = Guid.NewGuid(),
                    UserId = entity.UserId,
                    NotificationStatus = NotificationStatus.VehicleRequestShared,
                    CreatedByUserId = currentUser.UserId,
                    CreatedByUserName = currentUser.FullName,
                    CreatedOnDate = DateTime.Now,
                    AvatarUrl = Utils.FetchHost(me.AvatarUrl),
                    AdditionalData = new List<string> { entity.DestinationLocation, entity.Id.ToString() }
                };

                foreach (var approvedRequest in approvedRequests)
                {
                    notification.AdditionalData.Add(approvedRequest.RequestCode);
                }

                _dbContext.sm_TaskNotification.Add(notification);
                await _taskNotificationHandler.CreatePushNotification(
                    _mapper.Map<TaskNotificationViewModel>(notification));

                // Save to database
                await _dbContext.SaveChangesAsync();

                var response = Helper.CreateSuccessResponse(_mapper.Map<VehicleRequestViewModel>(entity),
                    "Duyệt ghép xe thành công");

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(ApproveVehicleSharing), id, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<VehicleRequestViewModel>(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Input: Id={Id}, Result: {IsSuccess}, Message: {Message}",
                    nameof(ApproveVehicleSharing), id, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        #region Private Methods

        /// <summary>
        /// Validate create/update model
        /// </summary>
        private async Task<Response<VehicleRequestViewModel>> ValidateCreateUpdateModel(
            VehicleRequestCreateUpdateModel model,
            Guid? id = null
        )
        {
            // Check if user exists
            var user = await _dbContext.IdmUser.FindAsync(model.UserId);
            if (user == null)
                return Helper.CreateBadRequestResponse<VehicleRequestViewModel>("Người dùng không tồn tại");

            // Check if department exists
            var department = await _dbContext.sm_CodeType
                .FirstOrDefaultAsync(x =>
                    x.Id == model.DepartmentId && x.Type == CodeTypeConstants.OrganizationStructure);
            if (department == null)
                return Helper.CreateBadRequestResponse<VehicleRequestViewModel>("Phòng ban không tồn tại");

            // Check if project exists (if provided)
            if (model.ProjectId.HasValue)
            {
                var project = await _dbContext.sm_Construction.FindAsync(model.ProjectId.Value);
                if (project == null)
                    return Helper.CreateBadRequestResponse<VehicleRequestViewModel>("Dự án không tồn tại");
            }

            // Check if vehicle exists (if provided)
            if (model.RequestedVehicleId.HasValue)
            {
                var vehicle = await _dbContext.sm_PhuongTien.FindAsync(model.RequestedVehicleId.Value);
                if (vehicle == null)
                    return Helper.CreateBadRequestResponse<VehicleRequestViewModel>("Xe không tồn tại");
            }

            // Check if start date is before end date
            if (model.StartDateTime > model.EndDateTime)
                return Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                    "Thời gian bắt đầu phải trước thời gian kết thúc");

            // Check required fields
            if (string.IsNullOrWhiteSpace(model.Purpose))
                return Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
                    "Mục đích chuyến đi không được để trống");

            if (string.IsNullOrWhiteSpace(model.DepartureLocation))
                return Helper.CreateBadRequestResponse<VehicleRequestViewModel>("Điểm xuất phát không được để trống");

            if (string.IsNullOrWhiteSpace(model.DestinationLocation))
                return Helper.CreateBadRequestResponse<VehicleRequestViewModel>("Nơi đến không được để trống");

            if (model.NumPassengers <= 0)
                return Helper.CreateBadRequestResponse<VehicleRequestViewModel>("Số lượng người đi phải lớn hơn 0");

            // Check if priority is valid
            if (!Enum.TryParse<VehicleRequestPriority>(model.Priority, out _))
                return Helper.CreateBadRequestResponse<VehicleRequestViewModel>("Độ ưu tiên không hợp lệ");

            // if (model.RequestedVehicleId.HasValue)
            // {
            //     var isRequestOverlapped = await _dbContext.sm_VehicleRequest
            //         .AnyAsync(x =>
            //             x.Id != id &&
            //             x.Status == VehicleRequestStatus.Approved &&
            //             x.RequestedVehicleId == model.RequestedVehicleId &&
            //             ((model.StartDateTime >= x.StartDateTime && model.StartDateTime <= x.EndDateTime) ||
            //              (model.EndDateTime >= x.StartDateTime && model.EndDateTime <= x.EndDateTime))
            //         );
            //     if (isRequestOverlapped)
            //         return Helper.CreateBadRequestResponse<VehicleRequestViewModel>(
            //             "Thời gian yêu cầu xe bị trùng với yêu cầu xe khác");
            // }

            return null;
        }

        /// <summary>
        /// Set entity names from related entities
        /// </summary>
        private async Task SetEntityNames(sm_VehicleRequest entity)
        {
            // Set user name
            var user = await _dbContext.IdmUser.FindAsync(entity.UserId);
            if (user != null)
                entity.UserName = user.Name;

            // Set department name
            var department = await _dbContext.sm_CodeType
                .FirstOrDefaultAsync(x =>
                    x.Id == entity.DepartmentId && x.Type == CodeTypeConstants.OrganizationStructure);
            if (department != null)
                entity.DepartmentName = department.Title;

            // Set project name
            if (entity.ProjectId.HasValue)
            {
                var project = await _dbContext.sm_Construction.FindAsync(entity.ProjectId.Value);
                if (project != null)
                    entity.ProjectName = project.Name;
            }

            // Set vehicle plate number
            if (entity.RequestedVehicleId.HasValue)
            {
                var vehicle = await _dbContext.sm_PhuongTien.FindAsync(entity.RequestedVehicleId.Value);
                if (vehicle != null)
                    entity.RequestedVehiclePlateNumber = vehicle.BienSoXe;
            }
        }

        /// <summary>
        /// Build query expression
        /// </summary>
        private Expression<Func<sm_VehicleRequest, bool>> BuildQuery(VehicleRequestQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var predicate = PredicateBuilder.New<sm_VehicleRequest>(true);

            if (currentUser.TenantId != null)
            {
                predicate = predicate.And(x => x.TenantId == currentUser.TenantId);
            }

            // Use FullTextSearch from base class to search in Purpose, UserName, and ProjectName
            if (!string.IsNullOrWhiteSpace(query.FullTextSearch))
            {
                var search = query.FullTextSearch.Trim().ToLower();
                predicate = predicate.And(x =>
                    x.Purpose.ToLower().Contains(search) ||
                    x.UserName.ToLower().Contains(search) ||
                    x.ProjectName.ToLower().Contains(search)
                );
            }

            // Filter by request code
            if (!string.IsNullOrWhiteSpace(query.RequestCode))
            {
                predicate = predicate.And(x => x.RequestCode.ToLower().Contains(query.RequestCode.Trim().ToLower()));
            }

            // Filter by creator
            if (query.CreatedByUserId.HasValue)
            {
                predicate = predicate.And(x => x.CreatedByUserId == query.CreatedByUserId.Value);
            }

            // Filter by priority
            if (!string.IsNullOrWhiteSpace(query.Priority))
            {
                predicate = Enum.TryParse<VehicleRequestPriority>(query.Priority, out var priority)
                    ? predicate.And(x => x.Priority == priority)
                    : predicate.And(x => false);
            }

            // Filter by creation date range
            if (query.CreatedDateRange is { Length: 2 })
            {
                if (query.CreatedDateRange[0].HasValue)
                {
                    predicate = predicate.And(x => x.CreatedOnDate.Date >= query.CreatedDateRange[0].Value.Date);
                }

                if (query.CreatedDateRange[1].HasValue)
                {
                    predicate = predicate.And(x => x.CreatedOnDate.Date <= query.CreatedDateRange[1].Value.Date);
                }
            }

            // Filter by usage date range
            if (query.UsageDateRange is { Length: 2 })
            {
                if (query.UsageDateRange[0].HasValue)
                {
                    predicate = predicate.And(x =>
                        x.StartDateTime.Date >= query.UsageDateRange[0].Value.Date ||
                        x.EndDateTime.Date >= query.UsageDateRange[0].Value.Date);
                }

                if (query.UsageDateRange[1].HasValue)
                {
                    predicate = predicate.And(x =>
                        x.StartDateTime.Date <= query.UsageDateRange[1].Value.Date ||
                        x.EndDateTime.Date <= query.UsageDateRange[1].Value.Date);
                }
            }

            // Filter by department
            if (query.DepartmentId.HasValue)
            {
                predicate = predicate.And(x => x.DepartmentId == query.DepartmentId.Value);
            }

            // Filter by user
            if (query.UserId.HasValue)
            {
                predicate = predicate.And(x => x.UserId == query.UserId.Value);
            }

            // Filter by vehicle type
            if (query.VehicleTypeId.HasValue)
            {
                predicate = predicate.And(x => x.RequestedVehicle.LoaiXeId == query.VehicleTypeId.Value);
            }

            // Filter by vehicle
            if (query.VehicleId.HasValue)
            {
                predicate = predicate.And(x => x.RequestedVehicleId == query.VehicleId.Value);
            }

            // Filter by status
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                var splittedStatus = query.Status.Split(',');
                var subPredicate = PredicateBuilder.New<sm_VehicleRequest>(true);

                foreach (var singleStatus in splittedStatus)
                {
                    if (Enum.TryParse<VehicleRequestStatus>(singleStatus, out var parsedStatus))
                    {
                        subPredicate = subPredicate.Or(x => x.Status == parsedStatus);
                    }
                }

                predicate = predicate.And(subPredicate);
            }

            // Filter by project
            if (query.ProjectId.HasValue)
            {
                predicate = predicate.And(x => x.ProjectId == query.ProjectId.Value);
            }

            return predicate;
        }

        /// <summary>
        /// Generate a new request code
        /// </summary>
        private async Task<string> GenerateNewRequestCode()
        {
            try
            {
                const string prefix = "VRN-";
                var today = DateTime.Now;
                var dateCode = today.ToString("ddMMyy");
                var code = prefix + dateCode;
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                // Find the highest sequence number for today
                var latestRequest = await _dbContext.sm_VehicleRequest
                    .Where(x => x.RequestCode.StartsWith(code) && x.TenantId == currentUser.TenantId)
                    .OrderByDescending(x => x.CreatedOnDate)
                    .FirstOrDefaultAsync();

                var sequenceNumber = 1;
                if (latestRequest == null)
                {
                    return code + sequenceNumber.ToString("D3");
                }

                // Extract the sequence number from the latest code
                var latestCode = latestRequest.RequestCode;
                if (latestCode.Length <= code.Length)
                {
                    return code + sequenceNumber.ToString("D3");
                }

                var sequencePart = latestCode[code.Length..];
                if (int.TryParse(sequencePart, out var latestSequence))
                {
                    sequenceNumber = latestSequence + 1;
                }

                // Format the sequence number with leading zeros (e.g., 001, 002, etc.)
                return code + sequenceNumber.ToString("D3");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating request code");
                throw new Exception("Error generating request code");
            }
        }

        #endregion

        #region Export Configuration

        /// <summary>
        /// Get vehicle request export configuration
        /// </summary>
        public async Task<Response<VehicleRequestExportConfig>> GetExportConfig()
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                // Find configuration record
                var configEntity = await _dbContext.sm_Configuration
                    .FirstOrDefaultAsync(x => x.Key == "VR_EXPORT" && x.TenantId == currentUser.TenantId);

                VehicleRequestExportConfig config;
                if (configEntity == null || string.IsNullOrWhiteSpace(configEntity.Value))
                {
                    // Return default empty configuration
                    config = new VehicleRequestExportConfig();
                }
                else
                {
                    // Deserialize JSON to configuration object
                    config = JsonConvert.DeserializeObject<VehicleRequestExportConfig>(configEntity.Value);
                }

                var response = Helper.CreateSuccessResponse(config);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Result: {IsSuccess}, Message: {Message}",
                    nameof(GetExportConfig), response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<VehicleRequestExportConfig>(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Result: {IsSuccess}, Message: {Message}",
                    nameof(GetExportConfig), errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        /// <summary>
        /// Update vehicle request export configuration
        /// </summary>
        public async Task<Response<VehicleRequestExportConfig>> UpdateExportConfig(VehicleRequestExportConfig config)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                // Validate and get names from database in single queries

                if (config.VehicleCoordinatorId.HasValue)
                {
                    var coordinator = await _dbContext.IdmUser
                        .FirstOrDefaultAsync(x => x.Id == config.VehicleCoordinatorId.Value);
                    if (coordinator == null)
                    {
                        var validationResponse =
                            Helper.CreateBadRequestResponse<VehicleRequestExportConfig>(
                                "Phụ trách điều xe không tồn tại");

                        Log.Information(
                            "VehicleRequestHandler.{MethodName} validation failed. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                            nameof(UpdateExportConfig), config, validationResponse.IsSuccess,
                            validationResponse.Message);

                        return validationResponse;
                    }

                    config.VehicleCoordinator = coordinator.Name;
                }

                // Find existing configuration record
                var configEntity = await _dbContext.sm_Configuration
                    .FirstOrDefaultAsync(x => x.Key == "VR_EXPORT" && x.TenantId == currentUser.TenantId);

                // Serialize configuration to JSON
                var jsonValue = JsonConvert.SerializeObject(config);

                if (configEntity == null)
                {
                    // Create new configuration record
                    configEntity = new sm_Configuration
                    {
                        Key = "VR_EXPORT",
                        Value = jsonValue,
                        Description = "Cấu hình xuất báo cáo yêu cầu xe",
                        CreatedByUserId = currentUser.UserId,
                        CreatedByUserName = currentUser.UserName,
                        CreatedOnDate = DateTime.Now,
                        LastModifiedByUserId = currentUser.UserId,
                        LastModifiedByUserName = currentUser.UserName,
                        LastModifiedOnDate = DateTime.Now,
                        TenantId = currentUser.TenantId
                    };

                    _dbContext.sm_Configuration.Add(configEntity);
                }
                else
                {
                    // Update existing configuration record
                    configEntity.Value = jsonValue;
                    configEntity.LastModifiedByUserId = currentUser.UserId;
                    configEntity.LastModifiedByUserName = currentUser.UserName;
                    configEntity.LastModifiedOnDate = DateTime.Now;
                }

                await _dbContext.SaveChangesAsync();

                var response = Helper.CreateSuccessResponse(config, "Cập nhật cấu hình xuất báo cáo thành công");

                Log.Information(
                    "VehicleRequestHandler.{MethodName} succeeded. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(UpdateExportConfig), config, response.IsSuccess, response.Message);

                return response;
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                var errorResponse = Helper.CreateExceptionResponse<VehicleRequestExportConfig>(e);

                Log.Information(
                    "VehicleRequestHandler.{MethodName} failed with exception. Input: {@Input}, Result: {IsSuccess}, Message: {Message}",
                    nameof(UpdateExportConfig), config, errorResponse.IsSuccess, errorResponse.Message);

                return errorResponse;
            }
        }

        #endregion
    }
}