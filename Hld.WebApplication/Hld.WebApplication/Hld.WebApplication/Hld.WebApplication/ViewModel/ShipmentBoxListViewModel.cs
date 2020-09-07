﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModels
{
    public class ShipmentBoxListViewModel
    {
        public string ShipmentId { get; set; }
        public string BoxId { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
        public int IdShipmentBox { get; set; }
        public int TotalShipedQty { get; set; }
        public int TotalSKUs { get; set; }
        public int TotalPOs { get; set; }
    }
}
