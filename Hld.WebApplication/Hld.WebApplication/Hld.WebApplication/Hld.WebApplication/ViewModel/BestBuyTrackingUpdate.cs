using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class BestBuyTrackingUpdate
    {
        public string scOrderID { get; set; }
        public string bbOrderID { get; set; }
        public DateTime shipDate { get; set; }
        public DateTime orderDateTimeFrom { get; set; }
        public DateTime orderDateTimeTo { get; set; }
        public string shippingServiceCode { get; set; }
        public string trackingNumber { get; set; }
        public string EmptyFirstTime { get; set; }
        public string BBStatus { get; set; }
        public int inBestbuy { get; set; }
        public double TotalCount { get; set; }
    }
}
