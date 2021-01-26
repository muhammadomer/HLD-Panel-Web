using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class PurchaseOrdersViewModel
    {
        public int POId { get; set; }
        public int CompanyId { get; set; }
        public int VendorId { get; set; }
        public int SKUCount { get; set; }
        public int OrderQty { get; set; }
        public int recQty { get; set; }
        public int QtyOpen { get; set; }
        public DateTime OrderedOn { get; set; }
        public string POAcceptanceDate { get; set; }
        public string POLastUpdate { get; set; }
        public int DefaultWarehouseID { get; set; }
        public int POStatus { get; set; }
        public string Vendor { get; set; }
        public int CurrencyCode { get; set; }
        public decimal AmountOrdered { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal AmountOpen { get; set; }
        public int ShippedQty { get; set; }
        public decimal ShippedAmount { get; set; }
    }
    public class PurchaseOrderModel
    {
        public string SKU { get; set; }
        public string Title { get; set; }
        public Boolean OpenQty { get; set; }
        public int VendorId { get; set; }
        public string Vendor { get; set; }
        public int POId { get; set; }
        public int CurrencyCode { get; set; }
        public int IsAccepted { get; set; }
        public int POStatus { get; set; }
        public string POAcceptanceDate { get; set; }
        public string POLastUpdate { get; set; }
        public string OrderedOn { get; set; }
        public string POArrivalDate { get; set; }
        public string Notes { get; set; }
        public decimal ExchangeRate { get; set; }
        public List<PurchaseOrderItemsViewModel> ListItems { get; set; }
    }
    public class PurchaseOrderItemsViewModel
    {

        public int ID { get; set; }
        public int idPurchaseOrdersItems { get; set; }
        public int? PurchaseID { get; set; }
        public string EmptyFirstTime { get; set; }
        public int? VendorID { get; set; }
        public string ProductID { get; set; }
        public string ProductTitle { get; set; }
        public int InternalPOId { get; set; }
        public string Currency { get; set; }
        public string Compress_image { get; set; }
        public string Vendor { get; set; }

        public string image_name { get; set; }
        public int QtyOrdered { get; set; }
        public int QtyReceived { get; set; }
        public int QtyOnHand { get; set; }
        public int SkuStatus { get; set; }
        public long OpenQty { get; set; }
        public int QtyOpen { get; set; }
        public int CurrencyCode { get; set; }
        public decimal OrderedAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal OpenAmount { get; set; }

        public decimal UnitPrice { get; set; }
        public DateTime OrderedOn { get; set; }
        public decimal UnitPriceUSD { get; set; }

        public int ShippedQty { get; set; }
        public decimal ShippedAmount { get; set; }

    }
    public class searchPOViewModel
    {

        public int startLimit { get; set; }
        public int endLimit { get; set; }
        public int vendorID { get; set; }
    }
    public class searchPOitemViewModel
    {

        public int POId { get; set; }
        public int vendorID { get; set; }
        public string SKU { get; set; }
        public string Title { get; set; }
        public Boolean OpenQty { get; set; }

    }
}
