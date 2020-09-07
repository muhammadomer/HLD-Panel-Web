using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ASINSKUMappingViewModel
    {
        public string ASIN { get; set; }
        public string SKU { get; set; }
        public decimal? MAXPrice { get; set; }
        public decimal? AmzPrice { get; set; }
        public string Title { get; set; }
    }
}
