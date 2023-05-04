using PayOut_Aulac_FPT.Core.Entities;

namespace PayOut_Aulac_FPT.DTO.ResultPayment
{
    public class ReconcilePaymentResponse
    {
        public string? error_code { get; set; }
        public string? error_desc { get; set; }
        public string? message { get; set; }
        public string? code { get; set; }
        public string? error { get; set; }
        public data? data { get; set; }
        public string? error_description { get; set; }
    }
}
