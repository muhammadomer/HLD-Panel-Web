using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincGetStatusFromZincViewModel
    {
        public string SKU { get; set; }
        public string ASIN { get; set; }
        public int TotalCount { get; set; }
        public List<ZincGetStatusFromZincViewModelResponce> SKUlist { get; set; }

    }
    public class ZincGetStatusFromZincViewModelResponce
    {
        public string SKU { get; set; }
    }
  
}
