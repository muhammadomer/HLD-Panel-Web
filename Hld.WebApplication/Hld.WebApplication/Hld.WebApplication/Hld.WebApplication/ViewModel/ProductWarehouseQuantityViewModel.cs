using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ProductWarehouseQuantityViewModel
    {
        public string ProductID { get; set; }
        public int AvailableQty { get; set; }
        public int PhysicalQty { get; set; }
        public int WarehouseID { get; set; }
    }
}
