namespace PayOut_Aulac_FPT.DTO.ConnectPayment
{
    public class QRCodeDTO
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public string? paintent_id { get; set; }
        public string? ho_ten_bn { get; set; }
        public string? vch_id { get; set; }
        public string? payment_type { get; set; }
        public float? amount { get; set; }
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
