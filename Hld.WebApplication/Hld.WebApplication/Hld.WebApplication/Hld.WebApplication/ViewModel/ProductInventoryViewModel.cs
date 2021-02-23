using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ProductInventorySearchViewModel
    {
        public string dropshipstatus { get; set; }
        public string dropshipstatusSearch { get; set; }
        public string DSTag { get; set; }
        public string WHQStatus { get; set; }
        public string Sku { get; set; }
        [Required(ErrorMessage = "Please Enter Sku List")]
        public string SearchFromSkuList { get; set; }
        public string asin { get; set; }
        public string Producttitle { get; set; }
        public string TypeSearch { get; set; }
    }
}
