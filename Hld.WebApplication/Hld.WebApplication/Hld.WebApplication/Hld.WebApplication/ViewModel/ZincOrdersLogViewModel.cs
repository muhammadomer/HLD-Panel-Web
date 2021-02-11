using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincOrdersLogViewModel
    {
        public string SC_Order_ID { get; set; }
        public string BB_Order_ID { get; set; }
        public string Zinc_Order_ID { get; set; }
        public string Zinc_Status { get; set; }
        public DateTime orderDateTimeFrom { get; set; }
        public DateTime orderDateTimeTo { get; set; }
        public DateTime order_datetime { get; set; }
        public string Amazon_AcName { get; set; }     
        public string EmptyFirstTime { get; set; }
        public string order_message { get; set; }
    }
}
