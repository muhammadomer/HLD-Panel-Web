using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class WatchListSummaryViewModel
    {
        public int Records { get; set; }
        public List<ZincWatchListSummaryViewModal> zincWatchList { get; set; }
    }
}
