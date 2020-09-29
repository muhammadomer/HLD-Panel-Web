﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class GetSendToZincOrderViewModel
    {
        public int OrderId { get; set; }
        public string order_type { get; set; }
        public string amazon_tracking { get; set; }
        public string _tracking { get; set; }
        public string carrier { get; set; }
        public string shpping_date { get; set; }
        public string merchant_order_id { get; set; }
        public string RequestId { get; set; }
        public string Asin { get; set; }
        public string Sku { get; set; }
        public string AccountDetail { get; set; }
        public string name_on_card { get; set; }
        public string UserName { get; set; }

        public DateTime Date { get; set; }
        public string TrackingNumber { get; set; }
        public string Response { get; set; }
        public string CompressedImage { get; set; }
        public string ImageName { get; set; }
        public DateTime LastUpdate { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}
