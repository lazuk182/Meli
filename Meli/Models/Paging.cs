using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meli.Models
{
    public class Paging
    {
        public string total { get; set; }
        public string offset { get; set; }
        public string limit { get; set; }
    }
}