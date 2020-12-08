
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class BestBuyOrdersViewModel
    {
        public DateTime? OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public String SellerCloudOrderID { get; set; }
        public string ShipmentAddress { get; set; }
        public int TotalQuantity { get; set; }
        public string EmptyFirstTime { get; set; }
        public string ParentOrderID { get; set; }
        public string IsParent { get; set; }
        public string IsNotes { get; set; }
        public string ShippingPrice { get; set; }
        public int OnOrder { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalComission { get; set; }
        public decimal ProfitAndLossInDollar { get; set; }
        public decimal ProfitAndLossInPercentage { get; set; }
        public decimal TotalAverageCost { get; set; }
        public int ZincAccountId { get; set; }
        public int CreditCardId { get; set; }

        public BestBuyOrderSearchTotalCountViewModel searchViewModel { get; set; }
        public List<BestBuyOrderDetailViewModel> BBProductDetail { get; set; }


    }
}
