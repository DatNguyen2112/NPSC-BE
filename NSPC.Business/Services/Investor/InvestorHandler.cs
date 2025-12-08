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

namespace NSPC.Business.Services.Investor
{
    public class InvestorHandler : InterfaceInvestorHandler {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;

        public InvestorHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<InvestorViewModel>> CreateAsync(InvestorCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var userId =  currentUser.UserId;
                var userName =  currentUser.UserName;
                
                if (_dbContext.sm_Investor.Any(x => x.Code == model.Code))
                {
                    return Helper.CreateBadRequestResponse<InvestorViewModel>(
                        $"Mã chủ đầu tư {model.Code} đã tồn tại!");
                }
                
                var entity = _mapper.Map<sm_Investor>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.FullName;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.sm_Investor.Add(entity);
                await _dbContext.SaveChangesAsync();
                
                Log.Information("Thêm mới thành công, Model: {@model}, UserId: {@userId}, UserName: {@userName}",
                    userId, model, userName);
                return Helper.CreateSuccessResponse(_mapper.Map<InvestorViewModel>(entity), "Thêm mới thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Thêm mới thất bại, UserName: {currentUser.FullName}, UserId: {currentUser.UserId}");
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<InvestorViewModel>(ex);
            }
        }
        
        public async Task<Response<InvestorViewModel>> GetById(Guid id,
            RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Investor
                    .Include(x => x.InvestorType)
                        .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Log.Information($"Không tìm thấy bản ghi chủ đầu tư với Id {entity.Id}");
                    return Helper.CreateBadRequestResponse<InvestorViewModel>("Không tìm thấy bản ghi");
                }
                
                var result =  _mapper.Map<InvestorViewModel>(entity);
                
                Log.Information($"Lấy chi tiết thành công,  UserName: {currentUser.FullName}, UserId: {currentUser.UserId}");
                return Helper.CreateSuccessResponse(result, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Lấy chi tiết thất bại, UserName: {currentUser.FullName}, UserId: {currentUser.UserId}");
                return Helper.CreateExceptionResponse<InvestorViewModel>(ex);
            }
        }
        
        public async Task<Response<InvestorViewModel>> DeleteAsync(Guid id,
            RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_Investor
                    .Include(x => x.InvestorType)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Log.Information($"Không tìm thấy bản ghi chủ đầu tư với Id {entity.Id}");
                    return Helper.CreateBadRequestResponse<InvestorViewModel>("Không tìm thấy bản ghi");
                }
                
                _dbContext.sm_Investor.Remove(entity);
                await _dbContext.SaveChangesAsync();

                Log.Information($"Xoá thành công, UserName: {currentUser.FullName}, UserId: {currentUser.UserId}");
                return Helper.CreateSuccessResponse(_mapper.Map<InvestorViewModel>(entity), "Xoá thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Xoá thất bại, UserName: {currentUser.FullName}, UserId: {currentUser.UserId}");
                return Helper.CreateExceptionResponse<InvestorViewModel>(ex);
            }
        }
        
        public async Task<Response<InvestorViewModel>> UpdateAsync(Guid id, InvestorCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var userId =  currentUser.UserId;
                var userName =  currentUser.UserName;
                
                var entity = await 
                    _dbContext.sm_Investor.Include(x => x.InvestorType)
                        .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    Log.Information($"Không tìm thấy bản ghi để cập nhật với Id {entity.Id}");
                    return Helper.CreateBadRequestResponse<InvestorViewModel>("Không tìm thấy bản ghi");
                }
                
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;
                
                entity.Name =  model.Name;
                entity.InvestorTypeId = model.InvestorTypeId;
                
                _dbContext.sm_Investor.Update(entity);
                await _dbContext.SaveChangesAsync();
                
                Log.Information("Cập nhật thành công, Model: {@model}, UserId: {@userId}, UserName: {@userName}",
                    userId, model, userName);
                return Helper.CreateSuccessResponse(_mapper.Map<InvestorViewModel>(entity), "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Cập nhật thất bại, UserName: {currentUser.FullName}, UserId: {currentUser.UserId}");
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<InvestorViewModel>(ex);
            }
        }

        private Expression<Func<sm_Investor, bool>> BuildQuery(InvestorQueryModel query)
        {
            var predicate =  PredicateBuilder.New<sm_Investor>(true);
            
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate = predicate.And(s => s.Code.ToLower().Contains(query.FullTextSearch.ToLower())
                                               || s.Name.ToLower().Contains(query.FullTextSearch.ToLower())
                );
            return predicate;
        }

        public async Task<Response<Pagination<InvestorViewModel>>> GetPageAsync(InvestorQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_Investor.AsNoTracking()
                    .Include(x => x.InvestorType)
                    .Where(predicate);
                
                var data = await queryResult.GetPageAsync(query);

                var result = _mapper.Map<Pagination<InvestorViewModel>>(data);
                
                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<Pagination<InvestorViewModel>>(ex);
            }
        }
    }
}

