using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ApprovedSearchViewModel
    {
        public int? Vendor { get; set; }
        public string VendorAlias { get; set; }
        public string SKU { get; set; }
        public string Title { get; set; }
        public string SearchFromSkuList { get; set; }

    }
}
