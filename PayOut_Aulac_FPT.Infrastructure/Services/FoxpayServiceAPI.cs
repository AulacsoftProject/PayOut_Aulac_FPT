using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PayOut_Aulac_FPT.Core.Exceptions;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Infrastructure.Services
{
    public class FoxpayServiceAPI : IFoxpayServiceAPI
    {
        private static string? access_token;
        private string? domain;
        private string? username;
        private string? password;
        private string? deviceID;
        private string? ipAddress;
        private string encoded;
        private readonly IConfiguration _configuration;
        private readonly IBase64Service _base64Service;
        public FoxpayServiceAPI(IConfiguration configuration, IBase64Service base64Service)
        {
            this._configuration = configuration;
            this._base64Service = base64Service;
            domain = configuration["Foxpay:Domain"];
            username = configuration["Foxpay:Username"];
            password = configuration["Foxpay:Password"];
            deviceID = configuration["Foxpay:device-id"];
            ipAddress = configuration["Foxpay:ip-address"];
        }
        private async Task<HttpResponseMessage> PostRequestAsync(string path, string encoded, string paramJson)
        {
            using HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(domain);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Add("device-id", deviceID);
            request.Headers.Add("ip-address", ipAddress);
            request.Headers.Add("Authorization", "Basic " + encoded);

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("client_credentials"), "grant_type");
            request.Content = content;

            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    access_token = null;
                    return await PostRequestAsync(path, encoded, paramJson);
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<HttpResponseMessage> PostRequestAsync(string path, Dictionary<string, string> dParam)
        {
            encoded = _base64Service.Base64Encode(username + ":" + password);
            return await PostRequestAsync(path, encoded, JsonConvert.SerializeObject(dParam));
        }

        private async Task GetToken()
        {
            var response = await PostRequestAsync("/oauth/get_token", new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials"}
            });
            if (response != null)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new NotificationException("Không thể kết nối đến Foxpay, vui lòng kiểm tra lại!");
                }
                var jsonString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Token>(jsonString);
                if (data?.access_token == null)
                {
                    throw new Exception(data?.error_description);
                }
                else
                {
                    access_token = data?.access_token;
                }
            }
            else
            {
                throw new NotificationException("Có lỗi xảy ra, vui lòng thử lại!");
            }
        }
    }

    internal class Token
    {
        private long? expires_in;
        private DateTime expirationTime;
        public string? access_token { get; set; }
        public long? expire_in_seconds
        {
            get
            {
                return expires_in;
            }

            set
            {
                expirationTime = DateTime.Now.AddSeconds((value ?? 0) - 100);
                //expirationTime = DateTimeOffset.FromUnixTimeSeconds(value ?? 0).DateTime;
                expires_in = value;
            }
        }
        public string? token_type { get; set; }
        public string? scope { get; set; }
        public string? refresh_token { get; set; }

        public bool IsValid()
        {
            if (expire_in_seconds == null)
            {
                return false;
            }
            //var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expire_in_seconds ?? 0).DateTime;
            var dateNow = DateTime.Now;
            return expirationTime > dateNow;
        }

        //Thông tin khi lỗi
        public string? error_code { get; set; }
        public string? error { get; set; }
        public string? error_description { get; set; }
    }
}
