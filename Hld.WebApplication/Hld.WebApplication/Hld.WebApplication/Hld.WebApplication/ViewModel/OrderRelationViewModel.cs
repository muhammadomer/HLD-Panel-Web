using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class OrderRelationViewModel
    {
        public int SC_ParentID { get; set; }
        public int SC_ChildID { get; set; }
        public string BB_OrderID { get; set; }
    }

    public class OrderRelationToSaveViewModel
    {

        public int SC_ChildID { get; set; }

    }
}
