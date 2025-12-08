using NSPC.Common;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NSPC.Business
{
    public class FacebookService
    {
        private readonly HttpClient _httpClient;

        public FacebookService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com/v13.0/")
            };
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<FacebookUserResource> GetUserFromFacebookAsync(string facebookToken)
        {
            var result = await GetAsync<dynamic>(facebookToken, "me", "fields=name,first_name,last_name,email,picture.width(200).height(200)");
            if (result == null)
            {
                throw new Exception("User from this token not exist");
            }

            var account = new FacebookUserResource()
            {
                Email = result.email,
                Name = result.name,
                UserId = result.id,
                FirstName = result.first_name,
                LastName = result.last_name,
                Picture = result.picture.data.url
            };

            return account;
        }

        private async Task<T> GetAsync<T>(string accessToken, string endpoint, string args = null)
        {
            var response = await _httpClient.GetAsync($"{endpoint}?access_token={accessToken}&{args}");
            if (!response.IsSuccessStatusCode)
                return default(T);

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}