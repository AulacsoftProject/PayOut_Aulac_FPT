namespace PayOut_Aulac_FPT.DTO.ResultPayment
{
    public class ReconcilePaymentRequest
    {
        public string? merchant_id { get; set; }
        public string? date_payment_from { get; set; }
        public string? date_payment_to { get; set; }
        public string? psn_payment_id { get; set; }
        public int? current_page { get; set; }
        public int? page_size { get; set; }
        public string? signature { get; set; }
    }
}
