using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ZincOrderLogViewModel
    {
        public int ZincOrderLogID { get; set; }
        public string SellerCloudOrderId { get; set; }
        public string RequestIDOfZincOrder { get; set; }
        public DateTime OrderDatetime { get; set; }
    }
}
