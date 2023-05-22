namespace PayOut_Aulac_FPT.DTO.ConnectPayment
{
    public class QRCodeCreateRequest
    {
        public int? MdcFilePrkID { get; set; }
        public int? VchPmntPrkID { get; set; }
        public string? HspInsCode { get; set; }
    }
}
