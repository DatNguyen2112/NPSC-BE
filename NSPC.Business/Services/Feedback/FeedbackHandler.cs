using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using NSPC.Business.Services.CashbookTransaction;
using NSPC.Business.Services.DebtTransaction;
using NSPC.Business.Services.InventoryNote;
using NSPC.Business.Services.StockTransaction;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.Feedback;
using Serilog;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services.Feedback
{
    public class FeedbackHandler : IFeedbackHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        // private readonly string _staticsFolder;
        private readonly ICashbookTransactionHandler _cashbookTransactionHandler;
        private readonly IStockTransactionHandler _stockTransactionHandler;
        // private readonly IVatTuHandler _vatTuHandler;
        private readonly IDebtTransactionHandler _debtTransactionHandler;
        private readonly IInventoryNoteHandler _inventoryNoteHandler;
        private readonly string _staticsFolder;
        public FeedbackHandler(SMDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            // _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
            _staticsFolder = Utils.GetConfig("StaticFiles:Folder");
        }
        public async Task<Response<FeedbackViewModel>> Create(FeedbackCreateModel model, RequestUser currentUser)
        {
            try
            {

                if (model.Content == null)
                {
                    return Helper.CreateBadRequestResponse<FeedbackViewModel>
                       (string.Format("Vui lòng nhập nội dung góp ý"));
                }   

                var entity = _mapper.Map<sm_Feedback>(model);
                entity.Id = Guid.NewGuid();
                entity.CreatedByUserId = currentUser.UserId;
                entity.CreatedByUserName = currentUser.UserName;
                entity.CreatedOnDate = DateTime.Now;

                _dbContext.sm_Feedback.Add(entity);
                var createResult = await _dbContext.SaveChangesAsync();


                return Helper.CreateSuccessResponse(_mapper.Map<FeedbackViewModel>(entity), "Thêm mới thành công");
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<FeedbackViewModel>(ex);
            }
        }
    }

}
