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
using NSPC.Data.Entity;
using static NSPC.Common.Helper;
using NSPC.Business.Services.ConstructionActitvityLog;

namespace NSPC.Business.Services
{
    public class CustomerServiceCommentHandler : ICustomerServiceCommentHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;
        private readonly IConstructionActivityLogHandler  _constructionActivityLogHandler;

        public CustomerServiceCommentHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IConstructionActivityLogHandler constructionActivityLogHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _constructionActivityLogHandler = constructionActivityLogHandler;
        }

        public async Task<Response<CustomerServiceCommentViewModel>> Create(CustomerServiceCommentCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var userId = currentUser.UserId;
                var userName = currentUser.FullName;
                
                var entity = _mapper.Map<sm_CustomerServiceComment>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.FullName;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.sm_CustomerServiceComment.Add(entity);
                
                await _dbContext.SaveChangesAsync();
                
                return Helper.CreateSuccessResponse(_mapper.Map<CustomerServiceCommentViewModel>(entity), "Thêm mới bình luận thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<CustomerServiceCommentViewModel>(ex);
            }
        }

        public async Task<Response<CustomerServiceCommentViewModel>> Update(Guid Id,
            CustomerServiceCommentCreateModel model, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_CustomerServiceComment.AsNoTracking()
                    .Include(x => x.CreatedByUser)
                    .FirstOrDefaultAsync(x => x.Id == Id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<CustomerServiceCommentViewModel>("Không tìm thấy comment");
                }
                
                entity.LastModifiedByUserId = currentUser.UserId;
                entity.LastModifiedByUserName = currentUser.FullName;
                entity.LastModifiedOnDate = DateTime.Now;
                
                entity.Content = model.Content;
                
                _dbContext.sm_CustomerServiceComment.Update(entity);
                await _dbContext.SaveChangesAsync();
                
                return Helper.CreateSuccessResponse(_mapper.Map<CustomerServiceCommentViewModel>(entity), "Chỉnh sửa bình luận thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<CustomerServiceCommentViewModel>(ex);
            }
        }

        public async Task<Response<CustomerServiceCommentViewModel>> Delete(Guid Id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_CustomerServiceComment.AsNoTracking()
                    .Include(x => x.CreatedByUser)
                    .FirstOrDefaultAsync(x => x.Id == Id);
                if (entity == null)
                    return Helper.CreateNotFoundResponse<CustomerServiceCommentViewModel>(string.Format("Không tìm thấy comment!"));

                _dbContext.Remove(entity);
               await _dbContext.SaveChangesAsync();
                
                return Helper.CreateSuccessResponse(_mapper.Map<CustomerServiceCommentViewModel>(entity), "Xóa bình luận thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@id}", Id);
                return Helper.CreateExceptionResponse<CustomerServiceCommentViewModel>(ex);
            }
        }

        public async Task<Response<Pagination<CustomerServiceCommentViewModel>>> GetPageAsync(
            CustomerServiceQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_CustomerServiceComment.AsNoTracking()
                    .Include(x => x.CreatedByUser)
                    .Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                return Helper.CreateSuccessResponse(_mapper.Map<Pagination<CustomerServiceCommentViewModel>>(data));

            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@query}", query);
                return Helper.CreateExceptionResponse<Pagination<CustomerServiceCommentViewModel>>(ex);
            }
        }
        
        private Expression<Func<sm_CustomerServiceComment, bool>> BuildQuery(CustomerServiceQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_CustomerServiceComment>(true);

            if (query.ConstructionId.HasValue)
            {
                predicate.And(x => x.ConstructionId == query.ConstructionId);
            }
            
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s => s.Content.ToLower().Contains(query.FullTextSearch.ToLower()));

            if (query.ParticipantId.HasValue)
                predicate.And(s => s.CreatedByUser.Id == query.ParticipantId);

            return predicate;
        }
    }
}

