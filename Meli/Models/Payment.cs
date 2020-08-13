using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meli.Models
{
    public class Payment
    {
        public string reason { get; set; }
        public string status_code { get; set; }
        public string total_paid_amount { get; set; }
        public string operation_type { get; set; }
        public string transaction_amount { get; set; }
        public string date_approved { get; set; }
        public string coupon_id { get; set; }
        public string installments { get; set; }
        public string authorization_code { get; set; }
        public string taxes_amount { get; set; }
        public string id { get; set; }
        public string date_last_modified { get; set; }
        public string coupon_amount { get; set; }
        public string shipping_cost { get; set; }
        public string installment_amount { get; set; }
        public string date_created { get; set; }
        public string status_detail { get; set; }
        public string payment_method_id { get; set; }
        public string payment_type { get; set; }
        public string site_id { get; set; }
        public string payer_id { get; set; }
        public string order_id { get; set; }
        public string currency_id { get; set; }
        public string status { get; set; }

    }
}