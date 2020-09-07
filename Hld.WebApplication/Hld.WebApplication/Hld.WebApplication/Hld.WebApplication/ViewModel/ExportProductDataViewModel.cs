using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ExportProductDataViewModel
    {
        public string ProductSKU { get; set; }
        public int best_buy_product_id { get; set; }

        public bool dropship_status { get; set; }
        public int HLD_CA1 { get; set; }
        public string ImageURL { get; set; }

    }
}
