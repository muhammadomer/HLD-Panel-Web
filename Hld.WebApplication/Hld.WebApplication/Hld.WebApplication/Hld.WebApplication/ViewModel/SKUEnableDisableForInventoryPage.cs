using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    /// <summary>
    /// this class is used from inventory page to bulk update the sku status 
    /// </summary>
    public class SKUEnableDisableForInventoryPage
    {
        public bool EnableOrDisableDropship { get; set; }
        public int totalSkuSelected { get; set; }
        public int DropshipQty { get; set; }
        public string Comments { get; set; }

    }
}
