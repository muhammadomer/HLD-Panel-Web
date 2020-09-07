using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class HldPanelViewModel
    {
        public string customer_firstName { get; set; }
        public string customer_lastName { get; set; }
        public string customer_city { get; set; }
        public string customer_country { get; set; }
        public string order_orderId { get; set; }
        public DateTime? order_acceptanceDecisionDate { get; set; }
        public DateTime? order_createdDate { get; set; }
        public string order_sellerCloudID { get; set; }
        public string orderLine_offer_sku { get; set; }
        public string orderLine_product_title { get; set; }
        public string orderLine_shipped_date { get; set; }
        public string orderLine_received_date { get; set; }
        public string orderLine_total_commission { get; set; }
        public string orderLine_total_price { get; set; }
        public string orderLine_order_line_state { get; set; }
        public string shipping_shippingCompany { get; set; }
        public string shipping_shippingTracking { get; set; }
    }
   
}
