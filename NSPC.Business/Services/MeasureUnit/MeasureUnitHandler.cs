using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSPC.Common;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.AssetCategories;
using Serilog;
using System.Linq.Expressions;

namespace NSPC.Business;

public class MeasureUnitHandler : IMeasureUnitHandler
{
    private readonly SMDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MeasureUnitHandler(
        SMDbContext dbContext,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<MeasureUnitViewModel>> Create(MeasureUnitCreateUpdateModel model)
    {
        try
        {
            var validationResult = await ValidateCreateUpdateModel<MeasureUnitViewModel>(model);
            if (validationResult != null)
            {
                return validationResult;
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
            var measureUnit = new sm_MeasureUnit
            {
                Id = Guid.NewGuid(),
                Code = model.Code,
                Name = model.Name,
                CreatedOnDate = DateTime.Now,
                CreatedByUserId = currentUser.UserId,
                CreatedByUserName = currentUser.UserName,
                LastModifiedOnDate = DateTime.Now,
                LastModifiedByUserId = currentUser.UserId,
                LastModifiedByUserName = currentUser.UserName,
                TenantId = currentUser.TenantId
            };

            _dbContext.sm_MeasureUnit.Add(measureUnit);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<MeasureUnitViewModel>(measureUnit),
                "Tạo đơn vị tính thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<MeasureUnitViewModel>(e);
        }
    }

    public async Task<Response<MeasureUnitViewModel>> Update(Guid id, MeasureUnitCreateUpdateModel model)
    {
        try
        {
            var measureUnit = await _dbContext.sm_MeasureUnit.FindAsync(id);
            if (measureUnit == null)
            {
                return Helper.CreateBadRequestResponse<MeasureUnitViewModel>("Đơn vị tính không tồn tại");
            }

            var validationResult = await ValidateCreateUpdateModel<MeasureUnitViewModel>(model, id);
            if (validationResult != null)
            {
                return validationResult;
            }

            var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

            measureUnit.Code = model.Code;
            measureUnit.Name = model.Name;
            measureUnit.LastModifiedOnDate = DateTime.Now;
            measureUnit.LastModifiedByUserId = currentUser.UserId;
            measureUnit.LastModifiedByUserName = currentUser.UserName;

            _dbContext.sm_MeasureUnit.Update(measureUnit);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse(_mapper.Map<MeasureUnitViewModel>(measureUnit),
                "Cập nhật đơn vị tính thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Model: {@model}", model);
            return Helper.CreateExceptionResponse<MeasureUnitViewModel>(e);
        }
    }

    public async Task<Response<Pagination<MeasureUnitViewModel>>> GetPage(MeasureUnitQueryModel query)
    {
        try
        {
            var predicate = BuildQuery(query);
            var queryResult = _dbContext.sm_MeasureUnit.Where(predicate);
            var data = await queryResult.GetPageAsync(query);
            var result = _mapper.Map<Pagination<MeasureUnitViewModel>>(data);

            return Helper.CreateSuccessResponse(result);
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Query: {@query}", query);
            return Helper.CreateExceptionResponse<Pagination<MeasureUnitViewModel>>(e);
        }
    }

    public async Task<Response<MeasureUnitViewModel>> GetById(Guid id)
    {
        try
        {
            var measureUnit = await _dbContext.sm_MeasureUnit.FindAsync(id);

            if (measureUnit == null)
            {
                return Helper.CreateBadRequestResponse<MeasureUnitViewModel>("Đơn vị tính không tồn tại");
            }

            return Helper.CreateSuccessResponse(_mapper.Map<MeasureUnitViewModel>(measureUnit));
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Id: {@id}", id);
            return Helper.CreateExceptionResponse<MeasureUnitViewModel>(e);
        }
    }

    public async Task<Response> Delete(Guid id)
    {
        try
        {
            var measureUnit = await _dbContext.sm_MeasureUnit.FindAsync(id);

            if (measureUnit == null)
            {
                return Helper.CreateBadRequestResponse("Đơn vị tính không tồn tại");
            }

            _dbContext.sm_MeasureUnit.Remove(measureUnit);
            await _dbContext.SaveChangesAsync();

            return Helper.CreateSuccessResponse("Xóa đơn vị tính thành công");
        }
        catch (Exception e)
        {
            Log.Error(e, string.Empty);
            Log.Information("Params: Id: {@id}", id);
            return Helper.CreateExceptionResponse(e);
        }
    }

    private Expression<Func<sm_MeasureUnit, bool>> BuildQuery(MeasureUnitQueryModel query)
    {
        var currentUser = Helper.GetRequestInfo(_httpContextAccessor);
        var predicate = PredicateBuilder.New<sm_MeasureUnit>(true);

        if (currentUser.TenantId != null)
        {
            predicate.And(x => x.TenantId == currentUser.TenantId);
        }

        if (!string.IsNullOrWhiteSpace(query.FullTextSearch))
        {
            predicate.And(x =>
                x.Code.ToLower().Contains(query.FullTextSearch.Trim().ToLower()) ||
                x.Name.ToLower().Contains(query.FullTextSearch.Trim().ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            predicate.And(x => x.Code.ToLower().Contains(query.Code.Trim().ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            predicate.And(x => x.Name.ToLower().Contains(query.Name.Trim().ToLower()));
        }

        return predicate;
    }

    private async Task<Response<T>> ValidateCreateUpdateModel<T>(MeasureUnitCreateUpdateModel model, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(model.Code))
        {
            return Helper.CreateBadRequestResponse<T>("Mã đơn vị tính không được để trống");
        }

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            return Helper.CreateBadRequestResponse<T>("Tên đơn vị tính không được để trống");
        }

        var exists = await _dbContext.sm_MeasureUnit
            .AnyAsync(x => x.Code.ToLower() == model.Code.ToLower() && (id == null || x.Id != id));

        if (exists)
        {
            return Helper.CreateBadRequestResponse<T>("Mã đơn vị tính đã tồn tại");
        }

        return null;
    }
}