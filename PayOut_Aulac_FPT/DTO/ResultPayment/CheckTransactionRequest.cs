namespace PayOut_Aulac_FPT.DTO.ResultPayment
{
    public class CheckTransactionRequest
    {
        public string? MdcFilePrkID { get; set; }
        public string? VchPmntPrkID { get;set; }
        public string? terminal_id { get; set; }
    }
}
