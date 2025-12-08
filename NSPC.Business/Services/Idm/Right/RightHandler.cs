using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;

namespace NSPC.Business
{
    public class RightHandler: IRightHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RightHandler(SMDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Response<RightViewModel>> Create(RightCreateUpdateModel model)
        {
            try
            {
                if (_dbContext.IdmRight.Any(x => x.Code == model.Code && x.GroupCode == model.GroupCode))
                    return Helper.CreateBadRequestResponse<RightViewModel>("Code đã tồn tại");

                var right = _mapper.Map<IdmRight>(model);
                _dbContext.Add(right);

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse(_mapper.Map<RightViewModel>(right));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<RightViewModel>(ex);
            }
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var right = await _dbContext.IdmRight.FirstOrDefaultAsync(x => x.Id == id);
                if (right == null)
                    return new Response(HttpStatusCode.NotFound, "Không tìm thấy bản ghi");

                _dbContext.Remove(right);
                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    RightCollection.Instance.LoadToHashSet();
                    return new Response(HttpStatusCode.OK, "Xóa thành công");
                }

                return new Response(HttpStatusCode.InternalServerError, "Đã có lỗi xảy ra");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse(ex);
            }
        }

        public async Task<Response<Pagination<RightViewModel>>> GetPage(RightQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.IdmRight.AsNoTracking().Where(predicate);
                var data = await queryResult.GetPageAsync(query);

                return Helper.CreateSuccessResponse(_mapper.Map<Pagination<RightViewModel>>(data));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<Pagination<RightViewModel>>(ex);
            }
        }

        public async Task<Response<List<RightViewModel>>> GetAll()
        {
            try
            {
                var list = await _dbContext.IdmRight.AsNoTracking().ToListAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<List<RightViewModel>>(list));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<List<RightViewModel>>(ex);
            }
        }

        public async Task<Response<RightViewModel>> GetById(Guid id)
        {
            try
            {
                var right = await _dbContext.IdmRight.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (right == null)
                    return Helper.CreateNotFoundResponse<RightViewModel>("Không tìm thấy bản ghi");

                var result = _mapper.Map<RightViewModel>(right);

                return Helper.CreateSuccessResponse(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<RightViewModel>(ex);
            }
        }

        public async Task<Response<RightViewModel>> Update(Guid id, RightCreateUpdateModel model)
        {
            try
            {
                if (_dbContext.IdmRight.Any(x => x.Code == model.Code && x.GroupCode == model.GroupCode && x.Id != id))
                    return Helper.CreateBadRequestResponse<RightViewModel>("Code đã tồn tại");

                var right = await _dbContext.IdmRight.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (right == null)
                    return Helper.CreateNotFoundResponse<RightViewModel>("Không tìm thấy bản ghi");

                _mapper.Map(model, right);

                await _dbContext.SaveChangesAsync();

                return Helper.CreateSuccessResponse(_mapper.Map<RightViewModel>(right));
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse<RightViewModel>(ex);
            }
        }

        public async Task<Response> SeedRight(string groupCode, string groupName)
        {
            try
            {
                var rightList = await _dbContext.IdmRight.Where(x => x.GroupCode == groupCode && x.GroupName == groupName).ToListAsync();
                var availableAction = new string[] { RightActionConstants.ADD, RightActionConstants.DELETE, RightActionConstants.UPDATE, RightActionConstants.VIEW, RightActionConstants.VIEWALL };
                foreach (var item in availableAction)
                {
                    var right = rightList.FirstOrDefault(x => x.Code == item);
                    if (right == null)
                    {
                        _dbContext.Add(new IdmRight
                        {
                            GroupCode = groupCode,
                            GroupName = groupName,
                            Code = item,
                            CreatedOnDate = DateTime.Now,
                        });
                    }
                }

                await _dbContext.SaveChangesAsync();
                return Helper.CreateSuccessResponse("Thành công");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return Helper.CreateExceptionResponse(ex);
            }
        }


        private Expression<Func<IdmRight, bool>> BuildQuery(RightQueryModel query)
        {
            var predicate = PredicateBuilder.New<IdmRight>(true);
            return predicate;
        }
    }
}

