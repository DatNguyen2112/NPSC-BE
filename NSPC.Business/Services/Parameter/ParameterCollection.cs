using NSPC.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public class ParameterCollection
    {
        private readonly IParameterHandler _handler;
        public HashSet<ParameterModel> Collection;
        public static ParameterCollection Instance { get; } = new ParameterCollection();

        protected ParameterCollection()
        {
            _handler = new ParameterHandler();
            if (Collection == null || Collection.Count() == 0)
            {
                LoadToHashSet();
            }
        }

        public void LoadToHashSet()
        {
            try
            {
                Collection = new HashSet<ParameterModel>();
                var listResponse = _handler.GetAll();
                if (listResponse.Code == HttpStatusCode.OK)
                {
                    // Add to hashset
                    if (listResponse is Response<List<ParameterModel>> listResponseObj)
                        foreach (var response in listResponseObj.Data)
                        {
                            Collection.Add(response);
                        }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: {@params}");
                Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<string> GetValue(string name)
        {
            var result = Collection.FirstOrDefault(u => u.Name == name);
            if (result != null)
            {
                return result.Value;
            }
            var value = await _handler.FindByNameAsync(name);
            if (value.Code == HttpStatusCode.OK)
            {
                if (value is Response<ParameterModel> value1)
                {
                    return value1.Data.Value;
                }
            }
            return string.Empty;
        }

        public async Task<string> GetDescription(string name)
        {
            var result = Collection.FirstOrDefault(u => u.Name == name);
            if (result != null)
            {
                return result.Description;
            }
            var value = await _handler.FindByNameAsync(name);
            if (value.Code == HttpStatusCode.OK)
            {
                if (value is Response<ParameterModel> value1)
                {
                    return value1.Data.Description;
                }
            }
            return string.Empty;
        }

        public async Task<bool> GetBoolValue(string name)
        {
            var result = await GetValue(name);
            if (string.IsNullOrEmpty(result) || result.Equals("0"))
                return false;

            if (result.Equals("true") || result.ToLower().Equals("true") || result.Equals("1"))
                return true;
            else return false;
        }

        public async Task<int> GetIntValue(string name)
        {
            var result = await GetValue(name);
            int t = 0;
            int.TryParse(result, out t);
            return t;
        }

        public async Task<decimal> GetDecimalValue(string name)
        {
            var result = await GetValue(name);
            decimal t = 0M;
            decimal.TryParse(result, out t);
            return t;
        }

        public async Task<float> GetFloatValue(string name)
        {
            var result = await GetValue(name);
            float t = 0;
            float.TryParse(result, out t);
            return t;
        }
    }
}