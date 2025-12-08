using NSPC.Common;
using NSPC.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using NSPC.Data.Data;
using NSPC.Data.Data.Entity.CodeType;

namespace NSPC.Business
{
    public class CodeTypeItemsCollection
    {
        public List<CodeTypeItemViewModel> AllCodeTypes;
        public static CodeTypeItemsCollection Instance { get; } = new CodeTypeItemsCollection();

        private MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddCollectionMappers();
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;
            cfg.CreateMap<sm_CodeType_Item, CodeTypeItemViewModel>();
        });

        IMapper mapper;

        protected CodeTypeItemsCollection()
        {
            mapper = new Mapper(config);

            if (AllCodeTypes == null || AllCodeTypes.Count() == 0)
            {
                LoadData();
            }
        }

        public void LoadData()
        {
            using (var _dataContext = new SMDbContext())
            {
                AllCodeTypes = new List<CodeTypeItemViewModel>();

                var result = _dataContext.sm_CodeType_Item.IgnoreQueryFilters().ToList();
                
                AllCodeTypes = mapper.Map<List<sm_CodeType_Item>, List<CodeTypeItemViewModel>>(result);
            }
        }

        public async void FillDataByTenant(Guid? TenantId)
        {
            // AllCodeTypes.RemoveAll(x => x.TenantId == TenantId);
            using (var _dataContext = new SMDbContext())
            {
                var entity = _dataContext.sm_CodeType_Item.IgnoreQueryFilters().Where(x => x.TenantId == TenantId).ToList();

                var result = mapper.Map<List<sm_CodeType_Item>, List<CodeTypeItemViewModel>>(entity);
                AllCodeTypes.AddRange(result);
            }
        }

        // public List<CodeTypeListModel> FetchListCode(List<string> listCode, string language)
        // {
        //     var x = new List<CodeTypeListModel>();
        //     if (listCode == null) return null;
        //
        //     foreach (var code in listCode)
        //     {
        //         var model = AllCodeTypes.Where(x => x.Code == code).FirstOrDefault();
        //
        //         if (model != null)
        //             x.Add(model);
        //     }
        //
        //     // Process language
        //     if (string.IsNullOrEmpty(language))
        //         language = LanguageConstants.English;
        //
        //     // foreach (var item in x)
        //     // {
        //     //     item.Title = item.Translations.Where(x => x.Language == language).FirstOrDefault()?.Title;
        //     // }
        //
        //     return x;
        // }

        public CodeTypeItemViewModel FetchItemsCode(string code, string language, Guid? tenantId)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            var result = AllCodeTypes.Where(x => x.Code == code).FirstOrDefault();

            // Process language
            if (string.IsNullOrEmpty(language))
                language = LanguageConstants.English;

/*            if (result != null && result.Translations != null)
                result.Title = result.Translations.Where(x => x.Language == language).FirstOrDefault()?.Title;*/

            return result;
        }

    }
}