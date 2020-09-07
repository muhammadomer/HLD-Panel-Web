using DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class CasePackSearchViewModel
    {
        public string SKU { get; set; }
        public string Title { get; set; }
        public int VendorId { get; set; }
        public string Vendor { get; set; }
        public List<CasePackViewModel> list { get; set; }
    }
}
