using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PayOut_Aulac_FPT.DTO.UserLogin
{
    public class UserLoginRequest
    {
        [Required]
        [MinLength(5), MaxLength(256)]
        [Display(Name = "Mã đăng nhập")]
        public string? UserPaymentID { get; set; }

        [Required]
        [MinLength(5), MaxLength(256)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string? UserPaymentPassWord { get; set; }
    }
}
