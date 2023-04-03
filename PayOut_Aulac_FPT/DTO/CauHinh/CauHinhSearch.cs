using PayOut_Aulac_FPT.Core.Utils.Models;

namespace PayOut_Aulac_FPT.DTO.CauHinh
{
    public class CauHinhSearch : BaseSearch
    {
        public Guid? Id { get; set; }
        public string? Ma { get; set; }
        public string? Ten { get; set; }
        public string? GiaTri { get; set; }
        public string? MoTa { get; set; }
        public Guid? TaoBoi { get; set; }
        public DateFilterRange? TaoLuc { get; set; }
        public Guid? CapNhatBoi { get; set; }
        public DateFilterRange? CapNhatLuc { get; set; }
    }
}
