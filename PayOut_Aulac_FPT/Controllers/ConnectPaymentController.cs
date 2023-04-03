using AutoMapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using PayOut_Aulac_FPT.DTO;
using PayOut_Aulac_FPT.DTO.ConnectPayment;
using System;
using Newtonsoft.Json.Linq;
using Minio;
using Microsoft.Extensions.Configuration;
using PayOut_Aulac_FPT.DTO.OTPPayment;
using System.Security.Cryptography.Xml;
using Newtonsoft.Json;

namespace PayOut_Aulac_FPT.Controllers
{
    [Tags("Connect Payment")]
    public class ConnectPaymentController : ControllerBase
    {
        //private string url = "https://portal- staging.FPT.vn/payment/get-transaction";
        private readonly ICompanyInfoService _companyInfoService;
        private readonly IQRCodeService _qRCodeService;
        private readonly IConfiguration _configuration;
        private readonly IBase64Service _base64Service;
        private readonly ISha256HexService _sha256HexService;
        private readonly IUserLoginService _userLoginService;
        private readonly IFoxpayServiceAPI _fboxpayServiceAPI;
        private readonly ILogger<ConnectPaymentController> _logger;
        private readonly IMapper _mapper;

        public ConnectPaymentController(ILogger<ConnectPaymentController> logger, ICompanyInfoService companyInfoService, IQRCodeService qRCodeService, IConfiguration configuration, IBase64Service base64Service, ISha256HexService sha256HexService, IUserLoginService userLoginService, IFoxpayServiceAPI foxpayServiceAPI, IMapper mapper)
        {
            _logger = logger;
            _companyInfoService = companyInfoService;
            _qRCodeService = qRCodeService;
            _configuration = configuration;
            _base64Service = base64Service;
            _sha256HexService = sha256HexService;
            _userLoginService = userLoginService;
            _fboxpayServiceAPI = foxpayServiceAPI;
            _mapper = mapper;
        }

        /// <summary>
        /// Khởi tạo QR code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("create-qr-code")]
        public ActionResult<SuccessResponse<QRCodeCreateResponse>> Info([FromQuery] QRCodeCreateRequest request)
        {
            try
            {
                var versionFoxpay = _configuration["Foxpay:Version"];
                var companyInfo = _companyInfoService.Get(new CompanyInfo());
                string maCSKCB = companyInfo.InsPrvnCode + "-" + companyInfo.InsOfficeCode;
                var loginInfo = _userLoginService.Get(new UserLogin
                {
                    UserPaymentID = companyInfo.InsPrvnCode + companyInfo.InsOfficeCode,
                    IsActive = true
                });

                var PntInfo = _qRCodeService.Get(new QRCode()
                {
                    MdcFilePrkID = request.MdcFilePrkID,
                    VchPmntPrkID = request.VchPmntPrkID
                });

                var hoTen = _base64Service.Base64Encode(PntInfo.ho_ten_bn);
                var contentPayment = _base64Service.Base64Encode(PntInfo.content_payment);
                var mSignature = PntInfo.version + loginInfo.merchant_id + loginInfo.terminal_id /*+ PntInfo.qr_type*/ + PntInfo.order_id + PntInfo.amount /*+ PntInfo.currency + PntInfo.description + PntInfo.expired_time + PntInfo.customer_code + PntInfo.merchant_secret_key*/ + loginInfo.secret_key;

                var qrCode = new QRCodeInfo()
                {
                    version = versionFoxpay,
                    merchant_id = loginInfo.merchant_id,
                    terminal_id = loginInfo.terminal_id,
                    order_id = PntInfo.order_id,
                    paintent_id = PntInfo.paintent_id,
                    ho_ten_bn = hoTen,
                    vch_id = PntInfo.vch_id,
                    payment_type = PntInfo.payment_type,
                    amount = PntInfo.amount,
                    currency_id = PntInfo.currency_id,
                    content_payment = contentPayment,
                    psn_payment_id = PntInfo.psn_payment_id,
                    create_time_payment = PntInfo.create_time_payment,
                    transaction_payment = PntInfo.transaction_payment,
                    signature = _sha256HexService.SHA256Hex(mSignature),
                    mcc = PntInfo.mcc,
                    country_id = PntInfo.country_id,
                    merchant_name = loginInfo.UserPaymentName,
                    city_id = PntInfo.city_id,
                    terminal_name = PntInfo.terminal_name,
                };

                return Ok(new SuccessResponse<QRCodeCreateResponse>(
                    new QRCodeCreateResponse()
                    {
                        MdcFilePrkID = PntInfo.MdcFilePrkID,
                        VchPmntPrkID = PntInfo.vch_id,
                        PntName = PntInfo.ho_ten_bn,
                        PntBirthday = PntInfo.PntBirthday,
                        SexName = PntInfo.SexName,
                        DateExam = PntInfo.DateExam,
                        AmtLineInExc = PntInfo.amount,
                        QRCode = qrCode
                    }
                ));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Foxpay thực hiện kết nối với Portal-Listeins
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("api/payment/connect")]
        //public ActionResult<SuccessResponse<PaymentResponse>> Items([FromQuery] ConnectPaymentRequest request)
        public ActionResult<SuccessResponse<ConnectPaymentResponse>> Items([FromQuery] string data)
        {
            try
            {
                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<ConnectPaymentRequest>(data);
                var txn = new txn();

                IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

                string conString = ConfigurationExtensions.GetConnectionString(Configuration, request.merchant_id);

                var qrCode = _qRCodeService.Gets(new QRCode()
                {
                    MdcFilePrkID = request.paintent_id,
                    VchPmntPrkID = request.vch_id
                });

                if (qrCode.Count() == 0)
                {
                    txn = new txn { result_code = "0", result = "WARNING", message = "Không tìm thấy thông tin thanh toán!" };
                }
                else
                {
                    txn = new txn { result_code = "200", result = "SUCCESS", message = "Kết nối thành công!" };
                }

                return Ok(new SuccessResponse<ConnectPaymentResponse>(
                        new ConnectPaymentResponse
                        {
                            version = request.version,
                            merchant_id = request.merchant_id,
                            terminal_id = request.terminal_id,
                            order_id = request.order_id,
                            paintent_id = request.paintent_id,
                            vch_id = request.vch_id,
                            amount = request.amount,
                            currency_id = request.currency_id,
                            amount_round = ConnectPaymentDTO.Round((double)request.amount, -3),
                            txn = txn,
                            signature = request.signature
                        }
                    ));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Foxpay gửi OTP cho Portal-Listeins
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("api/payment/otp")]
        //public ActionResult<SuccessResponse<PaymentResponse>> Items([FromQuery] ConnectPaymentRequest request)
        public ActionResult<SuccessResponse<OTPPaymentResponse>> Items_OTP([FromQuery] string data)
        {
            try
            {
                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<OTPPaymentRequest>(data);
                string resultCode = "", result = "", message = "";
                IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

                string conString = ConfigurationExtensions.GetConnectionString(Configuration, request.merchant_id);

                //var qrCode = _qRCodeService.Gets(new QRCode()
                //{
                //    MdcFilePrkID = request.paintent_id,
                //    VchPmntPrkID = request.vch_id
                //});

                if (request.otp_code.Equals("200"))
                {
                    resultCode = "200";
                    result = "SUCCESS";
                    message = "Giao dịch thành công!";
                }
                else
                {
                    resultCode = "0";
                    result = "WARNING";
                    message = "Giao dịch không thành công!";
                }

                return Ok(new SuccessResponse<OTPPaymentResponse>(
                        new OTPPaymentResponse
                        {
                            version = request.version,
                            merchant_id = request.merchant_id,
                            terminal_id = request.terminal_id,
                            order_id = request.order_id,
                            paintent_id = request.paintent_id,
                            result_code = resultCode,
                            result = result,
                            message = message,
                            signature = request.signature
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