using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class SaveQuotationMainVM
    {

        public int Quotation_main_id { get; set; }
        public string Sku { get; set; }
        public string Currency { get; set; }
        public string Notes { get; set; }
        public string Feature { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
