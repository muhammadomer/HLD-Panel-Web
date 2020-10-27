using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class CreateProductOnSallerCloudViewModel
    {
        public int CompanyId { get; set; }
        public string ProductName { get; set; }
        public string ProductSKU { get; set; }
        public string ProductTypeName { get; set; }
        public int PurchaserId { get; set; }
        public int SiteCost { get; set; }
        public decimal DefaultPrice { get; set; }
        public int ManufacturerId { get; set; }
        public bool AutoAssignUPC { get; set; }
        public string ProductNotes { get; set; }
    }
}
