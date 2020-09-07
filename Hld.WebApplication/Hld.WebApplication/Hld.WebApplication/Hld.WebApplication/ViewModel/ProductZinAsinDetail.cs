using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ProductZinAsinDetail
    {
        public int bb_product_zinc_id { get; set; }
        public string max_price { get; set; }
        public string amazon_price { get; set; }
        public bool prime { get; set; }
        public int priority { get; set; }
        public string ProductSku { get; set; }
        public string ASIN { get; set; }
        public string OrderId { get; set; }
    }
}
