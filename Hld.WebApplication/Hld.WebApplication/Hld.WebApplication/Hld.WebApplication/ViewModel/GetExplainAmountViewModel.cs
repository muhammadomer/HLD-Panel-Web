using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class GetExplainAmountViewModel
    {
        public int quantity { get; set; }
        public decimal unitprice { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal avg_cost { get; set; }
        public decimal total_commission { get; set; }
    }
}
