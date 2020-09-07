using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class POItemRecivedViewModel
    {
        public string ReceiveInvoiceNumber { get; set; }
        public string VendorOrderId { get; set; }
        public IList<Items> Items { get; set; }
    }
    public class Items
    {
        public int ID { get; set; }
        public int QtyToReceive { get; set; }
    }
}
