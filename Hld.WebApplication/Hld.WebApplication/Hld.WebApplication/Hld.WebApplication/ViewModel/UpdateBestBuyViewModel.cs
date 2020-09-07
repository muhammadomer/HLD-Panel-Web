using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class UpdateBestBuyViewModel
    {
        public string ProductSku { get; set; }
        public int ProductId { get; set; }
        public decimal UpdateSellingPrice { get; set; }
    }
}
