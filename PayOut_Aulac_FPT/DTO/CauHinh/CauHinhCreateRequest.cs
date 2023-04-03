using PayOut_Aulac_FPT.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PayOut_Aulac_FPT.DTO.CauHinh
{
    public class CauHinhCreateRequest
    {
        [RequiredCustom]
        [Display(Name = "Mã cấu hình")]
        public string? Ma { get; set; }
        [RequiredCustom]
        [Display(Name = "Tên")]
        public string? Ten { get; set; }
        [RequiredCustom]
        [Display(Name = "Giá trị")]
        public string? GiaTri { get; set; }
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
    }
}
