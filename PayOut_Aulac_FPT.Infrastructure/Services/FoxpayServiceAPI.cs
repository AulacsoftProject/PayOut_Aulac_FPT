using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayOut_Aulac_FPT.Core.Entities;
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
        private static Token? token;
        private string? domain;
        private string? domain2;
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
            domain2 = configuration["Foxpay:Domain2"];
            username = configuration["Foxpay:Username"];
            password = configuration["Foxpay:Password"];
            deviceID = configuration["Foxpay:device-id"];
            ipAddress = configuration["Foxpay:ip-address"];
        }
        private async Task<HttpResponseMessage> PostRequestAsync(string path, string paramJson, MultipartFormDataContent dataRaw, bool isLogin = false, bool isDomainOther = false)
        {
            encoded = _base64Service.Base64Encode(username + ":" + password);

            using HttpClient httpClient = new HttpClient();

            if (isDomainOther)
                httpClient.BaseAddress = new Uri(domain2);
            else
                httpClient.BaseAddress = new Uri(domain);

            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Headers.Add("device-id", deviceID);
            request.Headers.Add("ip-address", ipAddress);
            request.Headers.Add("Authorization", "Basic " + encoded);

            if (!isLogin)
            {
                if (token == null /*|| !token.IsValid()*/)
                {
                    await GetToken();
                }
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token?.access_token);
            }

            if (paramJson == null)
                request.Content = dataRaw;
            else
                request.Content = new StringContent(paramJson, null, "application/json");

            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    token = null;
                    return await PostRequestAsync(path, paramJson, dataRaw, true);
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<HttpResponseMessage> PostRequestAsync(string path, StringContent dParam, string valueParam, bool isLogin = false, bool isDomainOther = false)
        {
            var content = new MultipartFormDataContent();
            content.Add(dParam, valueParam);

            return await PostRequestAsync(path, null, content, isLogin, isDomainOther);
        }

        private async Task GetToken(bool isDomainOther = false)
        {
            var response = await PostRequestAsync("/oauth/get_token", new StringContent("client_credentials"), "grant_type", true, isDomainOther);
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
                    token = data;
                }
            }
            else
            {
                throw new NotificationException("Có lỗi xảy ra, vui lòng thử lại!");
            }
        }

        public async Task<ResultCheckFoxpay?> CheckTransaction(CheckTransaction resultFoxpay)
        {
            var result = JsonConvert.SerializeObject(resultFoxpay);
            var response = await PostRequestAsync("/api/hue-his/payment/check-transaction", result, null);
            if (response != null)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw new NotificationException("Dữ liệu không đúng cú pháp!");
                    case HttpStatusCode.Unauthorized:
                        throw new NotificationException("Chữ ký không hợp lệ!");
                    case HttpStatusCode.InternalServerError:
                        throw new NotificationException("Có lỗi xảy ra trong quá trình xử lý!");
                    case HttpStatusCode.OK:
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<ResultCheckFoxpay>(jsonString);
                        if (data?.result_code != null)
                        {
                            return data;
                        }
                        else
                        {
                            throw new Exception(data?.error_description);
                        }
                    default: throw new NotificationException("Không thể kiểm tra giao dịch, vui lòng thử lại!");
                }
            }
            else
            {
                throw new NotificationException("Có lỗi xảy ra, vui lòng thử lại!");
            }
        }

        public async Task<ResultCancelFoxpay?> CancelTransaction(CancelTransaction resultFoxpay)
        {
            var result = JsonConvert.SerializeObject(resultFoxpay);
            var response = await PostRequestAsync("/api/hue-his/payment/cancel-transaction", result, null);
            if (response != null)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw new NotificationException("Dữ liệu không đúng cú pháp!");
                    case HttpStatusCode.Unauthorized:
                        throw new NotificationException("Chữ ký không hợp lệ!");
                    case HttpStatusCode.InternalServerError:
                        throw new NotificationException("Có lỗi xảy ra trong quá trình xử lý!");
                    case HttpStatusCode.OK:
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<ResultCancelFoxpay>(jsonString);
                        if (data?.result_code != null)
                        {
                            return data;
                        }
                        else
                        {
                            var error = JsonConvert.DeserializeObject<Token>(jsonString);
                            throw new Exception(error?.error_description);
                        }
                    default: throw new NotificationException("Không thể hủy giao dịch, vui lòng thử lại!");
                }
            }
            else
            {
                throw new NotificationException("Có lỗi xảy ra, vui lòng thử lại!");
            }
        }

        public async Task<ResultReconcileFoxpay?> ReconcileTransaction(ReconcileTransaction resultFoxpay)
        {
            var result = JsonConvert.SerializeObject(resultFoxpay);
            var response = await PostRequestAsync("/api/hue-his/payment/reconcile-transaction", result, null, false, true);
            if (response != null)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw new NotificationException("Dữ liệu không đúng cú pháp!");
                    case HttpStatusCode.Unauthorized:
                        throw new NotificationException("Chữ ký không hợp lệ!");
                    case HttpStatusCode.InternalServerError:
                        throw new NotificationException("Có lỗi xảy ra trong quá trình xử lý!");
                    case HttpStatusCode.OK:
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<ResultReconcileFoxpay>(jsonString);
                        if (data?.error_code == null)
                        {
                            return data;
                        }
                        else
                        {
                            throw new Exception(data?.error_description);
                        }
                    default: throw new NotificationException("Không thể đối soát giao dịch, vui lòng thử lại!");
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
