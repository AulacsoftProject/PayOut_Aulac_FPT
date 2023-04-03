namespace PayOut_Aulac_FPT.DTO.ConnectPayment
{
    public class ConnectPaymentDTO
    {
        public class Txn
        {
            public string? id { get; set; }
            public string? payment_method { get; set; }
            public string? brand_card { get; set; }
            public string? issuer_place { get; set; }
            public string? result { get; set; }
            public string? result_code { get; set; }
            public string? message { get; set; }
            public DateTime? transaction_time { get; set; }
        }
        public class Order
        {
            public string? version { get; set; }
            public string? id { get; set; }
            public string? amount { get; set; }
            public string? currency { get; set; }
            public DateTime? creation_time { get; set; }
        }
        public class TransactionResponse
        {
            public Txn? txn { get; set; }
            public Order? order { get; set; }
        }

        public static double Round(double value, int digits)
        {
            if (digits >= 0) return Math.Round(value, digits);

            double n = Math.Pow(10, -digits);
            return Math.Round(value / n, 0) * n;
        }

        public static decimal Round(decimal d, int decimals)
        {
            if (decimals >= 0) return decimal.Round(d, decimals);

            decimal n = (decimal)Math.Pow(10, -decimals);
            return decimal.Round(d / n, 0) * n;
        }
    }
}
