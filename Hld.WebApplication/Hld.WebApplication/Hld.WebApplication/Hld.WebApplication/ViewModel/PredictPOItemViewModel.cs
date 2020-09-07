using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class PredictPOItemViewModel
    {

        public int ID { get; set; }
        public int idPurchaseOrdersItems { get; set; }

        public int VendorID { get; set; }
        public string ProductID { get; set; }

        public int InternalPOId { get; set; }
        public int POId { get; set; }
        public string Currency { get; set; }
        public string Compress_image { get; set; }
        public string Vendor { get; set; }

        public string image_name { get; set; }
        public int QtyOrdered { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
