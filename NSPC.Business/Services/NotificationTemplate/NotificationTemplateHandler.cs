using AutoMapper;
using NSPC.Common;
using NSPC.Data;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using NSPC.Data.Data;


namespace NSPC.Business
{
    public class NotificationTemplateHandler : INotificationTemplateHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string[] _availableNotificationTemplateTypes = NotificationTypeConstants.AvailableNotificationTypes;
        private readonly string[] _availableLanguages = LanguageConstants.AvailableLanguages;


        public NotificationTemplateHandler(IConfiguration configuration, SMDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var iqueryable =  _dbContext.sm_Notification_Template.Include(x => x.Translations).Where(x => x.Id == id);

                var template = await iqueryable.FirstOrDefaultAsync();

                if (template == null)
                    return new Response(HttpStatusCode.NotFound, null);

                var result = _mapper.Map<sm_Notification_Template, NotificationTemplateAdminViewModel>(template);

                return new Response<NotificationTemplateAdminViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> Delete(Guid NotificationTemplateId)
        {
            try
            {
                var NotificationTemplate = await _dbContext.sm_Notification_Template.Where(a => a.Id == NotificationTemplateId).FirstOrDefaultAsync();

                // UpdatePaid NotificationTemplate if exist
                if (NotificationTemplate == null)
                    return new ResponseError(HttpStatusCode.NotFound, "Not found record");

                _dbContext.sm_Notification_Template.Remove(NotificationTemplate);
                var status = await _dbContext.SaveChangesAsync();

                if (status > 0)
                {
                    return new ResponseDelete(HttpStatusCode.OK, "Đã xóa thành công", NotificationTemplateId, "");
                }

                return new ResponseError(HttpStatusCode.BadRequest, "Xóa bản ghi thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: NotificationTemplateId: {@params}", NotificationTemplateId);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdateNotificationTemplate(NotificationTemplateCreateUpdateModel param)
        {
            try
            {
                bool isUpdateTemplate = true;
                var template = await _dbContext.sm_Notification_Template.Where(a => a.Type == param.Type).FirstOrDefaultAsync();
                if (template == null)
                {
                    template = new sm_Notification_Template
                    {
                        CreatedOnDate = DateTime.Now,
                        Id = Guid.NewGuid()
                    };
                    isUpdateTemplate = false;
                }

                // Validate type
                if (!_availableNotificationTemplateTypes.Contains(param.Type) || string.IsNullOrEmpty(param.Type))
                    return new ResponseError(HttpStatusCode.BadRequest, "Invalid valid type, should be one of: " + string.Join(", ", _availableNotificationTemplateTypes));

                // Validate translation language
                if (param.Translations?.Count > 0)
                {
                    var languages = param.Translations.Select(x => x.Language).ToList();
                    foreach (var language in languages)
                    {
                        if (string.IsNullOrEmpty(language) || !_availableLanguages.Contains(language))
                            return new ResponseError(HttpStatusCode.BadRequest, "Translations have invalid language code, should be one of: " + string.Join(", ", _availableLanguages));
                    }
                }

                // UpdatePaid values
                template.Type = param.Type;
                template.Name = param.Name;
                template.LastModifiedOnDate = DateTime.Now;
                template.CreatedOnDate = DateTime.Now;

                // Add if not exst
                if (!isUpdateTemplate)
                    _dbContext.sm_Notification_Template.Add(template);

                // UpdatePaid translation
                if (param.Translations != null && param.Translations.Any())
                {
                    foreach (var translation in param.Translations)
                    {
                        bool isUpdateTranslation = true;

                        // Select translation
                        var templateTranslation = await _dbContext.sm_Notification_Template_Translation
                            .FirstOrDefaultAsync(a => a.NotificationTemplateId == template.Id && a.Language == translation.Language);

                        if (templateTranslation == null)
                        {
                            templateTranslation = new sm_Notification_Template_Translation();
                            templateTranslation.Id = Guid.NewGuid();
                            templateTranslation.CreatedOnDate = DateTime.Now;
                            isUpdateTranslation = false;
                        }

                        templateTranslation.NotificationTemplateId = template.Id;
                        templateTranslation.BodyHtmlTemplate = translation.BodyHtmlTemplate;
                        templateTranslation.BodyPlainTextTemplate = translation.BodyPlainTextTemplate;
                        templateTranslation.TitleTemplate = translation.TitleTemplate;
                        templateTranslation.Language = translation.Language;
                        templateTranslation.LastModifiedOnDate = DateTime.Now;

                        if (!isUpdateTranslation)
                            _dbContext.sm_Notification_Template_Translation.Add(templateTranslation);
                    }
                }

                await _dbContext.SaveChangesAsync();

                var data = _mapper.Map<sm_Notification_Template, NotificationTemplateAdminViewModel>(template);
                return new Response<NotificationTemplateAdminViewModel>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: {@Param}, template type: {@type}", param, param.Type);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetAdminListPageAsync(NotificationTemplateQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);

                var queryResult = _dbContext.sm_Notification_Template.Include(x => x.Translations).Where(predicate);

                // Type filter
                if (!string.IsNullOrEmpty(query.Type))
                    queryResult = queryResult.Where(c => c.Type == query.Type);

                var data = await queryResult.GetPageAsync(query);

                var ids = data.Content.Select(x => x.Id).ToList();

                var result = _mapper.Map<Pagination<sm_Notification_Template>, Pagination<NotificationTemplateViewModel>>(data);

                if (result != null)
                {
                    return new ResponsePagination<NotificationTemplateViewModel>(result);
                }

                return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Query {@params}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        private Expression<Func<sm_Notification_Template, bool>> BuildQuery(NotificationTemplateQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_Notification_Template>(true);

            if (!string.IsNullOrEmpty(query.FullTextSearch))
            {
                predicate.And(x => x.Name.ToLower().Contains(query.FullTextSearch.ToLower()) ||
                                     x.Type.ToLower().Contains(query.FullTextSearch.ToLower())
                                   || x.Translations.Any(c => c.TitleTemplate.Contains(query.FullTextSearch)
                                    || c.BodyPlainTextTemplate.Contains(query.FullTextSearch) || c.BodyPlainTextTemplate.Contains(query.FullTextSearch))
                );
            }
            if (!string.IsNullOrEmpty(query.Type))
            {
                predicate.And(x => x.Type == query.Type);
            }

            return predicate;
        }
    }
}