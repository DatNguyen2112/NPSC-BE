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
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.Cata;
using NSPC.Data.Data.Entity.CashbookTransaction;
using NSPC.Data.Data.Entity.VatTu;
using NSPC.Data.Entity;
using static NSPC.Common.Helper;


namespace NSPC.Business.Services
{
    public class IssueActivityLogHandler: IIssueActivityLogHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;

        public IssueActivityLogHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IAttachmentHandler attachmentHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<IssueActivityLogViewModel>> Create(IssueActivityLogCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var entity = _mapper.Map<sm_IssueActivityLog>(model);
                entity.AttachmentsResolve = _mapper.Map<List<jsonb_Attachment>>(model.AttachmentsResolve);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.FullName;
                entity.UserName = currentUser.UserName;
                entity.AvatarUrl = _dbContext.IdmUser.FirstOrDefault(x => x.Id == currentUser.UserId)?.AvatarUrl;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.sm_IssueActivityLog.Add(entity);
                await _dbContext.SaveChangesAsync();
                
                return Helper.CreateSuccessResponse(_mapper.Map<IssueActivityLogViewModel>(entity), null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<IssueActivityLogViewModel>(ex);
            }
        }
        public async Task<Response<IssueActivityLogViewModel>> GetById(Guid id, RequestUser currentUser)
        {
            try
            {
                var entity = await _dbContext.sm_IssueActivityLog.FirstOrDefaultAsync(x => x.OrderId == id);

                if (entity == null)
                {
                    return Helper.CreateBadRequestResponse<IssueActivityLogViewModel>("Không tìm th?y b?n ghi");
                }

                return Helper.CreateSuccessResponse<IssueActivityLogViewModel>(_mapper.Map<IssueActivityLogViewModel>(entity));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<IssueActivityLogViewModel>(ex);
            }
        }
    }
}

