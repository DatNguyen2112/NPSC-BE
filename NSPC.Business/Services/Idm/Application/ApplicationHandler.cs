using AutoMapper;
using LinqKit;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSPC.Common;
using NSPC.Data;
using NSPC.Data.Data;
using Serilog;
using System.Data.Entity;
using System.Linq.Expressions;
using static NSPC.Common.Helper;
using static NSPC.Common.NSPCConstants;
using Code = System.Net.HttpStatusCode;
namespace NSPC.Business
{
    public class ApplicationHandler : IApplicationHandler
    {
        private readonly IMapper _mapper;
        private SMDbContext _dbContext;
        private IConfiguration _configuration;
        private readonly string _fileFolder;
        public ApplicationHandler(SMDbContext dbContext, IMapper mapper, IConfiguration configuration) 
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public Task<Response<ApplicationModel>> Create(ApplicationCreateUpdateModel model, RequestUser requestUser)
        {
            throw new NotImplementedException();
        }

        public Task<Response<ApplicationModel>> Update(Guid id, ApplicationCreateUpdateModel model, RequestUser requestUser)
        {
            throw new NotImplementedException();
        }

        public Task<Response> Delete(Guid id, RequestUser requestUser)
        {
            throw new NotImplementedException();
        }

        public Task<Response<Pagination<ApplicationModel>>> GetPageAsync(ApplicationQueryModel query, RequestUser requestUser)
        {
            throw new NotImplementedException();
        }

        public Task<Response<ApplicationModel>> GetById(Guid id, RequestUser requestUser)
        {
            throw new NotImplementedException();
        }

        //public async Task<Response<ApplicationModel>> Create(ApplicationCreateUpdateModel model, RequestUser requestUser)
        //{
        //    try
        //    {
        //        if (_dbContext.Idm_Applications.Any(x => x.Code == model.Code))
        //            return Helper.CreateBadRequestResponse<ApplicationModel>("Mã ứng dụng đã tồn tại");
        //        var entity = _mapper.Map<Idm_Application>(model);
        //        entity.Status = ApplicationStatusConstants.Draft;
        //        _dbContext.Add(entity);
        //        await _dbContext.SaveChangesAsync();
        //        return new Response<ApplicationModel>(Code.OK,_mapper.Map<ApplicationModel>(entity), "Thành công");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        return Helper.CreateExceptionResponse<ApplicationModel>(ex);
        //    }
        //}

        //public async Task<Response<ApplicationModel>> Update(Guid id, ApplicationCreateUpdateModel model, Helper.RequestUser requestUser)
        //{
        //    try
        //    {
        //        var entity = await _dbContext.Idm_Applications.FirstOrDefaultAsync(x => x.Id == id);
        //        if (entity == null)
        //            return Helper.CreateNotFoundResponse<ApplicationModel>("Không có ứng dụng này");
        //        if (_dbContext.Idm_Applications.Any(x => x.Code == model.Code && x.Id != id))
        //            return Helper.CreateBadRequestResponse<ApplicationModel>("Mã ứng dụng đã tồn tại");
        //        _mapper.Map(model, entity);
        //        await _dbContext.SaveChangesAsync();
        //        return new Response<ApplicationModel>(Code.OK, _mapper.Map<ApplicationModel>(entity), "Cập nhật thành công");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        return Helper.CreateExceptionResponse<ApplicationModel>(ex);
        //    }
        //}

        //public async Task<Response> Delete(Guid id, Helper.RequestUser requestUser)
        //{
        //    try
        //    {
        //        var entity = await _dbContext.Idm_Applications.FirstOrDefaultAsync(x => x.Id == id);
        //        if (entity == null)
        //            return Helper.CreateNotFoundResponse<ApplicationModel>("Không có ứng dụng này");
        //        _dbContext.Remove(entity);
        //        await _dbContext.SaveChangesAsync();
        //        return new Response(Code.OK, "Xóa thành công");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        return Helper.CreateExceptionResponse(ex);
        //    }
        //}

        //public async Task<Response<Pagination<ApplicationModel>>> GetPageAsync(ApplicationQueryModel query, Helper.RequestUser requestUser)
        //{
        //    try
        //    {
        //        var predicate = BuildQuery(query);

        //        var entity = await _dbContext.Idm_Applications.Where(predicate).GetPageAsync(query);

        //        var result = _mapper.Map<Pagination<ApplicationModel>>(entity);

        //        return Helper.CreateSuccessResponse<Pagination<ApplicationModel>>(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        Log.Debug("Model: {0}", JsonConvert.SerializeObject(query));
        //        return Helper.CreateExceptionResponse<Pagination<ApplicationModel>>(ex);
        //    }
        //}

        //public async Task<Response<ApplicationModel>> GetById(Guid id, Helper.RequestUser requestUser)
        //{
        //    try
        //    {
        //        var entity = await _dbContext.Idm_Applications.FirstOrDefaultAsync(x => x.Id == id);
        //        if (entity == null)
        //            return Helper.CreateNotFoundResponse<ApplicationModel>("Không có ứng dụng này");
        //        return new Response<ApplicationModel>(Code.OK, _mapper.Map<ApplicationModel>(entity));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        return Helper.CreateExceptionResponse<ApplicationModel>(ex);
        //    }
        //}

        //private Expression<Func<Idm_Application, bool>> BuildQuery(ApplicationQueryModel query)
        //{
        //    try
        //    {
        //        var predicate = PredicateBuilder.New<Idm_Application>(true);
        //        if (!string.IsNullOrEmpty(query.FullTextSearch))
        //        {
        //            predicate.And(s => s.Name.Contains(query.FullTextSearch) || s.Code.Contains(query.FullTextSearch));
        //        }
        //        return predicate;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, string.Empty);
        //        Log.Debug("Model: {0}", JsonConvert.SerializeObject(query));
        //        return null;
        //    }

        //}
    }
}
