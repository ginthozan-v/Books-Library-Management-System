using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LMS.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LMS.Services
{
    public class LoginService
    {
        static HttpClient client = new HttpClient();
        static string BaseURL = Constants.BaseURL;
        ///////////////////////////////////////
        //** Login**/ //////////////////////// 
        public async Task<string> LoginAsync(string userName, string password)
        {
            string URL = BaseURL + "token";
            var accessToken = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    var keyValues = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("username", userName),
                        new KeyValuePair<string, string>("password", password),
                        new KeyValuePair<string, string>("grant_type", "password"),
                    };

                    var request = new HttpRequestMessage(
                        HttpMethod.Post, URL);

                    request.Content = new FormUrlEncodedContent(keyValues);

                    var response = client.SendAsync(request).Result;
                    using (HttpContent content = response.Content)
                    {
                        var json = content.ReadAsStringAsync();
                        JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(json.Result);

                        var accessTokenExpiration = jwtDynamic.Value<DateTime>(".expires");
                        accessToken = jwtDynamic.Value<string>("access_token");
                        var username = jwtDynamic.Value<string>("userName");
                        var AccessTokenExpirationDate = accessTokenExpiration;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            return accessToken;
        }
    }
}
