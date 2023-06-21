using PayOut_Aulac_FPT.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Interfaces.Services
{
    public interface IFoxpayServiceAPI
    {
        public string? Domain(string? domain);
        public string? DeviceID(string? deviceID);
        public string? IpAddress(string? ip);
        //public Task<HttpResponseMessage?> PostRequestAsync(string path, string encodedBase64, string paramJson, bool isLogin = false, bool hasLogin = false);
        public Task<ResultCheckFoxpay?> CheckTransaction(CheckTransaction resultPayment);
        public Task<ResultCancelFoxpay?> CancelTransaction(CancelTransaction resultPayment);
        public Task<ResultReconcileFoxpay?> ReconcileTransaction(ReconcileTransaction resultFoxpay);
        public Task<SoftOtpResponse?> SoftOTP(SoftOtpRequest resultFoxpay);
        public Task<ConfirmOtpResponse?> ConfirmOTP(ConfirmOtpRequest resultFoxpay);
    }
}
