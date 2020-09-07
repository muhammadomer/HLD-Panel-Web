using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hld.WebApplication.ViewModel
{
    public class SearchViewModelShipmentCasePack
    {
        public string ShipmentId { get; set; }
        public ShipmentViewHeaderViewModel Item { get; set; }
        public List<ShipmentViewProducListViewModel> list { get; set; }
    }
}
