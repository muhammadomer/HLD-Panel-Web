using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class UpdateSCDropshipStatusViewModel
    {
        public int SCOrderID { get; set; }
        public string StatusName { get; set; }
        public DateTime LogDate { get; set; }
        public bool IsTrackingUpdate { get; set; } = false;
    }
}
