using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincWatchListSummaryViewModal
    {
        public int JobID { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public decimal Total_ASIN { get; set; }
        public decimal Available { get; set; }
        public int Prime { get; set; }
        public int NoPrime { get; set; }
        public int Unavailable { get; set; }
        public string Status { get; set; }
    }
}
