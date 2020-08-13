namespace Meli.Models
{
    public class Address
    {
        public string id { get; set; }
        public string address_line { get; set; }
        public string street_name { get; set; }
        public string street_number { get; set; }
        public string comment { get; set; }
        public string zip_code { get; set; }
        public Location city { get; set; } = new Location();
        public Location state { get; set; } = new Location();
        public Location country { get; set; } = new Location();
        public Location neighborhood { get; set; } = new Location();
        public Location municipality { get; set; } = new Location();
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string geolocation_type { get; set; }
        public string receiver_name { get; set; }
        public string receiver_phone { get; set; }

    }
}