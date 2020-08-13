using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meli.Models
{
    public class OrderItem
    {
        public string id { get; set; }
        public string quantity { get; set; }
        public string sale_fee { get; set; }
        public string listing_type_id { get; set; }
        public string unit_price { get; set; }
        public string full_unit_price { get; set; }
        public string currency { get; set; }
        public Item item { get; set; } = new Item();
    }
}