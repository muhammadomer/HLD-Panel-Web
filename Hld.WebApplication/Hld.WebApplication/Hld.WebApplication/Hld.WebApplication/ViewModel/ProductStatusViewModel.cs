using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class ProductStatusViewModel
    {
        public int ProductStatusId { get; set; }
        public string ProductStatusName { get; set; }
        public bool IsActive { get; set; }
        public int SelectedProductStatusId { get; set; }
    }
}
