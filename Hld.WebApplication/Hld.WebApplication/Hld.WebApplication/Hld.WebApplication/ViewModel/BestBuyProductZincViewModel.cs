using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class BestBuyProductZincViewModel
    {
        public int BestBuyProductZincID { get; set; }
        public string ZincAsinCA { get; set; }
        public int Priority { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal AmazonPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Status { get; set; }
        public bool Prime { get; set; }
        public decimal Reviews { get; set; }
        public decimal Feedback { get; set; }
        public string ProductSKU { get; set; }
    }
}
