namespace PayOut_Aulac_FPT.DTO.UserLogin
{
    public class UserLoginDTO
    {
        public string? UserPaymentPrkID { get; set; }
        public string? UserPaymentID { get; set; }
        //public string? UserPaymentPassWord { get; set; }
        public string? UserPaymentName { get; set; }
        public string? UserNameFoxpay { get; set; }
        public string? PassWordFoxpay { get; set; }
        public string? Version { get; set; }
        public string? Domain { get; set; }
        public string? Domain2 { get; set; }
        public string? DeviceID { get; set; }
        public string? IpAddress { get; set; }
        public string? IsActiveSystem { get; set; }
        public Guid? merchant_id { get; set; }
        public Guid? terminal_id { get; set; }
        public string? terminal_name { get; set; }
        public string? secret_key { get; set; }
        public bool? IsActive { get; set; }
    }
}
