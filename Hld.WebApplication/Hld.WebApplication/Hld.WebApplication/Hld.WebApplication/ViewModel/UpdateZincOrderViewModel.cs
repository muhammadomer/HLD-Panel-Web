using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class UpdateZincOrderViewModel
    {
        public int OrderId { get; set; }
        public int RecievedOrderQty { get; set; }
        public DateTime RecievedOrderDate { get; set; }
    }
}
