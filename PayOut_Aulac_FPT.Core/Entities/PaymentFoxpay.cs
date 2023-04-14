using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Entities
{
    public class PaymentFoxpay
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public int? patient_id { get; set; }
        public string? transaction_payment { get; set; }
        public string? signature { get; set; }
    }

    public class ResultFoxpay
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
