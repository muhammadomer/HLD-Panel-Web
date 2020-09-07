using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ShipmentViewHeaderViewModel
    {
        public string ShipingCompany { get; set; }
        public string ShipmentId { get; set; }
        public string ShipmentName { get; set; }
        public int VendorId { get; set; }
        //public string VendorId { get; set; }
        public string Vendor { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Boxes { get; set; }
        public int SKUs { get; set; }
        public int POs { get; set; }
        public int Status { get; set; }
        public decimal ShipedAmountUSD { get; set; }
        public decimal ReceivedAmountUSD { get; set; }
        public decimal ShipedAmountCNY { get; set; }
        public decimal ReceivedAmountCNY { get; set; }
        public string TrakingNumber { get; set; }
        public string CourierCode { get; set; }
        public int TotalShipedQty { get; set; }
        public int TotalReceivedQty { get; set; }
        public int TotalOrderedQty { get; set; }
        public int TotalOpenQty { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public decimal GrossWt { get; set; }
        public decimal CBM { get; set; }
        public List<ShipmentViewProducListViewModel> List { get; set; }
    }
    public class ShipmentViewProducListViewModel
    {
        public int idShipmentProducts { get; set; }
        public int SCID { get; set; }
        public string BoxId { get; set; }
        public string CompressedImage { get; set; }
        public string ImageName { get; set; }
        public string SKU { get; set; }
        public int BalanceQty { get; set; }
        public int POId { get; set; }
        public int OpenQty { get; set; }
        public int ShipedQty { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReceivedQty { get; set; }
        public int OrderedQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitPriceUSD { get; set; }
        public int CurrencyCode { get; set; }
        public List<Boxes> Boxes { get; set; }
        public int BoxNo { get; set; }
        public int NoOfBoxes { get; set; }
        public string TrakingNumber { get; set; }
        public string CourierCode { get; set; }
        public string LocationNotes { get; set; }
        public string ShadowOf { get; set; }
        public string PhysicalInventory { get; set; }
        public int QtyPerCase { get; set; }

        public int QtyPerBox { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
    }

    public class Boxes
    {
        public string BoxId { get; set; }
        public int BoxNo { get; set; }
        public int SKUShipedQty { get; set; }

    }

    public class SearchViewModelShipment
    {
        public string ShipmentId { get; set; }
        public int POID { get; set; }
        public string SKU { get; set; }
        public string Title { get; set; }
        public int ReceivedQty { get; set; }
        public int OrderedQty { get; set; }
        string ItemType { get; set; }
        public int OpenQty { get; set; }
        public int ShipedQty { get; set; }
        public ShipmentViewHeaderViewModel Item { get; set; }
    }


}

