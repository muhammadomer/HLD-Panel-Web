using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class SaveWatchlistViewModel
    {
        public int frequency { get; set; }
        public int CheckafterDays { get; set; }
        public string ASIN { get; set; }
        public string ProductSKU { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public DateTime? NextUpdateDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
