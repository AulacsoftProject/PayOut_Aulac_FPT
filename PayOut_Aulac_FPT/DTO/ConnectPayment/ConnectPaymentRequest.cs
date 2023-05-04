using PayOut_Aulac_FPT.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PayOut_Aulac_FPT.DTO.ConnectPayment
{
    public class ConnectPaymentRequest
    {
        [RequiredCustom]
        public string? version { get; set; }
        [RequiredCustom]
        public string? merchant_id { get; set; }
        [RequiredCustom]
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public string? optional { get; set; }
        [RequiredCustom]
        public int? paintent_id { get; set; }
        public string? ho_ten_bn { get; set; }
        [RequiredCustom]
        public int? vch_id { get; set; }
        [RequiredCustom]
        public double? amount { get; set; }
        [RequiredCustom]
        public string? currency_id { get; set; }
        public string? content_payment { get; set; }
        [RequiredCustom]
        public string? psn_payment_id { get; set; }
        [RequiredCustom]
        public string? transaction_payment { get; set; }
        public string? create_time_foxpay { get; set; }
        public string? payment_method { get; set; }
        [RequiredCustom]
        public string? signature { get; set; }
    }
}
