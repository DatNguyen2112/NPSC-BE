using NSPC.Business.Services.InvestorType;
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using NSPC.Data.Entity;
using static NSPC.Common.Helper;
using System.Linq.Expressions;

namespace NSPC.Business.Services.InvestorType
{
    public class InvestorTypeHandler : InterfaceInvestorTypeHandler {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;

        public InvestorTypeHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IAttachmentHandler attachmentHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<InvestorTypeViewModel>> CreateAsync(InvestorTypeCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var userId =  currentUser.UserId;
                var userName =  currentUser.UserName;
                
                if (_dbContext.sm_InvestorType.Any(x => x.Code == model.Code))
                {
                    Log.Information($"Mã loại chủ đầu tư {model.Code} đã tồn tại");
                    return Helper.CreateBadRequestResponse<InvestorTypeViewModel>(
                        $"Mã loại chủ đầu tư {model.Code} đã tồn tại!");
                }
                
                var entity = _mapper.Map<sm_InvestorType>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.FullName;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.sm_InvestorType.Add(entity);
                await _dbContext.SaveChangesAsync();
                
                Log.Information("Thêm mới thành công, Model: {@model}, UserId: {@userId}, UserName: {@userName}",
                                    userId, model, userName);
                return Helper.CreateSuccessResponse(_mapper.Map<InvestorTypeViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Thêm mới thất bại, UserId: {currentUser.UserId}, UserName: {currentUser.FullName}");
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<InvestorTypeViewModel>(ex);
            }
        }
        
        public async Task<Response<InvestorTypeViewModel>> DeleteAsync(Guid id,
            RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_InvestorType
                    .Include(x => x.Investor)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Log.Information($"Không tìm thấy bản ghi loại chủ đầu tư, UserName: {currentUser.FullName}, UserId: {currentUser.UserId}");
                    return Helper.CreateBadRequestResponse<InvestorTypeViewModel>("Không tìm thấy bản ghi");
                }
                
                _dbContext.sm_InvestorType.Remove(entity);
                await _dbContext.SaveChangesAsync();
                
                Log.Information($"Xoá thành công với Id {entity.Id},  UserId: {currentUser.UserId}, UserName: {currentUser.FullName}");
                return Helper.CreateSuccessResponse(_mapper.Map<InvestorTypeViewModel>(entity), "Xoá thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Xoá thất bại, UserId: {currentUser.UserId}, UserName: {currentUser.FullName}");
                return Helper.CreateExceptionResponse<InvestorTypeViewModel>(ex);
            }
        }
        
        public async Task<Response<InvestorTypeViewModel>> GetById(Guid id,
            RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_InvestorType.Include(x => x.Investor)
                        .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Log.Information($"Không tìm thấy bản ghi loại chủ đầu tư, UserName: {currentUser.FullName}, UserId: {currentUser.UserId}");
                    return Helper.CreateBadRequestResponse<InvestorTypeViewModel>("Không tìm thấy bản ghi");
                }
                
                var result =  _mapper.Map<InvestorTypeViewModel>(entity);
                
                Log.Information($"Lấy chi tiết loại chủ đầu tư thành công với Id {entity.Id},  UserId: {currentUser.UserId}, UserName: {currentUser.FullName}");
                return Helper.CreateSuccessResponse(result, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Lấy chi tiết thất bại, UserId: {currentUser.UserId}, UserName: {currentUser.FullName}");
                return Helper.CreateExceptionResponse<InvestorTypeViewModel>(ex);
            }
        }
        
        public async Task<Response<InvestorTypeViewModel>> UpdateAsync(Guid id, InvestorTypeCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var userId =  currentUser.UserId;
                var userName =  currentUser.UserName;
                
                var entity = await 
                    _dbContext.sm_InvestorType.Include(x => x.Investor)
                        .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Log.Information($"Không tìm thấy bản ghi loại chủ đầu tư, UserName: {currentUser.FullName}, UserId: {currentUser.UserId}");
                    return Helper.CreateBadRequestResponse<InvestorTypeViewModel>("Không tìm thấy bản ghi");
                }
                
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;
                
                entity.Name =  model.Name;
                
                _dbContext.sm_InvestorType.Update(entity);
                await _dbContext.SaveChangesAsync();
                
                Log.Information("Cập nhật thành công, Model: {@model}, UserId: {@userId}, UserName: {@userName}",
                    userId, model, userName);
                return Helper.CreateSuccessResponse(_mapper.Map<InvestorTypeViewModel>(entity), "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Cập nhật thất bại, UserId: {currentUser.UserId}, UserName: {currentUser.FullName}");
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<InvestorTypeViewModel>(ex);
            }
        }

        private Expression<Func<sm_InvestorType, bool>> BuildQuery(InvestorTypeQueryModel query)
        {
            var predicate =  PredicateBuilder.New<sm_InvestorType>(true);
            
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate = predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower())
                                               || s.Name.ToLower().Contains(query.FullTextSearch.ToLower())
                );
            return predicate;
        }

        public async Task<Response<Pagination<InvestorTypeViewModel>>> GetPageAsync(InvestorTypeQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_InvestorType.AsNoTracking()
                    .Include(x => x.Investor)
                    .Where(predicate);
                
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<InvestorTypeViewModel>>(data);
                
                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<Pagination<InvestorTypeViewModel>>(ex);
            }
        }
    }
}

