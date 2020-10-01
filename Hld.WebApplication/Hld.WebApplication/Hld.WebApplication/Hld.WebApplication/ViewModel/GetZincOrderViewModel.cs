using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class GetZincOrderViewModel
    {
        public int OrderId { get; set; }
        public string SKU { get; set; }
        public string ASIN { get; set; }
        public decimal max_price { get; set; }
        public int RecivedOrderQty { get; set; }
        public DateTime RecievedOrderDate { get; set; }
    }
}
