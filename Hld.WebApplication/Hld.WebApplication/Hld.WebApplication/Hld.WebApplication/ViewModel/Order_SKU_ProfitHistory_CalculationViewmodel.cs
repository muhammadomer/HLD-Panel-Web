using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class Order_SKU_ProfitHistory_CalculationViewmodel
    {
        public decimal TaxesPercentage { get; set; }
        public decimal Taxes { get; set; }
        public decimal Percentage { get; set; }
        public decimal SellingFeePercentage { get; set; }
        public decimal SellingFee { get; set; }
        public decimal GrossRevnue { get; set; }
        public decimal ProfitPercentage { get; set; }
        public decimal Profit { get; set; }

        public string TaxesPercentage_str { get; set; }
        public string Taxes_str { get; set; }
        public string Percentage_str { get; set; }
        public string SellingFeePercentage_str { get; set; }
        public string SellingFee_str { get; set; }
        public string GrossRevnue_str { get; set; }
        public string ProfitPercentage_str { get; set; }
        public string Profit_str { get; set; }
        public string UnitsSold_str { get; set; }
        public string OrderQuantity_str { get; set; }
        public string ItemCost_str { get; set; }

        public int UnitsSold { get; set; }
        public int OrderQuantity { get; set; }
        public decimal ItemCost { get; set; }
        public string SKU { get; set; }
        public string Duration { get; set; }
        public DateTime? DateTime { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
