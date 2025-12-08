using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Business.Services.EInvoice;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.AdvanceRequest;
using NSPC.Data.Data.Entity.EInvoice;
using NSPC.Data.Data.Entity.JsonbEntity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services.ConstructionActitvityLog;

namespace NSPC.Business.Services.AdvanceRequest
{
    public class AdvanceRequestHandler : IAdvanceRequestHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IConstructionActivityLogHandler _constructionActivityLogHandler;
        
        public AdvanceRequestHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper,  IConstructionActivityLogHandler constructionActivityLogHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _constructionActivityLogHandler = constructionActivityLogHandler;
        }

        public async Task<Response<AdvanceRequestViewModel>> CreateAsync(AdvanceRequestCreateUpdateModel model, Helper.RequestUser currentUser)
        {
            try
            {
                #region Validate dữ liệu
                // validate value % Thuế nếu < 0 hoặc > 100
                if (model.VatPercent < 0)
                    return Helper.CreateBadRequestResponse<AdvanceRequestViewModel>("Giá trị % thuế không được nhỏ hơn 0");

                if (model.VatPercent > 100)
                    return Helper.CreateBadRequestResponse<AdvanceRequestViewModel>("Giá trị % thuế không được lớn hơn 100");

                if (model.AdvanceRequestItems != null && model.AdvanceRequestItems.Count > 0)
                {
                    // Check danh sách items số lượng phải > 0
                    var checkQuantityItems = model.AdvanceRequestItems.Where(x => x.Quantity <= 0).ToList();
                    if (checkQuantityItems.Count > 0)
                        return Helper.CreateBadRequestResponse<AdvanceRequestViewModel>($"Số lượng không được nhỏ hơn hoặc bằng 0.");

                    // Check danh sách items đơn giá phải > 0
                    var checkUnitPriceItems = model.AdvanceRequestItems.Where(x => x.UnitPrice < 0).ToList();
                    if (checkUnitPriceItems.Count > 0)
                        return Helper.CreateBadRequestResponse<AdvanceRequestViewModel>($"Đơn giá không được nhỏ hơn 0.");

                    #region Tính toán số thứ tự items
                    model.AdvanceRequestItems = model.AdvanceRequestItems.OrderBy(x => x.LineNumber).ToList();
                    for (int i = 0; i < model.AdvanceRequestItems.Count; i++)
                        model.AdvanceRequestItems[i].LineNumber = i + 1;
                    #endregion
                }

                if (model.AdvanceRequestItems == null || model.AdvanceRequestItems.Count == 0)
                    return Helper.CreateBadRequestResponse<AdvanceRequestViewModel>("Danh sách thông tin tạm ứng không được để trống.");
                #endregion

                // Mapping và tạo entity
                var entity = _mapper.Map<sm_AdvanceRequest>(model);

                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.FullName;
                entity.CreatedOnDate = DateTime.Now;

                entity.Id = Guid.NewGuid();
                entity.Code = await AutoGenerateAdvanceRequestCode(AdvanceRequestConstants.AdvanceRequestCodePrefix);

                string priorityLevelName;
                string priorityLevelColor;

                switch (model.PriorityLevelCode)
                {
                    case AdvanceRequestConstants.PriorityLevelCode.HIGH:
                        priorityLevelName = AdvanceRequestConstants.PriorityLevelName.HIGH;
                        priorityLevelColor = AdvanceRequestConstants.PriorityLevelColor.HIGH;
                        break;
                    case AdvanceRequestConstants.PriorityLevelCode.MEDIUM:
                        priorityLevelName = AdvanceRequestConstants.PriorityLevelName.MEDIUM;
                        priorityLevelColor = AdvanceRequestConstants.PriorityLevelColor.MEDIUM;
                        break;
                    default:
                        priorityLevelName = AdvanceRequestConstants.PriorityLevelName.LOW;
                        priorityLevelColor = AdvanceRequestConstants.PriorityLevelColor.LOW;
                        break;
                }

                entity.PriorityLevelName = priorityLevelName;
                entity.PriorityLevelColor = priorityLevelColor;

                entity.StatusCode = AdvanceRequestConstants.StatusCode.DRATF;
                string statusName;
                string statusColor;

                switch (entity.StatusCode)
                {
                    case AdvanceRequestConstants.StatusCode.DRATF:
                        statusName = AdvanceRequestConstants.StatusName.DRATF;
                        statusColor = AdvanceRequestConstants.StatusColor.DRATF;
                        break;
                    case AdvanceRequestConstants.StatusCode.PENDING_APPROVAL:
                        statusName = AdvanceRequestConstants.StatusName.PENDING_APPROVAL;
                        statusColor = AdvanceRequestConstants.StatusColor.PENDING_APPROVAL;
                        break;
                    case AdvanceRequestConstants.StatusCode.APPROVED:
                        statusName = AdvanceRequestConstants.StatusName.APPROVED;
                        statusColor = AdvanceRequestConstants.StatusColor.APPROVED;
                        break;
                    case AdvanceRequestConstants.StatusCode.REJECTED:
                        statusName = AdvanceRequestConstants.StatusName.REJECTED;
                        statusColor = AdvanceRequestConstants.StatusColor.REJECTED;
                        break;
                    case AdvanceRequestConstants.StatusCode.COMPLETED:
                        statusName = AdvanceRequestConstants.StatusName.COMPLETED;
                        statusColor = AdvanceRequestConstants.StatusColor.COMPLETED;
                        break;
                    default:
                        statusName = AdvanceRequestConstants.StatusName.DRATF;
                        statusColor = AdvanceRequestConstants.StatusColor.DRATF;
                        break;
                }

                entity.StatusName = statusName;
                entity.StatusColor = statusColor;

                #region Tính toán các thông tin items
                if (entity.AdvanceRequestItems != null && entity.AdvanceRequestItems.Count > 0)
                {
                    foreach (var item in entity.AdvanceRequestItems)
                    {
                        item.Id = Guid.NewGuid();
                        item.LineAmount = item.Quantity * item.UnitPrice;
                    }
                }
                #endregion

                entity.TotalLineAmount = entity.AdvanceRequestItems.Sum(x => x.LineAmount);
                entity.TotalAmount = entity.TotalLineAmount * (1 + entity.VatPercent / 100);
                entity.AdvanceRequestHistories.Add(new jsonb_AdvanceRequestHistory
                {
                    Id = Guid.NewGuid(),
                    Action = "đã tạo yêu cầu tạm ứng",
                    CreatedOnDate = DateTime.Now,
                    CreatedByUserName = currentUser.FullName
                });

                _dbContext.sm_AdvanceRequest.Add(entity);

                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động tạo yêu cầu vật tư vào bảng sm_ConstructionActivityLog
                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã tạo yêu cầu tạm ứng",
                        CodeLinkDescription = $"{entity.Code} - {entity.Content}",
                        OrderId =  entity.Id,
                        ConstructionId = entity.ConstructionId,
                    }, currentUser);
                    #endregion
                }

                var result = await GetByIdAsync(entity.Id);

                if (result.IsSuccess)
                    result.Message = "Thêm mới yêu cầu tạm ứng thành công.";

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<AdvanceRequestViewModel>(ex);
            }
        }

        public async Task<Response> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_AdvanceRequest.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse("Yêu cầu tạm ứng không tồn tại");

                if (entity.StatusCode != AdvanceRequestConstants.StatusCode.DRATF && entity.StatusCode != AdvanceRequestConstants.StatusCode.REJECTED)
                    return Helper.CreateBadRequestResponse($"Không thể xóa yêu cầu tạm ứng {entity.StatusName}");

                _dbContext.Remove(entity);

                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse("Xóa yêu cầu tạm ứng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse(ex);
            }
        }

        public async Task<Response<AdvanceRequestViewModel>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_AdvanceRequest
                    .AsNoTracking()
                    .Include(x => x.AdvanceRequestItems.OrderBy(x => x.LineNumber))
                    .Include(x => x.sm_Construction)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<AdvanceRequestViewModel>("Yêu cầu tạm ứng không tồn tại trong hệ thống.");

                // Sort AdvanceRequestHistories by CreatedOnDate
                entity.AdvanceRequestHistories = entity.AdvanceRequestHistories
                    .OrderByDescending(history => history.CreatedOnDate)
                    .ToList();

                var result = _mapper.Map<AdvanceRequestViewModel>(entity);

                return new Response<AdvanceRequestViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse<AdvanceRequestViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<AdvanceRequestViewModel>>> GetPageAsync(AdvanceRequestQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_AdvanceRequest
                    .AsNoTracking()
                    .Include(x => x.AdvanceRequestItems.OrderBy(x => x.LineNumber))
                    .Include(x => x.sm_Construction)
                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<AdvanceRequestViewModel>>(data);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@Param}", query);
                return Helper.CreateExceptionResponse<Pagination<AdvanceRequestViewModel>>(ex);
            }
        }

        private Expression<Func<sm_AdvanceRequest, bool>> BuildQuery(AdvanceRequestQueryModel query)
        {
            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            var predicate = PredicateBuilder.New<sm_AdvanceRequest>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate = predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.CreatedByUserName.ToLower().Contains(query.FullTextSearch.ToLower())
                || s.Content.ToLower().Contains(query.FullTextSearch.ToLower())
                );

            if (query.ConstructionId.HasValue && query.ConstructionId.Value != Guid.Empty)
                predicate = predicate.And(s => s.sm_Construction.Id == query.ConstructionId);

            if (query.DueDate != null)
                predicate = predicate.And(s => s.DueDate.Date == query.DueDate.Value.Date);

            if (query.CreatedOnDate != null)
                predicate = predicate.And(s => s.CreatedOnDate.Date == query.CreatedOnDate.Value.Date);

            if (!string.IsNullOrEmpty(query.StatusCode))
                predicate = predicate.And(s => s.StatusCode == query.StatusCode);

            if (!string.IsNullOrEmpty(query.PriorityLevelCode))
                predicate = predicate.And(s => s.PriorityLevelCode == query.PriorityLevelCode);

            if (query.TotalAmount.HasValue)
                predicate = predicate.And(s => s.TotalAmount == query.TotalAmount);
            
            if (query.DateRange != null && query.DateRange.Count() > 0)
            {
                if (query.DateRange[0].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date >= query.DateRange[0].Value.Date);

                if (query.DateRange[1].HasValue)
                    predicate.And(x => x.CreatedOnDate.Date <= query.DateRange[1].Value.Date);
            }

            return predicate;
        }

        public async Task<Response<AdvanceRequestViewModel>> UpdateAsync(Guid id, AdvanceRequestCreateUpdateModel model, Helper.RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_AdvanceRequest.Include(x => x.AdvanceRequestItems).FirstOrDefaultAsync(x => x.Id == id);

                #region Validate dữ liệu
                if (entity == null)
                    return Helper.CreateNotFoundResponse<AdvanceRequestViewModel>(string.Format("Yêu cầu tạm ứng không tồn tại trong hệ thống"));

                // validate value % Thuế nếu < 0 hoặc > 100
                if (model.VatPercent < 0)
                    return Helper.CreateBadRequestResponse<AdvanceRequestViewModel>("Giá trị % thuế không được nhỏ hơn 0");

                if (model.VatPercent > 100)
                    return Helper.CreateBadRequestResponse<AdvanceRequestViewModel>("Giá trị % thuế không được lớn hơn 100");

                if (model.AdvanceRequestItems != null && model.AdvanceRequestItems.Count > 0)
                {
                    // Check danh sách items số lượng phải > 0
                    var checkQuantityItems = model.AdvanceRequestItems.Where(x => x.Quantity <= 0).ToList();
                    if (checkQuantityItems.Count > 0)
                        return Helper.CreateBadRequestResponse<AdvanceRequestViewModel>($"Số lượng không được nhỏ hơn hoặc bằng 0.");

                    // Check danh sách items đơn giá phải > 0
                    var checkUnitPriceItems = model.AdvanceRequestItems.Where(x => x.UnitPrice < 0).ToList();
                    if (checkUnitPriceItems.Count > 0)
                        return Helper.CreateBadRequestResponse<AdvanceRequestViewModel>($"Đơn giá không được nhỏ hơn 0.");

                    #region Tính toán số thứ tự items
                    model.AdvanceRequestItems = model.AdvanceRequestItems.OrderBy(x => x.LineNumber).ToList();
                    for (int i = 0; i < model.AdvanceRequestItems.Count; i++)
                        model.AdvanceRequestItems[i].LineNumber = i + 1;
                    #endregion
                }

                if (model.AdvanceRequestItems == null || model.AdvanceRequestItems.Count == 0)
                    return Helper.CreateBadRequestResponse<AdvanceRequestViewModel>("Danh sách thông tin tạm ứng không được để trống.");
                #endregion

                // Cập nhật các thông tin
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;
                entity.Content = model.Content;
                entity.ConstructionId = model.ConstructionId;
                entity.PriorityLevelCode = model.PriorityLevelCode;

                string priorityLevelName;
                string priorityLevelColor;

                switch (model.PriorityLevelCode)
                {
                    case AdvanceRequestConstants.PriorityLevelCode.HIGH:
                        priorityLevelName = AdvanceRequestConstants.PriorityLevelName.HIGH;
                        priorityLevelColor = AdvanceRequestConstants.PriorityLevelColor.HIGH;
                        break;
                    case AdvanceRequestConstants.PriorityLevelCode.MEDIUM:
                        priorityLevelName = AdvanceRequestConstants.PriorityLevelName.MEDIUM;
                        priorityLevelColor = AdvanceRequestConstants.PriorityLevelColor.MEDIUM;
                        break;
                    default:
                        priorityLevelName = AdvanceRequestConstants.PriorityLevelName.LOW;
                        priorityLevelColor = AdvanceRequestConstants.PriorityLevelColor.LOW;
                        break;
                }

                entity.PriorityLevelName = priorityLevelName;
                entity.PriorityLevelColor = priorityLevelColor;
                entity.DueDate = model.DueDate;
                entity.Note = model.Note;
                entity.VatPercent = model.VatPercent;

                #region Tính toán các thông tin khác
                _dbContext.RemoveRange(entity.AdvanceRequestItems);
                entity.AdvanceRequestItems = new List<sm_AdvanceRequestItems>();

                foreach (var modelItem in model.AdvanceRequestItems)
                {
                    // Readd new item
                    var item = _mapper.Map<sm_AdvanceRequestItems>(modelItem);
                    entity.AdvanceRequestItems.Add(item);
                }

                if (entity.AdvanceRequestItems != null && entity.AdvanceRequestItems.Count > 0)
                {
                    foreach (var item in entity.AdvanceRequestItems)
                    {
                        item.Id = Guid.NewGuid();
                        item.LineAmount = item.Quantity * item.UnitPrice;
                        _dbContext.sm_AdvanceRequestItems.Add(item);
                    }
                }
                #endregion

                entity.TotalLineAmount = entity.AdvanceRequestItems.Sum(x => x.LineAmount);
                entity.TotalAmount = entity.TotalLineAmount * (1 + entity.VatPercent / 100);
                entity.AdvanceRequestHistories.Add(new jsonb_AdvanceRequestHistory
                {
                    Id = Guid.NewGuid(),
                    Action = "đã chỉnh sửa yêu cầu tạm ứng",
                    CreatedOnDate = DateTime.Now,
                    CreatedByUserName = currentUser.FullName
                });

                _dbContext.sm_AdvanceRequest.Update(entity);

                await _dbContext.SaveChangesAsync();

                var result = await GetByIdAsync(entity.Id);

                if (result.IsSuccess)
                    result.Message = "Chỉnh sửa yêu cầu tạm ứng thành công.";

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Model: {@model}", id, model);
                return Helper.CreateExceptionResponse<AdvanceRequestViewModel>(ex);
            }
        }

        public async Task<string> AutoGenerateAdvanceRequestCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_AdvanceRequest
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

        public async Task<Response<AdvanceRequestViewModel>> AddHistoryAsync(Guid id, jsonb_AdvanceRequestHistory model)
        {
            try
            {
                var entity = await _dbContext.sm_AdvanceRequest.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<AdvanceRequestViewModel>("Yêu cầu tạm ứng không tồn tại trong hệ thống");
                if (entity.AdvanceRequestHistories == null)
                    entity.AdvanceRequestHistories = new List<jsonb_AdvanceRequestHistory>();
                model.CreatedOnDate = DateTime.Now;
                entity.AdvanceRequestHistories.Add(model);

                _dbContext.sm_AdvanceRequest.Update(entity);
                await _dbContext.SaveChangesAsync();
                var result = await GetByIdAsync(entity.Id);

                if (result.IsSuccess)
                    result.Message = "Thêm mới lịch sử xử lý thành công";

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, Model: {@model}", id, model);
                return Helper.CreateExceptionResponse<AdvanceRequestViewModel>(ex);
            }
        }

        public async Task<Response> SendAsync(Guid id, Helper.RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_AdvanceRequest.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse("Yêu cầu tạm ứng không tồn tại");

                if (entity.StatusCode != AdvanceRequestConstants.StatusCode.DRATF && entity.StatusCode != AdvanceRequestConstants.StatusCode.REJECTED)
                    return Helper.CreateBadRequestResponse($"Không thể gửi duyệt yêu cầu tạm ứng {entity.StatusName}");

                entity.StatusCode = AdvanceRequestConstants.StatusCode.PENDING_APPROVAL;
                entity.StatusName = AdvanceRequestConstants.StatusName.PENDING_APPROVAL;
                entity.StatusColor = AdvanceRequestConstants.StatusColor.PENDING_APPROVAL;

                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;

                entity.AdvanceRequestHistories.Add(new jsonb_AdvanceRequestHistory
                {
                    Id = Guid.NewGuid(),
                    Action = "đã gửi duyệt yêu cầu tạm ứng",
                    CreatedOnDate = DateTime.Now,
                    CreatedByUserName = currentUser.FullName
                });

                _dbContext.Update(entity);

                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động gửi duyệt yêu cầu vật tư vào bảng sm_ConstructionActivityLog
                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã gửi duyệt yêu cầu tạm ứng",
                        CodeLinkDescription = $"{entity.Code} - {entity.Content}",
                        OrderId =  entity.Id,
                        ConstructionId = entity.ConstructionId,
                    }, currentUser);
                    #endregion
                }

                return Helper.CreateSuccessResponse("Gửi duyệt yêu cầu tạm ứng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse(ex);
            }
        }

        public async Task<Response> RejectAsync(Guid id, jsonb_AdvanceRequestHistory model, Helper.RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_AdvanceRequest.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse("Yêu cầu tạm ứng không tồn tại");

                if (entity.StatusCode != AdvanceRequestConstants.StatusCode.PENDING_APPROVAL)
                    return Helper.CreateBadRequestResponse($"Không thể từ chối yêu cầu tạm ứng {entity.StatusName}");

                entity.StatusCode = AdvanceRequestConstants.StatusCode.REJECTED;
                entity.StatusName = AdvanceRequestConstants.StatusName.REJECTED;
                entity.StatusColor = AdvanceRequestConstants.StatusColor.REJECTED;

                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;

                entity.AdvanceRequestHistories.Add(new jsonb_AdvanceRequestHistory
                {
                    Id = Guid.NewGuid(),
                    Action = "đã từ chối yêu cầu tạm ứng",
                    RejectionReason = model.RejectionReason,
                    CreatedOnDate = DateTime.Now,
                    CreatedByUserName = currentUser.FullName
                });

                _dbContext.Update(entity);

                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động từ chối yêu cầu vật tư vào bảng sm_ConstructionActivityLog
                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã từ chối yêu cầu tạm ứng",
                        CodeLinkDescription = $"{entity.Code} - {entity.Content}",
                        OrderId =  entity.Id,
                        ConstructionId = entity.ConstructionId,
                    }, currentUser);
                    #endregion
                }

                return Helper.CreateSuccessResponse("Từ chối yêu cầu tạm ứng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse(ex);
            }
        }

        public async Task<Response> ApproveAsync(Guid id, Helper.RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_AdvanceRequest.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse("Yêu cầu tạm ứng không tồn tại");

                if (entity.StatusCode != AdvanceRequestConstants.StatusCode.PENDING_APPROVAL)
                    return Helper.CreateBadRequestResponse($"Không thể phê duyệt yêu cầu tạm ứng {entity.StatusName}");

                entity.StatusCode = AdvanceRequestConstants.StatusCode.APPROVED;
                entity.StatusName = AdvanceRequestConstants.StatusName.APPROVED;
                entity.StatusColor = AdvanceRequestConstants.StatusColor.APPROVED;

                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;

                entity.AdvanceRequestHistories.Add(new jsonb_AdvanceRequestHistory
                {
                    Id = Guid.NewGuid(),
                    Action = "đã duyệt yêu cầu tạm ứng",
                    CreatedOnDate = DateTime.Now,
                    CreatedByUserName = currentUser.FullName
                });

                _dbContext.Update(entity);

                var createResult = await _dbContext.SaveChangesAsync();

                if (createResult > 0)
                {
                    #region Log lại hoạt động duyệt yêu cầu tạm ứng vào bảng sm_ConstructionActivityLog
                    await _constructionActivityLogHandler.Create(new ConstructionActivityLogCreateModel()
                    {
                        Description = "đã duyệt yêu cầu tạm ứng",
                        CodeLinkDescription = $"{entity.Code} - {entity.Content}",
                        OrderId =  entity.Id,
                        ConstructionId = entity.ConstructionId,
                    }, currentUser);
                    #endregion
                }

                return Helper.CreateSuccessResponse("Phê duyệt yêu cầu tạm ứng thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}", id);
                return Helper.CreateExceptionResponse(ex);
            }
        }
    }
}
