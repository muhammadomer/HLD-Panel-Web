using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ShipmentProductInventroyViewModel
    {
        public General General { get; set; }
        public Inventory Inventory { get; set; }
    }
    public class General
    {
        public int ProductTypeID { get; set; }
        public string LocationNotes { get; set; }
        public string ShadowOf { get; set; }

    }
    public class Inventory
    {
        public string PhysicalInventory { get; set; }
        public int QtyPerCase { get; set; }

    }

}
