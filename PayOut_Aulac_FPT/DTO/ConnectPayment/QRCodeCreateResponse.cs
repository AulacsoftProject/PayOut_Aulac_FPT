namespace PayOut_Aulac_FPT.DTO.ConnectPayment
{
    public class QRCodeCreateResponse
    {
        public int? MdcFilePrkID { get; set; }
        public int? VchPmntPrkID { get; set; }
        public int? VchPmtFoxPayPrkID { get; set; }
        public string? StatusReq { get; set; }
        public string? PntName { get; set; }
        public string? PntBirthday { get; set; }
        public string? SexName { get; set; }
        public string? SoCMND { get; set; }
        public string? InsNum { get; set; }
        public string? DateExam { get; set; }
        public string? AmtLineInExc { get; set; }
        public QRCodeInfo? QRCode { get; set; }
    }

    public class QRCodeInfo
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public int? paintent_id { get; set; }
        //public string? cccd_id { get; set; }
        //public string? bhyt_id { get;set; }
        public string? ho_ten_bn { get; set; }
        //public string? ngay_sinh_bn { get; set; }
        public int? vch_id { get; set; }
        public string? payment_type { get; set; }
        public string? amount { get; set; }
        public string? currency_id { get; set; }
        public string? content_payment { get; set; }
        public string? psn_payment_id { get; set; }
        public string? create_time_payment { get; set; }
        public string? transaction_payment { get; set; }
        public string? signature { get; set; }
        public string? mcc { get; set; }
        public string? country_id { get; set; }
        public string? merchant_name { get; set; }
        public string? city_id { get; set; }
        public string? terminal_name { get; set; }
    }

    public class ResultInfo
    {
        public string? result_code { get; set; }
        public string? result { get; set; }
        public string? message { get; set; }
    }
}
