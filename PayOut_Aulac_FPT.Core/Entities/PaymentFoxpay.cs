using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Core.Entities
{
    public class CheckTransaction
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? vch_id { get; set; }
        public int? patient_id { get; set; }
        public string? transaction_payment { get; set; }
        public string? signature { get; set; }
    }

    public class CancelTransaction
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? vch_id { get; set; }
        public string? transaction_payment { get; set; }
        public int? patient_id { get; set; }
        public string? create_time_payment { get; set; }
        public string? optional { get; set; }
        public string? signature { get; set; }
    }

    public class ReconcileTransaction
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? date_payment_from { get; set; }
        public string? date_payment_to { get; set; }
        public string? psn_payment_id { get; set; }
        public int? current_page { get; set; }
        public int? page_size { get; set; }
        public string? signature { get; set; }
    }

    public class ResultCheckFoxpay
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public int? patient_id { get; set; }
        public int? vch_id { get; set; }
        public string? signature { get; set; }
        public string? result_code { get; set; }
        public string? result { get; set; }
        public string? message { get; set; }
        public string? error { get; set; }
        public string? error_description { get; set; }
    }

    public class ResultCancelFoxpay
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public string? transaction_payment { get; set; }
        public string? patient_id { get; set; }
        public string? create_time_payment { get; set; }
        public string? id_wallet_bus { get; set; }
        public string? result_code { get; set; }
        public string? result { get; set; }
        public string? message { get; set; }
        public string? error_code { get; set; }
        public string? error_desc { get; set; }
        public string? signature { get; set; }
    }

    public class ResultReconcileFoxpay
    {
        public string? error_code { get; set; }
        public string? error_desc { get; set; }
        public string? message { get; set; }
        public string? code { get; set; }
        public string? error { get; set; }
        public data? data { get; set; }
        public string? error_description { get; set; }
    }

    public class data
    {
        public List<Reconcile>? transactions { get; set; }
        public string? current_page { get; set; }
        public string? page_size { get; set; }
        public string? total_record { get; set; }
    }

    public class Reconcile
    {
        public string? version { get; set; }
        public string? merchant_id { get; set; }
        public string? terminal_id { get; set; }
        public string? order_id { get; set; }
        public string? paintent_id { get; set; }
        public string? cccd_id { get; set; }
        public string? bhyt_id { get; set; }
        public string? ho_ten_bn { get; set; }
        public string? ngay_sinh_bn { get; set; }
        public string? vch_id { get; set; }
        public string? amount { get; set; }
        public string? currency_id { get; set; }
        public string? amount_round { get; set; }
        public string? content_payment { get; set; }
        public string? create_time_payment { get; set; }
        public string? transaction_payment { get; set; }
        public string? transaction_type { get; set; }
        public string? signature { get; set; }
    }

    public class ConfirmOtpRequest
    {
        public string? patient_id { get; set; }
        public string? transaction_reference { get; set; }
        public string? credential { get; set; }
        public string? signature { get; set; }
    }

    public class ConfirmOtpResponse
    {
        public string? error { get; set; }
        public string? error_description { get; set; }
        public string? patient_id { get; set; }
        public string? transaction_payment { get; set; }
        public string? transaction_reference { get; set; }
        public string? transaction_id { get; set; }
        public string? transaction_status { get; set; }
        public string? transaction_status_name { get; set; }
        public string? transaction_timestamp { get; set; }
        public string? signature { get;set; }
    }

    public class SoftOtpRequest
    {
        public string? transaction_reference { get; set; }
        public string? signature { get; set; }
    }

    public class SoftOtpResponse
    {
        public string? error { get; set; }
        public string? error_description { get; set; }
        public string? transaction_payment { get; set; }
        public string? transaction_id { get; set; }
        public string? credential { get; set; }
        public string? signature { get; set; }
    }
}
