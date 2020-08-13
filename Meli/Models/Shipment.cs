namespace Meli.Models
{
    public class Shipment
    {
        public string id { get; set; }
        public string mode { get; set; }
        public string created_by { get; set; }
        public string order_id { get; set; }
        public string order_cost { get; set; }
        public string base_cost { get; set; }
        public string site_id { get; set; }
        public string status { get; set; }
        public string substatus { get; set; }
        public string date_created { get; set; }
        public string last_updated { get; set; }
        public string tracking_number { get; set; }
        public string tracking_method { get; set; }
        public string service_id { get; set; }
        public string sender_id { get; set; }
        public Address sender_address { get; set; } = new Address();
        public string receiver_id { get; set; }
        public Address receiver_address { get; set; } = new Address();
        public ShippingOption shipping_option { get; set; } = new ShippingOption();
        public string market_place { get; set; }
        public string type { get; set; }
        public string logistic_type { get; set; }
    }
}