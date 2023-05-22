namespace PayOut_Aulac_FPT.DTO.ResultPayment
{
    public class CheckTransactionResponse
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public int? patient_id { get; set; }
        public int? vch_id { get; set; }
        public string? signature { get; set; }
        public string? result_code { get; set; }
        public string? result { get; set; }
        public string? message { get; set; }
        public string? error { get; set; }
        public string? error_description { get; set; }
    }
}
