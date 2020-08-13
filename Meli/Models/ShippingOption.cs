namespace Meli.Models
{
    public class ShippingOption
    {
        public string id { get; set; }
        public string shipping_method_id { get; set; }
        public string name { get; set; }
        public string currency_id { get; set; }
        public string list_cost { get; set; }
        public string cost { get; set; }
        public string delivery_type { get; set; }
    }
}