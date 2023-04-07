using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using PayOut_Aulac_FPT.Core.Utils.Enums;
using PayOut_Aulac_FPT.DTO;
using PayOut_Aulac_FPT.DTO.ResultPayment;

namespace PayOut_Aulac_FPT.Controllers
{
    [Tags("Result Payment")]
    public class ResultPaymentController : ControllerBase
    {
        //private string url = "https://portal- staging.FPT.vn/payment/get-transaction";
        private readonly ILogger<ResultPaymentController> _logger;
        private readonly IFoxpayServiceAPI _foxpayServiceAPI;
        private readonly IVchPaymentFoxpayService _vchPaymentFoxpayService;

        public ResultPaymentController(ILogger<ResultPaymentController> logger, IFoxpayServiceAPI foxpayServiceAPI, IVchPaymentFoxpayService vchPaymentFoxpayService)
        {
            _logger = logger;
            _foxpayServiceAPI = foxpayServiceAPI;
            _vchPaymentFoxpayService = vchPaymentFoxpayService;
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

                //Dùng API kiểm tra trạng thái giao dịch của Foxpay
                PaymentFoxpay paymentFoxpay = new PaymentFoxpay
                {
                    version = request.version,
                    merchant_id = request.merchant_id,
                    terminal_id = request.terminal_id,
                    order_id = request.order_id,
                    transaction_payment = request.transaction_payment,
                    patient_id = request.patient_id,
                    signature = request.signature
                };

                var resultTrangThaiGiaoDich = await _foxpayServiceAPI.CheckTransaction(paymentFoxpay);
                if (resultTrangThaiGiaoDich?.result_code == "200")
                {
                    //Ghi nhận lại thông tin kết quả giao dịch, Cập nhật lại thông tin thanh toán cho bệnh nhân
                    var query = _vchPaymentFoxpayService.Get(new VchPaymentFoxpay { patient_id = request.patient_id, vch_id = request.vch_id });
                    _vchPaymentFoxpayService.Update(new VchPaymentFoxpay
                    {
                        VchPmtFoxpayPrkID = query.VchPmtFoxpayPrkID,
                        order_id = request.order_id,
                        transaction_payment = request.transaction_payment,
                        StatusReq = (int)EStatusReq.Complated,
                        signature = request.signature
                    });
                }

                return Ok(new SuccessResponse<ResultPaymentResponse>(
                        new ResultPaymentResponse
                        {
                            version = request.version,
                            merchant_id = request.merchant_id,
                            terminal_id = request.terminal_id,
                            order_id = request.order_id,
                            paintent_id = request.patient_id,
                            vch_id = request.vch_id,
                            signature = request.signature,
                            result_code = resultTrangThaiGiaoDich?.result_code,
                            result = resultTrangThaiGiaoDich?.result,
                            message = resultTrangThaiGiaoDich?.message
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
