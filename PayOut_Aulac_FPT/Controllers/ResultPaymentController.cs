using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using PayOut_Aulac_FPT.Core.Services;
using PayOut_Aulac_FPT.Core.Utils.Enums;
using PayOut_Aulac_FPT.DTO;
using PayOut_Aulac_FPT.DTO.ResultPayment;
using PayOut_Aulac_FPT.Infrastructure.Services;

namespace PayOut_Aulac_FPT.Controllers
{
    [Tags("Result Payment")]
    public class ResultPaymentController : ControllerBase
    {
        //private string url = "https://portal- staging.FPT.vn/payment/get-transaction";
        private readonly ILogger<ResultPaymentController> _logger;
        private readonly IFoxpayServiceAPI _foxpayServiceAPI;
        private readonly IVchPaymentFoxpayService _vchPaymentFoxpayService;
        private readonly IConfiguration _configuration;
        private readonly ICompanyInfoService _companyInfoService;
        private readonly IUserLoginService _userLoginService;
        private readonly ISha256HexService _sha256HexService;
        private readonly IVchPaymentHISService _vchPaymentHISService;

        public ResultPaymentController(ILogger<ResultPaymentController> logger, IFoxpayServiceAPI foxpayServiceAPI, IVchPaymentFoxpayService vchPaymentFoxpayService, IConfiguration configuration, ICompanyInfoService companyInfoService, IUserLoginService userLoginService, ISha256HexService sha256HexService, IVchPaymentHISService vchPaymentHISService)
        {
            _logger = logger;
            _foxpayServiceAPI = foxpayServiceAPI;
            _vchPaymentFoxpayService = vchPaymentFoxpayService;
            _configuration = configuration;
            _companyInfoService = companyInfoService;
            _userLoginService = userLoginService;
            _sha256HexService = sha256HexService;
            _vchPaymentHISService = vchPaymentHISService;
        }

        /// <summary>
        /// Api Foxpay trả kết quả thanh toán cho Portal-Listeins
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("api/payment/result")]
        //public ActionResult<SuccessResponse<PaymentResponse>> Items([FromQuery] ResultPaymentRequest request)
        public async Task<ActionResult<SuccessResponse<ResultPaymentResponse>>> ItemsAsync([FromQuery] string data)
        {
            try
            {
                var request = JsonConvert.DeserializeObject<ResultPaymentRequest>(data);

                IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

                string conString = Microsoft
                       .Extensions
                       .Configuration
                       .ConfigurationExtensions
                       .GetConnectionString(Configuration, request.merchant_id);

                string resultCode = "", result = "", message = "";
                var versionFoxpay = _configuration["Foxpay:Version"];
                var companyInfo = _companyInfoService.Get(new CompanyInfo());
                var loginInfo = _userLoginService.Get(new UserLogin
                {
                    UserPaymentID = companyInfo.InsPrvnCode + companyInfo.InsOfficeCode,
                    IsActive = true
                });

                var mSignature = versionFoxpay + loginInfo.merchant_id + loginInfo.terminal_id /*+ PntInfo.qr_type*/ + request.order_id + request.transaction_payment + request.paintent_id /*+ qrCode.amount + PntInfo.currency + PntInfo.description + PntInfo.expired_time + PntInfo.customer_code + PntInfo.merchant_secret_key*/ + loginInfo.secret_key;

                if (_sha256HexService.SHA256Hex(mSignature) != request.signature)
                {
                    resultCode = "0";
                    result = "WARNING";
                    message = "Dữ liệu giao dịch không chính xác. Vui lòng kiểm tra lại!";
                }
                else
                {

                    //Dùng API kiểm tra trạng thái giao dịch của Foxpay
                    PaymentFoxpay paymentFoxpay = new PaymentFoxpay
                    {
                        version = request.version,
                        merchant_id = request.merchant_id,
                        terminal_id = request.terminal_id,
                        order_id = request.order_id,
                        transaction_payment = request.transaction_payment,
                        patient_id = request.paintent_id,
                        signature = request.signature
                    };

                    var resultTrangThaiGiaoDich = await _foxpayServiceAPI.CheckTransaction(paymentFoxpay);
                    var query = _vchPaymentFoxpayService.Get(new VchPaymentFoxpay { patient_id = request.paintent_id, vch_id = request.vch_id });
                    if (resultTrangThaiGiaoDich?.result_code == "200")
                    {
                        //Ghi nhận lại thông tin kết quả giao dịch, Cập nhật lại thông tin thanh toán cho bệnh nhân
                        _vchPaymentFoxpayService.Patch(new VchPaymentFoxpay
                        {
                            VchPmtFoxpayPrkID = query.VchPmtFoxpayPrkID,
                            order_id = request.order_id,
                            transaction_payment = request.transaction_payment,
                            StatusReq = (int)EStatusReq.Complated,
                            signature = request.signature
                        });

                        //Update trạng thái thanh toán ở VchPaymentHeader
                        _vchPaymentHISService.Patch(new VchPaymentHIS
                        {
                            VchPmntPrkID = query.vch_id,
                            MdcFilePrkID = query.patient_id,
                            StatusReq = (int)EStatusReq.Complated
                        });
                    }

                    resultCode = resultTrangThaiGiaoDich?.result_code;
                    result = resultTrangThaiGiaoDich?.result;
                    message = resultTrangThaiGiaoDich?.message;
                }

                return Ok(new SuccessResponse<ResultPaymentResponse>(
                        new ResultPaymentResponse
                        {
                            version = request.version,
                            merchant_id = request.merchant_id,
                            terminal_id = request.terminal_id,
                            order_id = request.order_id,
                            paintent_id = request.paintent_id,
                            vch_id = request.vch_id,
                            signature = request.signature,
                            result_code = resultCode,
                            result = result,
                            message = message
                        }
                    ));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorResponse(ex.Message));
            }
        }
    }
}
