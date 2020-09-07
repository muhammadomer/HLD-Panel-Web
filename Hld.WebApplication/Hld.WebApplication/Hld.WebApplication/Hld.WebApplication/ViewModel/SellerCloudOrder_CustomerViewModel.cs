using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class SellerCloudOrder_CustomerViewModel
    {
        public SellerCloudOrderViewModel   Order { get; set; }
        public SellerCloudCustomerDetail Customer { get; set; }
        public List<SellerCloudOrderDetailViewModel> orderDetail { get; set; }
    }

    

}
