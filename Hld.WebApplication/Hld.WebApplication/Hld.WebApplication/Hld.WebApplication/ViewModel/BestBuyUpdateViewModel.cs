using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class BestBuyUpdateViewModel
    {
        public int JobID { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public int TotalProduct { get; set; }
    }
}
