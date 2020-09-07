using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincWatchLogsSearchViewModel
    {
        public string ASIN { get; set; }
        public string JobID { get; set; }
        public string Available { get; set; }
        public int Offset { get; set; }
    }
}
