﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ChildDataViewModel
    {
        public int product_id { get; set; }
        public int ColorIds { get; set; }
        public string Sku { get; set; }
        public string title { get; set; }
        public string upc { get; set; }
        public bool productstatus { get; set; }
    }
}