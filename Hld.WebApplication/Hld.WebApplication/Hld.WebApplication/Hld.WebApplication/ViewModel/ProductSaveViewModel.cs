﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ProductSaveViewModel
    {
        public string SKU { get; set; }
        public string LocationNotes { get; set; }
        public string ImageUrl { get; set; }
        public string ProductName { get; set; }
        public string ShadowOf { get; set; }
        public int QtySold15 { get; set; }
        public int QtySold30 { get; set; }
        public int QtySold60 { get; set; }
        public int QtySold90 { get; set; }
        public int QtySold180 { get; set; }
        public int QtySold365 { get; set; }
        public int QtySoldYTD { get; set; }
        public int AggregatePhysicalQty { get; set; }
        public int PhysicalQty { get; set; }
        public int AggregateQty { get; set; }
        public int ReservedQty { get; set; }
        public int OnOrder { get; set; }
        public bool IsEndOfLife { get; set; }
        public int AggregateNonSellableQty { get; set; }
        public string ASINInActiveListing { get; set; }
        public string AmazonFBASKU { get; set; }
        public string PhysicalInventory { get; set; }
        public string ProductType { get; set; }
        public string UPC { get; set; }
    }
    public class ProductSaveListViewModel
    {
        public List<ProductSaveViewModel> Items { get; set; }
        public int TotalResults { get; set; }
    }
}
