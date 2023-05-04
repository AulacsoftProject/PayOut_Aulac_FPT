using PayOut_Aulac_FPT.Core.Interfaces;
using PayOut_Aulac_FPT.Core.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Entities
{
    public class VchPaymentFoxpay:IEntity
    {
        [PrimaryKey]
        public int? VchPmtFoxpayPrkID { get; set; }
        public int? patient_id { get; set; }
        public string? vch_id { get; set; }
        public string? payment_type { get; set; }
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
