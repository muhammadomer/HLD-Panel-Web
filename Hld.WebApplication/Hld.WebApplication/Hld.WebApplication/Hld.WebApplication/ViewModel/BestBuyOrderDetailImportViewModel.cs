using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class BestBuyOrderDetailImportViewModel
    {
        public string order_line_id { get; set; }
        public string offer_sku { get; set; }
        public string quantity { get; set; }
        public double total_priceOrerLine { get; set; }
        public double total_commissionOrderLine { get; set; }
        public string order_line_state { get; set; }
        public DateTime? received_date { get; set; }
        public DateTime? shipped_date { get; set; }
        public string product_title { get; set; }
        public double GST { get; set; }
        public double PST { get; set; }
    }
}
