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


namespace NSPC.Business.Services.ConstructionActitvityLog
{
    public class ConstructionActivityLogHandler: IConstructionActivityLogHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly string _staticsFolder;

        public ConstructionActivityLogHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper, IAttachmentHandler attachmentHandler)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<ConstructionActivityLogViewModel>> Create(ConstructionActivityLogCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var entity = _mapper.Map<sm_ConstructionActivityLog>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.FullName;
                entity.UserName = currentUser.UserName;
                entity.AvatarUrl = _dbContext.IdmUser.FirstOrDefault(x => x.Id == currentUser.UserId)?.AvatarUrl;
                entity.CreatedOnDate = DateTime.Now;
                _dbContext.sm_ConstructionActivityLog.Add(entity);
                await _dbContext.SaveChangesAsync();
                
                return Helper.CreateSuccessResponse(_mapper.Map<ConstructionActivityLogViewModel>(entity), null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<ConstructionActivityLogViewModel>(ex);
            }
        }
    }
}

