using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincWatchlistCountViewModel
    {
        public int Available { get; set; }
        public int UnAvailable { get; set; }
        public int ListingRemoved { get; set; }
        public int TotalCount { get; set; }
        public int Total{ get; set; }
        public int ErrorCount { get; set; }
    }
}
