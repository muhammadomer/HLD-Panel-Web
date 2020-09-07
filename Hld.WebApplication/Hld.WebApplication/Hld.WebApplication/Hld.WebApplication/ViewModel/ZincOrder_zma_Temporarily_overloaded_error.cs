using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincOrder_zma_Temporarily_overloaded_error
    {
        public class PaymentMethod
        {
        }

        public class ClientNotes
        {
            public int custom_order_id { get; set; }
        }

        public class RetailerCredentials
        {
        }

        public class AutoRetrySettings
        {
            public List<int> retry_delays { get; set; }
        }

        public class SellerSelectionCriteria
        {
            public int min_seller_percent_positive_feedback { get; set; }
            public bool international { get; set; }
            public List<string> condition_in { get; set; }
            public int min_seller_num_ratings { get; set; }
            public bool addon { get; set; }
        }

        public class Product
        {
            public string product_id { get; set; }
            public int quantity { get; set; }
            public SellerSelectionCriteria seller_selection_criteria { get; set; }
            public List<object> variants { get; set; }
        }

        public class ShippingAddress
        {
            public string city { get; set; }
            public string phone_number { get; set; }
            public string country { get; set; }
            public string first_name { get; set; }
            public string address_line2 { get; set; }
            public string address_line1 { get; set; }
            public string zip_code { get; set; }
            public string state { get; set; }
            public string last_name { get; set; }
        }

        public class Request
        {
            public bool addax { get; set; }
            public object gift_message { get; set; }
            public PaymentMethod payment_method { get; set; }
            public ClientNotes client_notes { get; set; }
            public object po_number { get; set; }
            public string shipping_method { get; set; }
            public RetailerCredentials retailer_credentials { get; set; }
            public object billing_address { get; set; }
            public string client_token { get; set; }
            public string retailer { get; set; }
            public double max_price { get; set; }
            public AutoRetrySettings auto_retry_settings { get; set; }
            public List<object> scheduled_delivery_windows { get; set; }
            public List<object> free_gifts { get; set; }
            public List<object> bundled_order_ids { get; set; }
            public List<object> promo_codes { get; set; }
            public List<Product> products { get; set; }
            public bool is_gift { get; set; }
            public ShippingAddress shipping_address { get; set; }
            public DateTime _created_at { get; set; }
            public DateTime _finalized_at { get; set; }
        }

        public class Data2
        {
            public string details { get; set; }
        }

        public class Data
        {
            public string _type { get; set; }
            public string code { get; set; }
            public Data2 data { get; set; }
            public string request_id { get; set; }
        }

        public class RootObject
        {
            public string _type { get; set; }
            public Request request { get; set; }
            public string code { get; set; }
            public Data data { get; set; }
            public string host { get; set; }
            public DateTime _created_at { get; set; }
            public string request_id { get; set; }
            public List<object> status_updates { get; set; }
        }
    }
}
