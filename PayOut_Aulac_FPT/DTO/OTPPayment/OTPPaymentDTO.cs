using PayOut_Aulac_FPT.Attributes;

namespace PayOut_Aulac_FPT.DTO.OTPPayment
{
    public class OTPPaymentDTO
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public string? paintent_id { get; set; }
        public string? create_time_payment { get; set; }
        public string? transaction_payment { get; set; }
        public string? signature { get; set; }
        public string? otp_code { get; set; }
    }
}
