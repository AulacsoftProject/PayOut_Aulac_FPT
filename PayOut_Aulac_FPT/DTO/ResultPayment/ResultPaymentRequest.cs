using PayOut_Aulac_FPT.Attributes;

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
        public string? paintent_id { get; set; }
        [RequiredCustom]
        public string? vch_id { get; set; }
        [RequiredCustom]
        public string? payer_id { get; set; }
        [RequiredCustom]
        public Guid? transaction_payment { get; set; }
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
