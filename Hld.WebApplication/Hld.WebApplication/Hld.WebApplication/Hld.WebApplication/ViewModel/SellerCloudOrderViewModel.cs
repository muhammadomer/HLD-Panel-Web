using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class SellerCloudOrderViewModel
    {
        public int totalCount { get; set; }
        public string dropShipStatus { get; set; }
        public decimal currencyRateFromUSD { get; set; }
        public DateTime lastUpdate { get; set; }
        public DateTime timeOfOrder { get; set; }
        public decimal taxTotal { get; set; }
        public string shippingStatus { get; set; }
        public int shippingWeightTotalOz { get; set; }
        public string orderCurrencyCode { get; set; }
        public string orderSourceOrderId { get; set; }
        public DateTime paymentDate { get; set; } //Order Date in our case
        public int sellerCloudID { get; set; }   //seller cloud order id
        public int ClientID { get; set; }
    }
}
