using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class BestBuyUpdateLogsViewModel
    {
        public string SKU { get; set; }
        public int ProductId { get; set; }
        public decimal UpdateSelllingPrice { get; set; }
        public decimal MSRP { get; set; }
        public string importId { get; set; }
        public int JobId { get; set; }
        public string Compress_image { get; set; }
        public string image_name { get; set; }
    }
}
