using NSPC.Data;
using NSPC.Common;
using LinqKit;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Net;

namespace NSPC.Business
{
    public class ParameterHandler : IParameterHandler
    {
        private readonly DbHandler<BsdParameter, ParameterModel, ParameterQueryModel> _dbHandler = DbHandler<BsdParameter, ParameterModel, ParameterQueryModel>.Instance;

        #region CRUD

        public Task<Response> FindAsync(Guid id)
        {
            return _dbHandler.FindAsync(id);
        }

        public async Task<Response> FindByNameAsync(string name)
        {
            using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
            {
                var parameterRepo = unitOfWork.GetRepository<BsdParameter>();
                var data = await parameterRepo.FindAsync(x => x.Name.Equals(name));
                if (data == null)
                {
                    return new ResponseError(HttpStatusCode.BadRequest, $"Vui lòng tạo tham số {name}");
                }
                var result = AutoMapperUtils.AutoMap<BsdParameter, ParameterModel>(data);
                return new Response<ParameterModel>(result);
            }
        }

        public async Task<Response> CreateAsync(ParameterCreateModel model, Guid applicationId, Guid userId)
        {
            BsdParameter request = AutoMapperUtils.AutoMap<ParameterCreateModel, BsdParameter>(model);
            request = request.InitCreate(applicationId, userId);
            request.Id = Guid.NewGuid();
            var result = await _dbHandler.CreateAsync(request);
            ParameterCollection.Instance.LoadToHashSet();
            return result;
        }

        public async Task<Response> UpdateAsync(Guid id, ParameterUpdateModel model, Guid applicationId, Guid userId)
        {
            BsdParameter request = AutoMapperUtils.AutoMap<ParameterUpdateModel, BsdParameter>(model);
            request = request.InitUpdate(applicationId, userId);
            request.Id = id;
            var result = await _dbHandler.UpdateAsync(id, request);
            ParameterCollection.Instance.LoadToHashSet();
            return result;
        }

        public async Task<Response> DeleteAsync(Guid id)
        {
            var result = await _dbHandler.DeleteAsync(id);
            ParameterCollection.Instance.LoadToHashSet();
            return result;
        }

        public async Task<Response> DeleteRangeAsync(List<Guid> listId)
        {
            var result = await _dbHandler.DeleteRangeAsync(listId, "Id");
            ParameterCollection.Instance.LoadToHashSet();
            return result;
        }

        #endregion CRUD

        public Response GetAll()
        {
            try { return _dbHandler.GetAll(); }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}");
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetAllAsync()
        {
            try
            {
                var predicate = PredicateBuilder.New<BsdParameter>(true);
                return await _dbHandler.GetAllAsync(predicate);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}");
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Task<Response> GetAllAsync(ParameterQueryModel query)
        {
            var predicate = BuildQuery(query);
            return _dbHandler.GetAllAsync(predicate, query.Sort);
        }

        public async Task<Response> GetPageAsync(ParameterQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                return await _dbHandler.GetPageAsync(predicate, query);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@params}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        private Expression<Func<BsdParameter, bool>> BuildQuery(ParameterQueryModel query)
        {
            var predicate = PredicateBuilder.New<BsdParameter>(true);
            if (!string.IsNullOrEmpty(query.Name))
            {
                predicate.And(s => s.Name == query.Name);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                predicate.And(s => s.Name == query.Name);
            }
            if (query.ListId != null && query.ListId.Count > 0)
            {
                predicate.And(s => query.ListId.Contains(s.Id));
            }
            if (query.ApplicationId.HasValue && !query.SearchAllApp)
            {
                predicate.And(s => s.ApplicationId == query.ApplicationId.Value);
            }
            if (!string.IsNullOrEmpty(query.FullTextSearch))
            {
                predicate.And(s => s.Name.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Description.ToLower().Contains(query.FullTextSearch.ToLower()));
            }
            return predicate;
        }

        public async Task SaveEntity(ParameterModel model)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var parameterRepo = unitOfWork.GetRepository<BsdParameter>();
                    if (model.Id == null || model.Id == Guid.Empty)
                    {
                        var param = await parameterRepo.FindAsync(x => x.Name.Equals(model.Name));
                        if (param == null)
                        {
                            var newParam = AutoMapperUtils.AutoMap<ParameterModel, BsdParameter>(model);
                            newParam.CreatedOnDate = DateTime.Now;
                            newParam.LastModifiedOnDate = DateTime.Now;
                            parameterRepo.Add(newParam);
                        }
                        else
                        {
                            param.Value = model.Value;
                            param.GroupCode = model.GroupCode;
                            param.Description = model.Description;
                            param.LastModifiedOnDate = DateTime.Now;
                            parameterRepo.Update(param);
                        }
                    }
                    else
                    {
                        var param = await parameterRepo.FindAsync(x => x.Name.Equals(model.Name));
                        if (param == null)
                        {
                            throw new Exception("Cannot find paramter " + model.Name + " for updating.");
                        }
                        param.Value = model.Value;
                        param.GroupCode = model.GroupCode;
                        param.Description = model.Description;
                        param.LastModifiedOnDate = DateTime.Now;
                        parameterRepo.Update(param);
                    }

                    var status = await unitOfWork.SaveAsync();
                    ParameterCollection.Instance.LoadToHashSet();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Model: {@params}", model);
                Utils.CreateExceptionResponseError(ex);
            }
        }
    }
}