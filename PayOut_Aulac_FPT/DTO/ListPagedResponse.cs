namespace PayOut_Aulac_FPT.DTO
{
    public class ListPagedResponse<T>
    {
        public IEnumerable<T>? Data { get; set; }
        public int TotalRow { get; set; }
    }
}
