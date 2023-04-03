namespace PayOut_Aulac_FPT.DTO.ConnectPayment
{
    public class ConnectPaymentResponse
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public string? paintent_id { get; set; }
        public string? vch_id { get; set; }
        public double? amount { get; set; }
        public string? currency_id { get; set; }
        public double? amount_round { get; set; }
        public txn? txn { get; set; }
        public string? signature { get; set; }
    }

    public class txn
    {
        public string? result_code { get; set; }
        public string? result { get; set; }
        public string? message { get; set; }
    }
}
