﻿namespace PayOut_Aulac_FPT.DTO.ConnectPayment
{
    public class QRCodeCreateResponse
    {
        public string? MdcFilePrkID { get; set; }
        public string? VchPmntPrkID { get; set; }
        public string? PntName { get; set; }
        public string? PntBirthday { get; set; }
        public string? SexName { get; set; }
        public string? DateExam { get; set; }
        public string? AmtLineInExc { get; set; }
        public QRCodeInfo? QRCode { get; set; }
    }

    public class QRCodeInfo
    {
        public string? version { get; set; }
        public Guid? merchant_id { get; set; }
        public Guid? terminal_id { get; set; }
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
    }
}
