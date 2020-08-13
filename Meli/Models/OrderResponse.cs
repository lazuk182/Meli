using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meli.Models
{
    public class OrderResponse
    {
        public string query { get; set; }
        public List<Order> results { get; set; }
        public string display { get; set; }
        public Paging paging { get; set; } = new Paging();
    }
}