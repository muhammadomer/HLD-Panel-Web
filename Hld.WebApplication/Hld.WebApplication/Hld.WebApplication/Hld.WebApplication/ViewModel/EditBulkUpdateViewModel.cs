﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class EditBulkUpdateViewModel
    {
        public string AmazonMerchantSKU { get; set; }
        public string AmazonEnabled { get; set; }
        public string ASIN { get; set; }
        public string FulfilledBy { get; set; }
        public string AmazonFBASKU { get; set; }
        public string WebsiteEnabled { get; set; }
        public string ChildSKU { get; set; }
        public string ShadowOffSKU { get; set; }
    }
}
