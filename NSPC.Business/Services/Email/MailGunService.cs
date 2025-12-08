using NSPC.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public interface IMailGunService
    {
        Task SendMailApiAsync(EmailMessage mess);
        Task<bool> SendMailApi(EmailMessage mess);
        Task<bool> SendMailWithTempalte(string[] toEmail, string keyNameEmailTempalte, string language = "en", params string[] argBodys);
        Task<bool> SendMailWithTempalte(string[] toEmail, string keyNameEmailTempalte, string language = "en");
        Task<bool> SendMailWithTempalte(List<string> listTo, string keyNameEmailTempalte, string language = "en", params string[] argBodys);
        Task<bool> SendMail(EmailMessage message);
    }

    public class MailGunService : IMailGunService 
    {
        private readonly IConfiguration _configuration;
        private readonly MailGunSetting _mailGunSetting;
        private readonly IEmailHandler _emailHandler;

        public MailGunService(IConfiguration configuration, IEmailHandler emailHandler)
        {
            _configuration = configuration;
            _mailGunSetting = _configuration.GetValue<MailGunSetting>("MailGunSettings");
            _emailHandler = emailHandler;
        }
        /// <summary>
        ///  Send mail Async sử dụng mailgun 
        /// </summary>
        /// <param name="to">To Email</param>
        /// <param name="subject">Tiêu đề mail</param>
        /// <param name="body">Nội dung mail</param>
        /// <returns></returns>
        public async Task SendMailApiAsync(EmailMessage mess)
        {

            using (var client = new HttpClient { BaseAddress = new Uri(_mailGunSetting.ApiBaseUri) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes("api:" + _mailGunSetting.ApiKey)));

                var content = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("from", _mailGunSetting.From),
                        new KeyValuePair<string, string>("to", string.Join(',', mess.To)),
                        new KeyValuePair<string, string>("subject", mess.Subject),
                        new KeyValuePair<string, string>("html", mess.Content)
                     });

                await client.PostAsync(_mailGunSetting.Domain + "/messages", content).ConfigureAwait(false);
            }
        }


        /// <summary>
        ///  Send mail sử dụng mailgun 
        /// </summary>
        /// <param name="mess">EmailMessage: To Email, Subject, Body</param>
        /// <returns></returns>
        public async Task<bool> SendMailApi(EmailMessage mess)
        {
            try
            {
                Log.Debug("SendMail: Start. To: " + mess.To);
                using (var client = new HttpClient { BaseAddress = new Uri(_mailGunSetting.ApiBaseUri) })
                {

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("api", _mailGunSetting.ApiKey);

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("domain", _mailGunSetting.Domain),
                        new KeyValuePair<string, string>("from", _mailGunSetting.From),
                        new KeyValuePair<string, string>("to", string.Join(',', mess.To)),
                        new KeyValuePair<string, string>("subject", mess.Subject),
                        new KeyValuePair<string, string>("html", mess.Content)
                     });

                    var response = await client.PostAsync(_mailGunSetting.Domain + "/messages", content);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                    Log.Error("SendMail: StatusCode: " + response.StatusCode.ToString());
                    Log.Error("SendMail: Content" + response.Content.ToString());
                }
                return false;
            }
            catch (Exception ex)
            {
                //log
                Log.Error("SendMail: " + ex.ToString());
                return false;
            }

        }

        /// <summary>
        /// Send Mail With Email Tempalte
        /// </summary>
        /// <param name="toEmail">Mail To</param>
        /// <param name="keyNameEmailTempalte">Key of Email Tempalte</param>
        /// <param name="args">Danh sách param truyền vào body Email, theo string.Format</param>
        /// <returns></returns>
        public async Task<bool> SendMailWithTempalte(string[] toEmail, string keyNameEmailTempalte, string language = "en", params string[] argBodys)
        {
            try
            {
                var emailTemplate = await _emailHandler.GetTemplateByCode(keyNameEmailTempalte, language);
                if (emailTemplate == null || !emailTemplate.IsSuccess)
                {
                    return false;
                }

                var template = emailTemplate as Response<EmailTemplateViewModel>;

                return await SendMail(new EmailMessage()
                {
                    To = toEmail,
                    Content = string.Format(template.Data.BodyTemplate, argBodys),
                    Subject = template.Data.TitleTemplate
                });
            }
            catch (Exception ex)
            {
                Log.Error("SendMail: Exception Message: " + ex.Message.ToString());
                Log.Error("SendMail: Exception: " + ex.ToString());

                return false;
            }
        }

        public async Task<bool> SendMailWithTempalte(string[] toEmail, string keyNameEmailTempalte, string language = "en")
        {
            try
            {
                var emailTemplate = await _emailHandler.GetTemplateByCode(keyNameEmailTempalte, language);
                if (emailTemplate == null || !emailTemplate.IsSuccess)
                {
                    return false;
                }

                var template = emailTemplate as Response<EmailTemplateViewModel>;

                return await SendMail(new EmailMessage()
                {
                    To = toEmail,
                    Content = template.Data.BodyTemplate,
                    Subject = template.Data.BodyTemplate
                });
            }
            catch (Exception ex)
            {
                Log.Error("SendMail: Exception Message: " + ex.Message.ToString());
                Log.Error("SendMail: Exception: " + ex.ToString());

                return false;
            }

        }

        public async Task<bool> SendMailWithTempalte(List<string> listTo, string keyNameEmailTempalte, string language = "en", params string[] argBodys)
        {
            try
            {
                var emailTemplate = await _emailHandler.GetTemplateByCode(keyNameEmailTempalte, language);
                if (emailTemplate == null || !emailTemplate.IsSuccess)
                {
                    return false;
                }

                var template = emailTemplate as Response<EmailTemplateViewModel>;

                return await SendMail(listTo, null, template.Data.TitleTemplate, string.Format(template.Data.BodyTemplate, argBodys));
            }
            catch (Exception ex)
            {
                Log.Error("SendMail: Exception Message: " + ex.Message.ToString());
                Log.Error("SendMail: Exception: " + ex.ToString());

                return false;
            }

        }

        public async Task<bool> SendMail(EmailMessage message)
        {
            return await SendMail(message.To.ToList(), null, message.Subject, message.Content);
        }

        public async Task<bool> SendMail(List<string> listTo, List<string> listCc, string subject, string content)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(_mailGunSetting.From);
                foreach (var x in listTo)
                {
                    if (!string.IsNullOrEmpty(x.Trim()))
                        message.To.Add(new MailAddress(x));
                }

                if (listCc != null)
                    foreach (var x in listCc)
                    {
                        if (!string.IsNullOrEmpty(x.Trim()))
                            message.CC.Add(new MailAddress(x));
                    }
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = content;
                smtp.Port = 587;
                smtp.Host = "smtp.mailgun.org";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_mailGunSetting.Username, _mailGunSetting.Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("SendMail: Exception Message: " + ex.Message.ToString());
                Log.Error("SendMail: Exception: " + ex.ToString());

                return false;
            }
        }

        public class MailGunSetting
        {
            public string ApiKey { get; set; }
            public string ApiBaseUri { get; set; }
            public string From { get; set; }
            public string Domain { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
