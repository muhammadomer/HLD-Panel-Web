using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class WarehouseViewModel
    {
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public int SCWarehouseID { get; set; }
        public bool Status { get; set; }
    }
}
