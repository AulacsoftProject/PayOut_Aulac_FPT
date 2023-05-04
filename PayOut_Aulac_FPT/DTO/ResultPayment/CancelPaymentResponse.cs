namespace PayOut_Aulac_FPT.DTO.ResultPayment
{
    public class CancelPaymentResponse
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public string? transaction_payment { get; set; }
        public string? patient_id { get; set; }
        public string? create_time_payment { get; set; }
        public string? optional { get; set; }
        public string? result_code { get; set; }
        public string? result { get; set; }
        public string? message { get; set; }
        public string? error_code { get; set; }
        public string? error_desc { get; set; }
        public string? signature { get; set; }
    }
}
