namespace PayOut_Aulac_FPT.DTO.UserLogin
{
    public class UserLoginResponse
    {
        public string? AccessToken { get; set; }
        public DateTime Expiration { get; set; }
        public UserLoginDTO? Data { get; set; }
        //public MenuNguoiDung[]? Permission { get; set; }
    }
}
