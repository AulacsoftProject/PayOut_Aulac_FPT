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
        public int? MdcFilePrkID { get; set; }
        public string? PntName { get; set; }
        public string? PntBirthday { get; set; }
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public int? paintent_id { get; set; }
        public string? cccd_id { get; set; }
        public string? bhyt_id { get; set; }
        public string? ho_ten_bn { get; set; }
        public string? ngay_sinh_bn { get; set; }
        public int? vch_id { get; set; }
        public string? payment_type { get; set; }
        public string? amount { get; set; }
        public string? currency_id { get; set; }
        public string? content_payment { get; set; }
        public string? psn_payment_id { get; set; }
        public string? create_time_payment { get; set; }
        public string? transaction_payment { get; set; }
        public string? transaction_reference { get;set; }
        public string? optional { get; set; }
        public string? signature { get; set; }
        public string? mcc { get; set; }
        public string? country_id { get; set; }
        public string? merchant_name { get; set; }
        public string? city_id { get; set; }
        public string? terminal_name { get; set; }
        [TableForeign("VchPaymentHeader", "VchPmntPrkID")]
        public int? VchPmntPrkID { get; set; }
        [TableForeign("Dm_SexTypes", "SexName")]
        public string? SexName { get; set; }
        [TableForeign("Task_JrnOutsideExamHeader", "DateExam")]
        public string? DateExam { get; set; }
        //[TableForeign("MdcFile_PntExam", "SoCMND")]
        public string? SoCMND { get; set; }
        [TableForeign("MdcFile_PntRdcObj", "InsNum")]
        public string? InsNum { get; set; }
        [TableForeign("VchPaymentFoxpay", "VchPmtFoxpayPrkID")]
        public int? VchPmtFoxpayPrkID { get; set; }
        [TableForeign("VchPaymentFoxpay", "StatusReq")]
        public string? StatusReq { get; set; }
    }
}
