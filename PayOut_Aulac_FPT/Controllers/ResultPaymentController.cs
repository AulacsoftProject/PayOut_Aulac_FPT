using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayOut_Aulac_FPT.DTO;
using PayOut_Aulac_FPT.DTO.ResultPayment;

namespace PayOut_Aulac_FPT.Controllers
{
    [Tags("Result Payment")]
    public class ResultPaymentController : ControllerBase
    {
        //private string url = "https://portal- staging.FPT.vn/payment/get-transaction";
        private readonly ILogger<ResultPaymentController> _logger;

        public ResultPaymentController(ILogger<ResultPaymentController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Api Foxpay trả kết quả thanh toán cho Portal-Listeins
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("api/payment/result")]
        //public ActionResult<SuccessResponse<PaymentResponse>> Items([FromQuery] ResultPaymentRequest request)
        public ActionResult<SuccessResponse<ResultPaymentResponse>> Items([FromQuery] string data)
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

                //Cập nhật lại thông tin thanh toán cho bệnh nhân

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
                            result_code = request.result_code,
                            result = request.result,
                            message = request.message
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
