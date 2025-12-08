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
using Serilog;

namespace NSPC.Business
{
    public class NotificationTemplateCollection
    {
        public List<NotificationTemplateAdminViewModel> AllNotificationTemplates;
        public static NotificationTemplateCollection Instance { get; } = new NotificationTemplateCollection();

        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddCollectionMappers();
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;
            cfg.CreateMap<sm_Notification_Template_Translation, NotificationTemplateTranslationModel>();
            cfg.CreateMap<sm_Notification_Template, NotificationTemplateAdminViewModel>()
                .ForMember(dest => dest.Translations, x => x.MapFrom(src => src.Translations));
        });

        IMapper mapper;

        protected NotificationTemplateCollection()
        {
            mapper = new Mapper(config);

            if (AllNotificationTemplates == null || AllNotificationTemplates.Count() == 0)
            {
                LoadData();
            }
        }

        public void LoadData()
        {
            using (var _dataContext = new SMDbContext())
            {
                AllNotificationTemplates = new List<NotificationTemplateAdminViewModel>();

                var result = _dataContext.sm_Notification_Template.Include(x => x.Translations).ToList();

                AllNotificationTemplates = mapper.Map<List<sm_Notification_Template>, List<NotificationTemplateAdminViewModel>>(result);
            }
        }


        public NotificationTemplateViewModel FetchNotificationTemplate(string type, string language)
        {
            try
            {
                if (string.IsNullOrEmpty(language))
                    language = LanguageConstants.English;

                var result = new NotificationTemplateViewModel();

                var data = AllNotificationTemplates.Where(x => x.Type == type).FirstOrDefault();

                result.BodyHtmlTemplate = data.Translations.Where(x => x.Language == language).FirstOrDefault()?.BodyHtmlTemplate;
                result.TitleTemplate = data.Translations.Where(x => x.Language == language).FirstOrDefault()?.TitleTemplate;
                result.BodyPlainTextTemplate = data.Translations.Where(x => x.Language == language).FirstOrDefault()?.BodyPlainTextTemplate;

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                return null;
            }
        }
    }
}