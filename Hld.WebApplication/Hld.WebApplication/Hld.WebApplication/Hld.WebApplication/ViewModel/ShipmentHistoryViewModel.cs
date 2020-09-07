using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ShipmentHistoryViewModel
    {
        public string ShipmentId { get; set; }
        public int VendorId { get; set; }
        public string Vendor { get; set; }
        public string CompressedImage { get; set; }
        public string ImageName { get; set; }
        public string SKU { get; set; }
        public int ShipedQty { get; set; }
        public string Title { get; set; }
        public int ReceivedQty { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public int POId { get; set; }
        //public DateTime CreatedOn { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<POIDs> POs { get; set; }
    }

    public class POIDs
    {
        public int POId { get; set; }
        public int ShipedQty { get; set; }
    }

    public class ShipmentHistorySearchViewModel
    {
        public int VendorId { get; set; }
        public string ShipmentId { get; set; }
        public string Vendor { get; set; }
        public string SKU { get; set; }
        public string Title { get; set; }
        public string EmptyFirstTime { get; set; }
        public string Status { get; set; }
        public string ItemType { get; set; }
        public DateTime orderDateTimeFrom { get; set; }
        public DateTime orderDateTimeTo { get; set; }
    }

}
