using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class GetSKUAndASINViewModel
    {
        public string SKU { get; set; }
        public string ASIN { get; set; }
        public decimal max_price { get; set; }
    }
}
