using NSPC.Common;
using NSPC.Data;
using LinqKit;
using Serilog;
using System.Linq.Expressions;
using System.Net;
using NSPC.Data.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace NSPC.Business
{
    public class RoleHandler : IRoleHandler
    {
        private readonly DbHandler<IdmRole, RoleModel, RoleQueryModel> _dbHandler =
            DbHandler<IdmRole, RoleModel, RoleQueryModel>.Instance;
        private readonly SMDbContext _dbContext;
        private readonly IMapper _mapper;

        public RoleHandler()
        {
            
        }
        public RoleHandler(SMDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext; 
            _mapper = mapper;
        }

        public async Task<Response> GetDetail(Guid id, Guid applicationId)
        {
            try
            {
                var model = await _dbHandler.FindAsync(id);

                if (model.Code == HttpStatusCode.OK)
                {
                    var modelData = model as Response<RoleModel>;
                    var result = AutoMapperUtils.AutoMap<RoleModel, RoleDetailModel>(modelData?.Data);
                    
                    return new Response<RoleDetailModel>(result);
                }

                return model;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@id}, ApplicationIds: {@applicationId}", id, applicationId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetPageAsync(RoleQueryModel query)
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

        public async Task<Response> GetPageAsync2(RoleQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var entity = await _dbContext.IdmRole.Where(predicate).GetPageAsync(query);
                var result = _mapper.Map<Pagination<RoleModel>>(entity);
                return Helper.CreateSuccessResponse<Pagination<RoleModel>>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query: {@params}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetAllAsync(RoleQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                return await _dbHandler.GetAllAsync(predicate, query.Sort);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetAllAsync()
        {
            try
            {
                var predicate = PredicateBuilder.New<IdmRole>(true);
                return await _dbHandler.GetAllAsync(predicate);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}");
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Response GetAll()
        {
            return _dbHandler.GetAll();
        }

        private static Expression<Func<IdmRole, bool>> BuildQuery(RoleQueryModel query)
        {
            var predicate = PredicateBuilder.New<IdmRole>(true);
            if (!string.IsNullOrEmpty(query.Code)) predicate.And(s => s.Code == query.Code);
            if (!string.IsNullOrEmpty(query.Name)) predicate.And(s => s.Name == query.Name);
            if (query.Id.HasValue) predicate.And(s => s.Id == query.Id);
            if (query.ListId != null) predicate.And(s => query.ListId.Contains(s.Id));
            if (query.IsEmployee.HasValue)
            {
                if (query.IsEmployee.Value == true)
                    predicate.And(x => x.Code != RoleConstants.FarmerSideRoleCode && x.Code != RoleConstants.OrderSideRoleCode && x.Code != RoleConstants.NormalUser && x.Code != RoleConstants.AdminRoleCode);
                if (query.IsEmployee.Value == false)
                    predicate.And(x => x.Code == RoleConstants.FarmerSideRoleCode || x.Code == RoleConstants.OrderSideRoleCode);
            }
            // if(query.ApplicationId.HasValue&& !query.SearchAllApp){
            //     predicate.And(s => s.ApplicationId == query.ApplicationId.Value);
            // }
            if (!string.IsNullOrEmpty(query.FullTextSearch))
                predicate.And(s =>
                    s.Code.ToLower().Contains(query.FullTextSearch.ToLower()) || s.Name.ToLower().Contains(query.FullTextSearch.ToLower()) ||
                    s.Description.ToLower().Contains(query.FullTextSearch.ToLower()));
            return predicate;
        }

        #region CRUD

        public async Task<Response> CreateAsync(RoleCreateModel model, Guid? appId, Guid? actorId)
        {
            try
            {
                //using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                //{
                //    var roleExist = await unitOfWork.GetRepository<IdmRole>().FindAsync(x => x.Code == model.Code);
                //    if (roleExist != null)
                //        return new ResponseError(HttpStatusCode.BadRequest, $"Code {model.Code} đã tồn tại", null);

                //    var request = AutoMapperUtils.AutoMap<RoleCreateModel, IdmRole>(model);
                //    request.Id = Guid.NewGuid();
                //    var result = await _dbHandler.CreateAsync(request, "Name", "Code");

                //    if (result.Code == HttpStatusCode.OK)
                //    {
                //        result.Message = "Thêm mới thành công";
                //        RoleCollection.Instance.LoadToHashSet();

                //    }

                //    return result;
                //}

                var roleExits = await _dbContext.IdmRole.FirstOrDefaultAsync(x => x.Code == model.Code);
                if(roleExits != null)
                    return new ResponseError(HttpStatusCode.BadRequest, $"Code {model.Code} đã tồn tại", null);
                var request = AutoMapperUtils.AutoMap<RoleCreateModel, IdmRole>(model);
                request.Id = Guid.NewGuid();
                _dbContext.Add(request);
                var status = await _dbContext.SaveChangesAsync();
                if(status > 0)
                {
                    RoleCollection.Instance.LoadToHashSet();
                    return Helper.CreateSuccessResponse("Thêm mới thành công");
                }
                else return Helper.CreateBadRequestResponse("Thêm mới thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params},AppIds: {@AppIds}, ActorIds: {@ActorIds}", model, appId, actorId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdateAsync(Guid id, RoleUpdateModel model, Guid? appId, Guid? actorId)
        {
            try
            {
                var entity =  await _dbContext.IdmRole.FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    return new ResponseError(HttpStatusCode.BadRequest, $"Không tìm thấy bản ghi", null);
                }
                
                if (_dbContext.IdmRole.Any(x => x.Code == model.Code &&  x.Id != id))
                    return new ResponseError(HttpStatusCode.BadRequest, $"Code {model.Code} đã tồn tại", null);

                entity.Code = model.Code;
                entity.Description = model.Description;
                entity.Name = model.Name;
                entity.LastModifiedOnDate = DateTime.Now;
                
                _dbContext.IdmRole.Update(entity);
                await _dbContext.SaveChangesAsync();
                
                RoleCollection.Instance.LoadToHashSet();
                return Helper.CreateSuccessResponse("Chỉnh sửa thành công");
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params},Models: {@Models} AppIds: {@AppIds}, ActorIds: {@ActorIds}", id, model, appId, actorId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }
        public async Task<Response> DeleteAsync(Guid id)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork(new DatabaseFactory()))
                {
                    var currentRole = await unitOfWork.GetRepository<IdmRole>().FindAsync(id);
                    if (currentRole == null)
                        return new ResponseError(HttpStatusCode.BadRequest, "Nhóm người dùng không tồn tại", null);

                    unitOfWork.GetRepository<IdmRole>().Delete(currentRole);

                    if (await unitOfWork.SaveAsync() > 0)
                    {
                        RoleCollection.Instance.LoadToHashSet();
                        return new ResponseDelete(HttpStatusCode.OK, "Xóa thành công", id, currentRole.Name);
                    }

                    Log.Error("The sql statement is not executed!");
                    return new ResponseError(HttpStatusCode.BadRequest, "Câu lệnh sql không thể thực thi");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> DeleteRangeAsync(List<Guid> listId)
        {
            try
            {
                var resultData = new List<ResponseDelete>();
                foreach (var id in listId)
                {
                    var model = await DeleteAsync(id) as ResponseDelete;
                    resultData.Add(model);
                }

                var result = new ResponseDeleteMulti(resultData);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}", listId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public Task<Response> FindAsync(Guid id)
        {
            return _dbHandler.FindAsync(id);
        }

        #endregion CRUD
    }
}