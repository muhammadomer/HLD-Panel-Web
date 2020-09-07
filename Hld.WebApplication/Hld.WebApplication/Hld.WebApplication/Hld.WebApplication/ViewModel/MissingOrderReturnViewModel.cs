using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class MissingOrderReturnViewModel
    {
        public int MissingOrderCount { get; set; }
        public int ExistingOrderCount { get; set; }
        public List<int> ExistingOrder { get; set; }
        public List<int> MissingOrder { get; set; }
    }
}
