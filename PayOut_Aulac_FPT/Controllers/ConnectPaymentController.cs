using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using PayOut_Aulac_FPT.DTO;
using PayOut_Aulac_FPT.DTO.ConnectPayment;
using PayOut_Aulac_FPT.DTO.OTPPayment;
using PayOut_Aulac_FPT.Core.Utils.Enums;
using Microsoft.AspNetCore.SignalR;
using PayOut_Aulac_FPT.Hub;

namespace PayOut_Aulac_FPT.Controllers
{
    [Tags("Connect Payment")]
    [ApiController]
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
        private readonly IVchPaymentFoxpayService _vchPaymentFoxpayService;
        private IHubContext<SignalRService, ISignalRService> _signalRService;
        private readonly ILogger<ConnectPaymentController> _logger;
        private readonly IMapper _mapper;

        public ConnectPaymentController(ILogger<ConnectPaymentController> logger, ICompanyInfoService companyInfoService, IQRCodeService qRCodeService, IConfiguration configuration, IBase64Service base64Service, ISha256HexService sha256HexService, IUserLoginService userLoginService, IFoxpayServiceAPI foxpayServiceAPI, IVchPaymentFoxpayService vchPaymentFoxpayService, IHubContext<SignalRService, ISignalRService> signalRService, IMapper mapper)
        {
            _logger = logger;
            _companyInfoService = companyInfoService;
            _qRCodeService = qRCodeService;
            _configuration = configuration;
            _base64Service = base64Service;
            _sha256HexService = sha256HexService;
            _userLoginService = userLoginService;
            _fboxpayServiceAPI = foxpayServiceAPI;
            _vchPaymentFoxpayService = vchPaymentFoxpayService;
            _signalRService = signalRService;
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
                var loginInfo = _userLoginService.Get(new UserLogin
                {
                    UserPaymentID = companyInfo.InsPrvnCode + companyInfo.InsOfficeCode,
                    IsActive = true
                });

                var PntInfo = new QRCode();

                if (request.MdcFilePrkID == null)
                    return Ok(new SuccessResponse<QRCodeCreateResponse>(
                    new QRCodeCreateResponse()
                    {
                        MdcFilePrkID = PntInfo?.MdcFilePrkID,
                        VchPmntPrkID = PntInfo?.vch_id,
                        SoCMND = PntInfo?.SoCMND,
                        InsNum = PntInfo?.InsNum,
                        PntName = PntInfo?.ho_ten_bn,
                        PntBirthday = PntInfo?.PntBirthday,
                        SexName = PntInfo?.SexName,
                        DateExam = PntInfo?.DateExam,
                        AmtLineInExc = PntInfo?.amount,
                        QRCode = new QRCodeInfo()
                    }
                ));

                PntInfo = _qRCodeService.Get(new QRCode()
                {
                    MdcFilePrkID = request.MdcFilePrkID,
                    VchPmntPrkID = request.VchPmntPrkID
                });

                if (PntInfo == null)
                    return Ok(new SuccessResponse<QRCodeCreateResponse>(
                    new QRCodeCreateResponse()
                    {
                        MdcFilePrkID = PntInfo?.MdcFilePrkID,
                        VchPmntPrkID = PntInfo?.vch_id,
                        SoCMND = PntInfo?.SoCMND,
                        InsNum = PntInfo?.InsNum,
                        PntName = PntInfo?.ho_ten_bn,
                        PntBirthday = PntInfo?.PntBirthday,
                        SexName = PntInfo?.SexName,
                        DateExam = PntInfo?.DateExam,
                        AmtLineInExc = PntInfo?.amount,
                        QRCode = new QRCodeInfo()
                    }
                ));

                var hoTen = _base64Service.Base64Encode(PntInfo.ho_ten_bn);
                var contentPayment = _base64Service.Base64Encode(PntInfo.content_payment);
                //var mSignature = versionFoxpay + loginInfo.merchant_id + loginInfo.terminal_id /*+ PntInfo.qr_type*/ + PntInfo.order_id + PntInfo.transaction_payment + PntInfo.paintent_id /*+ PntInfo.amount + PntInfo.currency + PntInfo.description + PntInfo.expired_time + PntInfo.customer_code + PntInfo.merchant_secret_key*/ + loginInfo.secret_key;

                var qrCode = new QRCodeInfo()
                {
                    version = versionFoxpay,
                    merchant_id = loginInfo.merchant_id,
                    terminal_id = loginInfo.terminal_id,
                    order_id = PntInfo.order_id,
                    paintent_id = PntInfo.paintent_id,
                    cccd_id = PntInfo.SoCMND,
                    bhyt_id = PntInfo.InsNum,
                    ho_ten_bn = hoTen,
                    ngay_sinh_bn = PntInfo.PntBirthday,
                    vch_id = PntInfo.vch_id,
                    payment_type = PntInfo.payment_type,
                    amount = PntInfo.amount,
                    currency_id = PntInfo.currency_id,
                    content_payment = contentPayment,
                    psn_payment_id = PntInfo.psn_payment_id,
                    create_time_payment = PntInfo.create_time_payment,
                    transaction_payment = PntInfo.transaction_payment,
                    signature = PntInfo.signature,
                    mcc = PntInfo.mcc,
                    country_id = PntInfo.country_id,
                    merchant_name = loginInfo.UserPaymentName,
                    city_id = PntInfo.city_id,
                    terminal_name = PntInfo.terminal_name,
                };

                var vchPaymentFoxpay = _vchPaymentFoxpayService.CheckExist(new VchPaymentFoxpay { VchPmtFoxpayPrkID = PntInfo.VchPmtFoxpayPrkID, patient_id = PntInfo.MdcFilePrkID, vch_id = PntInfo.vch_id });

                if (vchPaymentFoxpay.Count() == 0)
                {
                    var insert = _vchPaymentFoxpayService.Create(new VchPaymentFoxpay
                    {
                        patient_id = PntInfo.MdcFilePrkID,
                        vch_id = PntInfo.vch_id,
                        payment_type = PntInfo.payment_type,
                        version = versionFoxpay,
                        order_id = PntInfo.order_id,
                        transaction_payment = PntInfo.transaction_payment,
                        amount = double.Parse(PntInfo.amount),
                        StatusReq = (int)EStatusReq.ReqExam,
                        signature = PntInfo.signature
                    });
                }

                var qrCodeResponse = new QRCodeCreateResponse()
                {
                    MdcFilePrkID = PntInfo.MdcFilePrkID,
                    VchPmntPrkID = PntInfo.vch_id,
                    SoCMND = PntInfo.SoCMND,
                    InsNum = PntInfo.InsNum,
                    PntName = PntInfo.ho_ten_bn,
                    PntBirthday = PntInfo.PntBirthday,
                    SexName = PntInfo.SexName,
                    DateExam = PntInfo.DateExam,
                    AmtLineInExc = PntInfo.amount,
                    QRCode = qrCode
                };

                _signalRService.Clients.All.StartConnect(qrCodeResponse);

                return Ok(new SuccessResponse<QRCodeCreateResponse>(
                    qrCodeResponse
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

                var versionFoxpay = _configuration["Foxpay:Version"];
                var companyInfo = _companyInfoService.Get(new CompanyInfo());
                var loginInfo = _userLoginService.Get(new UserLogin
                {
                    UserPaymentID = companyInfo.InsPrvnCode + companyInfo.InsOfficeCode,
                    IsActive = true
                });

                IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

                string conString = ConfigurationExtensions.GetConnectionString(Configuration, request.merchant_id);

                var qrCode = _qRCodeService.Get(new QRCode()
                {
                    MdcFilePrkID = request.paintent_id,
                    VchPmntPrkID = request.vch_id
                });

                if (qrCode == null)
                {
                    txn = new txn { result_code = "404", result = "NOTFOUND", message = "Không tìm thấy thông tin thanh toán!" };
                }
                else
                {
                    var mSignature = versionFoxpay + loginInfo.merchant_id + loginInfo.terminal_id /*+ PntInfo.qr_type*/ + request.order_id + request.transaction_payment + qrCode.paintent_id /*+ qrCode.amount + PntInfo.currency + PntInfo.description + PntInfo.expired_time + PntInfo.customer_code + PntInfo.merchant_secret_key*/ + loginInfo.secret_key;

                    if (_sha256HexService.SHA256Hex(mSignature) != request.signature)
                    {
                        txn = new txn { result_code = "0", result = "WARNING", message = "Dữ liệu giao dịch không chính xác. Vui lòng kiểm tra lại!" };
                    }
                    else if (double.Parse(qrCode.amount) == request.amount)
                    {
                        txn = new txn { result_code = "200", result = "SUCCESS", message = "Kết nối thành công!" };

                        var vchPaymentFoxpay = _vchPaymentFoxpayService.CheckExist(new VchPaymentFoxpay { VchPmtFoxpayPrkID = qrCode.VchPmtFoxpayPrkID, patient_id = qrCode.MdcFilePrkID, vch_id = qrCode.vch_id });

                        if (vchPaymentFoxpay.Count() == 0)
                        {
                            var insert = _vchPaymentFoxpayService.Create(new VchPaymentFoxpay
                            {
                                patient_id = qrCode.MdcFilePrkID,
                                vch_id = qrCode.vch_id,
                                payment_type = qrCode.payment_type,
                                version = request.version,
                                order_id = request.order_id,
                                transaction_payment = request.transaction_payment,
                                amount = request.amount,
                                StatusReq = (int)EStatusReq.ReqExam,
                                signature = request.signature
                            });
                        }
                        else if (qrCode.StatusReq != ((int)EStatusReq.Complated).ToString())
                        {
                            var insert = _vchPaymentFoxpayService.Patch(new VchPaymentFoxpay
                            {
                                VchPmtFoxpayPrkID = qrCode.VchPmtFoxpayPrkID,
                                order_id = request.order_id,
                                transaction_payment = request.transaction_payment,
                                StatusReq = (int)EStatusReq.Examming
                            });
                        }
                        else
                            txn = new txn { result_code = "0", result = "WARNING", message = "Bệnh nhân đã hoàn thành thanh toán!" };
                    }
                    else
                        txn = new txn { result_code = "0", result = "WARNING", message = "Số tiền giao dịch không đúng!" };
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
                string resultCode = "", result = "", message = "";
                var versionFoxpay = _configuration["Foxpay:Version"];
                var companyInfo = _companyInfoService.Get(new CompanyInfo());
                var loginInfo = _userLoginService.Get(new UserLogin
                {
                    UserPaymentID = companyInfo.InsPrvnCode + companyInfo.InsOfficeCode,
                    IsActive = true
                });

                var mSignature = versionFoxpay + loginInfo.merchant_id + loginInfo.terminal_id /*+ PntInfo.qr_type*/ + request.order_id + request.transaction_payment + request.paintent_id /*+ qrCode.amount + PntInfo.currency + PntInfo.description + PntInfo.expired_time + PntInfo.customer_code + PntInfo.merchant_secret_key*/ + loginInfo.secret_key;

                var vchPaymentFoxpay = _vchPaymentFoxpayService.Get(new VchPaymentFoxpay { patient_id = request.paintent_id, transaction_payment = request.transaction_payment });

                if (vchPaymentFoxpay == null)
                {
                    resultCode = "404";
                    result = "NOTFOUND";
                    message = "Không tìm thấy thông tin thanh toán!";
                }
                else
                {
                    if (_sha256HexService.SHA256Hex(mSignature) != request.signature)
                    {
                        resultCode = "0";
                        result = "WARNING";
                        message = "Dữ liệu giao dịch không chính xác. Vui lòng kiểm tra lại!";
                    }
                    else
                    {
                        if (request.otp_code.Equals("200"))
                        {
                            resultCode = "200";
                            result = "INVOICE_SUCCESS";
                            message = "Giao dịch thành công!";
                        }
                        else if (request.otp_code.Equals("5"))
                        {
                            resultCode = "5";
                            result = "INVOICE_CANCEL";
                            message = "Giao dịch bị hủy!";
                        }
                        else if (request.otp_code.Equals("-1"))
                        {
                            resultCode = "-1";
                            result = "INVOICE_ERROR";
                            message = "Giao dịch lỗi!";
                        }
                        else if (request.otp_code.Equals("-2"))
                        {
                            resultCode = "-2";
                            result = "INVOICE_FAILURE";
                            message = "Giao dịch thất bại!";
                        }
                        else if (request.otp_code.Equals("0"))
                        {
                            resultCode = "0";
                            result = "INVOICE_INIT";
                            message = "Giao dịch đã được khởi tạo!";
                        }
                        else if (request.otp_code.Equals("1"))
                        {
                            resultCode = "1";
                            result = "INVOICE_INIT_TXN";
                            message = "Giao dịch đã khởi tạo txn!";
                        }
                        else if (request.otp_code.Equals("2"))
                        {
                            resultCode = "2";
                            result = "INVOICE_AUTHORIZED";
                            message = "Giao dịch đã xác thực!";
                        }
                    }
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