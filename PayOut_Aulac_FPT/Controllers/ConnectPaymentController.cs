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
        //private IHubContext<SignalRService, ISignalRService> _signalRService;
        private readonly IHubContext<SignalRService> _signalRService;
        private readonly ILogger<ConnectPaymentController> _logger;
        private readonly IMapper _mapper;

        public ConnectPaymentController(ILogger<ConnectPaymentController> logger, ICompanyInfoService companyInfoService, IQRCodeService qRCodeService, IConfiguration configuration, IBase64Service base64Service, ISha256HexService sha256HexService, IUserLoginService userLoginService, IFoxpayServiceAPI foxpayServiceAPI, IVchPaymentFoxpayService vchPaymentFoxpayService, IHubContext<SignalRService> signalRService, IMapper mapper)
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
        public async Task<ActionResult<SuccessResponse<QRCodeCreateResponse>>> Info([FromQuery] QRCodeCreateRequest request)
        {
            try
            {
                IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

                string merchant_id = string.IsNullOrEmpty(request.HspInsCode) ? "46187" : request.HspInsCode;

                string conString = ConfigurationExtensions.GetConnectionString(Configuration, merchant_id);

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
                    terminal_id = loginInfo.merchant_id + "_" + loginInfo.terminal_id,
                    order_id = PntInfo.order_id,
                    paintent_id = PntInfo.paintent_id,
                    //cccd_id = PntInfo.SoCMND,
                    //bhyt_id = PntInfo.InsNum,
                    ho_ten_bn = hoTen,
                    //ngay_sinh_bn = PntInfo.PntBirthday,
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
                    terminal_name = loginInfo.terminal_name,
                };

                var vchPaymentFoxpay = _vchPaymentFoxpayService.CheckExist(new VchPaymentFoxpay { VchPmtFoxpayPrkID = PntInfo.VchPmtFoxpayPrkID, patient_id = PntInfo.MdcFilePrkID, vch_id = PntInfo.vch_id.ToString() });

                var transactionReference = DateTime.Now.ToString("yyyyMMddHHmmss") + PntInfo.MdcFilePrkID;
                if (vchPaymentFoxpay.Count() == 0)
                {
                    var insert = _vchPaymentFoxpayService.Create(new VchPaymentFoxpay
                    {
                        patient_id = PntInfo.MdcFilePrkID,
                        terminal_id = loginInfo.merchant_id + "_" + loginInfo.terminal_id,
                        vch_id = PntInfo.vch_id.ToString(),
                        payment_type = PntInfo.payment_type,
                        id_wallet_bus = null,
                        version = versionFoxpay,
                        order_id = PntInfo.vch_id.ToString(),
                        transaction_reference = PntInfo.vch_id.ToString(),
                        create_time_payment = PntInfo.create_time_payment,
                        transaction_payment = PntInfo.transaction_payment,
                        optional = null,
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

                //_signalRService.Clients.All.StartConnect(qrCodeResponse);
                string roomName = string.Format("{0}", qrCode?.terminal_id);
                await _signalRService.Clients.Group(roomName).SendAsync("ReceiveMessage", qrCodeResponse);

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
        public async Task<ActionResult<SuccessResponse<ConnectPaymentResponse>>> ItemsAsync([FromQuery] string data)
        {
            var txn = new txn();
            string? terminal_id = "";
            try
            {
                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<ConnectPaymentRequest>(data);
                terminal_id = request?.terminal_id;
                var versionFoxpay = _configuration["Foxpay:Version"];

                if (string.IsNullOrEmpty(request.version) || string.IsNullOrEmpty(request.merchant_id) || string.IsNullOrEmpty(request.terminal_id) || request.paintent_id == 0 || request.vch_id == 0 || request.amount == 0 || string.IsNullOrEmpty(request.currency_id) || string.IsNullOrEmpty(request.psn_payment_id) || string.IsNullOrEmpty(request.transaction_payment) || string.IsNullOrEmpty(request.signature))
                    txn = new txn { result_code = "0", result = "invalid_parameter", message = "Tham số không đúng hoặc không đầy đủ!" };
                else if (versionFoxpay != request.version)
                    txn = new txn { result_code = "0", result = "invalid_version", message = "Version không hợp lệ!" };

                if (txn.result_code == "0")
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

                IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

                string conString = ConfigurationExtensions.GetConnectionString(Configuration, request.merchant_id);

                var loginInfo = _userLoginService.Get(new UserLogin
                {
                    merchant_id = request.merchant_id,
                    terminal_id = request.terminal_id.Split('_')[1].ToString(),
                    IsActive = true
                });

                var qrCode = _qRCodeService.Get(new QRCode()
                {
                    MdcFilePrkID = request.paintent_id,
                    VchPmntPrkID = request.vch_id
                });

                if (loginInfo == null)
                    txn = new txn { result_code = "0", result = "invalid_merchant_or_terminal", message = "Đối tác không hợp lệ!" };
                else if (qrCode == null)
                    txn = new txn { result_code = "404", result = "notfound", message = "Không tìm thấy hóa đơn cần thanh toán trên hệ thống HIS!" };
                else if (double.Parse(qrCode.amount) == request.amount)
                    txn = new txn { result_code = "0", result = "invalid_amount", message = "Số tiền không hợp lệ!" };
                else if (qrCode.currency_id != request.currency_id)
                    txn = new txn { result_code = "0", result = "invalid_currency", message = "Mã đơn vị tiền tệ không hợp lệ!" };
                else if (qrCode.psn_payment_id != request.psn_payment_id)
                    txn = new txn { result_code = "0", result = "invalid_psn_payment", message = "Mã cán bộ thu ngân không tồn tại!" };
                else
                {
                    var mSignature = versionFoxpay + loginInfo.merchant_id + loginInfo.merchant_id + "_" + loginInfo.terminal_id /*+ PntInfo.qr_type + request.order_id*/ + request.order_id + request.id_wallet_bus + request.transaction_payment + qrCode.paintent_id /*+ qrCode.amount + PntInfo.currency + PntInfo.description + PntInfo.expired_time + PntInfo.customer_code + PntInfo.merchant_secret_key*/ + loginInfo.secret_key;

                    if (_sha256HexService.SHA256Hex(mSignature) != request.signature)
                    {
                        txn = new txn { result_code = "0", result = "invalid_signature", message = "Chữ ký không hợp lệ!" };
                    }
                    else
                    {
                        txn = new txn { result_code = "200", result = "success_connect", message = "Thành công!" };

                        var vchPaymentFoxpay = _vchPaymentFoxpayService.CheckExist(new VchPaymentFoxpay { VchPmtFoxpayPrkID = qrCode.VchPmtFoxpayPrkID, patient_id = qrCode.MdcFilePrkID, vch_id = qrCode.vch_id.ToString() });

                        var transactionReference = DateTime.Now.ToString("yyyyMMddHHmmss") + qrCode.MdcFilePrkID;
                        if (vchPaymentFoxpay.Count() == 0)
                        {
                            var insert = _vchPaymentFoxpayService.Create(new VchPaymentFoxpay
                            {
                                patient_id = qrCode.MdcFilePrkID,
                                terminal_id = loginInfo.merchant_id + "_" + loginInfo.terminal_id,
                                vch_id = qrCode.vch_id.ToString(),
                                payment_type = qrCode.payment_type,
                                id_wallet_bus = request.id_wallet_bus,
                                version = request.version,
                                order_id = request.vch_id.ToString(),
                                create_time_payment = qrCode.create_time_payment,
                                transaction_payment = request.transaction_payment,
                                transaction_reference = request.vch_id.ToString(),
                                optional = request.optional,
                                payer_id = request.psn_payment_id,
                                amount = request.amount,
                                StatusReq = (int)EStatusReq.Examming,
                                signature = request.signature
                            });
                        }
                        else if (int.Parse(qrCode.StatusReq) < (int)EStatusReq.Complated)
                        {
                            var insert = _vchPaymentFoxpayService.Patch(new VchPaymentFoxpay
                            {
                                VchPmtFoxpayPrkID = qrCode.VchPmtFoxpayPrkID,
                                terminal_id = loginInfo.merchant_id + "_" + loginInfo.terminal_id,
                                order_id = request.vch_id.ToString(),
                                id_wallet_bus = request.id_wallet_bus,
                                payer_id = request.psn_payment_id,
                                transaction_payment = request.transaction_payment,
                                optional = request.optional,
                                StatusReq = (int)EStatusReq.Examming,
                                signature = request.signature
                            });

                            txn = new txn { result_code = "200", result = "success_connected", message = "Hóa đơn đã được liên kết thành công!" };
                        }
                        else if (int.Parse(qrCode.StatusReq) == (int)EStatusReq.Complated)
                            txn = new txn { result_code = "0", result = "qr_payment_completed", message = "QR đã được thanh toán!" };
                        else if (int.Parse(qrCode.StatusReq) == (int)EStatusReq.LostExam)
                            txn = new txn { result_code = "0", result = "invoice_canceled", message = "Hóa đơn đã bị hủy!" };
                    }
                }

                #region Sen hub
                if (txn.result_code == "200")
                {
                    var sendHub = new ResultInfo()
                    {
                        result_code = txn.result_code,
                        vch_id = request.vch_id.ToString(),
                        result = txn.result,
                        message = "Hệ thống đang thực hiện thanh toán cho bệnh nhân."
                    };

                    string roomName = string.Format("{0}", request?.terminal_id);
                    await _signalRService.Clients.Group(roomName).SendAsync("ReceiveMessage", sendHub);
                }
                #endregion

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
                txn = new txn { result_code = "500", result = "internal_server_error", message = "Lỗi hệ thống!" };
                var resultResponse = new SuccessResponse<ConnectPaymentResponse>(
                        new ConnectPaymentResponse
                        {
                            version = null,
                            merchant_id = null,
                            terminal_id = null,
                            order_id = null,
                            paintent_id = null,
                            vch_id = null,
                            amount = null,
                            currency_id = null,
                            amount_round = null,
                            txn = txn,
                            signature = null
                        }
                    );
                resultResponse.Success = false;

                var sendHub = new ResultInfo()
                {
                    result_code = txn.result_code,
                    payment_type = null,
                    result = txn.result,
                    message = txn.message
                };

                string roomName = string.Format("{0}", terminal_id);
                await _signalRService.Clients.Group(roomName).SendAsync("ReceiveMessage", sendHub);

                return Ok(resultResponse);
            }
        }

        /// <summary>
        /// Foxpay gửi OTP cho Portal-Listeins
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("api/payment/otp")]
        //public ActionResult<SuccessResponse<PaymentResponse>> Items([FromQuery] ConnectPaymentRequest request)
        public async Task<ActionResult<SuccessResponse<OTPPaymentResponse>>> Items_OTPAsync([FromQuery] string data)
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
                string resultCode = "", result = "", message = "", otpCode = "";
                var versionFoxpay = _configuration["Foxpay:Version"];
                var companyInfo = _companyInfoService.Get(new CompanyInfo());
                var loginInfo = _userLoginService.Get(new UserLogin
                {
                    UserPaymentID = companyInfo.InsPrvnCode + companyInfo.InsOfficeCode,
                    IsActive = true
                });

                var vchPaymentFoxpay = _vchPaymentFoxpayService.Get(new VchPaymentFoxpay { patient_id = request.paintent_id, vch_id = request.order_id });

                var mSignature = versionFoxpay + loginInfo.merchant_id + loginInfo.merchant_id + "_" + loginInfo.terminal_id + request.transaction_payment + vchPaymentFoxpay.patient_id /*+ qrCode.amount + PntInfo.currency + PntInfo.description + PntInfo.expired_time + PntInfo.customer_code + PntInfo.merchant_secret_key*/ + loginInfo.secret_key;

                if (vchPaymentFoxpay == null)
                {
                    resultCode = "404";
                    result = "NOTFOUND";
                    message = "Không tìm thấy thông tin thanh toán. Không thể xác thực!";
                }
                else
                {
                    if (_sha256HexService.SHA256Hex(mSignature) != request.signature)
                    {
                        resultCode = "401";
                        result = "UNAUTHORIZED";
                        message = "Chữ ký không chính xác. Vui lòng kiểm tra lại!";
                    }
                    else
                    {
                        #region check OTP
                        //if (vchPaymentFoxpay?.payment_type == "3")
                        //{
                        var mSignatureOTP = "";
                        var getOTP = new SoftOtpResponse();
                        //Kiểm tra mã OTP có trả về hay không
                        if (string.IsNullOrEmpty(request.otp_code))
                        {
                            mSignatureOTP = vchPaymentFoxpay.transaction_reference + loginInfo.secret_key;

                            SoftOtpRequest softOtp = new SoftOtpRequest()
                            {
                                transaction_reference = vchPaymentFoxpay.transaction_reference,
                                signature = _sha256HexService.SHA256Hex(mSignatureOTP)
                            };

                            getOTP = await _fboxpayServiceAPI.SoftOTP(softOtp);

                            if (getOTP?.error == null)
                            {
                                otpCode = getOTP?.credential;
                                mSignatureOTP = request.paintent_id + vchPaymentFoxpay.transaction_reference + otpCode + loginInfo.secret_key;
                            }
                            else
                            {
                                if (getOTP?.error == "unauthorized")
                                {
                                    resultCode = "401";
                                    result = "UNAUTHORIZED";
                                    message = "Chữ ký xác thực không hợp lệ!";//otpConfirm?.error_description;
                                }
                                else if (getOTP?.error == "internalservererrors")
                                {
                                    resultCode = "500";
                                    result = "INTERNALSERVERERRORS";
                                    message = getOTP?.error_description;
                                }
                                else
                                {
                                    resultCode = "0";
                                    result = "WARNING";
                                    message = getOTP?.error_description;
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
                        }
                        else
                        {
                            otpCode = request.otp_code;
                            mSignatureOTP = request.paintent_id + vchPaymentFoxpay.transaction_reference + otpCode + loginInfo.secret_key;
                        }

                        ConfirmOtpRequest otp = new ConfirmOtpRequest()
                        {
                            patient_id = request.paintent_id.ToString(),
                            transaction_reference = vchPaymentFoxpay.transaction_reference,
                            credential = otpCode,
                            signature = _sha256HexService.SHA256Hex(mSignatureOTP),
                        };

                        var otpConfirm = await _fboxpayServiceAPI.ConfirmOTP(otp);

                        if (otpConfirm?.error == null)
                        {
                            resultCode = "200";
                            result = "INVOICE_SUCCESS";
                            message = "Xác thực thành công!";

                            var insert = _vchPaymentFoxpayService.Patch(new VchPaymentFoxpay
                            {
                                VchPmtFoxpayPrkID = vchPaymentFoxpay.VchPmtFoxpayPrkID,
                                transaction_payment = request.transaction_payment,
                                StatusReq = (int)EStatusReq.Examming
                            });
                        }
                        else if (otpConfirm?.error == "unauthorized")
                        {
                            resultCode = "401";
                            result = "UNAUTHORIZED";
                            message = "Chữ ký xác thực không hợp lệ!";//otpConfirm?.error_description;
                        }
                        else if (otpConfirm?.error == "internalservererrors")
                        {
                            resultCode = "500";
                            result = "INTERNALSERVERERRORS";
                            message = otpConfirm?.error_description;
                        }
                        else
                        {
                            resultCode = "0";
                            result = "WARNING";
                            message = otpConfirm?.error_description;
                        }
                        //}
                        #endregion

                        //if (request.otp_code.Equals("200"))
                        //{
                        //    resultCode = "200";
                        //    result = "INVOICE_SUCCESS";
                        //    message = "Giao dịch thành công!";
                        //}
                        //else if (request.otp_code.Equals("5"))
                        //{
                        //    resultCode = "5";
                        //    result = "INVOICE_CANCEL";
                        //    message = "Giao dịch bị hủy!";
                        //}
                        //else if (request.otp_code.Equals("-1"))
                        //{
                        //    resultCode = "-1";
                        //    result = "INVOICE_ERROR";
                        //    message = "Giao dịch lỗi!";
                        //}
                        //else if (request.otp_code.Equals("-2"))
                        //{
                        //    resultCode = "-2";
                        //    result = "INVOICE_FAILURE";
                        //    message = "Giao dịch thất bại!";
                        //}
                        //else if (request.otp_code.Equals("0"))
                        //{
                        //    resultCode = "0";
                        //    result = "INVOICE_INIT";
                        //    message = "Giao dịch đã được khởi tạo!";
                        //}
                        //else if (request.otp_code.Equals("1"))
                        //{
                        //    resultCode = "1";
                        //    result = "INVOICE_INIT_TXN";
                        //    message = "Giao dịch đã khởi tạo txn!";
                        //}
                        //else if (request.otp_code.Equals("2"))
                        //{
                        //    resultCode = "2";
                        //    result = "INVOICE_AUTHORIZED";
                        //    message = "Giao dịch đã xác thực!";
                        //}
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