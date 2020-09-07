using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincSumbitOrderSuccessResponse
    {
        public class Product
        {
            public string product_id { get; set; }
            public int quantity { get; set; }
        }

        public class DeliveryDate
        {
            public List<Product> products { get; set; }
            public string delivery_date { get; set; }
        }

        public class ProductSubtotals
        {
            public int B01MA3RYJ9 { get; set; }
        }

        public class PriceComponents
        {
            public int shipping { get; set; }
            public int tax { get; set; }
            public int gift_certificate { get; set; }
            public ProductSubtotals product_subtotals { get; set; }
            public int total { get; set; }
            public int subtotal { get; set; }
        }

        public class PriceComponents2
        {
            public int subtotal { get; set; }
            public int shipping { get; set; }
            public int total { get; set; }
        }

        public class Data
        {
            public bool success { get; set; }
            public string merchant_order_id { get; set; }
            public string email_id { get; set; }
            public string email_first_line { get; set; }
            public DateTime? delivery_date { get; set; }
            public string shipping_speed { get; set; }
            public string shipping_address { get; set; }
            public PriceComponents2 price_components { get; set; }
        }

        public class StatusUpdate
        {
            public string message { get; set; }
            public string type { get; set; }
            public Data data { get; set; }
            public DateTime _created_at { get; set; }
        }

        public class Product2
        {
            public string seller_id { get; set; }
            public string product_id { get; set; }
        }

        public class MerchantOrderId
        {
            public string merchant { get; set; }
            public DateTime placed_at { get; set; }
            public List<Product2> products { get; set; }
            public List<string> product_ids { get; set; }
            public string merchant_order_id { get; set; }
            public string shipping_address { get; set; }
            public string tracking_url { get; set; }
            public string delivery_date { get; set; }
        }

        public class DeliveryDate2
        {
            public string month { get; set; }
            public string promise { get; set; }
            public string human { get; set; }
            public string year { get; set; }
            public object promise_quality { get; set; }
            public string day { get; set; }
        }

        public class CheckoutItem
        {
            public string seller_id { get; set; }
            public string product_id { get; set; }
            public bool has_prime_icon { get; set; }
            public string title { get; set; }
            public string line_item_id { get; set; }
            public DeliveryDate2 delivery_date { get; set; }
            public int quantity { get; set; }
        }

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

        public class Product3
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
            public List<Product3> products { get; set; }
            public bool is_gift { get; set; }
            public ShippingAddress shipping_address { get; set; }
            public DateTime _created_at { get; set; }
            public string environment { get; set; }
            public string git_version { get; set; }
            public string server_name { get; set; }
            public DateTime _finalized_at { get; set; }
        }

        public class RootObject
        {
            public double gift_balance_time { get; set; }
            public string _type { get; set; }
            public List<DeliveryDate> delivery_dates { get; set; }
            public PriceComponents price_components { get; set; }
            public string _driver { get; set; }
            public List<StatusUpdate> status_updates { get; set; }
            public DateTime _created_at { get; set; }
            public double prior_gift_balance_time { get; set; }
            public List<MerchantOrderId> merchant_order_ids { get; set; }
            public string request_id { get; set; }
            public List<string> offers_urls { get; set; }
            public List<CheckoutItem> _checkout_items { get; set; }
            public List<string> screenshot_urls { get; set; }
            public Request request { get; set; }
        }
    }
}
