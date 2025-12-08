using NSPC.Common;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace NSPC.Business
{
    public class RoleCollection
    {
        private readonly IRoleHandler _handler;
        public HashSet<RoleModel> Collection;
        public static RoleCollection Instance { get; } = new RoleCollection();

        protected RoleCollection()
        {
            _handler = new RoleHandler();
            LoadToHashSet();
        }

        public void LoadToHashSet()
        {
            Collection = new HashSet<RoleModel>();
            // Query to list
            var listResponse = _handler.GetAll();
            // Add to hashset
            if (listResponse.Code == HttpStatusCode.OK)
            {
                // Add to hashset
                if (listResponse is Response<List<RoleModel>> listResponseData)
                    foreach (var response in listResponseData.Data)
                    {
                        Collection.Add(response);
                    }
            }
        }

        public string GetName(Guid id)
        {
            var result = Collection.FirstOrDefault(u => u.Id == id);
            return result?.Name;
        }

        public BaseRoleModel GetModel(Guid id)
        {
            var result = Collection.FirstOrDefault(u => u.Id == id);
            return result;
        }

        public BaseRoleModel GetModel(string userName)
        {
            var result = Collection.FirstOrDefault(u => u.Name == userName);
            return result;
        }

        public List<BaseRoleModel> GetModel(Expression<Func<BaseRoleModel, bool>> predicate)
        {
            var result = Collection.AsQueryable().Where(predicate).OrderBy(x => x.Level);
            return result.ToList();
        }

        public List<BaseRoleModel> GetModelFromListCode(List<string> listCode)
        {
            var predicate = PredicateBuilder.New<BaseRoleModel>(true);

            if (listCode != null && listCode.Count() > 0)
            {
                predicate.And(x => listCode.Contains(x.Code));
                return GetModel(predicate);
            }
            else return null;
        }

        public List<BaseRoleModel> GetModelNotListCode(List<string> listCode)
        {
            var predicate = PredicateBuilder.New<BaseRoleModel>(true);

            if (listCode != null && listCode.Count() > 0)
            {
                predicate.And(x => !listCode.Contains(x.Code));         
            }
            return GetModel(predicate);
        }

        public BaseRoleModel GetCode(string cONTRIBUTOR_LV2_Code)
        {
            var result = Collection.FirstOrDefault(u => u.Code == cONTRIBUTOR_LV2_Code);
            return result;
        }
    }
}