using PayOut_Aulac_FPT.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PayOut_Aulac_FPT.DTO.ResultPayment
{
    public class ResultPaymentRequest
    {
        [RequiredCustom]
        public string? version { get; set; }
        [RequiredCustom]
        public string? merchant_id { get; set; }
        [RequiredCustom]
        public string? terminal_id { get; set; }
        [RequiredCustom]
        public string? order_id { get; set; }
        [RequiredCustom]
        public string? patient_id { get; set; }
        [RequiredCustom]
        public string? vch_id { get; set; }
        [RequiredCustom]
        [Display(Name = "Người thực hiện thanh toán")]
        public string? payer_id { get; set; }
        [RequiredCustom]
        public string? transaction_payment { get; set; }
        [RequiredCustom]
        public string? signature { get; set; }
        [RequiredCustom]
        public string? result_code { get; set; }
        [RequiredCustom]
        public string? result { get; set; }
        [RequiredCustom]
        public string? message { get; set; }
    }
}
