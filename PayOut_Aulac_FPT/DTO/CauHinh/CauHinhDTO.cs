namespace PayOut_Aulac_FPT.DTO.CauHinh
{
    public class CauHinhDTO
    {
        public Guid? Id { get; set; }
        public string? Ma { get; set; }
        public string? Ten { get; set; }
        public string? GiaTri { get; set; }
        public string? MoTa { get; set; }
        public Guid? TaoBoi { get; set; }
        public DateTime? TaoLuc { get; set; }
        public Guid? CapNhatBoi { get; set; }
        public DateTime? CapNhatLuc { get; set; }
    }
}
