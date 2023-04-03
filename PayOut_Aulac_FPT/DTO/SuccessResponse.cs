namespace PayOut_Aulac_FPT.DTO
{
    public class SuccessResponse<T>
    {
        public bool Success { get; set; } = true;
        public T? Data { get; set; }

        public SuccessResponse(T? data) { Data = data; }
    }
}
