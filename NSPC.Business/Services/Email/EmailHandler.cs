using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using NSPC.Common;
using NSPC.Data;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSPC.Common;
using NSPC.Data.Data;
using Serilog;
using NSPC.Common;

namespace NSPC.Business
{
    public class EmailHandler : IEmailHandler
    {
        private readonly SMDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string[] _availableLanguages = new string[] { LanguageConstants.English, LanguageConstants.Vietnamese};

        public EmailHandler(IConfiguration configuration, SMDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response> Delete(Guid id)
        {
            try
            {
                var eTemplate = await _dbContext.sm_Email_Templates.Where(a => a.Id == id).FirstOrDefaultAsync();
                if (eTemplate == null)
                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy bản ghi");
                _dbContext.sm_Email_Templates.Remove(eTemplate);

                var status = await _dbContext.SaveChangesAsync();
                if (status > 0)
                {
                    return new ResponseDelete(HttpStatusCode.OK, "Đã xóa thành công", id, "");
                }

                return new ResponseError(HttpStatusCode.BadRequest, "Xoá bản ghi thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetTemplateByCode(string code, string language)
        {
            try
            {
                var query = _dbContext.sm_Email_Templates.Where(x => x.Code == code);
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    var roles = _httpContextAccessor.HttpContext.Items[NSPCConstants.ClaimConstants.ROLES] != null
                        ? _httpContextAccessor.HttpContext.Items[NSPCConstants.ClaimConstants.ROLES].ToString()
                        : null;
                    var languageCode = _httpContextAccessor.HttpContext.Request.Headers[NSPCConstants.ClaimConstants.LANGUAGE].FirstOrDefault();

                    if (roles.Contains(RoleConstants.AdminRoleCode))
                    {
                        query = query.Include(x => x.sm_Email_Template_Translation);
                    }
                    else
                    {
                        query = query.Include(x => x.sm_Email_Template_Translation).Where(x => x.sm_Email_Template_Translation.Any(y => y.Language == languageCode));
                    }
                }
                var eTemplate = await query.FirstOrDefaultAsync();

                if (eTemplate == null)
                    return new Response(HttpStatusCode.NotFound, "Không tìm thấy bản ghi");

                var result = _mapper.Map<sm_Email_Template, EmailTemplateViewModel>(eTemplate);
                return new Response<EmailTemplateViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Code: {@params}", code);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var query = _dbContext.sm_Email_Templates.Include(y => y.sm_Email_Template_Translation).Where(x => x.Id == id);
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    var roles = _httpContextAccessor.HttpContext.Items[NSPCConstants.ClaimConstants.ROLES] != null
                        ? _httpContextAccessor.HttpContext.Items[NSPCConstants.ClaimConstants.ROLES].ToString()
                        : null;
                    var languageCode = _httpContextAccessor.HttpContext.Request.Headers[NSPCConstants.ClaimConstants.LANGUAGE].FirstOrDefault();

                    if (roles.Contains(RoleConstants.AdminRoleCode))
                    {
                        query = query.Include(x => x.sm_Email_Template_Translation);
                    }
                    else
                    {
                        query = query.Include(x => x.sm_Email_Template_Translation).Where(x => x.sm_Email_Template_Translation.Any(y => y.Language == languageCode));
                    }
                }
                var eTemplate = await query.FirstOrDefaultAsync();

                if (eTemplate == null)
                    return new Response(HttpStatusCode.NotFound, null);

                var result = _mapper.Map<sm_Email_Template, EmailTemplateViewModel>(eTemplate);

                return new Response<EmailTemplateViewModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> CreateEmailTemplate(EmailTemplateCreateUpdateModel request)
        {
            try
            {
                var emailTemplate = await _dbContext.sm_Email_Templates.Where(a => a.Code.ToLower() == request.Code.ToLower()).FirstOrDefaultAsync();

                if (emailTemplate != null)
                    return new ResponseError(HttpStatusCode.BadRequest, "Record existed!");

                // Validate translation language
                if (request.Translations?.Count > 0)
                {
                    var languages = request.Translations.Select(x => x.Language).ToList();
                    foreach (var language in languages)
                    {
                        if (string.IsNullOrEmpty(language))
                            return new ResponseError(HttpStatusCode.BadRequest, "Translations have invalid language code, should be one of: " + string.Join(", ", _availableLanguages));
                    }
                }

                emailTemplate = new sm_Email_Template();
                emailTemplate.Id = Guid.NewGuid();
                emailTemplate.Name = request.Name;
                emailTemplate.Code = request.Code;
                emailTemplate.Description = request.Description;
                emailTemplate.CreatedOnDate = DateTime.Now;

                if (request.Translations?.Count > 0)
                {
                    emailTemplate.sm_Email_Template_Translation = new List<sm_Email_Template_Translation>();
                    foreach (var itemTranslations in request.Translations)
                    {
                        var newEmailTemplateTranslations = new sm_Email_Template_Translation();
                        newEmailTemplateTranslations.EmailTemplateId = emailTemplate.Id;
                        newEmailTemplateTranslations.TitleTemplate = itemTranslations.TitleTemplate;
                        newEmailTemplateTranslations.BodyTemplate = itemTranslations.BodyTemplate;
                        newEmailTemplateTranslations.Language = itemTranslations.Language;
                        emailTemplate.sm_Email_Template_Translation.Add(newEmailTemplateTranslations);
                    }
                }

                _dbContext.Add(emailTemplate);

                var status = await _dbContext.SaveChangesAsync();
                var data = _mapper.Map<sm_Email_Template, EmailTemplateViewModel>(emailTemplate);
                return new Response<EmailTemplateViewModel>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Request: {@params}", request);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UpdateEmailTemplate(EmailTemplateCreateUpdateModel request, Guid id)
        {
            try
            {
                var eTemplate = await _dbContext.sm_Email_Templates.Where(a => a.Id == id).FirstOrDefaultAsync();

                if (eTemplate == null)
                    return new ResponseError(HttpStatusCode.NotFound, "Email Template not found");

                // Validate translation language
                if (request.Translations?.Count > 0)
                {
                    var languages = request.Translations.Select(x => x.Language).ToList();
                    foreach (var language in languages)
                    {
                        if (string.IsNullOrEmpty(language))
                            return new ResponseError(HttpStatusCode.BadRequest, "Translations have invalid language code, should be one of: " + string.Join(", ", _availableLanguages));
                    }
                }

                // UpdatePaid
                eTemplate.Code = request.Code;
                eTemplate.Name = request.Name;
                eTemplate.Description = request.Description;

                if (request.Translations != null && request.Translations.Any())
                {
                    foreach (var translations in request.Translations)
                    {
                        bool isUpdateTranslation = true;

                        // Select translation
                        var emailTeamplateTranslation = await _dbContext.sm_Email_Template_Translations
                            .FirstOrDefaultAsync(a => a.EmailTemplateId == id && a.Language == translations.Language);

                        if (emailTeamplateTranslation == null)
                        {
                            emailTeamplateTranslation = new sm_Email_Template_Translation();
                            emailTeamplateTranslation.Id = Guid.NewGuid();
                            emailTeamplateTranslation.CreatedOnDate = DateTime.Now;
                            isUpdateTranslation = false;
                        }

                        emailTeamplateTranslation.EmailTemplateId = eTemplate.Id;
                        emailTeamplateTranslation.BodyTemplate = translations.BodyTemplate;
                        emailTeamplateTranslation.TitleTemplate = translations.TitleTemplate;
                        emailTeamplateTranslation.Language = translations.Language;
                        emailTeamplateTranslation.LastModifiedOnDate = DateTime.Now;

                        if (!isUpdateTranslation)
                            _dbContext.sm_Email_Template_Translations.Add(emailTeamplateTranslation);
                    }
                }

                await _dbContext.SaveChangesAsync();

                var data = _mapper.Map<sm_Email_Template, EmailTemplateViewModel>(eTemplate);
                return new Response<EmailTemplateViewModel>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: Request: {@request}, Id: {@Id}", request, id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetListPageAsync(EmailTemplateQueryModel query)
        {
            try
            {
                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_Email_Templates.Include(x => x.sm_Email_Template_Translation).Where(predicate);

                var data = await queryResult.GetPageAsync(query);
                var result = _mapper.Map<Pagination<sm_Email_Template>, Pagination<EmailTemplateViewModel>>(data);

                if (result != null)
                {
                    if (!string.IsNullOrEmpty(query.Language))
                    {
                        foreach (var item in result.Content)
                        {
                            item.BodyTemplate = item.Translations.Where(x => x.Language == query.Language).FirstOrDefault()?.BodyTemplate;
                            item.TitleTemplate = item.Translations.Where(x => x.Language == query.Language).FirstOrDefault()?.TitleTemplate;
                        }
                    }
                    return new ResponsePagination<EmailTemplateViewModel>(result);
                }

                return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: Query: {@Param}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        private Expression<Func<sm_Email_Template, bool>> BuildQuery(EmailTemplateQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_Email_Template>(true);

            if (query.FromDate.HasValue)
            {
                predicate.And(x => x.CreatedOnDate >= query.FromDate.Value);
            }
            if (query.ToDate.HasValue)
            {
                predicate.And(x => x.CreatedOnDate <= query.ToDate.Value);
            }
            if (!string.IsNullOrEmpty(query.FullTextSearch))
            {
                predicate.And(x => x.Name.ToLower().Contains(query.FullTextSearch.ToLower())
                                   || x.sm_Email_Template_Translation.Any(c => c.TitleTemplate.Contains(query.FullTextSearch)
                                                                    || c.BodyTemplate.Contains(query.FullTextSearch))
                );
            }
            return predicate;
        }

        // Email subcribe handler

        // Subcribe email
        public async Task<Response> SubscribeEmail(EmailSubscribeModel request)
        {
            try
            {
                var emailSubcribe = await _dbContext.sm_Email_Subscribe.Where(a => a.Email == request.Email).FirstOrDefaultAsync();

                if (emailSubcribe != null)
                {
                    emailSubcribe.SubscribeDate = DateTime.Now;
                    emailSubcribe.LastModifiedOnDate = DateTime.Now;
                    if (emailSubcribe.UnsubscribeDate.HasValue)
                    {
                        emailSubcribe.UnsubscribeDate = null;
                        emailSubcribe.Status = EmailSubscriptionConstants.Email_Subscribed;
                    }
                }
                else
                {
                    emailSubcribe = new sm_Email_Subscribe();
                    emailSubcribe.Email = request.Email;
                    emailSubcribe.Status = EmailSubscriptionConstants.Email_Subscribed;
                    emailSubcribe.SubscribeDate = DateTime.Now;
                    emailSubcribe.TotalEmailSentCount = 0;

                    _dbContext.sm_Email_Subscribe.Add(emailSubcribe);
                }

                var status = await _dbContext.SaveChangesAsync();
                var result = _mapper.Map<sm_Email_Subscribe, EmailSubscriptionModel>(emailSubcribe);
                return new Response<EmailSubscriptionModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Request: {@params}", request);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetEmailSubscribeById(Guid id)
        {
            try
            {
                var eSubscription = await _dbContext.sm_Email_Subscribe.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (eSubscription == null)
                    return new Response(HttpStatusCode.NotFound, null);

                var result = _mapper.Map<sm_Email_Subscribe, EmailSubscriptionModel>(eSubscription);
                return new Response<EmailSubscriptionModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> UnsubscribeEmailById(Guid id)
        {
            try
            {
                var eSubscription = await _dbContext.sm_Email_Subscribe.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (eSubscription == null)
                    return new Response(HttpStatusCode.NotFound, null);

                eSubscription.Status = EmailSubscriptionConstants.Email_Unsubscribed;
                eSubscription.UnsubscribeDate = DateTime.Now;
                eSubscription.LastModifiedOnDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                var result = _mapper.Map<sm_Email_Subscribe, EmailSubscriptionModel>(eSubscription);
                return new Response<EmailSubscriptionModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Information("Params: Id: {@params}", id);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        public async Task<Response> GetListPageAsync(EmailSubscribeQueryModel query)
        {
            try
            {
                var currentUser = Helper.GetRequestInfo(_httpContextAccessor);

                var predicate = BuildQuery(query);
                var queryResult = _dbContext.sm_Email_Subscribe.Where(predicate);

                var data = await queryResult.GetPageAsync(query);
                var result = _mapper.Map<Pagination<sm_Email_Subscribe>, Pagination<EmailSubscriptionModel>>(data);
                
                if (result == null)
                {
                    return new ResponseError(HttpStatusCode.NotFound, "Không tìm thấy dữ liệu");
                }

                return new ResponsePagination<EmailSubscriptionModel>(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
                Log.Error("Param: Query: {@Param}", query);
                return Utils.CreateExceptionResponseError(ex);
            }
        }

        private Expression<Func<sm_Email_Subscribe, bool>> BuildQuery(EmailSubscribeQueryModel query)
        {
            var predicate = PredicateBuilder.New<sm_Email_Subscribe>(true);

            if (query.FromDate.HasValue)
            {
                predicate.And(x => x.SubscribeDate >= query.FromDate.Value);
            }
            if (query.ToDate.HasValue)
            {
                predicate.And(x => x.SubscribeDate <= query.ToDate.Value);
            }
            if (!string.IsNullOrEmpty(query.FullTextSearch))
            {
                predicate.And(x => x.Email != null && x.Email.Contains(query.FullTextSearch));
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                predicate.And(x => x.Status.ToLower() == query.Status.ToLower());
            }

            return predicate;
        }

        public async Task<Response> DeleteEmailSubscribe(Guid id)
        {
            var eSubcription = await _dbContext.sm_Email_Subscribe.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (eSubcription == null)
                return new Response(HttpStatusCode.NotFound, null);
            _dbContext.sm_Email_Subscribe.Remove(eSubcription);

            var status = await _dbContext.SaveChangesAsync();
            if (status > 0)
            {
                return new ResponseDelete(HttpStatusCode.OK, "Đã xóa thành công", id, "");
            }

            return new ResponseError(HttpStatusCode.BadRequest, "Xoá bản ghi thất bại");
        }
    }
}