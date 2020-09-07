using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class WareHouseProductQuantitylistViewModel
    {
        public string warehouseName { get; set; }
        public string sku { get; set; }
        public int AvailableQty { get; set; }
        public int physical_qty { get; set; }
        public int warehouseId { get; set; }
    }
}
