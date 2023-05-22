using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PayOut_Aulac_FPT.Core.Entities;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using PayOut_Aulac_FPT.Core.Services;
using PayOut_Aulac_FPT.Core.Utils.Enums;
using PayOut_Aulac_FPT.DTO;
using PayOut_Aulac_FPT.DTO.ConnectPayment;
using PayOut_Aulac_FPT.DTO.OTPPayment;
using PayOut_Aulac_FPT.DTO.ResultPayment;
using PayOut_Aulac_FPT.Infrastructure.Services;
using System.Security.Cryptography.Xml;
using System;
using Microsoft.AspNetCore.SignalR;
using PayOut_Aulac_FPT.Hub;
using PayOut_Aulac_FPT.Core.Exceptions;
using System.Net;

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
        private readonly IBase64Service _base64Service;
        private readonly IQRCodeService _qRCodeService;
        private readonly IVchPaymentHISService _vchPaymentHISService;
        //private IHubContext<SignalRService, ISignalRService> _signalRService;
        private readonly IHubContext<SignalRService> _signalRService;

        public ResultPaymentController(ILogger<ResultPaymentController> logger, IFoxpayServiceAPI foxpayServiceAPI, IVchPaymentFoxpayService vchPaymentFoxpayService, IConfiguration configuration, ICompanyInfoService companyInfoService, IUserLoginService userLoginService, ISha256HexService sha256HexService, IVchPaymentHISService vchPaymentHISService, IBase64Service base64Service, IQRCodeService qRCodeService, IHubContext<SignalRService> signalRService)
        {
            _logger = logger;
            _foxpayServiceAPI = foxpayServiceAPI;
            _vchPaymentFoxpayService = vchPaymentFoxpayService;
            _configuration = configuration;
            _companyInfoService = companyInfoService;
            _userLoginService = userLoginService;
            _sha256HexService = sha256HexService;
            _vchPaymentHISService = vchPaymentHISService;
            _base64Service = base64Service;
            _qRCodeService = qRCodeService;
            _signalRService = signalRService;
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
            string resultCode = "", result = "", message = "", messageHub = "", terminal_id = "";
            try
            {
                var request = JsonConvert.DeserializeObject<ResultPaymentRequest>(data);
                terminal_id = request?.terminal_id;
                IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

                string conString = Microsoft
                       .Extensions
                       .Configuration
                       .ConfigurationExtensions
                       .GetConnectionString(Configuration, request.merchant_id);

                var versionFoxpay = _configuration["Foxpay:Version"];
                var companyInfo = _companyInfoService.Get(new CompanyInfo());
                var loginInfo = _userLoginService.Get(new UserLogin
                {
                    merchant_id = request.merchant_id,
                    terminal_id = request.terminal_id.Split('_')[1].ToString(),
                    IsActive = true
                });

                var mSignature = versionFoxpay + loginInfo.merchant_id + loginInfo.merchant_id + "_" + loginInfo.terminal_id /*+ PntInfo.qr_type*/ + request.order_id + request.transaction_payment + request.paintent_id /*+ qrCode.amount + PntInfo.currency + PntInfo.description + PntInfo.expired_time + PntInfo.customer_code + PntInfo.merchant_secret_key*/ + loginInfo.secret_key;

                if (_sha256HexService.SHA256Hex(mSignature) != request.signature)
                {
                    resultCode = "0";
                    result = "invalid_signature";
                    message = "Chữ ký không hợp lệ!";
                }
                else
                {
                    mSignature = versionFoxpay + loginInfo.merchant_id + loginInfo.merchant_id + "_" + loginInfo.terminal_id /*+ PntInfo.qr_type*/ + request.vch_id + request.transaction_payment + request.paintent_id /*+ qrCode.amount + PntInfo.currency + PntInfo.description + PntInfo.expired_time + PntInfo.customer_code + PntInfo.merchant_secret_key*/ + loginInfo.secret_key;
                    //Dùng API kiểm tra trạng thái giao dịch của Foxpay
                    CheckTransaction checkTransaction = new CheckTransaction
                    {
                        version = request.version,
                        merchant_id = request.merchant_id,
                        terminal_id = request.terminal_id,
                        vch_id = request.vch_id.ToString(),
                        transaction_payment = request.transaction_payment,
                        patient_id = request.paintent_id,
                        signature = _sha256HexService.SHA256Hex(mSignature)
                    };

                    var resultTrangThaiGiaoDich = await _foxpayServiceAPI.CheckTransaction(checkTransaction);
                    var query = _vchPaymentFoxpayService.Get(new VchPaymentFoxpay { patient_id = request.paintent_id, vch_id = request.vch_id.ToString() });
                    if (resultTrangThaiGiaoDich?.result_code == "200")
                    {
                        //Ghi nhận lại thông tin kết quả giao dịch, Cập nhật lại thông tin thanh toán cho bệnh nhân
                        _vchPaymentFoxpayService.Patch(new VchPaymentFoxpay
                        {
                            VchPmtFoxpayPrkID = query.VchPmtFoxpayPrkID,
                            terminal_id = loginInfo.merchant_id + "_" + loginInfo.terminal_id,
                            order_id = request.order_id,
                            payer_id = request.payer_id,
                            transaction_payment = request.transaction_payment,
                            StatusReq = (int)EStatusReq.Complated,
                            signature = request.signature
                        });

                        //Update trạng thái thanh toán ở VchPaymentHeader
                        _vchPaymentHISService.Patch(new VchPaymentHIS
                        {
                            VchPmntPrkID = int.Parse(query.vch_id),
                            MdcFilePrkID = query.patient_id,
                            StatusReq = (int)EStatusReq.Complated
                        });

                        resultCode = "200";
                        result = "success_completed";
                        message = "Thành công.";

                        switch (query.payment_type)
                        {
                            case "0":
                                messageHub = "Kết nối ví thành công!";
                                break;
                            case "1":
                                messageHub = "Hệ thống đã thực hiện thanh toán tạm thu hoàn thành!";
                                break;
                            case "2":
                                messageHub = "Hệ thống đã thực hiện thanh toán thu thêm hoàn thành!";
                                break;
                            case "3":
                                messageHub = "Hệ thống đã thực hiện thanh toán hoàn ứng hoàn thành!";
                                break;
                        }
                    }
                    else
                    {
                        resultCode = "0";
                        result = "invalid_error";
                        messageHub = message = "Có lỗi xảy ra!";
                    }

                    var sendHub = new ResultInfo()
                    {
                        result_code = resultCode,
                        payment_type = query.payment_type,
                        result = result,
                        message = messageHub
                    };

                    string roomName = string.Format("{0}", request?.terminal_id);
                    await _signalRService.Clients.Group(roomName).SendAsync("ReceiveMessage", sendHub);
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
                resultCode = "500";
                result = "internal_server_error";
                message = "Lỗi hệ thống!";
                var resultResponse = new SuccessResponse<ResultPaymentResponse>(
                        new ResultPaymentResponse
                        {
                            version = null,
                            merchant_id = null,
                            terminal_id = null,
                            order_id = null,
                            paintent_id = null,
                            vch_id = null,
                            signature = null,
                            result_code = resultCode,
                            result = result,
                            message = message
                        }
                    );
                resultResponse.Success = false;

                var sendHub = new ResultInfo()
                {
                    result_code = resultCode,
                    payment_type = null,
                    result = result,
                    message = messageHub
                };

                string roomName = string.Format("{0}", terminal_id);
                await _signalRService.Clients.Group(roomName).SendAsync("ReceiveMessage", sendHub);

                return Ok(resultResponse);
            }
        }

        /// <summary>
        /// API hủy thanh toán Foxpay
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("api/payment/cancel")]
        //public ActionResult<SuccessResponse<PaymentResponse>> Items([FromQuery] ConnectPaymentRequest request)
        public async Task<ActionResult<SuccessResponse<CancelPaymentResponse>>> Items_CancelAsync([FromQuery] string data)
        {
            try
            {
                string resultCode = "", result = "", message = "";
                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<CancelPaymentRequest>(data);

                IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

                string conString = ConfigurationExtensions.GetConnectionString(Configuration, request?.merchant_id);

                var versionFoxpay = _configuration["Foxpay:Version"];
                var companyInfo = _companyInfoService.Get(new CompanyInfo());
                var loginInfo = _userLoginService.Get(new UserLogin
                {
                    UserPaymentID = companyInfo.InsPrvnCode + companyInfo.InsOfficeCode,
                    IsActive = true
                });

                var PntInfo = new QRCode();

                if (request?.VchPmtFoxpayPrkID == null)
                    return Ok(new SuccessResponse<CancelPaymentResponse>(
                    new CancelPaymentResponse()
                ));

                PntInfo = _qRCodeService.Get(new QRCode()
                {
                    VchPmtFoxpayPrkID = request.VchPmtFoxpayPrkID
                });

                if (PntInfo == null)
                    return Ok(new SuccessResponse<CancelPaymentResponse>(
                    new CancelPaymentResponse()
                ));

                var hoTen = _base64Service.Base64Encode(PntInfo.ho_ten_bn);
                var contentPayment = _base64Service.Base64Encode(PntInfo.content_payment);
                var mSignature = versionFoxpay + loginInfo.merchant_id + loginInfo.merchant_id + "_" + loginInfo.terminal_id /*+ PntInfo.qr_type*/ + PntInfo.vch_id + PntInfo.transaction_payment + PntInfo.paintent_id + PntInfo.create_time_payment + PntInfo.optional /*+ PntInfo.amount + PntInfo.currency + PntInfo.description + PntInfo.expired_time + PntInfo.customer_code + PntInfo.merchant_secret_key*/ + loginInfo.secret_key;

                var cancelTransaction = new CancelTransaction()
                {
                    version = versionFoxpay,
                    merchant_id = loginInfo.merchant_id,
                    terminal_id = loginInfo.terminal_id,
                    vch_id = PntInfo.vch_id.ToString(),
                    transaction_payment = PntInfo.transaction_payment,
                    patient_id = PntInfo.paintent_id,
                    create_time_payment = PntInfo.create_time_payment,
                    optional = PntInfo.optional,
                    signature = _sha256HexService.SHA256Hex(mSignature)
                };

                var resultTrangThaiGiaoDich = await _foxpayServiceAPI.CancelTransaction(cancelTransaction);
                var query = _vchPaymentFoxpayService.Get(new VchPaymentFoxpay { VchPmtFoxpayPrkID = request.VchPmtFoxpayPrkID });

                if (resultTrangThaiGiaoDich?.result_code == "200")
                {
                    _vchPaymentFoxpayService.Patch(new VchPaymentFoxpay
                    {
                        VchPmtFoxpayPrkID = query.VchPmtFoxpayPrkID,
                        StatusReq = (int)EStatusReq.LostExam
                    });

                    _vchPaymentHISService.Patch(new VchPaymentHIS
                    {
                        VchPmntPrkID = int.Parse(query.vch_id),
                        MdcFilePrkID = query.patient_id,
                        StatusReq = (int)EStatusReq.LostExam
                    });
                }

                return Ok(new SuccessResponse<CancelPaymentResponse>(
                        new CancelPaymentResponse()
                        {
                            version = versionFoxpay,
                            merchant_id = loginInfo.merchant_id,
                            terminal_id = loginInfo.terminal_id,
                            order_id = PntInfo.order_id,
                            transaction_payment = PntInfo.transaction_payment,
                            patient_id = PntInfo.paintent_id.ToString(),
                            create_time_payment = PntInfo.create_time_payment,
                            optional = PntInfo.optional,
                            result_code = resultTrangThaiGiaoDich.result_code,
                            result = resultTrangThaiGiaoDich.result,
                            message = resultTrangThaiGiaoDich.message,
                            error_code = resultTrangThaiGiaoDich.error_code,
                            error_desc = resultTrangThaiGiaoDich.error_desc,
                            signature = resultTrangThaiGiaoDich.signature
                        }
                    ));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// API cung cấp dữ liệu đối soát
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("api/payment/reconcile")]
        //public ActionResult<SuccessResponse<PaymentResponse>> Items([FromQuery] ConnectPaymentRequest request)
        public async Task<ActionResult<SuccessResponse<ReconcilePaymentResponse>>> Items_ReconcileAsync([FromQuery] string data)
        {
            string resultCode = "", result = "", message = "";
            try
            {
                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<ReconcilePaymentRequest>(data);

                if (string.IsNullOrEmpty(request?.merchant_id) || string.IsNullOrEmpty(request.date_payment_from) || string.IsNullOrEmpty(request.date_payment_to) || string.IsNullOrEmpty(request.psn_payment_id) || request.page_size == 0 || string.IsNullOrEmpty(request.signature))
                {
                    resultCode = "0";
                    result = "invalid_parameter";
                    message = "Tham số không đúng hoặc không đầy đủ!";
                }
                else
                {
                    try
                    {
                        DateTime tmpDateTime_From = new DateTime();
                        DateTime tmpDateTime_To = new DateTime();
                        tmpDateTime_From = Convert.ToDateTime(request.date_payment_from);
                        tmpDateTime_To = Convert.ToDateTime(request?.date_payment_to);

                        //if(tmpDateTime_From > DateTime.Now)
                        //{
                        //    resultCode = "0";
                        //    result = "greater_than_present_time";
                        //    message = "Ngày tìm kiếm không được lớn hơn ngày hiện tại!";
                        //}
                        //else if(tmpDateTime_From > tmpDateTime_To)
                        //{
                        //    resultCode = "0";
                        //    result = "date_payment_from_cannot_greater_than_date_payment_to";
                        //    message = "Thời gian tìm kiếm từ ngày không được lớn hơn đến ngày!";
                        //}
                    }
                    catch (Exception)
                    {
                        resultCode = "0";
                        result = "invalid_date_payment";
                        message = "Tham số không đúng hoặc không đầy đủ!";
                    }
                }

                IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

                string conString = ConfigurationExtensions.GetConnectionString(Configuration, request?.merchant_id);

                var versionFoxpay = _configuration["Foxpay:Version"];
                var companyInfo = _companyInfoService.Get(new CompanyInfo());
                var loginInfo = _userLoginService.Get(new UserLogin
                {
                    UserPaymentID = request?.merchant_id,
                    IsActive = true
                });

                if (loginInfo == null)
                {
                    resultCode = "0";
                    result = "invalid_merchant_or_terminal";
                    message = "Đối tác không hợp lệ!";
                }
                else if (loginInfo.IsActiveSystem == "0")
                {
                    resultCode = "503";
                    result = "server_maintenance";
                    message = "Hệ thống đang được bảo trì!";
                }

                if (!string.IsNullOrEmpty(resultCode))
                {
                    return Ok(new SuccessResponse<CheckTransactionResponse>(
                        new CheckTransactionResponse()
                        {
                            version = null,
                            merchant_id = null,
                            terminal_id = null,
                            order_id = null,
                            patient_id = null,
                            vch_id = null,
                            signature = null,
                            result_code = resultCode,
                            result = result,
                            message = message,
                            error = null,
                            error_description = null
                        }
                    ));
                }

                var mSignature = versionFoxpay + loginInfo.merchant_id + request?.date_payment_from + request?.date_payment_to + request?.psn_payment_id + request?.current_page + request?.page_size + loginInfo.secret_key;

                var reconcileTransaction = new ReconcileTransaction()
                {
                    version = versionFoxpay,
                    merchant_id = loginInfo.merchant_id,
                    date_payment_from = request?.date_payment_from,
                    date_payment_to = request?.date_payment_to,
                    psn_payment_id = request?.psn_payment_id,
                    current_page = request?.current_page,
                    page_size = request?.page_size,
                    signature = _sha256HexService.SHA256Hex(mSignature)
                };

                var resultTrangThaiGiaoDich = await _foxpayServiceAPI.ReconcileTransaction(reconcileTransaction);

                return Ok(new SuccessResponse<ReconcilePaymentResponse>(
                        new ReconcilePaymentResponse()
                        {
                            error_code = resultTrangThaiGiaoDich?.error_code,
                            error_desc = resultTrangThaiGiaoDich?.error_desc,
                            message = resultTrangThaiGiaoDich?.message,
                            code = resultTrangThaiGiaoDich?.code,
                            error = resultTrangThaiGiaoDich?.error,
                            data = resultTrangThaiGiaoDich?.data,
                            error_description = resultTrangThaiGiaoDich?.error_description
                        }
                    ));
            }
            catch (Exception ex)
            {
                resultCode = "500"; result = "internal_server_error"; message = "Lỗi hệ thống!";
                var resultResponse = new SuccessResponse<ReconcilePaymentResponse>(
                        new ReconcilePaymentResponse
                        {
                            error_code = resultCode,
                            error_desc = result,
                            message = message,
                            code = null,
                            error = null,
                            data = null,
                            error_description = null
                        }
                    );
                resultResponse.Success = false;
                return Ok(resultResponse);
            }
        }

        /// <summary>
        /// API kiểm tra trạng thái giao dịch Foxpay
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("api/payment/check-transaction")]
        //public ActionResult<SuccessResponse<PaymentResponse>> Items([FromQuery] ConnectPaymentRequest request)
        public async Task<ActionResult<SuccessResponse<CheckTransactionResponse>>> Items_CheckTransaction([FromQuery] CheckTransactionRequest request)
        {
            string resultCode = "", result = "", message = "";
            try
            {
                IConfiguration Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

                string conString = ConfigurationExtensions.GetConnectionString(Configuration, request?.HspInsCode);
                var paymentFoxpay = _vchPaymentFoxpayService.Get(new VchPaymentFoxpay { patient_id = int.Parse(request?.MdcFilePrkID), vch_id = request.VchPmntPrkID });

                var versionFoxpay = _configuration["Foxpay:Version"];
                var companyInfo = _companyInfoService.Get(new CompanyInfo());
                var loginInfo = _userLoginService.Get(new UserLogin
                {
                    merchant_id = request.HspInsCode,
                    terminal_id = paymentFoxpay.terminal_id.Split('_')[1].ToString(),
                    IsActive = true
                });

                if (loginInfo == null)
                {
                    resultCode = "0";
                    result = "invalid_merchant_or_terminal";
                    message = "Đối tác không hợp lệ!";
                }
                else if (paymentFoxpay == null)
                {
                    resultCode = "404";
                    result = "notfound";
                    message = "Giao dịch không tồn tại!";
                }
                else if (loginInfo.IsActiveSystem == "0")
                {
                    resultCode = "503";
                    result = "server_maintenance";
                    message = "Hệ thống đang được bảo trì!";
                }

                if (!string.IsNullOrEmpty(resultCode))
                {
                    return Ok(new SuccessResponse<CheckTransactionResponse>(
                        new CheckTransactionResponse()
                        {
                            version = null,
                            merchant_id = null,
                            terminal_id = null,
                            order_id = null,
                            patient_id = null,
                            vch_id = null,
                            signature = null,
                            result_code = resultCode,
                            result = result,
                            message = message,
                            error = null,
                            error_description = null
                        }
                    ));
                }

                var mSignature = versionFoxpay + loginInfo.merchant_id + loginInfo.merchant_id + "_" + loginInfo.terminal_id /*+ PntInfo.qr_type*/ + paymentFoxpay.vch_id + paymentFoxpay.transaction_payment + paymentFoxpay.patient_id /*+ qrCode.amount + PntInfo.currency + PntInfo.description + PntInfo.expired_time + PntInfo.customer_code + PntInfo.merchant_secret_key*/ + loginInfo.secret_key;
                //Dùng API kiểm tra trạng thái giao dịch của Foxpay
                CheckTransaction checkTransaction = new CheckTransaction
                {
                    version = paymentFoxpay.version,
                    merchant_id = loginInfo.merchant_id,
                    terminal_id = loginInfo.merchant_id + "_" + loginInfo.terminal_id,
                    vch_id = paymentFoxpay.vch_id,
                    transaction_payment = paymentFoxpay.transaction_payment,
                    patient_id = paymentFoxpay.patient_id,
                    signature = _sha256HexService.SHA256Hex(mSignature)
                };

                var checkFoxpay = await _foxpayServiceAPI.CheckTransaction(checkTransaction);
                if (checkFoxpay.result_code == "0")
                {
                    resultCode = "0";
                    result = "payment_unrealized";
                    message = "Chưa thanh toán!";
                }
                else if (checkFoxpay.result_code == "1")
                {
                    resultCode = "1";
                    result = "payment_completed";
                    message = "Thanh toán thành công!";
                }
                else if (checkFoxpay.result_code == "2")
                {
                    resultCode = "2";
                    result = "payment_failed";
                    message = "Thanh toán thất bại!";
                }
                else if (checkFoxpay.result_code == "3")
                {
                    resultCode = "3";
                    result = "payment_pending";
                    message = "Thanh toán chờ xử lý";
                }

                var sendHub = new ResultInfo()
                {
                    result_code = resultCode,
                    payment_type = null,
                    result = result,
                    message = message
                };

                string roomName = string.Format("{0}", paymentFoxpay.terminal_id);
                await _signalRService.Clients.Group(roomName).SendAsync("ReceiveMessage", sendHub);

                return Ok(new SuccessResponse<CheckTransactionResponse>(
                        new CheckTransactionResponse()
                        {
                            version = checkFoxpay?.version,
                            merchant_id = checkFoxpay?.merchant_id,
                            terminal_id = checkFoxpay?.terminal_id,
                            order_id = checkFoxpay?.order_id,
                            patient_id = checkFoxpay?.patient_id,
                            vch_id = checkFoxpay?.vch_id,
                            signature = checkFoxpay?.signature,
                            result_code = resultCode,
                            result = result,
                            message = message,
                            error = checkFoxpay?.error,
                            error_description = checkFoxpay?.error_description
                        }
                    ));
            }
            catch (Exception ex)
            {
                resultCode = "500"; result = "internal_server_error"; message = "Lỗi hệ thống!";
                var resultResponse = new SuccessResponse<CheckTransactionResponse>(
                        new CheckTransactionResponse
                        {
                            version = null,
                            merchant_id = null,
                            terminal_id = null,
                            order_id = null,
                            patient_id = null,
                            vch_id = null,
                            signature = null,
                            result_code = resultCode,
                            result = result,
                            message = message,
                            error = null,
                            error_description = null
                        }
                    );
                resultResponse.Success = false;
                return Ok(resultResponse);
            }
        }
    }
}
