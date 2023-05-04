namespace PayOut_Aulac_FPT.DTO.ResultPayment
{
    public class CancelPaymentRequest
    {
        //public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        //public string? order_id { get; set; }
        //public string? transaction_payment { get; set; }
        //public int? patient_id { get; set; }
        //public string? create_time_payment { get; set; }
        //public string? id_wallet_bus { get; set; }
        //public string? signature { get; set; }

        public int? VchPmtFoxpayPrkID { get; set; }
    }
}
