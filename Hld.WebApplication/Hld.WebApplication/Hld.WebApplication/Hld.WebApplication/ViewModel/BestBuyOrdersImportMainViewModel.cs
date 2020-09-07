using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class BestBuyOrdersImportMainViewModel
    {
        public BestBuyOrderImportViewModel OrderViewModel { get; set; }
        public List<BestBuyOrderDetailImportViewModel> orderDetailViewModel { get; set; }
        public BestBuyCustomerDetailImportViewModel customerDetailOrderViewModel { get; set; }
       
           
           
    }
}
