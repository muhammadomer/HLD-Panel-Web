using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class BestBuyOrdersViewPageModel
    {
        public DateTime? OrderDate { get; set; }
        public String SellerCloudOrderID { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string ParentOrderID { get; set; }
        public string IsParent { get; set; }
        public string IsNotes { get; set; }
        public string ShippingPrice { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string tracking_number { get; set; }
        public string Country { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalComission { get; set; }
        public decimal ProfitAndLossInDollar { get; set; }
        public decimal ProfitAndLossInPercentage { get; set; }
        public decimal TotalAverageCost { get; set; }
        public decimal ApprovedUnitPriceCAD { get; set; }

        public string PaymentStatus { get; set; }
        public int ZincAccountId { get; set; }
        public int CreditCardId { get; set; }
        public string ZincAccountName { get; set; }
        public string CreditCardName { get; set; }
        public List<BestBuyOrderDetailViewModel> BBProductDetail { get; set; }

    }
}
