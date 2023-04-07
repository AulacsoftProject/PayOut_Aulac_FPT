namespace PayOut_Aulac_FPT.DTO.VchPaymentFoxpay
{
    public class VchPaymentFoxpayDTO
    {
        public string? VchPmtFoxPayPrkID { get; set; }
        public string? patient_id { get; set; }
        public string? vch_id { get; set; }
        public string? payment_type { get; set; }
        public string? version { get; set; }
        public string? order_id { get; set; }
        public string? transaction_payment { get; set; }
        public string? amount { get; set; }
        public string? StatusReq { get; set; }
        public string? signature { get; set; }
    }
}
