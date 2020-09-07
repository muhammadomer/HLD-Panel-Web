﻿using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModels
{
    public class ShipmentBoxViewModel
    {
        public string ShipmentId { get; set; }
        public string BoxId { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
        public int IdShipmentBox { get; set; }
    }
    public class ShipmentBoxViewModeldata
    {
        public List<ShipmentBoxViewModel> data { get; set; }
    }

}
