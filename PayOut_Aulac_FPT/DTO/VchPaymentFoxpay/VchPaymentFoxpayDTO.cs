namespace PayOut_Aulac_FPT.DTO.VchPaymentFoxpay
{
    public class VchPaymentFoxpayDTO
    {
        public int? VchPmtFoxpayPrkID { get; set; }
        public int? patient_id { get; set; }
        public string? terminal_id { get; set; }
        public string? vch_id { get; set; }
        public string? payment_type { get; set; }
        public string? id_wallet_bus { get; set; }
        public string? version { get; set; }
        public string? order_id { get; set; }
        public string? transaction_reference { get; set; }
        public string? create_time_payment { get; set; }
        public string? transaction_payment { get; set; }
        public string? optional { get; set; }
        public string? payer_id { get; set; }
        public double? amount { get; set; }
        public int? StatusReq { get; set; }
        public string? signature { get; set; }
    }
}
