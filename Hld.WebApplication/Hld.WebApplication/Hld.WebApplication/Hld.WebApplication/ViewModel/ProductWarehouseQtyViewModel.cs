using Hld.WebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ProductWarehouseQtyViewModel
    {
        public int ProductWarehouseID { get; set; }
        public int WarehouseID { get; set; }
        public int AvailableQty { get; set; }
        public string ProductSku { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseGroup { get; set; }
        public string WarehouseQtyString { get; set; }
        public string LocationNotes { get; set; }
        public decimal OnOrder { get; set; }
        public List<ProductStatusViewModel> ProductStatus { get; set; }
        public List<ApprovedPriceViewModel> approvedPrices { get; set; }
    }
}
