using PayOut_Aulac_FPT.Core.Interfaces;
using PayOut_Aulac_FPT.Core.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Entities
{
    public class QRCode:IEntity
    {
        public string? MdcFilePrkID { get; set; }
        public string? PntName { get; set; }
        public string? PntBirthday { get; set; }
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public string? paintent_id { get; set; }
        public string? ho_ten_bn { get; set; }
        public string? vch_id { get; set; }
        public string? payment_type { get; set; }
        public string? amount { get; set; }
        public string? currency_id { get; set; }
        public string? content_payment { get; set; }
        public string? psn_payment_id { get; set; }
        public string? create_time_payment { get; set; }
        public Guid? transaction_payment { get; set; }
        public string? signature { get; set; }
        public string? mcc { get; set; }
        public string? country_id { get; set; }
        public string? merchant_name { get; set; }
        public string? city_id { get; set; }
        public string? terminal_name { get; set; }
        [TableForeign("VchPaymentHeader", "VchPmntPrkID")]
        public string? VchPmntPrkID { get; set; }
        [TableForeign("Dm_SexTypes", "SexName")]
        public string? SexName { get; set; }
        [TableForeign("Task_JrnOutsideExamHeader", "DateExam")]
        public string? DateExam { get; set; }
    }
}
