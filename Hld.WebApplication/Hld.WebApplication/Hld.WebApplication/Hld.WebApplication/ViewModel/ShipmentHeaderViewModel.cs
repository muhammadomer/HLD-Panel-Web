using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModels
{
    public class ShipmentHeaderViewModel
    {
        public string ShipmentId { get; set; }
        public int VendorId { get; set; }
        public string Vendor { get; set; }
        public string ShipmentName { get; set; }

        public string Notes { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ShipmentAutoID { get; set; }
        public List<ShipmentBoxListViewModel> List { get; set; }
    }
}

