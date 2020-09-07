using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincSubmitOrderResponse
    {
        public class ClientNotes
        {
            public string our_internal_order_id { get; set; }
        }

        public class AutoRetrySettings
        {
            public List<int> retry_delays { get; set; }
        }

        public class Shipping
        {
            public string order_by { get; set; }
            public int max_days { get; set; }
            public int max_price { get; set; }
        }

        public class Product
        {
            public string product_id { get; set; }
            public int quantity { get; set; }
            public List<object> variants { get; set; }
        }

        public class ShippingAddress
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string address_line1 { get; set; }
            public string address_line2 { get; set; }
            public string zip_code { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string phone_number { get; set; }
        }

        public class Request
        {
            public bool addax { get; set; }
            public string retailer { get; set; }
            public int max_price { get; set; }
            public string gift_message { get; set; }
            public ClientNotes client_notes { get; set; }
            public string client_token { get; set; }
            public AutoRetrySettings auto_retry_settings { get; set; }
            public List<object> scheduled_delivery_windows { get; set; }
            public List<object> free_gifts { get; set; }
            public List<object> bundled_order_ids { get; set; }
            public List<object> retry_request_ids { get; set; }
            public List<object> promo_codes { get; set; }
            public Shipping shipping { get; set; }
            public List<Product> products { get; set; }
            public bool is_gift { get; set; }
            public ShippingAddress shipping_address { get; set; }
            public DateTime _created_at { get; set; }
            public DateTime _finalized_at { get; set; }
        }

        public class Details
        {
            public string extra { get; set; }
            public string details { get; set; }
            public List<string> affected_product_ids { get; set; }
        }

        public class Data
        {
            public Details details { get; set; }
            public object current_url { get; set; }
        }

        public class RootObject
        {
            public string _type { get; set; }
            public Request request { get; set; }
            public string code { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
            public string host { get; set; }
            public DateTime _created_at { get; set; }
            public string request_id { get; set; }
            public List<object> status_updates { get; set; }
        }
    }
}
