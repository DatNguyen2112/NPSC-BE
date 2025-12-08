using System;
using System.Threading.Tasks;
using NSPC.Common;
namespace NSPC.Business
{
    public interface IEmailHandler
    {
        Task<Response> GetById(Guid id);

        Task<Response> GetTemplateByCode(string code, string language);

        Task<Response> Delete(Guid ịd);

        Task<Response> CreateEmailTemplate(EmailTemplateCreateUpdateModel request);

        Task<Response> UpdateEmailTemplate(EmailTemplateCreateUpdateModel request, Guid id);

        Task<Response> GetListPageAsync(EmailTemplateQueryModel query);

        // Email subcribe
        Task<Response> SubscribeEmail(EmailSubscribeModel request);

        Task<Response> GetEmailSubscribeById(Guid id);

        Task<Response> UnsubscribeEmailById(Guid id);

        Task<Response> DeleteEmailSubscribe(Guid id);

        Task<Response> GetListPageAsync(EmailSubscribeQueryModel emailSubcribeQuery);
    }
}