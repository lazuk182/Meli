using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meli.Models
{
    public class Order
    {
        public string id { get; set; }
        public string date_created { get; set; }
        public string date_closed { get; set; }
        public List<OrderItem> order_items { get; set; } = new List<OrderItem>();
        public List<Payment> payments { get; set; } = new List<Payment>();
        public List<string> tags { get; set; } = new List<string>();
        public Buyer buyer { get; set; } = new Buyer();
        public string total_amount { get; set; }
        public string paid_amount { get; set; }
        public string currency_id { get; set; }
        public string status { get; set; }
        public Shipment shipping { get; set; } = new Shipment();
        public Seller seller { get; set; } = new Seller();
    }
}