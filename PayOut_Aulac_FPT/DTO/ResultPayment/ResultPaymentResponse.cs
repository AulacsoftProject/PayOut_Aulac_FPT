namespace PayOut_Aulac_FPT.DTO.ResultPayment
{
    public class ResultPaymentResponse
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public int? paintent_id { get; set; }
        public int? vch_id { get; set; }
        public string? signature { get; set; }
        public string? result_code { get; set; }
        public string? result { get; set; }
        public string? message { get; set; }
    }
}
