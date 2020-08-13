using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meli.Models
{
    public class Item
    {
        public string seller_custom_field { get; set; }
        public string condition { get; set; }
        public string category_id { get; set; }
        public string variation_id { get; set; }
        public string seller_sku { get; set; }
        public string warranty { get; set; }
        public string id { get; set; }
        public string title { get; set; }
    }
}