using AutoMapper;
using Microsoft.AspNetCore.Http;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Entity;
using Serilog;
using static NSPC.Common.Helper;

namespace NSPC.Business.Services
{
    public class ProductInventoryHandler : IProductInventoryHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ProductInventoryHandler(SMDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Response<ProductInventoryViewModel>> Create(ProductInventoryCreateModel model,
            RequestUser currentUser)
        {
            try
            {
                var productInventory = new sm_ProductInventory()
                {
                    Id = Guid.NewGuid(),
                    ProductId = model.ProductId,
                    WarehouseCode = model.WarehouseCode,
                    SellableQuantity = model.SellableQuantity,
                    CreatedByUserName = currentUser.FullName,
                    CreatedByUserId = currentUser.UserId,
                    CreatedOnDate = DateTime.Now,
                };
                
                _dbContext.sm_ProductInventory.Add(productInventory);

                await _dbContext.SaveChangesAsync();

                return new Response<ProductInventoryViewModel>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@model}", model);
                return Helper.CreateExceptionResponse<ProductInventoryViewModel>(ex);
            }
        }
    }  
}

