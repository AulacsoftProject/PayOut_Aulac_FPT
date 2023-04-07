using PayOut_Aulac_FPT.Attributes;
using System.ComponentModel;

namespace PayOut_Aulac_FPT.DTO.VchPaymentFoxpay
{
    public class VchPaymentFoxpayUpdateRequest
    {
        [RequiredCustom]
        [DisplayName("Mã quản lý thanh toán")]
        public string? VchPmtFoxPayPrkID { get; set; }
        [RequiredCustom]
        [DisplayName("Mã khám chữa bệnh của bệnh nhân")]
        public string? patient_id { get; set; }
        [RequiredCustom]
        [DisplayName("Mã giao dịch")]
        public string? vch_id { get; set; }
        [RequiredCustom]
        [DisplayName("Loại thanh toán")]
        public string? payment_type { get; set; }
        [DisplayName("Phiên bản Foxpay")]
        public string? version { get; set; }
        [DisplayName("Ví thanh toán CSKCB")]
        public string? order_id { get; set; }
        [DisplayName("Chuỗi định danh phiên giao dịch")]
        public string? transaction_payment { get; set; }
        [RequiredCustom]
        [DisplayName("Số tiền giao dịch")]
        public string? amount { get; set; }
        [RequiredCustom]
        [DisplayName("Trạng thái thanh toán")]
        public string? StatusReq { get; set; }
        [DisplayName("Chữ ký số")]
        public string? signature { get; set; }
    }
}
