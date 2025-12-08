using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NSPC.Business.Services;

namespace NSPC.Business
{
    public class LichSuChamSocHandler : ILichSuChamSocHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        private readonly ICustomerServiceCommentHandler _customerServiceCommentHandler;     
        
     
        public LichSuChamSocHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, ICustomerServiceCommentHandler customerServiceCommentHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _customerServiceCommentHandler = customerServiceCommentHandler;
        }
        

        public async Task<Response<LichSuChamSocViewModel>> Create(LichSuChamSocCreateUpdateModel model)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var entity = _mapper.Map<sm_LichSuChamSoc>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;

                if (model.Type == LichSuChamSocConstants.Type.DATE_CUSTOMER_SERVICE)
                {
                    entity.Code = await GetNewCode(LichSuChamSocConstants.CodePrefix.DATE_CUSTOMER_SERVICE_CODE);
                    entity.StatusCode = LichSuChamSocConstants.NOW; // Mới tạo
                }
                else
                {
                    entity.Code = await GetNewCode(LichSuChamSocConstants.CodePrefix.HISTORY_CUSTOMER_SERVICE_CODE);
                    entity.StatusCode = LichSuChamSocConstants.COMPLETED; // Hoàn thành
                }
                _dbContext.sm_LichSuChamSoc.Add(entity);
                
                // Cập nhật lại số lần chăm sóc và ngày chăm sóc gần nhất
                var khachHang = await _dbContext.sm_Customer.FirstOrDefaultAsync(x=>x.Id == model.CustomerId);

                khachHang.LastCareOnDate = DateTime.Now;
                khachHang.TotalCareTimes = _dbContext.sm_LichSuChamSoc.Where(x => x.CustomerId == model.CustomerId).Count();
                _dbContext.sm_Customer.Update(khachHang);
                
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<LichSuChamSocViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<LichSuChamSocViewModel>(ex);
            }
        }

        public async Task<Response<LichSuChamSocViewModel>> Update(Guid id, LichSuChamSocCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.sm_LichSuChamSoc
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.mk_DuAn)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LichSuChamSocViewModel>(string.Format("Lịch sử chăm sóc không tồn tại trong hệ thống!"));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                
                string message = string.Empty;
                
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                if (model.StatusCode != null)
                {
                    entity.StatusCode = model.StatusCode;
                }
                else
                {
                    entity.StatusCode = entity.StatusCode;
                }
               

                if (model.Priority != null)
                {
                    entity.Priority = model.Priority;
                }
                else
                {
                    entity.Priority = entity.Priority;
                }

                if (model.Participants != null)
                {
                    entity.Participants = model.Participants;
                }
                else
                {
                    entity.Participants = entity.Participants;
                }

                if (model.DateRange != null)
                {
                    entity.DateRange = model.DateRange;
                }
                else
                {
                    entity.DateRange = entity.DateRange;
                }
                
                if (model.GhiChu != null)
                {
                    entity.GhiChu = model.GhiChu;
                }
                else
                {
                    entity.GhiChu = entity.GhiChu;
                }

                if (model.CustomerServiceContent != null)
                {
                    entity.CustomerServiceContent = model.CustomerServiceContent;
                }
                else
                {
                    entity.CustomerServiceContent = entity.CustomerServiceContent;
                }

                if (model.DanhGia != 0)
                {
                    entity.DanhGia = model.DanhGia;
                }
                else
                {
                    entity.DanhGia = entity.DanhGia;
                }
                
                if (model.ProjectId != null)
                {
                    entity.ProjectId = model.ProjectId;
                }
                else
                {
                    entity.ProjectId = entity.ProjectId;
                }
                
                
                
                _dbContext.sm_LichSuChamSoc.Update(entity);
                var result = await _dbContext.SaveChangesAsync();

                // if (result > 0)
                // {
                //     await _customerServiceCommentHandler.Create(new CustomerServiceCommentCreateModel()
                //     {
                //         CustomerServiceId = entity.Id,
                //         IsSystemLog = true,
                //         Content = model.Type == LichSuChamSocConstants.Type.DATE_CUSTOMER_SERVICE ? 
                //             "Đã chỉnh sửa lịch hẹn chăm sóc" : "Đã chỉnh sửa lịch sử chăm sóc",
                //     }, currentUser);
                // }
                return Helper.CreateSuccessResponse(_mapper.Map<LichSuChamSocViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<LichSuChamSocViewModel>(ex);
            }
        }
        
        public async Task<Response<LichSuChamSocViewModel>> ChangeActivity(Guid id, LichSuChamSocCreateUpdateModel model)
        {
            try
            {
                var entity = await _dbContext.sm_LichSuChamSoc
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.mk_DuAn)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LichSuChamSocViewModel>(string.Format("Lịch sử chăm sóc không tồn tại trong hệ thống!"));

                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                
                string message = string.Empty;
                
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.UserName;
                entity.LastModifiedOnDate = DateTime.Now;

                if (model.StatusCode != null)
                {
                    entity.StatusCode = model.StatusCode;
                }
                else
                {
                    entity.StatusCode = entity.StatusCode;
                }
               

                if (model.Priority != null)
                {
                    entity.Priority = model.Priority;
                }
                else
                {
                    entity.Priority = entity.Priority;
                }

                if (model.Participants != null)
                {
                    entity.Participants = model.Participants;
                }
                else
                {
                    entity.Participants = entity.Participants;
                }

                if (model.DateRange != null)
                {
                    entity.DateRange = model.DateRange;
                }
                else
                {
                    entity.DateRange = entity.DateRange;
                }
                
                if (model.GhiChu != null)
                {
                    entity.GhiChu = model.GhiChu;
                }
                else
                {
                    entity.GhiChu = entity.GhiChu;
                }

                if (model.CustomerServiceContent != null)
                {
                    entity.CustomerServiceContent = model.CustomerServiceContent;
                }
                else
                {
                    entity.CustomerServiceContent = entity.CustomerServiceContent;
                }

                if (model.DanhGia != 0)
                {
                    entity.DanhGia = model.DanhGia;
                }
                else
                {
                    entity.DanhGia = entity.DanhGia;
                }
                
                if (model.ProjectId != null)
                {
                    entity.ProjectId = model.ProjectId;
                }
                else
                {
                    entity.ProjectId = entity.ProjectId;
                }

                if (model.StatusCode != null)
                {
                    switch (model.StatusCode)
                    {
                        case LichSuChamSocConstants.NOW:
                            message = "Đã chuyển đổi trạng thái lịch hẹn chăm sóc thành “Mới tạo”.";
                            break;
                        case LichSuChamSocConstants.PENDING:
                            message = "Đã chuyển đổi trạng thái lịch hẹn chăm sóc thành “Đang thực hiện”.";
                            break;
                        case LichSuChamSocConstants.COMPLETED:
                            message = "Đã chuyển đổi trạng thái lịch hẹn chăm sóc thành “Hoàn thành”.";
                            break;
                    }
                
                    // await _customerServiceCommentHandler.Create(new CustomerServiceCommentCreateModel()
                    // {
                    //     CustomerServiceId = id,
                    //     IsSystemLog = true,
                    //     Content = message,
                    // }, currentUser);
                }
                else
                {
                    // await _customerServiceCommentHandler.Create(new CustomerServiceCommentCreateModel()
                    // {
                    //     CustomerServiceId = id,
                    //     IsSystemLog = true,
                    //     Content = "Đã thay đổi đánh giá từ khách.",
                    // }, currentUser);
                }
                
                _dbContext.sm_LichSuChamSoc.Update(entity);
                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<LichSuChamSocViewModel>(entity), "Chỉnh sửa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<LichSuChamSocViewModel>(ex);
            }
        }
        
        private async Task<string> GetNewCode(string defaultPrefix)
        {
            try
            {
                var code = defaultPrefix + DateTime.Now.ToString("ddMMyy");

                var result = await _dbContext.sm_LichSuChamSoc.AsNoTracking().Where(x => x.Code.Contains(code)).OrderByDescending(x => x.CreatedOnDate).FirstOrDefaultAsync();

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

        public async Task<Response<LichSuChamSocViewModel>> GetById(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_LichSuChamSoc.AsNoTracking()
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.mk_DuAn)
                    .Where(x => x.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LichSuChamSocViewModel>(string.Format("Lịch sử chăm sóc không tồn tại trong hệ thống!"));

                return Helper.CreateSuccessResponse(_mapper.Map<LichSuChamSocViewModel>(entity));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<LichSuChamSocViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<LichSuChamSocViewModel>>> GetPage(LichSuChamSocQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_LichSuChamSoc.AsNoTracking()
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.mk_DuAn)
                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                return Helper.CreateSuccessResponse(_mapper.Map<Pagination<LichSuChamSocViewModel>>(data));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@query}", query);
                return Helper.CreateExceptionResponse<Pagination<LichSuChamSocViewModel>>(ex);
            }
        }

        private Expression<Func<sm_LichSuChamSoc, bool>> BuildQuery(LichSuChamSocQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_LichSuChamSoc>(true);
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.GhiChu.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.CustomerId.HasValue)
                predicate.And(s => s.CustomerId == query.CustomerId.Value);

            return predicate;
        }

        public async Task<Response<LichSuChamSocViewModel>> Delete(Guid id)
        {
            try
            {
                var entity = await _dbContext.sm_LichSuChamSoc.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LichSuChamSocViewModel>(string.Format("Lịch sử chăm sóc không tồn tại trong hệ thống!"));

                _dbContext.Remove(entity);
                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<LichSuChamSocViewModel>(entity), "Xóa thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<LichSuChamSocViewModel>(ex);
            }
        }

        public async Task<Response<LichSuChamSocViewModel>> ConfirmTaskCompletion(Guid id)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                
                var entity = await _dbContext.sm_LichSuChamSoc.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LichSuChamSocViewModel>(string.Format("Lịch sử chăm sóc không tồn tại trong hệ thống!"));

                entity.StatusCode = LichSuChamSocConstants.COMPLETED;

                // await _customerServiceCommentHandler.Create(new CustomerServiceCommentCreateModel()
                // {
                //     CustomerServiceId = id,
                //     IsSystemLog = true,
                //     Content = LichSuChamSocConstants.CONFIRMED_TASK_COMPLETED,
                // }, currentUser);
                
                await _dbContext.SaveChangesAsync();
                
                return Helper.CreateSuccessResponse(_mapper.Map<LichSuChamSocViewModel>(entity), "Cập nhật hoàn thành công việc thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<LichSuChamSocViewModel>(ex);
            }
        }

        public async Task<Response<LichSuChamSocViewModel>> RestoreTask(Guid id)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
                
                var entity = await _dbContext.sm_LichSuChamSoc.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<LichSuChamSocViewModel>(string.Format("Lịch sử chăm sóc không tồn tại trong hệ thống!"));

                entity.StatusCode = LichSuChamSocConstants.PENDING;
                
                // await _customerServiceCommentHandler.Create(new CustomerServiceCommentCreateModel()
                // {
                //     CustomerServiceId = id,
                //     IsSystemLog = true,
                //     Content = LichSuChamSocConstants.RESTORED_TASK,
                // }, currentUser);
                
                await _dbContext.SaveChangesAsync();
                
                return Helper.CreateSuccessResponse(_mapper.Map<LichSuChamSocViewModel>(entity), "Khôi phục công việc thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", id);
                return Helper.CreateExceptionResponse<LichSuChamSocViewModel>(ex);
            }
        }
    }
}
