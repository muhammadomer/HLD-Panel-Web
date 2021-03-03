using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ProductDisplayInventoryViewModel
    {
        public int ProductId { get; set; }
        public string ProductSKU { get; set; }
        public string ProductTitle { get; set; }

        public string ColorName { get; set; }

        public string AvgCost { get; set; }
        public string ShipmentWeight { get; set; }
        public string ShipmentLWH { get; set; }
        public byte[] Image { get; set; }
        public bool Continue { get; set; }
        public decimal BBMSRP { get; set; }
        public decimal BBSellingPrice { get; set; }
        public string BestBuyProductSKU { get; set; }
        public string ImageURL { get; set; }
        public bool dropship_status { get; set; }
        public int dropship_Qty { get; set; }
        public int BBProductID { get; set; }
        public int AggregatedQty { get; set; }
        public decimal ApprovedUnitPrice { get; set; }
        public bool Remark { get; set; }

        public List<ProductWarehouseQtyViewModel> ProductrWarehouseQtyViewModel { get; set; }
        public List<SkuTagOrderViewModel> skuTags { get; set; }
        public List<ApprovedPriceForInventoryViewModel> ApprovedPriceList { get; set; }
    }
    //public class ApprovedPriceViewModel
    //{
    //    public int idApprovedPrice { get; set; }
    //    public string SKU { get; set; }
    //    public string ProductTitle { get; set; }
    //    public int? VendorId { get; set; }
    //    public string VendorAlias { get; set; }
    //    public decimal ApprovedUnitPrice { get; set; }
    //    public decimal CAD { get; set; }
    //    public decimal USD { get; set; }
    //    public decimal YEN { get; set; }
    //    public string Currency { get; set; }
    //    public bool PriceStatus { get; set; }
    //    public bool History { get; set; }
    //    public DateTime Date { get; set; }
    //    public string ImageName { get; set; }
    //    public string CompressedImage { get; set; }
    //    public string HasNotes { get; set; }
    //}

}
