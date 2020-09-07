using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class DraftPOViewModel
    {
        public int InternalPOId { get; set; }
        public int VendorId { get; set; }
        public int SKUs { get; set; }
        public DateTime OrderedOn { get; set; }
        public string Notes { get; set; }
        public string Vendor { get; set; }
    }
}
