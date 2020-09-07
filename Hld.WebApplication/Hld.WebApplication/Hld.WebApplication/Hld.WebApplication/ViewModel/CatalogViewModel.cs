using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    //public class CatalogViewModel
    //{

    //}
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CatalogViewModel
    {
        public string ImageUrl { get; set; }
        public double Weight { get; set; }
        public double ShippingWeight { get; set; }
        public double WeightOz { get; set; }
        public double PackageWeightLbs { get; set; }
        public double PackageWeightOz { get; set; }
        public string LocationNotes { get; set; }
        public string eBayCategory { get; set; }
        public string eBayStoreCategory { get; set; }
        public string eBayItemCondition { get; set; }
        public double MAPPrice { get; set; }
        public string ShadowOf { get; set; }
        public double AverageCost { get; set; }
        public int Priority { get; set; }
        public int Rating { get; set; }
        public string ProductName { get; set; }
        public string Buyer { get; set; }
        public string ASIN { get; set; }
        public int QtySold15 { get; set; }
        public int QtySold30 { get; set; }
        public int QtySold60 { get; set; }
        public int QtySold90 { get; set; }
        public int QtySoldYTD { get; set; }
        public bool IsEndOfLife { get; set; }
        public double ShippingCost { get; set; }
        public int AggregatePhysicalQtyFBA { get; set; }
        public int AggregatePhysicalQty { get; set; }
        public int AggregateQty { get; set; }
        public string ProductType { get; set; }
        public string DefaultVendor { get; set; }
        public int HomeCountryCode { get; set; }
        public string Country { get; set; }
        public string AmazonMarketPlaceID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNameAbbreviation { get; set; }
        public object FirstKitChild { get; set; }
        public int WarehousePhysicalQty { get; set; }
        public double WarehousePhysicalQtyValue { get; set; }
        public double WarehouseReservedQtyValue { get; set; }
        public int AggregatedQty { get; set; }
        public double AggregatePhysicalSellableQtyIncludingPhysicalValue { get; set; }
        public int ReservedQty { get; set; }
        public double ReserveQtyTotalValue { get; set; }
        public int PhysicalQty { get; set; }
        public int AggregateNonSellableQty { get; set; }
        public double StorePrice { get; set; }
        public string ManufacturerSKU { get; set; }
        public string ASINInActiveListing { get; set; }
        public double WeightLbs { get; set; }
        public string UPC { get; set; }
        public string AmazonFBASKU { get; set; }
        public int FulfilledBy { get; set; }
        public int SalesRank { get; set; }
        public int AmazonProductCondition { get; set; }
        public string AmazonCondition { get; set; }
        public object MainProductID { get; set; }
        public int FixedPriceQuantity { get; set; }
        public double BuyItNowPrice { get; set; }
        public object ShippingTemplateID { get; set; }
        public object eBayCategory1ID { get; set; }
        public object eBayCategory1Name { get; set; }
        public string eBayTopTitle { get; set; }
        public int ProductTitleCount { get; set; }
        public string DescriptionTemplateID { get; set; }
        public string ProductConditionName { get; set; }
        public int NotesCount { get; set; }
        public int OrderReserveTotal { get; set; }
        public object VendorOfProduct { get; set; }
        public double VendorPrice { get; set; }
        public int ReplacementQty { get; set; }
        public string WarehouseName { get; set; }
        public bool IsSellAble { get; set; }
        public int WarehouseCount { get; set; }
        public int CompanyID { get; set; }
        public List<int> EnabledOnChannels { get; set; }
        public bool WebEnabled { get; set; }
        public bool BuyDotComEnabled { get; set; }
        public bool MagentoEnabled { get; set; }
        public bool SearsEnabled { get; set; }
        public bool PriceGrabberEnabled { get; set; }
        public bool OverStockEnabled { get; set; }
        public bool PriceFallEnabled { get; set; }
        public bool UnbeatableSalesEnabled { get; set; }
        public bool eBayEnabled { get; set; }
        public bool AmazonEnabled { get; set; }
        public bool WayfairEnabled { get; set; }
        public bool VendorCentralEnabled { get; set; }
        public bool BonanzaEnabled { get; set; }
        public bool SmartBargainsEnabled { get; set; }
        public bool ATGStoresEnabled { get; set; }
        public bool BestBuyEnabled { get; set; }
        public bool KohlsEnabled { get; set; }
        public bool HomeDepotEnabled { get; set; }
        public bool StaplesEnabled { get; set; }
        public bool MeijerEnabled { get; set; }
        public bool OneStopPlusEnabled { get; set; }
        public bool SonsiEnabled { get; set; }
        public bool KMartEnabled { get; set; }
        public bool ElevenMainEnabled { get; set; }
        public bool GroupOnGoodsEnabled { get; set; }
        public bool GrouponMarketplaceEnabled { get; set; }
        public bool WalmartEnabled { get; set; }
        public bool WalmartAPIEnabled { get; set; }
        public bool WishEnabled { get; set; }
        public bool JETEnabled { get; set; }
        public bool EtsyEnabled { get; set; }
        public bool FingerHutEnabled { get; set; }
        public bool TopHatterEnabled { get; set; }
        public bool TangaEnabled { get; set; }
        public bool TargetEnabled { get; set; }
        public bool NewEggDotComEnabled { get; set; }
        public bool NewEggBizEnabled { get; set; }
        public bool YahooStoreEnabled { get; set; }
        public bool HayneedleEnabled { get; set; }
        public bool ReverbEnabled { get; set; }
        public bool DropShipCentralEnabled { get; set; }
        public bool DrugStoresEnabled { get; set; }
        public bool CdiscountEnabled { get; set; }
        public bool BedBathAndBeyondEnabled { get; set; }
        public bool ShopHQEnabled { get; set; }
        public bool GoogleShoppingEnabled { get; set; }
        public bool IsMatrixParent { get; set; }
        public int InventoryDependantOption { get; set; }
        public int OnOrder { get; set; }
        public double AmazonPrice { get; set; }
        public int VendorID { get; set; }
        public DateTime LastAggregateDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string AmazonMarketplace { get; set; }
        public int OnBackOrder { get; set; }
        public bool BackOrderVisible { get; set; }
        public double WholeSalePrice { get; set; }
        public bool WholeSalePriceVisible { get; set; }
        public double SitePrice { get; set; }
        public double SiteCost { get; set; }
        public bool SiteCostVisible { get; set; }
        public double LastCost { get; set; }
        public bool LastCostVisible { get; set; }
        public int InventoryAvailableQty { get; set; }
        public List<object> ChildProducts { get; set; }
        public string ID { get; set; }

    }

    public class Root
    {
        public List<CatalogViewModel> Items { get; set; }
        public int TotalResults { get; set; }
    }


}
